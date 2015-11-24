using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

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

        public static IEnumerable<string> GetAllPrefabAssetPathsDependantOn(System.Type tp)
        {
            foreach(var p in AssetDatabase.GetAllAssetPaths())
            {
                if(p.EndsWith(".prefab"))
                {
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                    if (PrefabHasComponent(go, tp)) yield return p;
                }
            }
        }




        private static bool PrefabHasComponent(GameObject prefab, System.Type tp)
        {
            Component c = prefab.GetComponent(tp);
            if (!object.ReferenceEquals(c, null)) return true;

            foreach(var child in GameObjectUtil.GetAllChildren(prefab))
            {
                c = child.GetComponent(tp);
                if (!object.ReferenceEquals(c, null)) return true;
            }

            return false;
        }

        #endregion

    }
}
