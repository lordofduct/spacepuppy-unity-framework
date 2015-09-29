using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(FixedPercent))]
    public class FixedPercentPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = this.fieldInfo.GetCustomAttributes(typeof(FixedPercent.ConfigAttribute), false).FirstOrDefault() as FixedPercent.ConfigAttribute;
            decimal min = FixedPercent.MIN_VALUE;
            decimal max = FixedPercent.MAX_VALUE;
            bool useSlider = false;
            bool asPercent = false;
            if (attrib != null)
            {
                useSlider = attrib.displayAsRange;
                if (attrib.min > (float)min && attrib.min < (float)FixedPercent.MAX_VALUE) min = (decimal)attrib.min;
                if (attrib.max < (float)max && attrib.max > (float)FixedPercent.MIN_VALUE) max = (decimal)attrib.max;
                asPercent = attrib.displayAsPercent;
            }



            if (asPercent) label.text += " (Percentage)";
            position = EditorGUI.PrefixLabel(position, label);

            var valueProp = property.FindPropertyRelative("_value");
            decimal value = (decimal)valueProp.intValue / (decimal)FixedPercent.PRECISION;
            
            if(asPercent)
            {
                value *= 100M;
                min *= 100M;
                max *= 100M;
            }

            EditorGUI.BeginChangeCheck();
            if(useSlider)
            {
                value = (decimal)EditorGUI.Slider(position, (float)value, (float)min, (float)max);
            }
            else
            {
                //value = EditorGUI.FloatField(position, value);
                string sval = EditorGUI.TextField(position, value.ToString("0.######"));
                value = ConvertUtil.ToDecimal(sval);
            }

            if(EditorGUI.EndChangeCheck())
            {
                if (asPercent)
                {
                    value /= 100M;
                    min /= 100M;
                    max /= 100M;
                }
                
                if (value > max) value = max;
                if (value < min) value = min;
                valueProp.intValue = (int)(value * (decimal)FixedPercent.PRECISION);
            }

        }

    }
}
