using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(MinRangeAttribute))]
    public class MinRangePropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as MinRangeAttribute;

            switch(attrib != null ? property.propertyType : SerializedPropertyType.Generic)
            {
                case SerializedPropertyType.Float:
                    property.floatValue = Mathf.Max(EditorGUI.FloatField(position, label, property.floatValue), attrib.Min);
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = (int)Mathf.Max(EditorGUI.IntField(position, label, property.intValue), attrib.Min);
                    break;
                default:
                    SPEditorGUI.DefaultPropertyField(position, property, label);
                    break;
            }
        }

    }
}
