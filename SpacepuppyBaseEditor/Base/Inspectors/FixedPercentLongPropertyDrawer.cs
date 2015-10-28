using UnityEngine;
using UnityEditor;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(FixedPercentLong))]
    public class FixedPercentLongPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("_value");
            var fractProp = property.FindPropertyRelative("_fract");
            decimal value = (decimal)valueProp.longValue + (decimal)fractProp.intValue / FixedPercentLong.RNG_FRACT;


            EditorGUI.BeginChangeCheck();
            value = (FixedPercentDecimal)EditorGUI.DoubleField(position, label, (double)value);
            if (EditorGUI.EndChangeCheck())
            {
                valueProp.longValue = (long)System.Math.Floor(value);
                fractProp.intValue = (int)((value % 1M) * FixedPercentLong.RNG_FRACT);
            }
        }

    }
}
