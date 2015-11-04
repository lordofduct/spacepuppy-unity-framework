using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

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
        private string _relativePath;

        [SerializeField()]
        private string[] _paths;

        [System.NonSerialized()]
        private string[] _relativePaths;

        #endregion

        #region Fields

        public string RelativePath
        {
            get { return _relativePath; }
        }
        
        public string[] UnsafePaths { get { return _paths; } }

        #endregion

        #region Methods

        public IEnumerable<string> GetRelativePaths()
        {
            if (_paths == null) return ArrayUtil.Empty<string>();
            if (string.IsNullOrEmpty(_relativePath))
                return _paths;

            if(_relativePaths == null)
            {
                _relativePaths = new string[_paths.Length];
                for (int i = 0; i < _paths.Length; i++)
                {
                    _relativePaths[i] = _paths[i].EnsureNotStartWith(_relativePath).EnsureNotStartWith("/");
                }
            }
            return _relativePaths;
        }

        #endregion


        #region IAssetBundle Interface

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
            //there's really no way to test this...
            return true;
        }

        public UnityEngine.Object LoadAsset(string path)
        {
            //if (!this.Contains(path)) return null;
            return AssetBundles.Resources.LoadAsset(path);
        }

        public UnityEngine.Object LoadAsset(string path, System.Type tp)
        {
            //if (!this.Contains(path)) return null;
            return AssetBundles.Resources.LoadAsset(path, tp);
        }

        public T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            //if (!this.Contains(path)) return null;
            return AssetBundles.Resources.LoadAsset<T>(path);
        }

        void IAssetBundle.UnloadAsset(UnityEngine.Object asset)
        {
            AssetBundles.Resources.UnloadAsset(asset);
        }

        void IAssetBundle.UnloadAllAssets()
        {
            AssetBundles.Resources.UnloadAllAssets();
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            (this as IAssetBundle).UnloadAllAssets();
            //UnityEngine.Object.Destroy(this);
        }

        #endregion
        
    }
}
