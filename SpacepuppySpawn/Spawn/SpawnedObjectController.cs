using UnityEngine;
using System.Collections;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [AddComponentMenu("SpacePuppy/Spawn/Spawned Object")]
    public class SpawnedObjectController : SPNotifyingComponent, IKillableEntity
    {

        #region Fields

        [System.NonSerialized()]
        private SpawnPool _pool;
        [System.NonSerialized]
        private GameObject _prefab;
        [System.NonSerialized()]
        private string _sCacheName;

        [System.NonSerialized()]
        private bool _isSpawned;

        #endregion

        #region CONSTRUCTOR

        internal void Init(SpawnPool pool, GameObject prefab)
        {
            _pool = pool;
            _prefab = prefab;
            _sCacheName = null;
        }

        /// <summary>
        /// Initialize with a reference to the pool that spawned this object. Include a cache name if this gameobject is cached, otherwise no cache name should be included.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="sCacheName"></param>
        internal void Init(SpawnPool pool, string sCacheName)
        {
            _pool = pool;
            _sCacheName = sCacheName;
        }

        internal void DeInit()
        {
            _pool = null;
            _prefab = null;
            _sCacheName = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(!GameLoopEntry.ApplicationClosing && _pool != null)
            {
                _pool.Purge(this);
            }
        }

        #endregion

        #region Properties

        public bool IsSpawned
        {
            get { return _isSpawned; }
        }

        /// <summary>
        /// The pool that created this object.
        /// </summary>
        public SpawnPool Pool
        {
            get { return _pool; }
        }

        public GameObject Prefab
        {
            get { return _prefab; }
        }

        /// <summary>
        /// This object is a managed cached object, when killed it'll be returned to the pool.
        /// </summary>
        public bool IsCachedObject
        {
            get { return _sCacheName != null; }
        }

        /// <summary>
        /// This value will only be non-null if it is a managed cached object.
        /// </summary>
        public string CacheName
        {
            get { return _sCacheName; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method ONLY called by SpawnPool
        /// </summary>
        internal void SetSpawned()
        {
            _isSpawned = true;
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// This method ONLY called by SpawnPool
        /// </summary>
        internal void SetDespawned()
        {
            _isSpawned = false;
            this.gameObject.SetActive(false);
        }

        public GameObject CloneObject(bool fromPrefab = false)
        {
            if(fromPrefab && _prefab != null)
                return _pool.Spawn(_prefab, this.transform.position, this.transform.rotation);
            else
                return _pool.Spawn(this.gameObject, this.transform.position, this.transform.rotation);
        }

        #endregion

        #region IKillableEntity Interface

        public bool IsDead
        {
            get { return !_isSpawned; }
        }

        public void Kill()
        {
            if(_pool.Despawn(this))
            {
                using (var lst = TempCollection.GetList<Transform>())
                {
                    GameObjectUtil.GetAllChildrenAndSelf(this.transform, lst);
                    for(int i = 0; i < lst.Count; i++)
                    {
                        var rb = lst[i].GetComponent<Rigidbody>();
                        if (rb != null && !rb.isKinematic)
                        {
                            rb.velocity = Vector3.zero;
                            rb.angularVelocity = Vector3.zero;
                        }

                        var rb2 = lst[i].GetComponent<Rigidbody2D>();
                        if (rb2 != null && !rb2.isKinematic)
                        {
                            rb2.velocity = Vector3.zero;
                            rb2.angularVelocity = 0f;
                        }

                        //NOTE - this is handled by the components themselves
                        //Notification.PurgeHandlers(lst[i].gameObject);
                    }
                }
            }
        }

        #endregion

    }

}