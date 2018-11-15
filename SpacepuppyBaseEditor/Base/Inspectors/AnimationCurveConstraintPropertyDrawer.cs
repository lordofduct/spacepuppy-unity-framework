using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(AnimationCurveConstraintAttribute))]
    [CustomPropertyDrawer(typeof(AnimationCurveEaseScaleAttribute))]
    public class AnimationCurveConstraintPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.propertyType != SerializedPropertyType.AnimationCurve)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
                return;
            }

            
            if(this.attribute is AnimationCurveConstraintAttribute)
            {
                var attrib = this.attribute as AnimationCurveConstraintAttribute;
                var ranges = new Rect(attrib.x, attrib.y, attrib.width, attrib.height);
                property.animationCurveValue = EditorGUI.CurveField(position, label, property.animationCurveValue, attrib.color, ranges);
            }
            else if (this.attribute is AnimationCurveEaseScaleAttribute)
            {
                var attrib = this.attribute as AnimationCurveEaseScaleAttribute;
                var ranges = new Rect(0f, -Mathf.Max(0f, attrib.overscan), 1f, Mathf.Max(1f, 1f + attrib.overscan * 2f));
                property.animationCurveValue = EditorGUI.CurveField(position, label, property.animationCurveValue, attrib.color, ranges);
            }
            else
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
            }
        }

    }

}
