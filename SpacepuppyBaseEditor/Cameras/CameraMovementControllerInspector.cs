using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Cameras;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Cameras
{

    [CustomEditor(typeof(CameraMovementController), true)]
    public class CameraMovementControllerInspector : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();
            this.DrawDefaultInspectorExcept("_startingCameraStyle");
            this.serializedObject.ApplyModifiedProperties();

            var targ = this.target as CameraMovementController;

            var ids = new List<string>();
            ids.Add("None");
            ids.AddRange(from c in targ.GetComponents<ICameraMovementControllerState>() select c.GetType().Name);

            int i = (targ.StartingCameraStyle != null) ? ids.IndexOf(targ.StartingCameraStyle.GetType().Name) : -1;
            if (i < 0) i = 0;

            EditorGUI.BeginChangeCheck();
            i = EditorGUILayout.Popup("Starting Camera Style", i, ids.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                if (i < 0) i = 0;

                if (i == 0)
                {
                    targ.StartingCameraStyle = null;
                }
                else
                {
                    var id = ids[i];
                    targ.StartingCameraStyle = (from c in targ.GetComponents<ICameraMovementControllerState>() where c.GetType().Name == id select c).FirstOrDefault();
                }

                this.serializedObject.Update();
            }

        }

    }
}
