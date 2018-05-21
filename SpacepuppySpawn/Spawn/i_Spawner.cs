using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [AddComponentMenu("SpacePuppy/Spawn/I_Spawner")]
    public class i_Spawner : TriggerableMechanism, ISpawner, IObservableTrigger
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
        [Tooltip("Objects available for spawning. When spawn is called with no arguments a prefab is selected at random, unless a ISpawnSelector is available on the SpawnPoint.")]
        private GameObject[] _prefabs;

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

        public GameObject[] Prefabs
        {
            get { return _prefabs; }
            set { _prefabs = value; }
        }

        public int PrefabCount
        {
            get { return (_prefabs != null) ? _prefabs.Length : 0; }
        }
        
        #endregion

        #region Methods

        public GameObject Spawn(int index)
        {
            if (!this.enabled) return null;

            if (_prefabs == null || index < 0 || index >= _prefabs.Length) return null;
            return this.Spawn(_prefabs[index]);
        }

        public GameObject Spawn(string name)
        {
            if (!this.enabled) return null;

            if (_prefabs == null) return null;
            for (int i = 0; i < _prefabs.Length; i++)
            {
                if (this.Prefabs[i].CompareName(name)) return this.Spawn(_prefabs[i]);
            }
            return null;
        }

        private GameObject Spawn(GameObject prefab)
        {
            var go = _spawnMechanism.Spawn(prefab, this.transform.position, this.transform.rotation, (_spawnAsChild) ? this.transform : null);

            if (_onSpawnedObject != null && _onSpawnedObject.Count > 0)
                _onSpawnedObject.ActivateTrigger(this, go);

            return go;
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

        #region ISpawner Interface

        public SelfTrackingSpawnerMechanism Mechanism { get { return _spawnMechanism; } }

        public int TotalCount { get { return _spawnMechanism.TotalCount; } }

        public int ActiveCount { get { return _spawnMechanism.ActiveCount; } }

        public GameObject Spawn()
        {
            if (!this.enabled) return null;

            int index = SpawnPointHelper.SelectFromMultiple(this, this.PrefabCount);
            if (index < 0 || index >= this.PrefabCount) return null;
            
            return this.Spawn(_prefabs[index]);
        }

        void ISpawner.Spawn()
        {
            this.Spawn();
        }

        #endregion

        #region ITriggerable Interface

        public override bool CanTrigger
        {
            get { return base.CanTrigger && _prefabs != null && _prefabs.Length > 0; }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            if (arg is string)
            {
                switch ((arg as string).ToUpper())
                {
                    case TRIGGERARG_RANDOM:
                        return this.Spawn(Random.Range(0, this.PrefabCount)) != null;
                    default:
                        return this.Spawn(arg as string) != null;
                }
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

        #region INotificationDispatcher Interface

        [System.NonSerialized]
        private NotificationDispatcher _observers;
        
        protected virtual void OnDespawn()
        {
            if (_observers != null) _observers.PurgeHandlers();
        }
        
        public NotificationDispatcher Observers
        {
            get
            {
                if (_observers == null) _observers = new NotificationDispatcher(this);
                return _observers;
            }
        }

        #endregion

    }


    [System.Obsolete("Use i_Spawner instead.")]
    public class Spawner : i_Spawner
    {

    }

}