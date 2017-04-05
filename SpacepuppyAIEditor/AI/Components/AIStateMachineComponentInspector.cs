using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.AI;
using com.spacepuppy.AI.Components;
using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI
{

    [CustomEditor(typeof(AIStateMachineComponent), true)]
    public class AIStateMachineComponentInspector : SPEditor
    {

        public const string PROP_DEFAULTSTATE = "_defaultState";

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            //this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            //var cache = SPGUI.DisableIfPlaying();
            //var stateProp = this.serializedObject.FindProperty(PROP_DEFAULTSTATE);

            //Component[] states = null;
            //var src = GameObjectUtil.GetGameObjectFromSource(this.serializedObject.targetObject);
            //if (src != null) states = ParentComponentStateSupplier<IAIState>.GetComponentsOnTarg(src, false).Cast<Component>().ToArray();
            //else states = new Component[] { };

            //var componentLabels = (from c in states select string.Format("{0} ({1})", c.name, c.GetType().Name)).ToArray();
            //stateProp.objectReferenceValue = SPEditorGUILayout.SelectComponentField(stateProp.displayName, states, componentLabels, stateProp.objectReferenceValue as Component);
            //cache.Reset();

            //this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_DEFAULTSTATE);

            this.DrawDefaultInspector();

            this.serializedObject.ApplyModifiedProperties();


            if (Application.isPlaying)
            {
                var machine = this.target as  AIStateMachineComponent;
                if (machine.Current != null)
                {
                    var state = machine.Current;
                    var msg = string.Format("Currently active state is '{0} ({1})'.", state.DisplayName, state.GetType().Name);
                    EditorGUILayout.HelpBox(msg, MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Currently active state is null.", MessageType.Info);
                }
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

    }

}
