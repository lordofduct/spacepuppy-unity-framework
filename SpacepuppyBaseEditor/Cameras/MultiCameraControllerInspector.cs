using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Cameras;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Cameras
{

    [CustomEditor(typeof(MultiCameraController))]
    public class MultiCameraControllerInspector : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();
            
            this.DrawDefaultInspectorExcept("_defaultCamera");

            this.DrawDefaultCameraField();

            this.serializedObject.ApplyModifiedProperties();
        }

        private void DrawDefaultCameraField()
        {
            var targ = this.target as MultiCameraController;

            var prop = this.serializedObject.FindProperty("_defaultCamera");
            var cameras = (from go in targ.GetAllChildren()
                           let c = go.GetComponent<Camera>()
                           where c != null && c.IsEnabled()
                           select c).ToArray();
            if(cameras.Length > 0)
            {
                var cameraNames = (from c in cameras select c.name).ToArray();
                int index = (prop.objectReferenceValue != null) ? cameras.IndexOf(prop.objectReferenceValue as Camera) : -1;
                index = EditorGUILayout.Popup("Default Camera", index, cameraNames);
                if (index < 0) index = 0;
                prop.objectReferenceValue = cameras[index];
            }
            else
            {
                EditorGUILayout.Popup("Default Camera", -1, new string[0]);
                prop.objectReferenceValue = null;
            }
        }

    }
}
