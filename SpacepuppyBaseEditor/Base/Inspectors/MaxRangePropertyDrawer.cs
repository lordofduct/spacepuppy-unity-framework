using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(MaxRangeAttribute))]
    public class MaxRangePropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as MaxRangeAttribute;

            switch (attrib != null ? property.propertyType : SerializedPropertyType.Generic)
            {
                case SerializedPropertyType.Float:
                    property.floatValue = Mathf.Min(EditorGUI.FloatField(position, label, property.floatValue), attrib.Max);
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = (int)Mathf.Min(EditorGUI.IntField(position, label, property.intValue), attrib.Max);
                    break;
                default:
                    SPEditorGUI.DefaultPropertyField(position, property, label);
                    break;
            }
        }

    }

}
