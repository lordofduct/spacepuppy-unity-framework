using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace com.spacepuppy.Project
{
    public sealed class AssetBundleManager : IEnumerable<IAssetBundle>
    {

        #region CONSTRUCTOR

        private AssetBundleManager()
        {
            //Singleton enforced
        }

        #endregion

        #region Properties

        public int Count { get { return _bundles.Count + 1; } }

        #endregion

        #region Methods

        public bool Contains(IAssetBundle bundle)
        {
            if (bundle is AssetBundlePackage) return _bundles.Contains(bundle as AssetBundlePackage);
            if (bundle == AssetBundleManager.Resources) return true;

            return false;
        }

        public void UnloadAll()
        {
            AssetBundleManager.Resources.UnloadAllAssets();

            if(_bundles.Count > 0)
            {
                using (var lst = com.spacepuppy.Collections.TempCollection.GetList<AssetBundlePackage>())
                {
                    var e = _bundles.GetEnumerator();
                    while(e.MoveNext())
                    {
                        lst.Add(e.Current);
                    }

                    var e2 = lst.GetEnumerator();
                    while(e2.MoveNext())
                    {
                        e2.Current.Dispose(true);
                    }
                }
            }
        }

        #endregion

        #region IEnumerable Interface

        public Enumerator GetEnumerator()
        {
            return new Enumerator();
        }

        IEnumerator<IAssetBundle> IEnumerable<IAssetBundle>.GetEnumerator()
        {
            return new Enumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator();
        }

        #endregion

        #region Static Interface

        private static AssetBundleManager _instance;
        private static HashSet<AssetBundlePackage> _bundles = new HashSet<AssetBundlePackage>(new AssetBundlePackageEqualityComparer());
        
        public static AssetBundleManager Bundles
        {
            get
            {
                if (_instance == null) _instance = new AssetBundleManager();
                return _instance;
            }
        }

        /// <summary>
        /// A reference to an SPAssetBundle that wraps around the 'Resources' class so it can treated similar to an AssetBundle.
        /// </summary>
        public static ResourcesAssetBundle Resources { get { return ResourcesAssetBundle.Instance; } }



        public static IAssetBundle LoadFromFile(string path)
        {
            var bundle = ExtractBundleMonitor(AssetBundle.CreateFromFile(path));
            if (bundle == null) return null;
            _bundles.Add(bundle);
            return bundle;
        }

        /// <summary>
        /// Creates an SPAssetBundle from a AssetBundle loaded in a manner other than the SPAssetBundle factory methods. 
        /// Note, now that it's managed, allow SPAssetBundle to handle the AssetBundle directly, and only load/unload via the SPAssetBundle interface.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public static IAssetBundle Create(AssetBundle bundle)
        {
            return ExtractBundleMonitor(bundle);
        }

        public static void UnloadLoadedBundle(IAssetBundle bundle, bool unloadAllLoadedObjects)
        {
            if (!(bundle is AssetBundlePackage)) return;
            (bundle as AssetBundlePackage).Dispose(unloadAllLoadedObjects);
        }

        internal static void RemoveAssetBundle(AssetBundlePackage bundle)
        {
            _bundles.Remove(bundle);
        }

        private static AssetBundlePackage ExtractBundleMonitor(AssetBundle bundle)
        {
            if (bundle.mainAsset is AssetBundlePackage)
            {
                var spbundle = bundle.mainAsset as AssetBundlePackage;
                spbundle.Init(bundle);
                return spbundle;
            }
            else
            {
                var spbundle = ScriptableObject.CreateInstance<AssetBundlePackage>();
                spbundle.Init(bundle);
                return spbundle;
            }
        }
        
        #endregion
        
        #region Special Types

        private class AssetBundlePackageEqualityComparer : IEqualityComparer<AssetBundlePackage>
        {
            public bool Equals(AssetBundlePackage x, AssetBundlePackage y)
            {
                return x.UniqueId == y.UniqueId;
            }

            public int GetHashCode(AssetBundlePackage obj)
            {
                return obj.UniqueId;
            }
        }

        public struct Enumerator : IEnumerator<IAssetBundle>
        {

            private HashSet<AssetBundlePackage>.Enumerator _e;
            private int _state;
            
            public IAssetBundle Current
            {
                get
                {
                    switch(_state)
                    {
                        case 0:
                            return null;
                        case 1:
                            return AssetBundleManager.Resources;
                        case 2:
                            return _e.Current;
                        default:
                            return null;
                    }
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Dispose()
            {
                _e.Dispose();
                _state = 3;
            }

            public bool MoveNext()
            {
                switch(_state)
                {
                    case 0:
                        _state = 1;
                        return true;
                    case 1:
                        _state = 2;
                        _e = AssetBundleManager._bundles.GetEnumerator();
                        return _e.MoveNext();
                    case 2:
                        if(_e.MoveNext())
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

            void IEnumerator.Reset()
            {
                throw new System.NotSupportedException();
            }
        }

        #endregion


    }
}
