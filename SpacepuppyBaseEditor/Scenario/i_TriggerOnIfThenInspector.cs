using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_TriggerOnIfThen), true)]
    public class i_TriggerOnIfThenInspector : SPEditor
    {
        public const string PROP_ORDER = "_order";
        public const string PROP_CONDITIONS = "_conditions";
        public const string PROP_ELSECONDITION = "_elseCondition";
        private const string PROP_CONDITIONBLOCK_CONDITION = "_condition";
        private const string PROP_CONDITIONBLOCK_TRIGGER = "_trigger";

        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_ORDER);


            EditorGUILayout.BeginVertical("Box");

            //draw conditions blocks
            var conditionsArrayProp = this.serializedObject.FindProperty(PROP_CONDITIONS);
            if (conditionsArrayProp.arraySize == 0) conditionsArrayProp.arraySize = 1;

            for (int i = 0; i < conditionsArrayProp.arraySize; i++)
            {
                EditorGUILayout.LabelField((i == 0) ? "IF" : "ELSE IF");
                var conditionBlockProp = conditionsArrayProp.GetArrayElementAtIndex(i);
                var conditionProp = conditionBlockProp.FindPropertyRelative(PROP_CONDITIONBLOCK_CONDITION);

                var r = EditorGUILayout.GetControlRect(false, _variantDrawer.GetPropertyHeight(conditionProp, GUIContent.none));
                _variantDrawer.OnGUI(r, conditionProp, GUIContent.none);

                var triggerProp = conditionBlockProp.FindPropertyRelative(PROP_CONDITIONBLOCK_TRIGGER);
                SPEditorGUILayout.PropertyField(triggerProp);
            }

            //draw else
            EditorGUILayout.LabelField("ELSE");
            SPEditorGUILayout.PropertyField(this.serializedObject.FindProperty(PROP_ELSECONDITION));

            //draw add buttons
            var fullRect = EditorGUILayout.GetControlRect();
            var leftRect = new Rect(fullRect.xMin, fullRect.yMin, fullRect.width / 2f, fullRect.height);
            var rightRect = new Rect(fullRect.xMin + leftRect.width, fullRect.yMin, fullRect.width / 2f, fullRect.height);
            if (GUI.Button(leftRect, "Add Condition"))
            {
                conditionsArrayProp.arraySize++;
            }
            if (GUI.Button(rightRect, "Remove Condition"))
            {
                conditionsArrayProp.arraySize--;
            }

            EditorGUILayout.EndVertical();

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_ORDER, PROP_CONDITIONS, PROP_ELSECONDITION);


            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
