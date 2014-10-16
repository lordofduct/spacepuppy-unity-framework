using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace com.spacepuppyeditor
{
    public static class ScriptableObjectHelper
    {

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAsset<T>(UnityEngine.Object relativeTo, string name = null) where T : ScriptableObject
        {
            string path = AssetDatabase.GetAssetPath(relativeTo);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(path), "");
            }
            if (!path.EndsWith("/")) path += "/";

            if (name == null) name = "New " + typeof(T).Name;

            return CreateAsset<T>(path + name + ".asset");
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }

    }
}
