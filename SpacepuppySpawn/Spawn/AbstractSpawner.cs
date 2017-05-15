using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [System.Obsolete("DO NOT USE ANYMORE!")]
    public abstract class AbstractSpawner : SPNotifyingComponent, ISpawner, ITriggerableMechanism, IObservableTrigger
    {

        public const string TRG_ONFINISH = "OnSpawned";

        #region Fields

        [SerializeField()]
        private int _order;

        [SerializeField()]
        private SelfTrackingSpawnerMechanism _spawnMechanism = new SelfTrackingSpawnerMechanism();

        [SerializeField()]
        [Tooltip("The object will be spawned as a child of the SpawnPoint GameObject.")]
        private bool _spawnAsChild = false;

        [SerializeField()]
        private Trigger _onSpawnedObject = new Trigger(TRG_ONFINISH);

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _spawnMechanism.Init(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _spawnMechanism.Active = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _spawnMechanism.Active = false;
        }

        #endregion

        #region Properties

        public bool UsesDefaultSpawnPool
        {
            get { return _spawnMechanism.UsesDefaultSpawnPool; }
        }

        public SpawnPool SpawnPool
        {
            get { return _spawnMechanism.SpawnPool; }
            set
            {
                _spawnMechanism.SpawnPool = value;
            }
        }

        public bool SpawnAsChild
        {
            get { return _spawnAsChild; }
            set { _spawnAsChild = value; }
        }

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

        protected GameObject Spawn(GameObject prefab, System.Action<GameObject> initializeProperties = null)
        {
            if (!this.enabled) return null;
            if (prefab == null) return null;

            Transform par = (_spawnAsChild) ? this.transform : null;
            var pos = this.transform.position;
            var rot = this.transform.rotation;
            var spawnedObject = _spawnMechanism.Spawn(prefab, pos, rot, par, initializeProperties);

            if (spawnedObject == null) return null;

            if (_onSpawnedObject != null && _onSpawnedObject.Count > 0) _onSpawnedObject.ActivateTrigger(this, spawnedObject);
            return spawnedObject;
        }

        protected GameObject SelectPrefab(GameObject[] prefabs)
        {
            if (!this.enabled) return null;

            int index = SpawnPointHelper.SelectFromMultiple(this, prefabs.Length);
            if (index < 0 || index >= prefabs.Length) return null;

            return prefabs[index];
        }

        public GameObject[] GetActiveGameObjects()
        {
            return _spawnMechanism.GetActiveGameObjects();
        }

        public SpawnedObjectController[] GetActiveObjectControllers()
        {
            return _spawnMechanism.GetActiveObjectControllers();
        }

        #endregion

        #region Abstract Interface

        public virtual bool SupportsSelectionModifier { get { return true; } }

        protected abstract GameObject[] GetAvailablePrefabs();

        #endregion

        #region ISpawner Interface

        public SelfTrackingSpawnerMechanism Mechanism { get { return _spawnMechanism; } }

        public int ActiveCount { get { return _spawnMechanism.ActiveCount; } }

        public int TotalCount { get { return _spawnMechanism.TotalCount; } }

        void ISpawner.Spawn()
        {
            this.Spawn();
        }

        #endregion

        #region ITriggerableMechanism Interface

        public int Order
        {
            get { return _order; }
        }

        public virtual bool CanTrigger
        {
            get { return this.isActiveAndEnabled; }
        }
        
        public void Trigger()
        {
            this.Trigger(null, null);
        }

        public abstract bool Trigger(object sender, object arg);

        #endregion


        #region IObserverableTarget Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onSpawnedObject };
        }

        #endregion

    }
}
