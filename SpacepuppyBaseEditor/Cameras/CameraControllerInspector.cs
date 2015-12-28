using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Cameras;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Components;

namespace com.spacepuppyeditor.Cameras
{

    [CustomEditor(typeof(CameraMovementController), true)]
    public class CameraControllerInspector : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();
            this.DrawDefaultInspectorExcept("StartingCameraStyle");
            this.serializedObject.ApplyModifiedProperties();

            var targ = this.target as CameraMovementController;

            var states = this.GetMovementControllerStates();
            var ids = new List<string>();
            ids.Add("None");
            ids.AddRange(DefaultComponentChoiceSelector.GetUniqueComponentNames(states));

            int i = (targ.StartingCameraStyle != null) ? ids.IndexOf(targ.StartingCameraStyle.GetType().Name) : -1;
            if (i < 0) i = 0;

            EditorGUI.BeginChangeCheck();
            i = EditorGUILayout.Popup("Starting Camera Style", i, ids.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                if (i <= 0)
                {
                    targ.StartingCameraStyle = null;
                }
                else
                {
                    targ.StartingCameraStyle = states[i - 1] as ICameraMovementControllerState;
                }

                this.serializedObject.Update();
            }


            if(Application.isPlaying)
            {
                try
                {
                    var nm = ids[states.IndexOf(targ.States.Current) + 1];
                    EditorGUILayout.HelpBox("Currently active state is '" + nm + "'.", MessageType.Info);
                }
                catch
                {

                }
            }

        }


        private Component[] GetMovementControllerStates()
        {
            var targ = this.target as CameraMovementController;
            bool allowChildObjects = this.serializedObject.FindProperty("_allowStatesAsChildren").boolValue;
            
            if(allowChildObjects)
            {
                return com.spacepuppy.StateMachine.ParentComponentStateSupplier<ICameraMovementControllerState>.GetComponentsOnTarg(targ.gameObject, true).Cast<Component>().ToArray();
            }
            else
            {
                return com.spacepuppy.StateMachine.ComponentStateSupplier<ICameraMovementControllerState>.GetComponentsOnTarg(targ.gameObject).Cast<Component>().ToArray();
            }
        }

    }
}
