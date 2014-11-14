using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [AddComponentMenu("SpacePuppy/Spawn/Spawn Pool")]
    public class SpawnPool : SPNotifyingComponent, ISerializationCallbackReceiver
    {

        #region Static Multiton Interface

        private static SpawnPool _defaultPool;
        private static UniqueList<SpawnPool> _pools = new UniqueList<SpawnPool>(ObjectReferenceEqualityComparer<SpawnPool>.Default);

        public static SpawnPool DefaultPool
        {
            get
            {
                if (_defaultPool == null) CreatePrimaryPool();
                return _defaultPool;
            }
        }

        public static SpawnPool Pool(int index)
        {
            if (_defaultPool == null) CreatePrimaryPool();
            return (index == 0) ? _defaultPool : _pools[index - 1];
        }

        public static SpawnPool Pool(string name)
        {
            if (_defaultPool != null && _defaultPool.name == name) return _defaultPool;
            return (from p in _pools where p.name == name select p).FirstOrDefault();
        }

        public static int PoolCount { get { return _pools.Count + 1; } }

        private static void CreatePrimaryPool()
        {
            if (_defaultPool != null) return;

            var point = (from p in GameObject.FindObjectsOfType<SpawnPool>() orderby p.name == "Spacepuppy.PrimarySpawnPool" select p).FirstOrDefault();
            if (!Object.ReferenceEquals(point, null))
            {
                _defaultPool = point;
            }
            else
            {
                var go = new GameObject("Spacepuppy.PrimarySpawnPool");
                _defaultPool = go.AddComponent<SpawnPool>();
            }
        }

        #endregion

        #region Fields

        [SerializeField()]
        private List<PrefabCacheOptions> _registeredPrefabs = new List<PrefabCacheOptions>();
        [System.NonSerialized()]
        private Dictionary<GameObject, PrefabCacheOptions> _prefabToCache = new Dictionary<GameObject, PrefabCacheOptions>();

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
            if (Object.ReferenceEquals(this, _defaultPool))
            {
                _pools.Add(this);
            }
        }

        protected override void Start()
        {
            base.Start();

            foreach (var cache in _registeredPrefabs)
            {
                cache.Load();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Object.ReferenceEquals(this, _defaultPool))
            {
                _defaultPool = null;
            }
            else
            {
                _pools.Remove(this);
            }
        }

        #endregion

        #region Properties

        public int Count { get { return _registeredPrefabs.Count; } }

        #endregion

        #region Methods

        public GameObject Spawn(int index, Transform par = null, System.Action<GameObject> initializeProperties = null)
        {
            if (index < 0 || index >= _registeredPrefabs.Count) throw new System.ArgumentException("Index out of range.", "index");

            var cache = _registeredPrefabs[index];
            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;
            var obj = cache.Spawn(pos, rot, par, initializeProperties);
            this.SignalSpawned(obj);
            return obj;
        }

        public GameObject Spawn(int index, Vector3 position, Quaternion rotation, Transform par = null, System.Action<GameObject> initializeProperties = null)
        {
            if (index < 0 || index >= _registeredPrefabs.Count) throw new System.ArgumentException("Index out of range.", "index");

            var cache = _registeredPrefabs[index];
            var obj = cache.Spawn(position, rotation, par, initializeProperties);
            this.SignalSpawned(obj);
            return obj;
        }

        public GameObject Spawn(string sname, Transform par = null, System.Action<GameObject> initializeProperties = null)
        {
            var cache = (from o in _registeredPrefabs where o.ItemName == sname select o).FirstOrDefault();
            if (cache == null) return null;

            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;
            var obj = cache.Spawn(pos, rot, par, initializeProperties);
            this.SignalSpawned(obj);
            return obj;
        }

        public GameObject Spawn(string sname, Vector3 position, Quaternion rotation, Transform par = null, System.Action<GameObject> initializeProperties = null)
        {
            var cache = (from o in _registeredPrefabs where o.ItemName == sname select o).FirstOrDefault();
            if (cache == null) return null;

            var obj = cache.Spawn(position, rotation, par, initializeProperties);
            this.SignalSpawned(obj);
            return obj;
        }

        public GameObject Spawn(GameObject prefab, Transform par = null, System.Action<GameObject> initializeProperties = null)
        {
            var cache = this.FindPrefabCache(prefab);
            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;

            if (cache != null)
            {
                var obj = cache.Spawn(pos, rot, par, initializeProperties);
                this.SignalSpawned(obj);
                return obj;
            }
            else
            {
                var obj = GameObject.Instantiate(prefab) as GameObject;
                if (obj != null)
                {
                    obj.transform.parent = par;
                    obj.transform.position = pos;
                    obj.transform.rotation = rot;
                    if (initializeProperties != null) initializeProperties(obj);
                    this.SignalSpawned(obj);
                }
                return obj;
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform par = null, System.Action<GameObject> initializeProperties = null)
        {
            var cache = this.FindPrefabCache(prefab);
            if (cache != null)
            {
                var obj = cache.Spawn(position, rotation, par, initializeProperties);
                this.SignalSpawned(obj);
                return obj;
            }
            else
            {
                var obj = GameObject.Instantiate(prefab) as GameObject;
                if (obj != null)
                {
                    obj.transform.parent = par;
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;
                    if (initializeProperties != null) initializeProperties(obj);
                    this.SignalSpawned(obj);
                }
                return obj;
            }
        }

        //public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform par = null, System.Action<T> initializeProperties = null) where T : UnityEngine.Component
        //{
        //    var cache = this.FindPrefabCache(prefab.gameObject);
        //    if(cache != null)
        //    {
        //        System.Action<GameObject> act = (initializeProperties == null) ? null : new System.Action<GameObject>(delegate (GameObject go) { initializeProperties(go.GetComponent<T>()); });
        //        var obj = cache.Spawn(position, rotation, par, act);
        //        this.SignalSpawned(obj);
        //        return obj.GetComponent<T>();
        //    }
        //    else
        //    {
        //        var obj = GameObject.Instantiate(prefab) as T;
        //        if(obj != null)
        //        {
        //            obj.transform.parent = par;
        //            obj.transform.position = position;
        //            obj.transform.rotation = rotation;
        //            if (initializeProperties != null) initializeProperties(obj);
        //            this.SignalSpawned(obj.gameObject);
        //        }
        //        return obj;
        //    }
        //}

        /// <summary>
        /// Return an object to the pool.
        /// </summary>
        /// <param name="obj"></param>
        public void Despawn(SpawnedObjectController obj)
        {
            foreach (var cache in _registeredPrefabs)
            {
                if (cache.Contains(obj))
                {
                    obj.BroadcastMessage(SPConstants.MSG_ONDESPAWN, SendMessageOptions.DontRequireReceiver);
                    var n = new DeSpawnNotification(obj.gameObject);
                    Notification.PostNotification<DeSpawnNotification>(obj, n, false);
                    Notification.PostNotification<DeSpawnNotification>(this, n, false);
                    cache.Despawn(obj);
                    return;
                }
            }
        }



        /// <summary>
        /// Match an object to its prefab if this pool manages the GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private PrefabCacheOptions FindPrefabCache(GameObject obj)
        {
            //test if the object is the prefab in question
            if (_prefabToCache.ContainsKey(obj)) return _prefabToCache[obj];

            var controller = obj.FindComponent<SpawnedObjectController>();
            if (controller != null && controller.Pool == this)
            {
                foreach (var cache in _registeredPrefabs)
                {
                    if (cache.Contains(controller))
                    {
                        return cache;
                    }
                }
            }

            return null;
        }

        private void SignalSpawned(GameObject obj)
        {
            if (obj == null) return;
            obj.BroadcastMessage(SPConstants.MSG_ONSPAWN, SendMessageOptions.DontRequireReceiver);
            var n = new SpawnNotification(obj);
            Notification.PostNotification(obj, n, false);
            Notification.PostNotification(this, n, false);
        }

        #endregion

        #region ISerializationCallbackReceiver Interface

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _registeredPrefabs.RemoveAll((c) => Object.ReferenceEquals(c, null));
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _prefabToCache.Clear();

            _registeredPrefabs.RemoveAll((c) => Object.ReferenceEquals(c, null));
            foreach (var cache in _registeredPrefabs)
            {
                cache.Init(this);
                if (!Object.ReferenceEquals(cache.Prefab, null)) _prefabToCache[cache.Prefab] = cache;
            }
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        public class PrefabCacheOptions
        {

            public string ItemName;
            public GameObject Prefab;

            public int CacheSize;
            public int ResizeBuffer;
            public int LimitAmount;

            [System.NonSerialized()]
            private SpawnPool _owner;
            [System.NonSerialized()]
            private SpawnedObjectController[] _instances;

            public PrefabCacheOptions()
            {

            }

            public int Count
            {
                get { return (_instances == null) ? 0 : _instances.Length; }
            }

            public bool Contains(SpawnedObjectController obj)
            {
                if (_instances == null) return false;

                return System.Array.IndexOf(_instances, obj) >= 0;
            }

            internal void Init(SpawnPool owner)
            {
                _owner = owner;
            }

            internal void Load()
            {
                if (_instances != null) return;

                this.Clear();

                if (Prefab == null) return;

                _instances = new SpawnedObjectController[CacheSize];

                for (int i = 0; i < CacheSize; i++)
                {
                    CreateCacheInstance(i);
                }
            }

            internal void Clear()
            {
                if (_instances == null) return;

                foreach (var obj in _instances)
                {
                    GameObject.Destroy(obj);
                }

                _instances = null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="par"></param>
            /// <param name="initializeProperties">A method that initializes properties on the gameobject before OnSpawned message is called.</param>
            /// <returns></returns>
            internal GameObject Spawn(Vector3 pos, Quaternion rot, Transform par, System.Action<GameObject> initializeProperties)
            {
                int cnt = this.Count;

                for (int i = 0; i < cnt; i++)
                {
                    if (!_instances[i].IsSpawned)
                    {
                        var cntrl = _instances[i];

                        cntrl.transform.parent = par;
                        cntrl.transform.position = pos;
                        cntrl.transform.rotation = rot;
                        cntrl.SetSpawned();

                        if (initializeProperties != null) initializeProperties(cntrl.gameObject);

                        return cntrl.gameObject;
                    }
                }

                int newSize = cnt + ResizeBuffer;
                if (LimitAmount > 0) newSize = Mathf.Min(newSize, LimitAmount);

                if (newSize > cnt)
                {
                    System.Array.Resize(ref _instances, newSize);

                    for (int i = cnt; i < newSize; i++)
                    {
                        CreateCacheInstance(i);
                    }

                    //grab instance
                    var cntrl = _instances[cnt];
                    cntrl.transform.parent = par;
                    cntrl.transform.position = pos;
                    cntrl.transform.rotation = rot;
                    cntrl.transform.parent = par;
                    cntrl.SetSpawned();

                    if (initializeProperties != null) initializeProperties(cntrl.gameObject);

                    return cntrl.gameObject;
                }

                return null;
            }

            internal void Despawn(SpawnedObjectController obj)
            {
                if (_instances == null) return;

                int i = System.Array.IndexOf(_instances, obj);
                if (i < 0) return;

                _instances[i].SetDespawned();
                _instances[i].transform.parent = _owner.transform;
                _instances[i].transform.localPosition = Vector3.zero;
                _instances[i].transform.rotation = Quaternion.identity;
            }





            private GameObject CreateCacheInstance(int index)
            {
                if (index < 0 || index >= _instances.Length) return null;
                if (_instances[index] != null) return _instances[index].gameObject;

                var obj = (GameObject)GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity);
                obj.name = this.ItemName + (index + 1).ToString("000");
                var cntrl = obj.AddComponent<SpawnedObjectController>();
                cntrl.Init(_owner, this.ItemName);

                _instances[index] = cntrl;

                obj.transform.parent = _owner.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;

                obj.DeactivateAll();

                return obj;
            }

        }

        #endregion



    }

}