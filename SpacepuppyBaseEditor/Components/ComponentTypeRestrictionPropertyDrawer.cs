using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Components
{

    [CustomPropertyDrawer(typeof(ComponentTypeRestrictionAttribute))]
    public class ComponentTypeRestrictionPropertyDrawer : PropertyDrawer
    {

        #region Fields
        
        private SelectableComponentPropertyDrawer _selectComponentDrawer;

        #endregion

        #region Utils

        private bool ValidateFieldType()
        {
            bool isArray = this.fieldInfo.FieldType.IsListType();
            var fieldType = (isArray) ? this.fieldInfo.FieldType.GetElementTypeOfListType() : this.fieldInfo.FieldType;
            if (!TypeUtil.IsType(fieldType, typeof(Component))) return false;

            var attrib = this.attribute as ComponentTypeRestrictionAttribute;
            return attrib.InheritsFromType == null ||
                attrib.InheritsFromType.IsInterface ||
                TypeUtil.IsType(attrib.InheritsFromType, fieldType);
        }

        #endregion

        #region Drawer Overrides

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as ComponentTypeRestrictionAttribute;
            if (attrib.HideTypeDropDown)
                return EditorGUIUtility.singleLineHeight;
            else
            {
                if (_selectComponentDrawer == null) _selectComponentDrawer = new SelectableComponentPropertyDrawer();
                return _selectComponentDrawer.GetPropertyHeight(property, label);
            }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!this.ValidateFieldType())
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            //get base type
            var attrib = this.attribute as ComponentTypeRestrictionAttribute;
            var inheritsFromType = attrib.InheritsFromType ?? typeof(Component);

            bool isArray = this.fieldInfo.FieldType.IsListType();
            var fieldType = (isArray) ? this.fieldInfo.FieldType.GetElementTypeOfListType() : this.fieldInfo.FieldType;

            if (attrib.HideTypeDropDown)
            {
                //draw object field
                var fieldObjType = (inheritsFromType.IsInterface) ? typeof(Component) : inheritsFromType;
                var comp = SPEditorGUI.ComponentField(position, label, property.objectReferenceValue as Component, fieldObjType, true, fieldType);
                if (comp == null)
                    property.objectReferenceValue = null;
                else if (TypeUtil.IsType(comp.GetType(), inheritsFromType))
                    property.objectReferenceValue = comp;
                else
                    property.objectReferenceValue = comp.GetComponent(inheritsFromType);
            }
            else
            {
                //draw complex field
                if(_selectComponentDrawer == null)
                {
                    _selectComponentDrawer = new SelectableComponentPropertyDrawer();
                }

                _selectComponentDrawer.RestrictionType = inheritsFromType;
                _selectComponentDrawer.ShowXButton = true;

                _selectComponentDrawer.OnGUI(position, property, label);
            }

            EditorGUI.EndProperty();
        }
        
        #endregion


    }
}
