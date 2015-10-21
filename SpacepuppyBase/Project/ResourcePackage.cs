using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Project
{

    /// <summary>
    /// A ScriptableObject that can be placed inside a 'Resources' folder and records the various assets in that folder. 
    /// When loaded it can be used to index the contents of that folder as a bundle of asssets. Facilitating the IAssetBundle 
    /// interface, and any factories that may use it.
    /// </summary>
    public class ResourcePackage : ScriptableObject, IAssetBundle
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
        
        /// <summary>
        /// ResourcePackage names are actually paths in the Resources hierarchy, this will return those asset names/paths 
        /// that exist in some specific folder.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
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

        /// <summary>
        /// ResourcePackage names are full paths.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllAssetNames()
        {
            return _paths;
        }

        public bool Contains(string path)
        {
            return System.Array.IndexOf(_paths, path) >= 0;
        }

        bool IAssetBundle.Contains(UnityEngine.Object asset)
        {
            return AssetBundleManager.Resources.Contains(asset);
        }

        public UnityEngine.Object LoadAsset(string path)
        {
            if (!this.Contains(path)) return null;
            return AssetBundleManager.Resources.LoadAsset(path);
        }

        public UnityEngine.Object LoadAsset(string path, System.Type tp)
        {
            if (!this.Contains(path)) return null;
            return AssetBundleManager.Resources.LoadAsset(path, tp);
        }

        public T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            if (!this.Contains(path)) return null;
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
