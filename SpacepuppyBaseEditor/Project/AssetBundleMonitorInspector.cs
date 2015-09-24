using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Project
{

    [CustomEditor(typeof(AssetBundleMonitor))]
    public class AssetBundleMonitorInspector : SPEditor
    {

        #region Menu Entries

        [MenuItem("Assets/Create/AssetBundle Monitor", priority = 1001)]
        private static void CreateResourceMonitor()
        {
            var spath = (Selection.activeGameObject != null) ? AssetDatabase.GetAssetPath(Selection.activeObject) : "Assets/";
            if (!spath.StartsWith("Assets")) return;
            if (Path.HasExtension(spath)) spath = Path.GetDirectoryName(spath);

            if (!spath.EndsWith("/")) spath += "/";
            spath += "AssetBundleMonitor.asset";

            ScriptableObjectHelper.CreateAsset<AssetBundleMonitor>(spath);
        }
        
        #endregion

        #region Inspector

        protected override void OnSPInspectorGUI()
        {
            EditorGUILayout.HelpBox("TODO - must implement building an AssetBundle for the contents of this folder.", MessageType.Info);
        }

        #endregion

    }
}
