using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Project
{

    /// <summary>
    /// An interface that represents a bundle of resources that can be loaded on demand. This facilitates wrappers 
    /// around the global 'Resources' (see: ResourceAssetBundle), portions of 'Resources' (see: ResourceMonitor), 
    /// 'AssetBundle' (see: AssetBundleMonitor), as well as groups of bundles (see: AssetBundleGroup).
    /// </summary>
    public interface IAssetBundle
    {
        
        bool Contains(string name);

        bool Contains(UnityEngine.Object asset);

        UnityEngine.Object LoadAsset(string name);
        UnityEngine.Object LoadAsset(string name, System.Type tp);

        T LoadAsset<T>(string name) where T : UnityEngine.Object;

        void UnloadAsset(UnityEngine.Object asset);
        void UnloadAllAssets();

    }

    /// <summary>
    /// A wrapper around the global 'Resources' class so it can be used as an IAssetBundle.
    /// </summary>
    public sealed class ResourcesAssetBundle : IAssetBundle
    {

        #region Singleton Interface

        private static ResourcesAssetBundle _instance;
        public static ResourcesAssetBundle Instance
        {
            get
            {
                if (_instance == null) _instance = new ResourcesAssetBundle();
                return _instance;
            }
        }

        #endregion

        #region Fields

        #endregion

        #region CONSTRUCTOR

        private ResourcesAssetBundle()
        {
            //enforce as singleton
        }

        #endregion

        #region Methods
        
        public bool Contains(string path)
        {
            //there's no way to test it, so we assume true
            return true;
        }

        public bool Contains(UnityEngine.Object asset)
        {
            //there's no way to test it, so we assume true
            return true;
        }

        public UnityEngine.Object LoadAsset(string path)
        {
            return Resources.Load(path);
        }

        public UnityEngine.Object LoadAsset(string path, System.Type tp)
        {
            return Resources.Load(path, tp);
        }

        public T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load(path, typeof(T)) as T;
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            Resources.UnloadAsset(asset);
        }

        public void UnloadAllAssets()
        {
            //technically this doesn't act the same as LoadedAssetBundle, it only unloads ununsed assets
            Resources.UnloadUnusedAssets();
        }

        #endregion

        #region Equality Overrides

        public override int GetHashCode()
        {
            return 1;
        }

        #endregion

    }

    /// <summary>
    /// A collection of IAssetBundles that can be treated as one. Good for merging bundles together for factory patterns. 
    /// A good example is a game with downloadable content that expands the available items in the game. The factory 
    /// receives a group of IAssetBundles as this object.
    /// </summary>
    public sealed class AssetBundleGroup : IAssetBundle, ICollection<IAssetBundle>
    {

        #region Fields

        private ResourcesAssetBundle _resources;
        private HashSet<IAssetBundle> _bundles = new HashSet<IAssetBundle>();

        #endregion

        #region CONSTRUCTOR

        public AssetBundleGroup()
        {

        }

        public AssetBundleGroup(IEnumerable<IAssetBundle> e)
        {
            foreach(var b in e)
            {
                this.Add(b);
            }
        }

        #endregion

        #region Methods

        public bool ContainsDeep(IAssetBundle item)
        {
            if (item == null) return false;
            if (item == _resources) return true;
            
            if(_bundles.Count > 0)
            {
                var e = _bundles.GetEnumerator();
                while(e.MoveNext())
                {
                    if (e.Current is AssetBundleGroup && (e.Current as AssetBundleGroup).ContainsDeep(item)) return true;
                }
            }

            return false;
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get
            {
                if (_resources != null)
                    return _bundles.Count + 1;
                else
                    return _bundles.Count;
            }
        }

        bool ICollection<IAssetBundle>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(IAssetBundle bundle)
        {
            if (bundle == null) throw new System.ArgumentNullException("bundle");
            if (bundle is AssetBundleGroup)
            {
                if (bundle == this || (bundle as AssetBundleGroup).ContainsDeep(this)) throw new System.ArgumentException("Bundle being added to group would result in a circular relationship.");
            }
            else if (bundle is ResourcesAssetBundle)
            {
                _resources = bundle as ResourcesAssetBundle;
            }
            else
            {
                _bundles.Add(bundle);
            }
        }

        public void Clear()
        {
            _resources = null;
            _bundles.Clear();
        }

        public bool Contains(IAssetBundle item)
        {
            if (item == null) return false;
            if (item == _resources) return true;
            return _bundles.Contains(item);
        }

        public void CopyTo(IAssetBundle[] array, int arrayIndex)
        {
            if (array == null) throw new System.ArgumentNullException("array");
            if (arrayIndex < 0) throw new System.ArgumentOutOfRangeException("arrayIndex");
            if (array.Length - arrayIndex < this.Count) throw new System.ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.", "array");

            if (_resources != null)
            {
                array[arrayIndex] = _resources;
                arrayIndex++;
            }
            
            if(_bundles.Count > 0)
            {
                var e = _bundles.GetEnumerator();
                while(e.MoveNext())
                {
                    array[arrayIndex] = e.Current;
                    arrayIndex++;
                }
            }
        }

        public bool Remove(IAssetBundle item)
        {
            if (item == null) return false;
            if (item == _resources)
            {
                _resources = null;
                return true;
            }

            return _bundles.Remove(item);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        
        IEnumerator<IAssetBundle> IEnumerable<IAssetBundle>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region SPAssetBundle Interface

        public bool Contains(string name)
        {
            //NOTE - this is only because Resources can't be tested, if we find a way to test contains, this should be changed
            if (_resources != null) return true;
            if (_bundles.Count == 0) return false;

            var e = _bundles.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.Contains(name)) return true;
            }

            return false;
        }

        public bool Contains(UnityEngine.Object asset)
        {
            //NOTE - this is only because Resources can't be tested, if we find a way to test contains, this should be changed
            if (_resources != null) return true;
            if (_bundles.Count == 0) return false;

            var e = _bundles.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Contains(asset)) return true;
            }

            return false;
        }

        public UnityEngine.Object LoadAsset(string name)
        {
            if(_bundles.Count > 0)
            {
                var e = _bundles.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Contains(name)) return e.Current.LoadAsset(name);
                }
            }

            if(_resources != null)
            {
                return _resources.LoadAsset(name);
            }

            return null;
        }

        public UnityEngine.Object LoadAsset(string name, System.Type tp)
        {
            if (_bundles.Count > 0)
            {
                var e = _bundles.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Contains(name)) return e.Current.LoadAsset(name, tp);
                }
            }

            if (_resources != null)
            {
                return _resources.LoadAsset(name, tp);
            }

            return null;
        }

        public T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            return this.LoadAsset(name, typeof(T)) as T;
        }

        public void UnloadAllAssets()
        {
            if (_resources != null) _resources.UnloadAllAssets();

            if (_bundles.Count > 0)
            {
                var e = _bundles.GetEnumerator();
                while(e.MoveNext())
                {
                    e.Current.UnloadAllAssets();
                }
            }
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (asset == null) return;

            if (_bundles.Count > 0)
            {
                var e = _bundles.GetEnumerator();
                while (e.MoveNext())
                {
                    if(e.Current.Contains(asset))
                    {
                        e.Current.UnloadAsset(asset);
                        return;
                    }
                }
            }

            if(_resources != null)
            {
                _resources.UnloadAsset(asset);
            }
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<IAssetBundle>
        {

            private AssetBundleGroup _bundle;
            private HashSet<IAssetBundle>.Enumerator _e;
            private int _state;

            internal Enumerator(AssetBundleGroup coll)
            {
                if (coll == null) throw new System.ArgumentNullException("coll");
                _bundle = coll;
                _e = default(HashSet<IAssetBundle>.Enumerator);
                _state = 0;
            }


            public IAssetBundle Current
            {
                get
                {
                    if (_bundle == null) return null;

                    switch(_state)
                    {
                        case 0:
                            return null;
                        case 1:
                            return _bundle._resources;
                        case 2:
                            return _e.Current;
                        default:
                            return null;
                    }
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Dispose()
            {
                _bundle = null;
                _e.Dispose();
                _state = 3;
            }

            public bool MoveNext()
            {
                if (_bundle == null) return false;

                switch (_state)
                {
                    case 0:
                        if(_bundle._resources == null)
                        {
                            _state = 2;
                            _e = _bundle._bundles.GetEnumerator();
                            return _e.MoveNext();
                        }
                        else
                        {
                            _state = 1;
                            return true;
                        }
                    case 1:
                        _state = 2;
                        _e = _bundle._bundles.GetEnumerator();
                        return _e.MoveNext();
                    case 2:
                        if (_e.MoveNext())
                        {
                            return true;
                        }
                        else
                        {
                            _state = 3;
                            return false;
                        }
                    default:
                        return false;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                throw new System.NotSupportedException();
            }
        }

        #endregion

    }

}
