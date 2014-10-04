using UnityEngine;
using System.Collections;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [AddComponentMenu("SpacePuppy/Spawn/Spawned Object")]
    public class SpawnedObjectController : SPComponent
    {

        #region Fields

        private SpawnPool _pool;
        private string _sCacheName;

        private bool _isSpawned;

        #endregion

        #region CONSTRUCTOR

        internal void Init(SpawnPool pool, string sCacheName)
        {
            _pool = pool;
            _sCacheName = sCacheName;
        }

        #endregion

        #region Properties

        public bool IsSpawned
        {
            get { return _isSpawned; }
        }

        public SpawnPool Pool
        {
            get { return _pool; }
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

        public void DespawnObject()
        {
            foreach (var go in this.GetAllChildrenAndSelf())
            {
                if (go.rigidbody != null)
                {
                    go.rigidbody.velocity = Vector3.zero;
                    go.rigidbody.angularVelocity = Vector3.zero;
                }
                if (go.rigidbody2D != null)
                {
                    go.rigidbody2D.velocity = Vector3.zero;
                    go.rigidbody2D.angularVelocity = 0f;
                }

                //NOTE - this is handled by the components themselves
                //Notification.PurgeHandlers(go.gameObject);
            }

            _pool.Despawn(this);
        }

        public GameObject CloneObject()
        {
            return _pool.Spawn(this.gameObject);
        }

        #endregion

    }

}