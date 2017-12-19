using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_Enable))]
    public class i_EnableInspector : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            if(this.serializedObject.isEditingMultipleObjects)
            {
                base.OnSPInspectorGUI();
                return;
            }

            this.serializedObject.Update();

            var obj = this.serializedObject.targetObject as i_Enable;
            if (obj == null) return;

            string lbl;
            switch(obj.Mode)
            {
                case i_Enable.EnableMode.TriggerArg:
                    lbl = "Resolve from Arg";
                    break;
                case i_Enable.EnableMode.Enable:
                    lbl = "ENABLE";
                    break;
                case i_Enable.EnableMode.Disable:
                    lbl = "DISABLE";
                    break;
                case i_Enable.EnableMode.Toggle:
                    lbl = "TOGGLE";
                    break;
                default:
                    lbl = "ENABLE";
                    break;
            }

            EditorGUILayout.HelpBox(lbl, MessageType.None);

            this.DrawDefaultInspector();

            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
