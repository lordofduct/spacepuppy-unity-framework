using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Components
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

            if (this.ChoiceSelector == null)
            {
                this.ChoiceSelector = new DefaultComponentChoiceSelector();
            }

            this.ChoiceSelector.BeforeGUI(property, _restrictionType);

            if(property.objectReferenceValue == null)
            {
                //SPEditorGUI.DefaultPropertyField(position, property, label);
                this.DrawObjectRefField(position, property, label);
            }
            else
            {
                float w = (label == GUIContent.none) ? position.width : Mathf.Max(position.width - EditorGUIUtility.labelWidth, 0);
                var ra = new Rect(position.xMin, position.yMin, w * this.PopupWidthScale, position.height);
                var rb = new Rect(ra.xMax, position.yMin, w - ra.width, position.height);

                var components = this.ChoiceSelector.GetComponents();
                var names = this.ChoiceSelector.GetPopupEntries();
                int oi = this.ChoiceSelector.GetPopupIndexOfComponent(property.objectReferenceValue as Component);
                int ni = EditorGUI.Popup(ra, label, oi, names);
                if(oi != ni) property.objectReferenceValue = this.ChoiceSelector.GetComponentAtPopupIndex(ni);

                this.DrawObjectRefField(rb, property, GUIContent.none);
            }

            this.ChoiceSelector.GUIComplete(property);
        }

        private void DrawObjectRefField(Rect position, SerializedProperty property, GUIContent label)
        {
            if(TypeUtil.IsType(_restrictionType, typeof(Component)))
            {
                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, _restrictionType, this.AllowSceneObject);
            }
            else
            {
                var ogo = GameObjectUtil.GetGameObjectFromSource(property.objectReferenceValue);
                var ngo = EditorGUI.ObjectField(position, label, ogo, typeof(GameObject), this.AllowSceneObject) as GameObject;
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

    }

}
