using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Project
{

    [CustomEditor(typeof(ResourceMonitor))]
    public class ResourceMonitorInspector : SPEditor
    {

        #region Menu Entries

        [MenuItem("Assets/Create/Resource Monitor", priority =1000)]
        private static void CreateResourceMonitor()
        {
            var spath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!spath.StartsWith("Assets")) return;
            if (Path.HasExtension(spath)) spath = Path.GetDirectoryName(spath);

            if (!spath.EndsWith("/")) spath += "/";
            spath += "ResourceMonitor.asset";

            ScriptableObjectHelper.CreateAsset<ResourceMonitor>(spath);
        }

        [MenuItem("Assets/Create/Resource Monitor", validate = true)]
        private static bool ValidateCreateResourceMonitor()
        {
            var spath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (spath == null) return false;
            return spath.Contains("Resources");
        }

        #endregion

        #region Inspector

        protected override void OnSPInspectorGUI()
        {
            this.DrawSyncButton();

            var pathsProp = this.serializedObject.FindProperty("_paths");

            EditorGUILayout.BeginVertical("Box");
            for(int i = 0; i < pathsProp.arraySize; i++)
            {
                EditorGUILayout.LabelField(pathsProp.GetArrayElementAtIndex(i).stringValue);
            }
            EditorGUILayout.EndVertical();
        }


        private void DrawSyncButton()
        {
            if (this.serializedObject.isEditingMultipleObjects) return;

            var r = EditorGUILayout.GetControlRect();
            var w = r.width * 0.9f;

            r = new Rect(r.xMin + (r.width - w) / 2f, r.yMin, w, r.height);

            if(GUI.Button(r, "Sync Resource Paths"))
            {
                var spath = AssetDatabase.GetAssetPath(this.serializedObject.targetObject);
                var dir = System.IO.Path.GetDirectoryName(spath);
                if(!dir.Contains("Resources"))
                {
                    var pathsProp = this.serializedObject.FindProperty("_paths");
                    pathsProp.arraySize = 0;
                }
                else
                {
                    var pathsProp = this.serializedObject.FindProperty("_paths");
                    var paths = (from p in AssetDatabase.GetAllAssetPaths() where p.StartsWith(dir) && Path.HasExtension(p) let p2 = ConvertFullPathToResourcePath(p) orderby p2 select p2).ToArray();
                    pathsProp.arraySize = paths.Length;
                    for (int i = 0; i < paths.Length; i++)
                    {
                        pathsProp.GetArrayElementAtIndex(i).stringValue = paths[i];
                    }
                }

                this.serializedObject.ApplyModifiedProperties();
            }
        }

        private static string ConvertFullPathToResourcePath(string spath)
        {
            int i = spath.IndexOf("Resources") + 10;
            spath = spath.Substring(i);
            return Path.Combine(Path.GetDirectoryName(spath), Path.GetFileNameWithoutExtension(spath)).Replace(@"\", "/");
        }

        #endregion

    }

}
