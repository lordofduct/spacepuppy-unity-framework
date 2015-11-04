using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace com.spacepuppy.Project
{
    public static class AssetBundles
    {

        #region Fields
        
        #endregion

        #region Properties

        /// <summary>
        /// A reference to an SPAssetBundle that wraps around the 'Resources' class so it can treated similar to an AssetBundle.
        /// </summary>
        public static ResourcesAssetBundle Resources { get { return ResourcesAssetBundle.Instance; } }

        #endregion

        #region Methods

        public static IAssetBundle LoadFromFile(string path)
        {
            return ExtractBundleMonitor(AssetBundle.CreateFromFile(path));
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

        #endregion

    }
}
