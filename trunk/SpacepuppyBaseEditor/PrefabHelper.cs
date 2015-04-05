using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppyeditor
{

    [InitializeOnLoad()]
    public static class PrefabHelper
    {

        #region Events

        static PrefabHelper()
        {
            EditorSceneEvents.OnPrefabAddedToScene -= OnPrefabAddedToScene;
            EditorSceneEvents.OnPrefabAddedToScene += OnPrefabAddedToScene;
        }

        private static void OnPrefabAddedToScene(GameObject go)
        {
            
            if (Event.current.shift)
            {
                PrefabUtility.DisconnectPrefabInstance(go);
            }

        }

        #endregion

        #region Static Methods

        public static IEnumerable<string> GetAllPrefabAssetPaths()
        {
            return (from p in AssetDatabase.GetAllAssetPaths() where p.EndsWith(".prefab") select p);
        }

        public static IEnumerable<string> GetAllPrefabAssetPathsDependentOn(MonoScript script)
        {
            string scriptPath = AssetDatabase.GetAssetPath(script);
            return (from p in AssetDatabase.GetAllAssetPaths() where p.EndsWith(".prefab") && AssetDatabase.GetDependencies(new string[] { p }).Contains(scriptPath) select p);
        }

        #endregion

    }
}
