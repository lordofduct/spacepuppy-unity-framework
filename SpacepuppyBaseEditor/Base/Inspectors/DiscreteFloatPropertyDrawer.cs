using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(DiscreteFloat))]
    public class DiscreteFloatPropertyDrawer : PropertyDrawer
    {


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("_value");
            var value = EditorGUI.FloatField(position, label, valueProp.floatValue);
            //if the value increased ever so much, ceil the value, good for the mouse scroll
            valueProp.floatValue = NormalizeValue(valueProp.floatValue, value);
        }


        public static float NormalizeValue(float oldValue, float newValue)
        {
            return (newValue != oldValue && MathUtil.Shear(newValue) == oldValue) ? Mathf.Ceil(newValue) : Mathf.Floor(newValue);
        }

    }
}
