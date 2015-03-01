using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomPropertyDrawer(typeof(SelectableComponentAttribute))]
    public class SelectableComponentPropertyDrawer : PropertyDrawer
    {

        #region Fields

        public bool AllowSceneObject;
        private System.Type _restrictionType = typeof(Component);

        public System.Type RestrictionType
        {
            get { return _restrictionType; }
            set
            {
                if (value == null || !ObjUtil.IsType(value, typeof(Component), typeof(IComponent))) throw new TypeArgumentMismatchException(value, typeof(Component), "value");
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

            if (property.propertyType != SerializedPropertyType.ObjectReference || !ObjUtil.IsType(_restrictionType, typeof(Component), typeof(IComponent)))
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
                const float POPUP_WIDTH = 100f;
                if (position.width > POPUP_WIDTH + EditorGUIUtility.labelWidth)
                {
                    var ra = new Rect(position.xMin, position.yMin, POPUP_WIDTH + EditorGUIUtility.labelWidth, position.height);
                    var rb = new Rect(ra.xMax, position.yMin, position.width - ra.width, position.height);

                    var go = (property.objectReferenceValue as Component).gameObject;
                    var components = go.GetComponents(_restrictionType);
                    var names = (from c in components select new GUIContent(c.GetType().Name)).ToArray();
                    int oi = components.IndexOf(property.objectReferenceValue);
                    int ni = EditorGUI.Popup(ra, label, oi, names);
                    if(oi != ni)
                    {
                        property.objectReferenceValue = components[ni];
                    }

                    this.DrawObjectRefField(rb, property);
                }
                else
                {
                    this.DrawObjectRefField(position, property);
                }
            }

        }

        private void DrawObjectRefField(Rect position, SerializedProperty property)
        {
            if(ObjUtil.IsType(_restrictionType, typeof(Component)))
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

    }

}
