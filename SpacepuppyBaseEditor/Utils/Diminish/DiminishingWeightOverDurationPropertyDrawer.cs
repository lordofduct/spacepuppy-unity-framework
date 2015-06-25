using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils.Diminish;

namespace com.spacepuppyeditor.Utils.Diminish
{

    [CustomPropertyDrawer(typeof(DiminishingWeightOverDuration))]
    public class DiminishingWeightOverDurationPropertyDrawer : PropertyDrawer
    {

        public const string PROP_WEIGHT = "_weight";
        public const string PROP_DIMINISHRATE = "_diminishRate";
        public const string PROP_DIMINISHPERIOD = "_diminishPeriodDuration";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 3f : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            if(property.isExpanded)
            {
                var r1 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                var r2 = new Rect(position.xMin, r1.yMax, position.width, EditorGUIUtility.singleLineHeight);
                var r3 = new Rect(position.xMin, r2.yMax, position.width, EditorGUIUtility.singleLineHeight);

                property.isExpanded = EditorGUI.Foldout(r1, property.isExpanded, GUIContent.none);
                SPEditorGUI.DefaultPropertyField(r1, property.FindPropertyRelative(PROP_WEIGHT), label);
                EditorGUI.indentLevel++;
                SPEditorGUI.DefaultPropertyField(r2, property.FindPropertyRelative(PROP_DIMINISHRATE), EditorHelper.TempContent("Diminish Rate"));
                SPEditorGUI.DefaultPropertyField(r3, property.FindPropertyRelative(PROP_DIMINISHPERIOD), EditorHelper.TempContent("Diminish Period Duration"));
                EditorGUI.indentLevel--;
            }
            else
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
                SPEditorGUI.DefaultPropertyField(position, property.FindPropertyRelative("_weight"), label);
            }
            EditorGUI.EndProperty();
        }

    }
}
