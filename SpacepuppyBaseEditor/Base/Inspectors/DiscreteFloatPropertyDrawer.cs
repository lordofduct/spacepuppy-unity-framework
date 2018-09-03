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

            EditorGUI.BeginChangeCheck();

            var value = EditorGUI.FloatField(position, label, valueProp.floatValue);
            
            if (EditorGUI.EndChangeCheck())
            {
                //if the value increased ever so much, ceil the value, good for the mouse scroll
                value = NormalizeValue(valueProp.floatValue, value);

                if (this.fieldInfo != null)
                {
                    var attribs = this.fieldInfo.GetCustomAttributes(typeof(DiscreteFloat.ConfigAttribute), false) as DiscreteFloat.ConfigAttribute[];
                    foreach (var attrib in attribs)
                    {
                        value = attrib.Normalize(value);
                    }

                    //if the value increased ever so much, ceil the value, good for the mouse scroll
                    value = NormalizeValue(valueProp.floatValue, value);
                }

                valueProp.floatValue = value;
            }
        }


        public static float NormalizeValue(float oldValue, float newValue)
        {
            return (newValue != oldValue && MathUtil.Shear(newValue) == oldValue) ? Mathf.Ceil(newValue) : Mathf.Floor(newValue);
        }

        public static float GetValue(SerializedProperty prop)
        {
            if(prop != null)
            {
                var propValue = prop.FindPropertyRelative("_value");
                return propValue != null && propValue.propertyType == SerializedPropertyType.Float ? propValue.floatValue : float.NaN;
            }

            return float.NaN;
        }

        public static float SetValue(SerializedProperty prop, float value)
        {
            if (prop != null)
            {
                var propValue = prop.FindPropertyRelative("_value");
                if (propValue != null && propValue.propertyType == SerializedPropertyType.Float)
                {
                    propValue.floatValue = Mathf.Round(value);
                }
            }

            return float.NaN;
        }

    }
}
