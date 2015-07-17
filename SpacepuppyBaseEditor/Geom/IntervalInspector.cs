using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Geom
{

    [CustomPropertyDrawer(typeof(Interval))]
    public class IntervalInspector : PropertyDrawer
    {
        private GUIContent[] _defaultLabels = new GUIContent[] { new GUIContent("Min"), new GUIContent("Max") };
        private GUIContent[] _reverseLabels = new GUIContent[] { new GUIContent("Max"), new GUIContent("Min") };
        private float[] _values = new float[2];

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targ = (Interval)EditorHelper.GetTargetObjectOfProperty(property);

            var attrib = this.fieldInfo.GetCustomAttributes(typeof(Interval.ConfigAttribute), false).Cast<Interval.ConfigAttribute>().FirstOrDefault();
            bool reverse = (attrib != null) ? attrib.Reverse : false;
            float minValue = (attrib != null) ? attrib.MinValue : float.NegativeInfinity;
            float maxValue = (attrib != null) ? attrib.MaxValue : float.PositiveInfinity;
            bool discreteValuesOnly = (attrib != null) ? attrib.DiscreteValuesOnly : false;

            if(reverse)
            {
                var subLabels = (attrib != null) ? new GUIContent[] { EditorHelper.TempContent(attrib.MaxLabel), EditorHelper.TempContent(attrib.MinLabel) } : _defaultLabels;
                float labelWidth = (attrib != null) ? attrib.LabelWidth : 30f;
                _values[1] = targ.Min;
                _values[0] = targ.Max;
                EditorGUI.BeginChangeCheck();
                SPEditorGUI.MultiFloatField(position, label, subLabels, _values, labelWidth);
                if (EditorGUI.EndChangeCheck())
                {
                    _values[0] = Mathf.Clamp(_values[0], minValue, maxValue);
                    _values[1] = Mathf.Clamp(_values[1], minValue, maxValue);
                    if (discreteValuesOnly) _values[0] = Mathf.Round(_values[0]);
                    if (discreteValuesOnly) _values[1] = Mathf.Round(_values[1]);

                    targ.SetExtents(_values[1], _values[0]);
                    EditorHelper.SetTargetObjectOfProperty(property, targ);
                }
            }
            else
            {
                var subLabels = (attrib != null) ? new GUIContent[] { EditorHelper.TempContent(attrib.MinLabel), EditorHelper.TempContent(attrib.MaxLabel) } : _defaultLabels;
                float labelWidth = (attrib != null) ? attrib.LabelWidth : 30f;
                _values[0] = targ.Min;
                _values[1] = targ.Max;
                EditorGUI.BeginChangeCheck();
                SPEditorGUI.MultiFloatField(position, label, subLabels, _values, labelWidth);
                if (EditorGUI.EndChangeCheck())
                {
                    _values[0] = Mathf.Clamp(_values[0], minValue, maxValue);
                    _values[1] = Mathf.Clamp(_values[1], minValue, maxValue);
                    if (discreteValuesOnly) _values[0] = Mathf.Round(_values[0]);
                    if (discreteValuesOnly) _values[1] = Mathf.Round(_values[1]);

                    targ.SetExtents(_values[0], _values[1]);
                    EditorHelper.SetTargetObjectOfProperty(property, targ);
                }
            }
            
        }

    }

}
