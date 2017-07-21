using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.AI;
using com.spacepuppy.AI.BehaviourTree;
using com.spacepuppy.AI.BehaviourTree.Components;
using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.BehaviourTree.Components
{

    [CustomEditor(typeof(a_ChangeAIState), true)]
    public class a_ChangeAIStateInspector : SPEditor
    {

        public static string PROP_STATEMACHINE = "_stateMachine";
        public static string PROP_STATE = "_state";
        public static string PROP_WAITON = "_waitOn";


        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();


            this.DrawDefaultInspectorExcept(PROP_STATEMACHINE, PROP_STATE, PROP_WAITON);

            var stateMachineProp = this.serializedObject.FindProperty(PROP_STATEMACHINE);
            SPEditorGUILayout.PropertyField(stateMachineProp);

            var src = GameObjectUtil.GetGameObjectFromSource(stateMachineProp.objectReferenceValue);
            if(src != null)
            {
                var states = ParentComponentStateSupplier<IAIState>.GetComponentsOnTarg(src, false).ToArray();
                var stateProp = this.serializedObject.FindProperty(PROP_STATE);

                int index = System.Array.IndexOf(states, stateProp.objectReferenceValue);
                var names = (from s in states select EditorHelper.TempContent(s.DisplayName)).ToArray();

                EditorGUI.BeginChangeCheck();
                index = EditorGUILayout.Popup(EditorHelper.TempContent("State"), index, names);
                if(EditorGUI.EndChangeCheck())
                {
                    stateProp.objectReferenceValue = (index >= 0) ? states[index] as UnityEngine.Object : null;
                }
            }
            else
            {
                EditorGUILayout.LabelField("State", "*Select a State Machine first*");
            }


            this.DrawPropertyField(PROP_WAITON);


            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
