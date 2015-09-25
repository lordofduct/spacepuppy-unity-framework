using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Project
{

    /// <summary>
    /// Include as the mainAsset of an AssetBundle to facilitate indexing the contents of that bundle for use as an IAssetBundle.
    /// 
    /// In Unity 4.x and earlier, AssetBundle is rather gimped, and needs a little extra oomph to help index its contents. Unity 5 
    /// comes with extra tools to make that easier. This class can still benefit you in Unity5 though, since it implements IAssetBundle 
    /// for use in factory patterns that use IAssetBundles.
    /// </summary>
    public class AssetBundlePackage : ScriptableObject, IAssetBundle, System.IDisposable
    {

        #region Fields

        [SerializeField()]
        private string[] _names;

        [System.NonSerialized()]
        private AssetBundle _bundle;
        [System.NonSerialized()]
        private HashSet<UnityEngine.Object> _loadedAssets = new HashSet<UnityEngine.Object>(com.spacepuppy.Collections.ObjectInstanceIDEqualityComparer<UnityEngine.Object>.Default);

        #endregion

        #region CONSTRUCTOR

        internal void Init(AssetBundle bundle)
        {
            //should only ever be called by SPAssetBundle factory methods
            _bundle = bundle;
        }

        void OnDisable()
        {
            this.Dispose(true);
        }

        #endregion

        #region Properties

        public int UniqueId
        {
            get
            {
                if (_bundle == null) return this.GetInstanceID();

                return _bundle.GetInstanceID();
            }
        }

        public AssetBundle AssetBundle { get { return _bundle; } }

        #endregion

        #region Methods

        public IEnumerable<string> GetAllAssetNames()
        {
            return _names ?? Enumerable.Empty<string>();
        }

        public bool Contains(string name)
        {
            if (_bundle == null) return false;
            return _bundle.Contains(name);
        }

        public bool Contains(UnityEngine.Object asset)
        {
            return _loadedAssets.Contains(asset);
        }

        public UnityEngine.Object LoadAsset(string name)
        {
            if (_bundle == null) return null;
            var asset = _bundle.Load(name);
            if (asset == null) return null;
            _loadedAssets.Add(asset);
            return asset;
        }

        public UnityEngine.Object LoadAsset(string name, System.Type tp)
        {
            if (_bundle == null) return null;
            var asset = _bundle.Load(name, tp);
            if (asset == null) return null;
            _loadedAssets.Add(asset);
            return asset;
        }

        public T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            return this.LoadAsset(name, typeof(T)) as T;
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (_bundle == null) return;

            _loadedAssets.Remove(asset);
            UnityEngine.Object.Destroy(asset);
        }

        public void UnloadAllAssets()
        {
            if (_loadedAssets.Count == 0) return;

            var e = _loadedAssets.GetEnumerator();
            while (e.MoveNext())
            {
                UnityEngine.Object.Destroy(e.Current);
            }
            _loadedAssets.Clear();
        }

        #endregion

        #region Equality Overrides

        public override int GetHashCode()
        {
            return this.UniqueId;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose(bool unloadAllLoadedObjects)
        {
            if (_bundle != null)
            {
                _bundle.Unload(unloadAllLoadedObjects);
                AssetBundleManager.RemoveAssetBundle(this);
                _bundle = null;
            }
        }

        void System.IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        #endregion

    }
}
