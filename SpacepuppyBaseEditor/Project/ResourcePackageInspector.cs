using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Project
{

    [CustomEditor(typeof(ResourcePackage), true)]
    public class ResourcePackageInspector : SPEditor
    {

        public const string PROP_RELATIVEPATH = "_relativePath";
        //public const string PROP_RESOURCES = "_resources";
        public const string PROP_PATHS = "_paths";


        #region Menu Entries

        [MenuItem("Assets/Create/Resource Package", priority =1000)]
        private static void CreateResourceMonitor()
        {
            var spath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!spath.StartsWith("Assets")) return;
            if (Path.HasExtension(spath)) spath = Path.GetDirectoryName(spath);

            if (!spath.EndsWith("/")) spath += "/";
            spath += "InventoryResourcePackage.asset";

            ScriptableObjectHelper.CreateAsset<ResourcePackage>(spath);
        }

        [MenuItem("Assets/Create/Resource Package", validate = true)]
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
            this.serializedObject.Update();

            this.DrawDefaultInspectorExcept(PROP_RELATIVEPATH, PROP_PATHS);

            this.DrawEntries();
            
            this.DrawSyncButton();
            
            this.serializedObject.ApplyModifiedProperties();
        }

        protected void DrawEntries()
        {
            EditorGUILayout.LabelField("Relative Path", this.serializedObject.FindProperty(PROP_RELATIVEPATH).stringValue, EditorStyles.textField);

            var pathsProp = this.serializedObject.FindProperty(PROP_PATHS);
            pathsProp.isExpanded = EditorGUILayout.Foldout(pathsProp.isExpanded, "Resources");
            if(pathsProp.isExpanded)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.indentLevel++;
                for (int i = 0; i < pathsProp.arraySize; i++)
                {
                    EditorGUILayout.LabelField(pathsProp.GetArrayElementAtIndex(i).stringValue, EditorStyles.textField);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }

        protected void DrawSyncButton()
        {
            if (this.serializedObject.isEditingMultipleObjects) return;

            var r = EditorGUILayout.GetControlRect();
            var w = r.width * 0.9f;

            r = new Rect(r.xMin + (r.width - w) / 2f, r.yMin, w, r.height);

            if(GUI.Button(r, "Sync Resource Paths"))
            {
                var spath = AssetDatabase.GetAssetPath(this.serializedObject.targetObject);
                var dir = System.IO.Path.GetDirectoryName(spath);

                
                var relPathProp = this.serializedObject.FindProperty(PROP_RELATIVEPATH);
                var pathsProp = this.serializedObject.FindProperty(PROP_PATHS);
                if (!dir.Contains("Resources"))
                {
                    relPathProp.stringValue = string.Empty;
                    pathsProp.arraySize = 0;
                }
                else
                {
                    var relDir = AssetHelper.GetRelativeResourcePath(dir);
                    relPathProp.stringValue = relDir;

                    var paths = (from p in AssetDatabase.GetAllAssetPaths() where p.StartsWith(dir) && p != spath && Path.HasExtension(p) let p2 = AssetHelper.GetRelativeResourcePath(p) orderby p2 select p2).ToArray();
                    pathsProp.arraySize = paths.Length;
                    for(int i = 0; i < paths.Length; i++)
                    {
                        var elProp = pathsProp.GetArrayElementAtIndex(i);
                        elProp.stringValue = paths[i];
                    }
                }

                this.serializedObject.FindProperty(PROP_PATHS).isExpanded = true;
                this.serializedObject.ApplyModifiedProperties();
            }
        }
        
        #endregion

    }

}
