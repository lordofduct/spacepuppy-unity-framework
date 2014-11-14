using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{
    public abstract class AbstractSpawnPoint : SPNotifyingComponent, com.spacepuppy.Scenario.ITriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private int _order;

        [SerializeField()]
        [Tooltip("If left empty the default SpawnPool will be used instead.")]
        private SpawnPool _spawnPool;

        [SerializeField()]
        [Tooltip("The object will be spawned as a child of the SpawnPoint GameObject.")]
        private bool _spawnAsChild = false;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Properties

        public bool UsesDefaultSpawnPool
        {
            get { return Object.ReferenceEquals(_spawnPool, null) || Object.ReferenceEquals(_spawnPool, SpawnPool.DefaultPool); }
        }

        public SpawnPool SpawnPool
        {
            get { return _spawnPool; }
            set
            {
                _spawnPool = value;
            }
        }

        public bool SpawnAsChild
        {
            get { return _spawnAsChild; }
            set { _spawnAsChild = value; }
        }

        #endregion

        #region Abstract Interface

        public virtual bool SupportsSelectionModifier { get { return true; } }

        protected abstract GameObject[] GetAvailablePrefabs();

        #endregion

        #region Methods

        public virtual GameObject Spawn()
        {
            if (!this.enabled) return null;

            var prefabs = this.GetAvailablePrefabs();
            var prefab = this.SelectPrefab(prefabs);
            if (prefab == null) return null;
            return this.Spawn(prefab);
        }

        protected GameObject SelectPrefab(GameObject[] prefabs)
        {
            if (this.SupportsSelectionModifier)
            {
                var selector = (from c in this.GetLikeComponents<ISpawnPointPrefabSelector>() where c.enabled select c).FirstOrDefault();
                if (selector != null) return selector.SelectPrefab(prefabs);
            }

            if (prefabs == null || prefabs.Length == 0) return null;
            var i = (prefabs.Length > 1) ? Random.Range(0, prefabs.Length) : 0;
            return prefabs[i];
        }

        protected GameObject Spawn(GameObject prefab, System.Action<GameObject> initializeProperties = null)
        {
            if (prefab == null) return null;

            var modifiers = this.GetComponents<OnSpawnModifier>().OrderBy((c) => c.order).ToArray();

            var beforeNotif = new SpawnPointBeforeSpawnNotification(prefab);
            foreach (var m in modifiers)
            {
                m.OnBeforeSpawnNotification(beforeNotif);
            }
            Notification.PostNotification<SpawnPointBeforeSpawnNotification>(this, beforeNotif, false);
            if (beforeNotif.Cancelled) return null;

            if (_spawnPool == null)
            {
                _spawnPool = SpawnPool.DefaultPool;
            }

            GameObject go;
            Transform par = (_spawnAsChild) ? this.transform : null;
            //var pos = (par != null) ? Vector3.zero : this.transform.position;
            //var rot = (par != null) ? Quaternion.identity : this.transform.rotation;
            var pos = this.transform.position;
            var rot = this.transform.rotation;
            go = _spawnPool.Spawn(prefab, pos, rot, par, initializeProperties);

            if (go == null) return null;

            var spawnNotif = new SpawnPointTriggeredNotification(go);
            foreach (var m in modifiers)
            {
                m.OnSpawnedNotification(spawnNotif);
            }
            Notification.PostNotification<SpawnPointTriggeredNotification>(this, spawnNotif, false);
            return go;
        }

        #endregion

        #region ITriggerableMechanism Interface

        public int Order
        {
            get { return _order; }
        }

        public virtual bool CanTrigger
        {
            get { return this.enabled; }
        }

        public abstract object Trigger(object arg);

        #endregion

    }
}
