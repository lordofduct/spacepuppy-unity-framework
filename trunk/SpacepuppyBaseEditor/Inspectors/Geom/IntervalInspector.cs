using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors.Geom
{

    [CustomPropertyDrawer(typeof(Interval))]
    public class IntervalInspector : PropertyDrawer
    {


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targ = (Interval)EditorHelper.GetTargetObjectOfProperty(property);


            var r = EditorGUI.PrefixLabel(position, label);
            var w = r.width / 2.0f;
            var r1 = new Rect(r.xMin, r.yMin, w, r.height);
            var r2 = new Rect(r.xMin + w, r.yMin, w, r.height);

            EditorGUI.BeginChangeCheck();
            float min = EditorGUI.FloatField(r1, targ.Min);
            float max = EditorGUI.FloatField(r2, targ.Max);
            if(EditorGUI.EndChangeCheck())
            {
                targ.SetExtents(min, max);
                EditorHelper.SetTargetObjectOfProperty(property, targ);
            }
            
        }

    }

}
