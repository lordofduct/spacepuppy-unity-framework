using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace com.spacepuppy.Project
{
    public class ResourceMonitor : ScriptableObject, IAssetBundle
    {

        #region Fields

        [SerializeField()]
        private string[] _paths;

        #endregion

        #region Fields

        /// <summary>
        /// Should only ever be used for fast access to READ the paths. Modifying this array modifies the underlying data.
        /// </summary>
        public string[] UnsafePaths { get { return _paths; } }

        #endregion

        #region Methods

        public string[] GetAllAssetPaths()
        {
            return _paths.Clone() as string[];
        }

        public bool ContainsPath(string path)
        {
            return System.Array.IndexOf(_paths, path) >= 0;
        }

        public IEnumerable<string> GetAssetPathsIn(string folderPath)
        {
            if (_paths == null) yield break;

            for(int i = 0; i < _paths.Length; i++)
            {
                if (_paths[i].StartsWith(folderPath)) yield return _paths[i];
            }
        }

        #endregion

        #region IAssetBundle Interface

        bool IAssetBundle.Contains(string path)
        {
            return this.ContainsPath(path);
        }

        bool IAssetBundle.Contains(UnityEngine.Object asset)
        {
            return AssetBundleManager.Resources.Contains(asset);
        }

        public UnityEngine.Object LoadAsset(string path)
        {
            if (!this.ContainsPath(path)) return null;
            return AssetBundleManager.Resources.LoadAsset(path);
        }

        public UnityEngine.Object LoadAsset(string path, Type tp)
        {
            if (!this.ContainsPath(path)) return null;
            return AssetBundleManager.Resources.LoadAsset(path, tp);
        }

        public T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            if (!this.ContainsPath(path)) return null;
            return AssetBundleManager.Resources.LoadAsset<T>(path);
        }

        void IAssetBundle.UnloadAsset(UnityEngine.Object asset)
        {
            AssetBundleManager.Resources.UnloadAsset(asset);
        }

        void IAssetBundle.UnloadAllAssets()
        {
            AssetBundleManager.Resources.UnloadAllAssets();
        }

        #endregion

    }
}
