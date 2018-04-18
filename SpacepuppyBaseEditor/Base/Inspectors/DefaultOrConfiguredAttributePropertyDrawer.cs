using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(DefaultOrConfiguredAttribute), true)]
    public class DefaultOrConfiguredAttributePropertyDrawer : PropertyDrawer
    {


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as DefaultOrConfiguredAttribute;
            if(attrib == null)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
                return;
            }

            
            if(attrib.DrawAsDefault(EditorHelper.GetPropertyValue(property)))
            {
                var value = attrib.GetValueToDisplayAsDefault();

                label.text += " (Default Value)";
                EditorGUI.BeginChangeCheck();
                value = SPEditorGUI.DefaultPropertyField(position, label, value, attrib.FieldType);
                if (EditorGUI.EndChangeCheck())
                    EditorHelper.SetPropertyValue(property, value);
            }
            else
            {
                var r0 = new Rect(position.xMin, position.yMin, Mathf.Max(0f, position.width - SPEditorGUI.X_BTN_WIDTH), position.height);
                SPEditorGUI.DefaultPropertyField(r0, property, label);

                var w = position.width = r0.width;
                if(w > 1f)
                {
                    var r1 = new Rect(position.xMax - w, position.yMin, w, EditorGUIUtility.singleLineHeight);
                    if (SPEditorGUI.XButton(r1, "Reset to default value."))
                    {
                        EditorHelper.SetPropertyValue(property, attrib.GetDefaultValue());
                    }
                }
            }
        }

    }
}
