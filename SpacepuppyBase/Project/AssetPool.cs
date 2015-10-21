using System.Collections.Generic;
using System.Collections;


using com.spacepuppy.Utils;

namespace com.spacepuppy.Project
{

    /// <summary>
    /// Represents an ID table for an IAssetBundle. This way you can associate specific ids/names with the long form path name in the IAssetBundle. 
    /// As you load Assets they'll be recorded inside the Pool for easy unloading when needed. This works great in factory patterns that 
    /// follow strict naming mechanics, but need to work with different unique AssetBundles that might have conflicting path names to the id 
    /// required by the factory.
    /// </summary>
    public class AssetPool : IEnumerable<AssetPool.ResourceEntry>
    {

        #region Fields

        private IAssetBundle _bundle;
        private Dictionary<string, ResourceEntry> _table = new Dictionary<string, ResourceEntry>();
        private AssetCollection _assetColl;

        #endregion

        #region CONSTRUCTOR

        public AssetPool()
        {
            _bundle = AssetBundleManager.Resources;
        }

        public AssetPool(IAssetBundle bundle)
        {
            _bundle = bundle ?? AssetBundleManager.Resources;
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return _table.Count;
            }
        }
        
        public ResourceEntry this[string id]
        {
            get
            {
                ResourceEntry entry;
                if (_table.TryGetValue(id, out entry))
                {
                    return entry;
                }
                return null;
            }
        }

        /// <summary>
        /// Direct access to the assets, accessing any asset through this collection will automatically load it if not already loaded.
        /// </summary>
        public AssetCollection Assets
        {
            get
            {
                if (_assetColl == null)
                    _assetColl = new AssetCollection(this);
                return _assetColl;
            }
        }

        #endregion

        #region Methods

        public void Add(string id, string path)
        {
            if (id == null) throw new System.ArgumentNullException("id");
            _table.Add(id, new ResourceEntry(_bundle, id, path));
        }

        public bool Contains(string id)
        {
            return _table.ContainsKey(id);
        }

        public bool Remove(string id)
        {
            ResourceEntry entry;
            if(_table.TryGetValue(id, out entry))
            {
                entry.Unload();
                _table.Remove(id);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            this.UnloadAll();
            _table.Clear();
        }


        public bool TryGetEntry(string key, out ResourceEntry value)
        {
            return _table.TryGetValue(key, out value);
        }

        public void LoadAll()
        {
            var e = _table.Values.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Load();
            }
        }

        public void UnloadAll()
        {
            var e = _table.Values.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Unload();
            }
        }

        public bool AnyLoaded()
        {
            var e = _table.Values.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.Loaded) return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable Interface

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<ResourceEntry> IEnumerable<ResourceEntry>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Special Types

        public class ResourceEntry
        {

            private IAssetBundle _bundle;
            private string _id;
            private string _path;
            private UnityEngine.Object _asset;
            private bool _loaded;

            internal ResourceEntry(IAssetBundle bundle, string id, string path)
            {
                _bundle = bundle;
                _id = id;
                _path = path;
            }

            public string Id { get { return _id; } }

            public string Path { get { return _path; } }

            public UnityEngine.Object Asset { get { return _asset; } }

            public bool Loaded { get { return _loaded; } }



            public void Load()
            {
                _loaded = true;
                if (_asset == null)
                {
                    _asset = _bundle.LoadAsset(_path);
                }
            }

            public void Unload()
            {
                _loaded = false;
                if (_asset != null)
                {
                    _bundle.UnloadAsset(_asset);
                    _asset = null;
                }
            }

        }
        
        public struct Enumerator : IEnumerator<ResourceEntry>
        {

            private AssetPool _coll;
            private Dictionary<string, ResourceEntry>.Enumerator _e;

            internal Enumerator(AssetPool coll)
            {
                if (coll == null) throw new System.ArgumentNullException("coll");
                _coll = coll;
                _e = _coll._table.GetEnumerator();
            }

            public ResourceEntry Current
            {
                get
                {
                    return _e.Current.Value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _e.Current.Value;
                }
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void IEnumerator.Reset()
            {
                (_e as IEnumerator).Reset();
            }
        }

        public class AssetCollection : IEnumerable<UnityEngine.Object>
        {

            #region Fields

            private AssetPool _source;

            #endregion

            #region CONSTRUCTOR

            internal AssetCollection(AssetPool source)
            {
                if (source == null) throw new System.ArgumentNullException("source");
                _source = source;
            }

            #endregion

            #region Properties

            public UnityEngine.Object this[string id]
            {
                get
                {
                    ResourceEntry entry;
                    if (_source._table.TryGetValue(id, out entry))
                    {
                        if (!entry.Loaded) entry.Load();
                        return entry.Asset;
                    }

                    return null;
                }
            }

            public AssetPool Source
            {
                get { return _source; }
            }

            #endregion

            #region Methods

            public bool TryGetAsset(string id, out UnityEngine.Object asset)
            {
                ResourceEntry entry;
                if (_source._table.TryGetValue(id, out entry))
                {
                    if (!entry.Loaded) entry.Load();
                    asset = entry.Asset;
                    return asset != null;
                }

                asset = null;
                return false;
            }

            public bool TryGetAsset<T>(string id, out T asset) where T : class
            {
                ResourceEntry entry;
                if (_source._table.TryGetValue(id, out entry))
                {
                    if(entry.Loaded)
                    {
                        asset = ObjUtil.GetAsFromSource<T>(entry.Asset);
                        return asset != null;
                    }
                    else
                    {
                        entry.Load();
                        asset = ObjUtil.GetAsFromSource<T>(entry.Asset);
                        if(asset == null)
                        {
                            entry.Unload();
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                asset = null;
                return false;
            }
            
            #endregion


            #region IEnumerable Interface

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_source);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_source);
            }

            IEnumerator<UnityEngine.Object> IEnumerable<UnityEngine.Object>.GetEnumerator()
            {
                return new Enumerator(_source);
            }

            #endregion

            #region Special AssetCollection Types

            public struct Enumerator : IEnumerator<UnityEngine.Object>
            {
                
                private AssetPool _source;
                private Dictionary<string, ResourceEntry>.Enumerator _e;
                
                internal Enumerator(AssetPool coll)
                {
                    if (coll == null) throw new System.ArgumentNullException("coll");
                    _source = coll;
                    _e = coll._table.GetEnumerator();
                }


                public UnityEngine.Object Current
                {
                    get
                    {
                        var entry = _e.Current.Value;
                        if(entry != null)
                        {
                            if (!entry.Loaded) entry.Load();
                            return entry.Asset;
                        }
                        return null;
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return this.Current;
                    }
                }

                public bool MoveNext()
                {
                    return _e.MoveNext();
                }

                public void Dispose()
                {
                    _e.Dispose();
                }

                void IEnumerator.Reset()
                {
                    (_e as IEnumerator).Reset();
                }

            }

            #endregion

        }

        #endregion

    }
}
