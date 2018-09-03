using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.spacepuppyeditor.Internal
{
    internal class HackVector2PropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector2) return;
            property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
        }

    }

    internal class HackVector3PropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector3) return;
            property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
        }

    }
}
