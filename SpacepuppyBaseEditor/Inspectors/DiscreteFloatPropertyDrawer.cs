using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomPropertyDrawer(typeof(DiscreteFloat))]
    public class DiscreteFloatPropertyDrawer : PropertyDrawer
    {


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("_value");
            var value = EditorGUI.FloatField(position, label, valueProp.floatValue);
            //if the value increased ever so much, ceil the value, good for the mouse scroll
            valueProp.floatValue = (value != valueProp.floatValue && MathUtil.Shear(value) == valueProp.floatValue) ? Mathf.Ceil(value) : Mathf.Floor(value);
        }

    }
}
