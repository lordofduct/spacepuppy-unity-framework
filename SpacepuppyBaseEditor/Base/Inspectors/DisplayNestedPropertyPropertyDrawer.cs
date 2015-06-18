using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(DisplayNestedPropertyAttribute))]
    public class DisplayNestedPropertyPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as DisplayNestedPropertyAttribute;
            if(attrib != null)
            {
                var p = property.FindPropertyRelative(attrib.InnerPropName);
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }

        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as DisplayNestedPropertyAttribute;
            if(attrib == null)
            {
                EditorGUI.LabelField(position, label, EditorHelper.TempContent("DisplayNestedPropertyAttribute could not be found."));
                return;
            }

            var p = property.FindPropertyRelative(attrib.InnerPropName);
            if (p != null)
            {
                if(attrib.Label != null) label = EditorHelper.TempContent(attrib.Label, attrib.Tooltip);
                SPEditorGUI.DefaultPropertyField(position, p, label);
            }
            else
            {
                EditorGUI.LabelField(position, label, EditorHelper.TempContent(string.Format("Nested Property with name {0} could not be found.", attrib.InnerPropName)));
            }
        }

    }

}
