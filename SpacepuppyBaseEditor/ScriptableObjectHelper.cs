using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using com.spacepuppy.Utils;

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
            if (StringUtil.IsNullOrWhitespace(path)) throw new System.ArgumentException("Path must not be null or whitespace.", "path");
            //make sure folder exists
            CreateFolderIfNotExist(System.IO.Path.GetDirectoryName(path));
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static ScriptableObject CreateAsset(System.Type tp, string path)
        {
            if (StringUtil.IsNullOrWhitespace(path)) throw new System.ArgumentException("Path must not be null or whitespace.", "path");
            //make sure folder exists
            CreateFolderIfNotExist(System.IO.Path.GetDirectoryName(path));
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            var asset = ScriptableObject.CreateInstance(tp);
            AssetDatabase.CreateAsset(asset, path);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }

        public static bool FolderExists(string path)
        {
            if (string.IsNullOrEmpty(path)) return true;
            return Directory.Exists(Application.dataPath + "/" + path.EnsureNotStartWith("Assets").EnsureNotStartWith("/"));
        }

        public static void CreateFolderIfNotExist(string folder)
        {
            if (string.IsNullOrEmpty(folder)) return;
            if (!FolderExists(folder))
            {
                AssetDatabase.CreateFolder(Path.GetDirectoryName(folder), Path.GetFileName(folder));
            }
        }

    }
}
