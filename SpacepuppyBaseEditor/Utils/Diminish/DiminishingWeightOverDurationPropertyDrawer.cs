using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils.Diminish;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Utils.Diminish
{

    [CustomPropertyDrawer(typeof(DiminishingWeightOverDuration))]
    public class DiminishingWeightOverDurationPropertyDrawer : PropertyDrawer
    {

        public const string PROP_WEIGHT = "_weight";
        public const string PROP_MAXCOUNT = "_maxCount._value";
        public const string PROP_DIMINISHRATE = "_diminishRate";
        public const string PROP_DIMINISHPERIOD = "_diminishPeriodDuration";

        public bool DrawFoldout = true;

        private float[] _line1 = new float[1];
        private float[] _line2 = new float[3];
        private GUIContent[] _line1Labels = new GUIContent[] {new GUIContent("Weight")};
        private GUIContent[] _line2Labels = new GUIContent[] { new GUIContent("Max Count"), new GUIContent("Dimin Rate"), new GUIContent("Dimin Period") };


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return property.isExpanded ? EditorGUIUtility.singleLineHeight * 3f : EditorGUIUtility.singleLineHeight;
            return (!this.DrawFoldout || property.isExpanded) ? EditorGUIUtility.singleLineHeight * 2f : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //EditorGUI.BeginProperty(position, label, property);
            //if(property.isExpanded)
            //{
            //    var r1 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            //    var r2 = new Rect(position.xMin, r1.yMax, position.width, EditorGUIUtility.singleLineHeight);
            //    var r3 = new Rect(position.xMin, r2.yMax, position.width, EditorGUIUtility.singleLineHeight);

            //    property.isExpanded = EditorGUI.Foldout(r1, property.isExpanded, GUIContent.none);
            //    SPEditorGUI.DefaultPropertyField(r1, property.FindPropertyRelative(PROP_WEIGHT), label);
            //    EditorGUI.indentLevel++;
            //    SPEditorGUI.DefaultPropertyField(r2, property.FindPropertyRelative(PROP_DIMINISHRATE), EditorHelper.TempContent("Diminish Rate"));
            //    SPEditorGUI.DefaultPropertyField(r3, property.FindPropertyRelative(PROP_DIMINISHPERIOD), EditorHelper.TempContent("Diminish Period Duration"));
            //    EditorGUI.indentLevel--;
            //}
            //else
            //{
            //    property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
            //    SPEditorGUI.DefaultPropertyField(position, property.FindPropertyRelative(PROP_WEIGHT), label);
            //}
            //EditorGUI.EndProperty();


            if(!this.DrawFoldout || property.isExpanded)
            {
                const float LINE2_MARGIN = 20f;
                var r1 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                var r2 = new Rect(position.xMin + LINE2_MARGIN, r1.yMax, Mathf.Max(position.width - LINE2_MARGIN, 0f), EditorGUIUtility.singleLineHeight);

                if(this.DrawFoldout) property.isExpanded = SPEditorGUI.PrefixFoldoutLabel(r1, property.isExpanded, GUIContent.none);

                var weightProp = property.FindPropertyRelative(PROP_WEIGHT);

                _line1[0] = weightProp.floatValue;

                EditorGUI.BeginChangeCheck();
                SPEditorGUI.MultiFloatField(r1, label, _line1Labels, _line1, 50f);
                if (EditorGUI.EndChangeCheck())
                    weightProp.floatValue = _line1[0];

                var maxCountProp = property.FindPropertyRelative(PROP_MAXCOUNT);
                var rateProp = property.FindPropertyRelative(PROP_DIMINISHRATE);
                var periodProp = property.FindPropertyRelative(PROP_DIMINISHPERIOD);

                _line2[0] = maxCountProp.floatValue;
                _line2[1] = rateProp.floatValue;
                _line2[2] = periodProp.floatValue;

                EditorGUI.BeginChangeCheck();
                SPEditorGUI.DelayedMultiFloatField(r2, _line2Labels, _line2, 80f);
                if(EditorGUI.EndChangeCheck())
                {
                    maxCountProp.floatValue = DiscreteFloatPropertyDrawer.NormalizeValue(maxCountProp.floatValue, _line2[0]);
                    rateProp.floatValue = _line2[1];
                    periodProp.floatValue = _line2[2];
                }
            }
            else
            {
                if (this.DrawFoldout) property.isExpanded = SPEditorGUI.PrefixFoldoutLabel(position, property.isExpanded, GUIContent.none);

                var weightProp = property.FindPropertyRelative(PROP_WEIGHT);
                _line1[0] = weightProp.floatValue;
                SPEditorGUI.MultiFloatField(position, label, _line1Labels, _line1, 50f);
                
            }
        }

    }
}
