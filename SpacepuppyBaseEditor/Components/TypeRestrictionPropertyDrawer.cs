using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Components
{

    [CustomPropertyDrawer(typeof(TypeRestrictionAttribute))]
    public class TypeRestrictionPropertyDrawer : PropertyDrawer
    {

        #region Fields
        
        private SelectableComponentPropertyDrawer _selectComponentDrawer;

        #endregion

        #region Utils

        private bool ValidateFieldType()
        {
            bool isArray = this.fieldInfo.FieldType.IsListType();
            var fieldType = (isArray) ? this.fieldInfo.FieldType.GetElementTypeOfListType() : this.fieldInfo.FieldType;
            if (!TypeUtil.IsType(fieldType, typeof(UnityEngine.Object)))
                return false;
            //if (!TypeUtil.IsType(fieldType, typeof(Component))) return false;

            var attrib = this.attribute as TypeRestrictionAttribute;
            return attrib.InheritsFromType == null ||
                attrib.InheritsFromType.IsInterface ||
                TypeUtil.IsType(attrib.InheritsFromType, fieldType);
        }

        #endregion

        #region Drawer Overrides

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as TypeRestrictionAttribute;
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
            var attrib = this.attribute as TypeRestrictionAttribute;

            bool isArray = this.fieldInfo.FieldType.IsListType();
            var fieldType = (isArray) ? this.fieldInfo.FieldType.GetElementTypeOfListType() : this.fieldInfo.FieldType;
            bool fieldIsComponentType = TypeUtil.IsType(fieldType, typeof(Component));
            bool objIsComponentType = property.objectReferenceValue is Component;
            var inheritsFromType = attrib.InheritsFromType ?? ((fieldIsComponentType) ? typeof(Component) : fieldType);

            if (attrib.HideTypeDropDown || !objIsComponentType)
            {
                //draw object field
                UnityEngine.Object targ;
                if(fieldIsComponentType)
                {
                    var fieldCompType = (TypeUtil.IsType(fieldType, typeof(Component))) ? fieldType : typeof(Component);
                    targ = SPEditorGUI.ComponentField(position, label, property.objectReferenceValue as Component, inheritsFromType, true, fieldCompType);
                }
                else
                {
                    targ = EditorGUI.ObjectField(position, label, property.objectReferenceValue, fieldType, true);
                }

                if (targ == null)
                {
                    property.objectReferenceValue = null;
                }
                else
                {
                    var o = ObjUtil.GetAsFromSource(inheritsFromType, targ) as UnityEngine.Object;
                    if (attrib.AllowProxy && o == null)
                        o = ObjUtil.GetAsFromSource<IProxy>(targ) as UnityEngine.Object;
                    property.objectReferenceValue = o;
                }
            }
            else
            {
                //draw complex field
                if (_selectComponentDrawer == null)
                {
                    _selectComponentDrawer = new SelectableComponentPropertyDrawer();
                }

                _selectComponentDrawer.RestrictionType = inheritsFromType ?? typeof(Component);
                _selectComponentDrawer.AllowProxy = attrib.AllowProxy;
                _selectComponentDrawer.ShowXButton = true;
                _selectComponentDrawer.AllowNonComponents = true;

                _selectComponentDrawer.OnGUI(position, property, label);
            }

            EditorGUI.EndProperty();
        }
        
        #endregion
        
    }
    
    [System.Obsolete("Use TypeRestrictionPropertyDrawer Instead")]
    [CustomPropertyDrawer(typeof(ComponentTypeRestrictionAttribute))]
    public class ComponentTypeRestrictionPropertyDrawer : TypeRestrictionPropertyDrawer
    {

    }

}
