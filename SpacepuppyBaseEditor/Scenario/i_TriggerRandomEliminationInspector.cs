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
        public const string PROP_ACTIVATEON = "_activateOn";
        public const string PROP_TARGETS = "_targets";

        private TriggerPropertyDrawer _targetsDrawer = new TriggerPropertyDrawer()
        {
            DrawWeight = true
        };

        protected override void OnEnable()
        {
            base.OnEnable();
            _targetsDrawer.CustomizeEntryLabel = CustomizeLabel;
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

        private void CustomizeLabel(GUIContent label, int index)
        {
            if (!Application.isPlaying) return;
            if (this.serializedObject.isEditingMultipleObjects) return;

            var targ = this.serializedObject.targetObject as i_TriggerRandomElimination;
            if (targ == null) return;

            if(targ.TargetHasBeenUsed(index))
            {
                label.text = "X " + label.text;
            }
        }

    }
}
