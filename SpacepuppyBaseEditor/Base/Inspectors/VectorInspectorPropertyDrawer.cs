using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(VectorInspectorAttribute))]
    public class VectorInspectorPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, label, property);

            var attr = this.attribute as EulerRotationInspectorAttribute;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = ConvertUtil.ToQuaternion(EditorGUI.Vector4Field(position, label, ConvertUtil.ToVector4(property.quaternionValue)));
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = EditorGUI.Vector3Field(position, label, property.vector4Value);
                    break;
                default:
                    SPEditorGUI.DefaultPropertyField(position, property, label);
                    break;
            }

            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }


        

    }

}
