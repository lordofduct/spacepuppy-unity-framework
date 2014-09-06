using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = this.attribute as LabelAttribute;
            if (attr != null)
            {
                label.tooltip = attr.ToolTip;
                if (!string.IsNullOrEmpty(attr.Label)) label.text = attr.Label;
            }

            EditorGUI.PropertyField(position, property, label);
        }

    }

}