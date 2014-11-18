using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var attrib = this.attribute as EnumFlagsAttribute;
            var tp = (attrib.EnumType != null && attrib.EnumType.IsEnum) ? attrib.EnumType : this.fieldInfo.FieldType;
            if(tp.IsEnum)
            {
                property.intValue = SPEditorGUI.EnumFlagField(position, tp, label, property.intValue);
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.LabelField(position, label);
            }

            EditorGUI.EndProperty();
        }

    }
}
