using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Project;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{
    public class i_SpawnFromResources : TriggerableMechanism, ISpawner, IObservableTrigger
    {

        public const string TRG_ONSPAWNED = "OnSpawned";

        public const string TRIGGERARG_RANDOM = "RANDOM";

        #region Fields

        [SerializeField()]
        private SelfTrackingSpawnerMechanism _spawnMechanism = new SelfTrackingSpawnerMechanism();

        [SerializeField()]
        [Tooltip("The object will be spawned as a child of the SpawnPoint GameObject.")]
        private bool _spawnAsChild = false;

        [SerializeField()]
        [OneOrMany()]
        [ResourceLink.Config(typeof(GameObject))]
        [Tooltip("Objects available for spawning. When spawn is called with no arguments a prefab is selected at random, unless a ISpawnSelector is available on the SpawnPoint.")]
        private ResourceLink[] _assets;

        [SerializeField()]
        private Trigger _onSpawnedObject = new Trigger(TRG_ONSPAWNED);

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

        public ResourceLink[] Assets
        {
            get { return _assets; }
            set { _assets = value; }
        }

        public int AssetCount
        {
            get { return _assets != null ? _assets.Length : 0; }
        }

        #endregion

        #region Methods

        public GameObject Spawn(int index)
        {
            if (!this.enabled) return null;

            if (_assets == null || index < 0 || index >= _assets.Length) return null;
            return this.DoSpawn(_assets[index]);
        }
        
        public GameObject[] GetActiveGameObjects()
        {
            return _spawnMechanism.GetActiveGameObjects();
        }

        public SpawnedObjectController[] GetActiveObjectControllers()
        {
            return _spawnMechanism.GetActiveObjectControllers();
        }




        private GameObject DoSpawn(ResourceLink link)
        {
            var prefab = link.GetResource<GameObject>();
            if (prefab == null) return null;

            var go = _spawnMechanism.Spawn(prefab, this.transform.position, this.transform.rotation, (_spawnAsChild) ? this.transform : null);

            if (_onSpawnedObject != null && _onSpawnedObject.Count > 0)
                _onSpawnedObject.ActivateTrigger(this, go);

            return go;
        }

        #endregion





        #region ISpawner Interface

        public int TotalCount { get { return _spawnMechanism.TotalCount; } }

        public int ActiveCount { get { return _spawnMechanism.ActiveCount; } }

        public GameObject Spawn()
        {
            if (!this.enabled) return null;
            
            int index = SpawnPointHelper.SelectFromMultiple(this, this.AssetCount);
            if (index < 0 || index >= this.AssetCount) return null;

            return this.DoSpawn(_assets[index]);
        }

        void ISpawner.Spawn()
        {
            this.Spawn();
        }

        #endregion

        #region ITriggerable Interface

        public override bool CanTrigger
        {
            get { return base.CanTrigger && _assets != null && _assets.Length > 0; }
        }

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            if (arg is string && string.Equals(arg as string, TRIGGERARG_RANDOM, System.StringComparison.OrdinalIgnoreCase))
            {
                return this.Spawn(Random.Range(0, this.AssetCount)) != null;
            }
            else if (ConvertUtil.ValueIsNumericType(arg))
            {
                return this.Spawn(ConvertUtil.ToInt(arg)) != null;
            }
            else
            {
                return this.Spawn() != null;
            }
        }

        #endregion

        #region IObserverableTarget Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onSpawnedObject };
        }

        #endregion

    }
}
