#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Project
{

    [CreateAssetMenu(fileName = "AssetSet", menuName = "Spacepuppy/Asset Set")]
    public class QueryableAssetSet : ScriptableObject, IAssetBundle
    {

        #region Fields

        [SerializeField]
        [ReorderableArray()]
        private UnityEngine.Object[] _assets;

        [System.NonSerialized]
        private Dictionary<string, UnityEngine.Object> _table;
        [System.NonSerialized]
        private bool _clean;

        #endregion
        
        #region Methods

        private void SetupTable()
        {
            if (_table == null)
                _table = new Dictionary<string, Object>();
            else
                _table.Clear();

            for (int i = 0; i < _assets.Length; i++)
            {
                _table[_assets[i].name] = _assets[i];
            }
            _clean = true;
        }

        public UnityEngine.Object GetAsset(string name)
        {
            if (!_clean) this.SetupTable();

            UnityEngine.Object obj;
            if (_table.TryGetValue(name, out obj))
                return obj;
            else
                return null;
        }

        public IEnumerable<UnityEngine.Object> GetAllAssets()
        {
            if (!_clean) this.SetupTable();

            return _table.Values;
        }

        public IEnumerable<T> GetAllAssets<T>() where T : class
        {
            if (!_clean) this.SetupTable();

            var e = _table.Values.GetEnumerator();
            while(e.MoveNext())
            {
                var obj = ObjUtil.GetAsFromSource<T>(e.Current);
                if(!object.ReferenceEquals(obj, null))
                {
                    yield return obj;
                }
            }
        }

        /// <summary>
        /// Replaces the internal collection with a new set of assets.
        /// </summary>
        /// <param name="assets"></param>
        public void ResetAssets(IEnumerable<UnityEngine.Object> assets)
        {
            _assets = assets.ToArray();
            if(_table != null)
            {
                _table.Clear();
                this.SetupTable();
            }
        }

        #endregion

        #region IAssetBundle Interface

        public bool Contains(string name)
        {
            if (!_clean) this.SetupTable();

            return _table.ContainsKey(name);
        }

        public bool Contains(UnityEngine.Object asset)
        {
            return System.Array.IndexOf(_assets, asset) >= 0;
        }

        public IEnumerable<string> GetAllAssetNames()
        {
            if (!_clean) this.SetupTable();

            return _table.Keys;
        }

        UnityEngine.Object IAssetBundle.LoadAsset(string name)
        {
            return this.GetAsset(name);
        }

        UnityEngine.Object IAssetBundle.LoadAsset(string name, System.Type tp)
        {
            var obj = this.GetAsset(name);
            if (object.ReferenceEquals(obj, null)) return null;

            if (TypeUtil.IsType(obj.GetType(), tp)) return obj;
            else return null;
        }

        T IAssetBundle.LoadAsset<T>(string name)
        {
            return this.GetAsset(name) as T;
        }

        public void UnloadAllAssets()
        {
            if (_table != null) _table.Clear();
            for(int i = 0; i < _assets.Length; i++)
            {
                if(!(_assets[i] is GameObject))
                {
                    Resources.UnloadAsset(_assets[i]);
                }
            }
            _clean = false;
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            if(this.Contains(asset))
            {
                Resources.UnloadAsset(asset);
            }
        }



        public void Dispose()
        {
            Resources.UnloadAsset(this);
        }

        #endregion

    }
}
