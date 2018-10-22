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

    [CustomEditor(typeof(i_TriggerSequence))]
    public class i_TriggerSequenceInspector : SPEditor
    {

        private const string PROP_CURRENTINDEX = "_currentIndex";
        private const string PROP_TRIGGER = "_trigger";

        protected override void OnSPInspectorGUI()
        {
            if(this.serializedObject.isEditingMultipleObjects || !(this.serializedObject.targetObject is i_TriggerSequence))
            {
                this.DrawDefaultInspector();
                return;
            }

            this.serializedObject.Update();
            this.DrawDefaultInspectorExcept(PROP_CURRENTINDEX);

            var seq = this.serializedObject.targetObject as i_TriggerSequence;
            var prop = this.serializedObject.FindProperty(PROP_CURRENTINDEX);
            EditorGUI.BeginChangeCheck();
            int i = EditorGUILayout.IntField("Current Index", prop.intValue);
            if(EditorGUI.EndChangeCheck())
            {
                prop.intValue = Mathf.Clamp(i, 0, seq.TriggerSequence.Count - 1);
            }

            Rect rect;
            rect = ITriggerableMechanismAddonDrawer.GetActivateButtonControlRect();
            if (GUI.Button(rect, "Reset Index"))
            {
                prop.intValue = 0;
            }
            rect = ITriggerableMechanismAddonDrawer.GetActivateButtonControlRect();
            if (GUI.Button(rect, "Move Ahead Index"))
            {
                prop.intValue = Mathf.Clamp(i + 1, 0, seq.TriggerSequence.Count - 1);
            }

            this.serializedObject.ApplyModifiedProperties();
        }


    }

}
