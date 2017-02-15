using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(t_StateMachine), true)]
    public class t_StateMachineInspector : SPEditor
    {

        private const string PROP_STATES = "_states";
        private const string PROP_STATENAME = "_name";
        private const string PROP_INITIALSTATE = "_initialState";
        private const string PROP_NOTIFYONSTART = "_notifyFirstStateOnStart";

        private TriggerPropertyDrawer _triggerDrawer = new TriggerPropertyDrawer();
        
        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            //this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawDefaultInspectorExcept(PROP_INITIALSTATE);

            var statesProp = this.serializedObject.FindProperty(PROP_STATES);
            string[] states = new string[statesProp.arraySize];
            for (int i = 0; i < statesProp.arraySize; i++)
            {
                states[i] = statesProp.GetArrayElementAtIndex(i).FindPropertyRelative(PROP_STATENAME).stringValue;
            }

            //notify on state
            //this.DrawPropertyField(PROP_NOTIFYONSTART);

            //initial state
            var initialStateProp = this.serializedObject.FindProperty(PROP_INITIALSTATE);
            initialStateProp.intValue = EditorGUILayout.Popup("Initial State", initialStateProp.intValue, states);

            //draw actual states
            //TODO - draw pretty like

            if(Application.isPlaying)
            {
                var obj = this.serializedObject.targetObject as t_StateMachine;
                if (obj != null)
                    EditorGUILayout.LabelField("Current State", obj.Current);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        
    }

}

