using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomPropertyDrawer(typeof(ComponentTypeRestrictionAttribute))]
    public class ComponentTypeRestrictionPropertyDrawer : PropertyDrawer
    {

        #region Fields

        private Dictionary<string, System.Type> _overrideBaseTypeDict = new Dictionary<string, System.Type>();

        #endregion

        #region Utils

        private bool ValidateFieldType()
        {
            bool isArray = this.fieldInfo.FieldType.IsListType();
            var fieldType = (isArray) ? this.fieldInfo.FieldType.GetElementTypeOfListType() : this.fieldInfo.FieldType;
            if (!ObjUtil.IsType(fieldType, typeof(Component))) return false;

            var attrib = this.attribute as ComponentTypeRestrictionAttribute;
            return attrib.InheritsFromType == null ||
                ObjUtil.IsType(attrib.InheritsFromType, fieldType) ||
                ObjUtil.IsType(attrib.InheritsFromType, typeof(IComponent));
        }

        private GUIContent GetHeaderLabel(SerializedProperty property, GUIContent label, System.Type inheritsFromType)
        {
            string suffix = (property.objectReferenceValue != null) ? "(" + property.objectReferenceValue.GetType().Name + ")" : "(" + inheritsFromType.Name + ")";
            string tooltip = inheritsFromType.Name;
            if (property.objectReferenceValue != null) tooltip += " - " + suffix;

            var tooltipAttrib = this.fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), false).FirstOrDefault() as TooltipAttribute;
            if (tooltipAttrib != null) tooltip += "\n\n" + tooltipAttrib.tooltip;

            return new GUIContent(label.text + " " + suffix, tooltip);
        }

        #endregion

        #region Drawer Overrides

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (this.ValidateFieldType())
            {
                if (property.isExpanded)
                {
                    return 3f * EditorGUIUtility.singleLineHeight + 2f;
                }
                else
                {
                    return EditorGUIUtility.singleLineHeight;
                }
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(!this.ValidateFieldType())
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            //get base type
            var inheritsFromType = (this.attribute as ComponentTypeRestrictionAttribute).InheritsFromType ?? typeof(Component);
            
            bool isArray = this.fieldInfo.FieldType.IsListType();
            var fieldType = (isArray) ? this.fieldInfo.FieldType.GetElementTypeOfListType() : this.fieldInfo.FieldType;

            //draw
            var h = EditorGUIUtility.singleLineHeight;
            var r0 = new Rect(position.xMin, position.yMin, position.width, h);
            bool isExpanded = property.isExpanded;
            property.isExpanded = EditorGUI.Foldout(r0, isExpanded, this.GetHeaderLabel(property, label, inheritsFromType));

            if (isExpanded)
            {
                EditorGUI.indentLevel += 1;

                var r1 = new Rect(position.xMin, position.yMin + h + 1f, position.width, h);
                var r2 = new Rect(position.xMin, position.yMin + (2f * h) + 2f, position.width, h);

                //draw component type selector
                if (!_overrideBaseTypeDict.ContainsKey(label.text) || _overrideBaseTypeDict[label.text] == null)
                {
                    if (property.objectReferenceValue != null)
                        _overrideBaseTypeDict[label.text] = property.objectReferenceValue.GetType();
                    else
                        _overrideBaseTypeDict[label.text] = inheritsFromType;
                }
                _overrideBaseTypeDict[label.text] = SPEditorGUI.TypeDropDown(r1, new GUIContent("Restrict Type"), inheritsFromType, _overrideBaseTypeDict[label.text], true, true, inheritsFromType, (this.attribute as ComponentTypeRestrictionAttribute).MenuListingStyle);
                inheritsFromType = _overrideBaseTypeDict[label.text];

                //draw object field
                var refLabel = new GUIContent("Reference");
                property.objectReferenceValue = SPEditorGUI.ComponentField(r2, refLabel, property.objectReferenceValue as Component, inheritsFromType, true, fieldType);
                property.serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.EndProperty();
        }

        #endregion


    }
}
