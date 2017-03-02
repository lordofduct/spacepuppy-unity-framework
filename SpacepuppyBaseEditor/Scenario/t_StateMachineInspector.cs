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

        private ReorderableArrayPropertyDrawer _arrayDrawer = new ReorderableArrayPropertyDrawer()
        {
            DrawElementAtBottom = true,
            ChildPropertyAsLabel = PROP_STATENAME
        };

        protected override void OnEnable()
        {
            base.OnEnable();

            _arrayDrawer.OnAddCallback = this.OnStateAdded;
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            var statesProp = this.serializedObject.FindProperty(PROP_STATES);
            var label = EditorHelper.TempContent("States");
            var r = EditorGUILayout.GetControlRect(true, _arrayDrawer.GetPropertyHeight(statesProp, label));
            _arrayDrawer.OnGUI(r, statesProp, label);

            string[] states = new string[statesProp.arraySize];
            for (int i = 0; i < statesProp.arraySize; i++)
            {
                states[i] = statesProp.GetArrayElementAtIndex(i).FindPropertyRelative(PROP_STATENAME).stringValue;
            }

            //notify on state
            this.DrawPropertyField(PROP_NOTIFYONSTART);

            //initial state
            var initialStateProp = this.serializedObject.FindProperty(PROP_INITIALSTATE);
            initialStateProp.intValue = EditorGUILayout.Popup("Initial State", initialStateProp.intValue, states);

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_STATES, PROP_INITIALSTATE, PROP_NOTIFYONSTART);

            if (Application.isPlaying)
            {
                var obj = this.serializedObject.targetObject as t_StateMachine;
                if (obj != null)
                    EditorGUILayout.LabelField("Current State", (obj.Current != null) ? obj.Current.Name : string.Empty);
            }

            this.serializedObject.ApplyModifiedProperties();
        }


        private void OnStateAdded(ReorderableList lst)
        {
            lst.serializedProperty.arraySize++;
            lst.index = lst.serializedProperty.arraySize - 1;
            
            var stateProp = lst.serializedProperty.GetArrayElementAtIndex(lst.index);
            stateProp.FindPropertyRelative(PROP_STATENAME).stringValue = "State " + lst.serializedProperty.arraySize.ToString();
        }


    }

}

