using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_TriggerRandomElimination), true)]
    public class i_TriggerRandomEliminationInspector : SPEditor
    {

        public const string PROP_ORDER = EditorHelper.PROP_ORDER;
        public const string PROP_ACTIVATEON = EditorHelper.PROP_ACTIVATEON;
        public const string PROP_TARGETS = "_targets";

        private TriggerPropertyDrawer _targetsDrawer = new TriggerPropertyDrawer()
        {
            DrawWeight = true
        };

        protected override void OnEnable()
        {
            base.OnEnable();
            _targetsDrawer.OnDrawCustomizedEntryLabel = CustomizeLabel;
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_ORDER);
            this.DrawPropertyField(PROP_ACTIVATEON);

            var targetsProp = this.serializedObject.FindProperty(PROP_TARGETS);
            var label = EditorHelper.TempContent(targetsProp.displayName);
            var area = EditorGUILayout.GetControlRect(false, _targetsDrawer.GetPropertyHeight(targetsProp, label));
            _targetsDrawer.OnGUI(area, targetsProp, label);

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_ORDER, PROP_ACTIVATEON, PROP_TARGETS);


            this.serializedObject.ApplyModifiedProperties();
        }

        private void CustomizeLabel(Rect area, SerializedProperty property, int index)
        {
            var targ = this.serializedObject.targetObject as i_TriggerRandomElimination;

            if (!Application.isPlaying ||
                this.serializedObject.isEditingMultipleObjects ||
                targ == null)
            {
                TriggerPropertyDrawer.DrawDefaultListElementLabel(area, property, index);
            }
            else
            {
                if (targ.TargetHasBeenUsed(index))
                {
                    var r0 = new Rect(area.xMin, area.yMin, Mathf.Min(36f, area.width), EditorGUIUtility.singleLineHeight);
                    var r1 = new Rect(r0.xMax, area.yMin, Mathf.Max(0f, area.width - r0.width), EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(r0, index.ToString("X 00:"));
                    TriggerTargetPropertyDrawer.DrawTriggerActivationTypeDropdown(r1, property, false);
                }
                else
                {
                    TriggerPropertyDrawer.DrawDefaultListElementLabel(area, property, index);
                }
            }


        }

    }
}
