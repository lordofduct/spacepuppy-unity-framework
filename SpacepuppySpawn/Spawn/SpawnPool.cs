using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [AddComponentMenu("SpacePuppy/Spawn/Spawn Pool")]
    public class SpawnPool : SPNotifyingComponent, ISpawnFactory, ISerializationCallbackReceiver
    {

        #region Static Multiton Interface

        public const string DEFAULT_SPAWNPOOL_NAME = "Spacepuppy.PrimarySpawnPool";

        private static SpawnPool _defaultPool;
        private static List<SpawnPool> _pools = new List<SpawnPool>();

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
            if(index < 0 || index > _pools.Count) throw new System.IndexOutOfRangeException();
            if (_defaultPool == null) CreatePrimaryPool();
            if (index == 0) return _defaultPool;

            return _pools[index - 1];
        }

        public static SpawnPool Pool(string name)
        {
            if (_defaultPool != null && _defaultPool.name == name) return _defaultPool;

            //TODO - should cache 'name' for access so this doesn't generate garbage
            var e = _pools.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.name == name) return e.Current;
            }
            return null;
        }

        public static int PoolCount { get { return _pools.Count + 1; } }

        public static void CreatePrimaryPool()
        {
            if (PrimaryPoolExists) return;

            var go = new GameObject(DEFAULT_SPAWNPOOL_NAME);
            _defaultPool = go.AddComponent<SpawnPool>();
        }

        public static bool PrimaryPoolExists
        {
            get
            {
                if (_defaultPool != null) return true;

                _defaultPool = null;
                var point = (from p in GameObject.FindObjectsOfType<SpawnPool>() orderby p.name == DEFAULT_SPAWNPOOL_NAME select p).FirstOrDefault();
                if (!object.ReferenceEquals(point, null))
                {
                    _defaultPool = point;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region Fields

        [SerializeField()]
        [ReorderableArray(DrawElementAtBottom = true, ChildPropertyToDrawAsElementLabel = "ItemName", ChildPropertyToDrawAsElementEntry="_prefab")]
        private List<PrefabCacheOptions> _registeredPrefabs = new List<PrefabCacheOptions>();
        [System.NonSerialized()]
        private Dictionary<GameObject, PrefabCacheOptions> _prefabToCache = new Dictionary<GameObject, PrefabCacheOptions>(com.spacepuppy.Collections.ObjectInstanceIDEqualityComparer<GameObject>.Default);

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
            if (!Object.ReferenceEquals(this, _defaultPool))
            {
                _pools.Add(this);
            }
        }

        protected override void Start()
        {
            base.Start();

            var e = _registeredPrefabs.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Load();
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

        public PrefabCacheOptions Register(GameObject prefab, string sname, int cacheSize = 0, int resizeBuffer = 1, int limitAmount = 1)
        {
            if (object.ReferenceEquals(prefab, null)) throw new System.ArgumentNullException("prefab");
            if (_prefabToCache.ContainsKey(prefab)) throw new System.ArgumentException("Already manages prefab.", "prefab");

            var options = new PrefabCacheOptions(prefab)
            {
                ItemName = sname,
                CacheSize = cacheSize,
                ResizeBuffer = resizeBuffer,
                LimitAmount = limitAmount
            };
            _registeredPrefabs.Add(options);
            _prefabToCache[prefab] = options;
            return options;
        }


        public GameObject Spawn(int index, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null)
        {
            if (index < 0 || index >= _registeredPrefabs.Count) throw new System.IndexOutOfRangeException();

            var cache = _registeredPrefabs[index];
            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;
            var obj = cache.Spawn(pos, rot, par, initializeProperties);
            this.SignalSpawned(obj, optionalSpawnPoint);
            return obj.gameObject;
        }

        public GameObject Spawn(int index, Vector3 position, Quaternion rotation, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null)
        {
            if (index < 0 || index >= _registeredPrefabs.Count) throw new System.IndexOutOfRangeException();

            var cache = _registeredPrefabs[index];
            var obj = cache.Spawn(position, rotation, par, initializeProperties);
            this.SignalSpawned(obj, optionalSpawnPoint);
            return obj.gameObject;
        }

        public GameObject Spawn(string sname, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null)
        {
            //var cache = (from o in _registeredPrefabs where o.ItemName == sname select o).FirstOrDefault();
            PrefabCacheOptions cache = null;
            var e = _registeredPrefabs.GetEnumerator();
            while(e.MoveNext())
            {
                if(e.Current.ItemName == sname)
                {
                    cache = e.Current;
                    break;
                }
            }
            if (cache == null) return null;

            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;
            var obj = cache.Spawn(pos, rot, par, initializeProperties);
            this.SignalSpawned(obj, optionalSpawnPoint);
            return obj.gameObject;
        }

        public GameObject Spawn(string sname, Vector3 position, Quaternion rotation, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null)
        {
            //var cache = (from o in _registeredPrefabs where o.ItemName == sname select o).FirstOrDefault();
            PrefabCacheOptions cache = null;
            var e = _registeredPrefabs.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.ItemName == sname)
                {
                    cache = e.Current;
                    break;
                }
            }
            if (cache == null) return null;

            var obj = cache.Spawn(position, rotation, par, initializeProperties);
            this.SignalSpawned(obj, optionalSpawnPoint);
            return obj.gameObject;
        }

        public GameObject Spawn(GameObject prefab, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null)
        {
            var controller = SpawnAsController(prefab, par, initializeProperties, optionalSpawnPoint);
            return (controller != null) ? controller.gameObject : null;
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null)
        {
            var controller = SpawnAsController(prefab, position, rotation, par, initializeProperties, optionalSpawnPoint);
            return (controller != null) ? controller.gameObject : null;
        }
        
        public T SpawnAsComponent<T>(T prefab, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null) where T : class
        {
            var go = GameObjectUtil.GetGameObjectFromSource(prefab);
            if (object.ReferenceEquals(go, null)) return null;

            var controller = SpawnAsController(go, par, initializeProperties, optionalSpawnPoint);
            if (controller == null) return null;

            var result = controller.GetComponent<T>();
            if(result == null)
            {
                this.Despawn(controller);
                return null;
            }
            else
            {
                return result;
            }
        }
        
        public T SpawnAsComponent<T>(T prefab, Vector3 position, Quaternion rotation, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null) where T : class
        {
            var go = GameObjectUtil.GetGameObjectFromSource(prefab);
            if (object.ReferenceEquals(go, null)) return null;

            var controller = SpawnAsController(go, position, rotation, par, initializeProperties, optionalSpawnPoint);
            if (controller == null) return null;

            var result = controller.GetComponent<T>();
            if (result == null)
            {
                this.Despawn(controller);
                return null;
            }
            else
            {
                return result;
            }
        }

        public T SpawnAsComponent<T>(GameObject prefab, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null) where T : class
        {
            if (object.ReferenceEquals(prefab, null)) return null;

            var controller = SpawnAsController(prefab, par, initializeProperties, optionalSpawnPoint);
            if (controller == null) return null;

            var result = controller.GetComponent<T>();
            if (result == null)
            {
                this.Despawn(controller);
                return null;
            }
            else
            {
                return result;
            }
        }

        public T SpawnAsComponent<T>(GameObject prefab, Vector3 position, Quaternion rotation, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null) where T : class
        {
            if (object.ReferenceEquals(prefab, null)) return null;

            var controller = SpawnAsController(prefab, position, rotation, par, initializeProperties, optionalSpawnPoint);
            if (controller == null) return null;

            var result = controller.GetComponent<T>();
            if (result == null)
            {
                this.Despawn(controller);
                return null;
            }
            else
            {
                return result;
            }
        }

        public SpawnedObjectController SpawnAsController(GameObject prefab, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint = null)
        {
            var cache = this.FindPrefabCache(prefab);
            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;

            if (cache != null)
            {
                var controller = cache.Spawn(pos, rot, par, initializeProperties);
                this.SignalSpawned(controller, optionalSpawnPoint);
                return controller;
            }
            else
            {
                var obj = PrefabUtil.Create(prefab, pos, rot, par);
                if (obj != null)
                {
                    var controller = obj.AddOrGetComponent<SpawnedObjectController>();
                    controller.Init(this, prefab);
                    if (initializeProperties != null) initializeProperties(obj);
                    controller.SetSpawned();
                    this.SignalSpawned(controller, optionalSpawnPoint);
                    return controller;
                }
                else
                {
                    return null;
                }
            }
        }

        public SpawnedObjectController SpawnAsController(GameObject prefab, Vector3 position, Quaternion rotation, Transform par = null, System.Action<GameObject> initializeProperties = null, ISpawner optionalSpawnPoint= null)
        {
            var cache = this.FindPrefabCache(prefab);
            if (cache != null)
            {
                var controller = cache.Spawn(position, rotation, par, initializeProperties);
                this.SignalSpawned(controller, optionalSpawnPoint);
                return controller;
            }
            else
            {
                var obj = PrefabUtil.Create(prefab, position, rotation, par);
                if (obj != null)
                {
                    var controller = obj.AddOrGetComponent<SpawnedObjectController>();
                    controller.Init(this, prefab);
                    if (initializeProperties != null) initializeProperties(obj);
                    controller.SetSpawned();
                    this.SignalSpawned(controller, optionalSpawnPoint);
                    return controller;
                }
                else
                {
                    return null;
                }
            }
        }
        

        /// <summary>
        /// Return an object to the pool.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Returns true if the object was returned to the pool, false if destroyed.</returns>
        internal bool Despawn(SpawnedObjectController obj)
        {
            if (Object.ReferenceEquals(obj, null)) throw new System.ArgumentNullException("obj");

            obj.BroadcastMessage(SPSpawnConstants.MSG_ONDESPAWN, SendMessageOptions.DontRequireReceiver);
            var n = DeSpawnNotification.Create(obj);
            Notification.PostNotification<DeSpawnNotification>(obj, n, false);
            Notification.PostNotification<DeSpawnNotification>(this, n, false);
            Notification.Release(n);

            var e = _registeredPrefabs.GetEnumerator();
            while(e.MoveNext())
            {
                if(e.Current.Contains(obj))
                {
                    e.Current.Despawn(obj);
                    return true;
                }
            }

            //if we reached here, it's not managed by this pool... destroy
            if (UnityEngine.Application.isPlaying)
            {
                UnityEngine.Object.Destroy(obj.gameObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj.gameObject);
            }
            return false;
        }

        /// <summary>
        /// If a GameObject is being destroyed, this method will purge it from the SpawnPool.
        /// </summary>
        /// <param name="obj"></param>
        internal void Purge(SpawnedObjectController obj)
        {
            if (Object.ReferenceEquals(obj, null)) throw new System.ArgumentNullException("obj");

            var n = SpawnPoolPurgedNotification.Create(obj);
            Notification.PostNotification<SpawnPoolPurgedNotification>(obj, n, false);
            Notification.PostNotification<SpawnPoolPurgedNotification>(this, n, false);
            Notification.Release(n);

            var e = _registeredPrefabs.GetEnumerator();
            while(e.MoveNext())
            {
                if(e.Current.Contains(obj))
                {
                    e.Current.Purge(obj);
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
            //TODO - figure out the best way to match a gameobject to the cache pool.
            //as it stands this depends on the prefab being a shared instance across all scripts... 
            //I am unsure if that's how unity works, and what the limitations are on that.
            //consider creating a system of relating equal prefabs

            //test if the object is the prefab in question
            if (_prefabToCache.ContainsKey(obj)) return _prefabToCache[obj];

            var controller = obj.FindComponent<SpawnedObjectController>();
            if (controller != null && controller.Pool == this)
            {
                var e = _registeredPrefabs.GetEnumerator();
                while(e.MoveNext())
                {
                    if (e.Current.Contains(controller)) return e.Current;
                }
            }

            return null;
        }

        private void SignalSpawned(SpawnedObjectController obj, ISpawner spawnPoint)
        {
            if (obj == null) return;


            var n = SpawnNotification.Create(this, obj.gameObject, spawnPoint);
            obj.gameObject.BroadcastMessage(SPSpawnConstants.MSG_ONSPAWN, n, SendMessageOptions.DontRequireReceiver);
            Notification.PostNotification(obj, n, false);
            Notification.PostNotification(this, n, false);
            Notification.Release(n);
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

            var e = _registeredPrefabs.GetEnumerator();
            PrefabCacheOptions cache;
            while(e.MoveNext())
            {
                cache = e.Current;
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
            [SerializeField]
            [UnityEngine.Serialization.FormerlySerializedAs("Prefab")]
            private GameObject _prefab;

            public int CacheSize = 0;
            public int ResizeBuffer = 1;
            public int LimitAmount = 1;

            [System.NonSerialized()]
            private SpawnPool _owner;
            [System.NonSerialized()]
            private HashSet<SpawnedObjectController> _instances = new HashSet<SpawnedObjectController>(com.spacepuppy.Collections.ObjectReferenceEqualityComparer<SpawnedObjectController>.Default);
            [System.NonSerialized()]
            private HashSet<SpawnedObjectController> _activeInstances = new HashSet<SpawnedObjectController>(com.spacepuppy.Collections.ObjectReferenceEqualityComparer<SpawnedObjectController>.Default);

            internal PrefabCacheOptions(GameObject prefab)
            {
                _prefab = prefab;
            }

            public GameObject Prefab
            {
                get
                {
                    return _prefab;
                }
            }

            public int Count
            {
                get { return _instances.Count + _activeInstances.Count; }
            }

            public bool Contains(SpawnedObjectController obj)
            {
                return _instances.Contains(obj) || _activeInstances.Contains(obj);
            }

            internal void Init(SpawnPool owner)
            {
                _owner = owner;
            }

            internal void Load()
            {
                if (_instances != null) return;

                this.Clear();
                if (_prefab == null) return;

                for (int i = 0; i < CacheSize; i++)
                {
                    _instances.Add(this.CreateCacheInstance(i));
                }
            }

            internal void Clear()
            {
                using(var lst = com.spacepuppy.Collections.TempCollection.GetList<SpawnedObjectController>(_instances))
                {
                    _instances.Clear();

                    var e = lst.GetEnumerator();
                    while (e.MoveNext())
                    {
                        e.Current.DeInit();
                        Object.Destroy(e.Current);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="par"></param>
            /// <param name="initializeProperties">A method that initializes properties on the gameobject before OnSpawned message is called.</param>
            /// <returns></returns>
            internal SpawnedObjectController Spawn(Vector3 pos, Quaternion rot, Transform par, System.Action<GameObject> initializeProperties)
            {
                if(_instances.Count == 0)
                {
                    int cnt = this.Count;
                    int newSize = cnt + ResizeBuffer;
                    if (LimitAmount > 0) newSize = Mathf.Min(newSize, LimitAmount);

                    if (newSize > cnt)
                    {
                        for (int i = cnt; i < newSize; i++)
                        {
                            _instances.Add(CreateCacheInstance(i));
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                //var cntrl = _instances[0];
                //_instances.RemoveAt(0);
                var cntrl = _instances.Pop();

                _activeInstances.Add(cntrl);

                par.AddChild(cntrl.transform);
                cntrl.transform.position = pos;
                cntrl.transform.rotation = rot;
                if (initializeProperties != null) initializeProperties(cntrl.gameObject);
                cntrl.SetSpawned();

                return cntrl;
            }

            internal void Despawn(SpawnedObjectController obj)
            {
                if (!_activeInstances.Contains(obj)) return;

                _activeInstances.Remove(obj);

                obj.SetDespawned();
                obj.transform.parent = _owner.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;

                _instances.Add(obj);
            }

            internal void Purge(SpawnedObjectController obj)
            {
                if(_activeInstances.Contains(obj))
                {
                    _activeInstances.Remove(obj);
                }
                if(_instances.Contains(obj))
                {
                    _instances.Remove(obj);
                }
            }




            private SpawnedObjectController CreateCacheInstance(int index)
            {
                var obj = PrefabUtil.Create(this.Prefab, Vector3.zero, Quaternion.identity);
                obj.name = this.ItemName + (index + 1).ToString("000");
                var cntrl = obj.AddComponent<SpawnedObjectController>();
                cntrl.Init(_owner, this.ItemName);

                obj.transform.parent = _owner.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;

                //obj.DeactivateAll();
                obj.SetActive(false);

                return cntrl;
            }

        }

        #endregion



        #region Static Methods

        public static void DeSpawn(GameObject go)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            var c = go.GetComponent<SpawnedObjectController>();
            if (c == null) return;

            c.Kill();
        }

        #endregion


    }

}