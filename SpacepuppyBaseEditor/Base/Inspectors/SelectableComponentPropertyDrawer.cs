using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(SelectableComponentAttribute))]
    public class SelectableComponentPropertyDrawer : PropertyDrawer
    {

        #region Fields

        public const float DEFAULT_POPUP_WIDTH_SCALE = 0.4f;

        public bool AllowSceneObject;
        public float PopupWidthScale = DEFAULT_POPUP_WIDTH_SCALE;
        public IComponentChoiceSelector ChoiceSelector;

        private System.Type _restrictionType = typeof(Component);

        public System.Type RestrictionType
        {
            get { return _restrictionType; }
            set
            {
                if (value == null || !TypeUtil.IsType(value, typeof(Component), typeof(IComponent))) throw new TypeArgumentMismatchException(value, typeof(Component), "value");
                _restrictionType = value;
            }
        }

        #endregion

        #region CONSTRUCTOR

        private void Init()
        {
            if (this.fieldInfo != null)
            {
                var tp = this.fieldInfo.FieldType;
                if (tp.IsListType()) tp = tp.GetElementTypeOfListType();
                _restrictionType = tp;
            }

            if (this.attribute != null && this.attribute is SelectableComponentAttribute)
            {
                this.AllowSceneObject = (this.attribute as SelectableComponentAttribute).AllowSceneObjects;
            }
        }

        #endregion


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.Init();

            if (property.propertyType != SerializedPropertyType.ObjectReference || !TypeUtil.IsType(_restrictionType, typeof(Component), typeof(IComponent)))
            {
                this.DrawAsMismatchedAttribute(position, property, label);
                return;
            }

            if(property.objectReferenceValue == null)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
            }
            else
            {
                float w = (label == GUIContent.none) ? position.width : Mathf.Max(position.width - EditorGUIUtility.labelWidth, 0);
                var ra = new Rect(position.xMin, position.yMin, w * this.PopupWidthScale, position.height);
                var rb = new Rect(ra.xMax, position.yMin, w - ra.width, position.height);

                var go = (property.objectReferenceValue as Component).gameObject;
                var components = go.GetComponents(_restrictionType);
                if(this.ChoiceSelector == null)
                {
                    this.ChoiceSelector = new DefaultComponentChoiceSelector();
                }

                this.ChoiceSelector.BeforeGUI(property, components);
                var names = this.ChoiceSelector.GetPopupEntries(property, components);
                int oi = this.ChoiceSelector.GetIndexOfComponent(property, components, names, property.objectReferenceValue as Component);
                int ni = EditorGUI.Popup(ra, label, oi, names);
                if(oi != ni) property.objectReferenceValue = this.ChoiceSelector.GetComponent(property, components, names, ni);
                this.ChoiceSelector.GUIComplete(property, components);

                this.DrawObjectRefField(rb, property);
            }

        }

        private void DrawObjectRefField(Rect position, SerializedProperty property)
        {
            if(TypeUtil.IsType(_restrictionType, typeof(Component)))
            {
                property.objectReferenceValue = EditorGUI.ObjectField(position, property.objectReferenceValue, _restrictionType, this.AllowSceneObject);
            }
            else
            {
                var ogo = GameObjectUtil.GetGameObjectFromSource(property.objectReferenceValue);
                var ngo = EditorGUI.ObjectField(position, ogo, typeof(GameObject), this.AllowSceneObject) as GameObject;
                if(ogo != ngo)
                {
                    property.objectReferenceValue = (ngo == null) ? null : ngo.GetFirstLikeComponent(_restrictionType);
                }
            }
        }


        private void DrawAsMismatchedAttribute(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, label, EditorHelper.TempContent("Mismatched type of PropertyDrawer attribute with field."));
        }




        public interface IComponentChoiceSelector
        {

            void BeforeGUI(SerializedProperty property, Component[] components);

            GUIContent[] GetPopupEntries(SerializedProperty property, Component[] components);
            int GetIndexOfComponent(SerializedProperty property, Component[] components, GUIContent[] popupEntries, Component comp);
            Component GetComponent(SerializedProperty property, Component[] components, GUIContent[] popupEntries, int index);

            void GUIComplete(SerializedProperty property, Component[] components);

        }

        public class DefaultComponentChoiceSelector : IComponentChoiceSelector
        {

            public virtual void BeforeGUI(SerializedProperty property, Component[] components)
            {

            }

            public virtual GUIContent[] GetPopupEntries(SerializedProperty property, Component[] components)
            {
                //return (from c in components select new GUIContent(c.GetType().Name)).ToArray();
                return (from s in DefaultComponentChoiceSelector.GetUniqueComponentNames(components) select new GUIContent(s)).ToArray();
            }

            public virtual int GetIndexOfComponent(SerializedProperty property, Component[] components, GUIContent[] popupEntries, Component comp)
            {
                return components.IndexOf(property.objectReferenceValue);
            }

            public virtual Component GetComponent(SerializedProperty property, Component[] components, GUIContent[] popupEntries, int index)
            {
                if (index < 0 || index >= components.Length) return null;
                return components[index];
            }

            public virtual void GUIComplete(SerializedProperty property, Component[] components)
            {

            }



            private static Dictionary<System.Type, int> _uniqueCount = new Dictionary<System.Type, int>();
            public static IEnumerable<string> GetUniqueComponentNames(Component[] components)
            {
                _uniqueCount.Clear();
                for (int i = 0; i < components.Length; i++)
                {
                    var tp = components[i].GetType();
                    if (_uniqueCount.ContainsKey(tp))
                    {
                        _uniqueCount[tp]++;
                        yield return tp.Name + " " + _uniqueCount[tp].ToString();
                    }
                    else
                    {
                        _uniqueCount.Add(tp, 1);
                        yield return tp.Name;
                    }
                }
                _uniqueCount.Clear();
            }
        }

    }

}
