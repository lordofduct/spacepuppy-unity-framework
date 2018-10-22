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

    [CustomEditor(typeof(i_TriggerOnEval), true)]
    public class i_TriggerOnEvalInspector : SPEditor
    {

        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(EditorHelper.PROP_ORDER);
            this.DrawPropertyField(EditorHelper.PROP_ACTIVATEON);


            EditorGUILayout.BeginVertical("Box");
            var conditionsArrayProp = this.serializedObject.FindProperty("_conditions");

            for (int i = 0; i < conditionsArrayProp.arraySize; i++)
            {
                EditorGUILayout.LabelField(string.Format("Condition {0}", i));
                var conditionBlockProp = conditionsArrayProp.GetArrayElementAtIndex(i);
                var conditionProp = conditionBlockProp.FindPropertyRelative("_condition");

                var r = EditorGUILayout.GetControlRect(false, _variantDrawer.GetPropertyHeight(conditionProp, GUIContent.none));
                _variantDrawer.OnGUI(r, conditionProp, GUIContent.none);

                var triggerProp = conditionBlockProp.FindPropertyRelative("_trigger");
                SPEditorGUILayout.PropertyField(triggerProp);
            }

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

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, EditorHelper.PROP_ORDER, EditorHelper.PROP_ACTIVATEON, "_conditions");


            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
