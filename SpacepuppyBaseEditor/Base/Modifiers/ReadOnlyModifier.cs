using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyModifier : PropertyModifier
    {

        private bool? _cached = null;

        protected internal override void OnBeforeGUI(SerializedProperty property)
        {
            if(this.IsDrawer)
            {
                _cached = null;
            }
            else
            {
                _cached = GUI.enabled;
                GUI.enabled = false;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch(property.propertyType)
            {
                case SerializedPropertyType.String:
                    EditorGUI.TextField(position, label, property.stringValue);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.IntField(position, label, property.intValue);
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.FloatField(position, label, property.floatValue);
                    break;
                case SerializedPropertyType.Vector2:
                    EditorGUI.Vector2Field(position, label, property.vector2Value);
                    break;
                case SerializedPropertyType.Vector3:
                    EditorGUI.Vector2Field(position, label, property.vector3Value);
                    break;
                case SerializedPropertyType.Vector4:
                    EditorGUI.Vector2Field(position, label, property.vector4Value);
                    break;
                case SerializedPropertyType.Quaternion:
                    EditorGUI.Vector3Field(position, label, property.quaternionValue.eulerAngles);
                    break;
                default:
                    var cache = GUI.enabled;
                    GUI.enabled = false;
                    base.OnGUI(position, property, label);
                    GUI.enabled = cache;
                    break;
            }
        }

        protected internal override void OnPostGUI(SerializedProperty property)
        {
            if (_cached != null)
            {
                GUI.enabled = (bool)_cached.Value;
            }
        }

    }

}
