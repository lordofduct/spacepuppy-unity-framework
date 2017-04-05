#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [AddComponentMenu("SpacePuppy/Spawn/I_Attachment Spawn Point")]
    public class i_AttachmentSpawnPoint : TriggerableMechanism, ISpawner, IObservableTrigger
    {

        public const string TRG_ONFINISH = "OnSpawned";

        public enum StartupEvent
        {
            Never = -2,
            OnAwake = -1,
            OnFirstEnable = 0,
            OnEnable = 1,
            OnStart = 2,
            OnStartOrEnable = 3

        }

        #region Fields

        [SerializeField()]
        private SelfTrackingSpawnerMechanism _spawnMechanism = new SelfTrackingSpawnerMechanism();

        [SerializeField()]
        [OneOrMany()]
        [Tooltip("Objects available for spawning.")]
        private GameObject[] _prefabs = new GameObject[] { };

        [SerializeField()]
        [Tooltip("When should the attachment be created.\nOnAwake - this will occur no matter what, even if not enabled\nOnFirstEnable - during the enable event, the first time it's called\nOnEnable - any time the enable event occurs\nOnStart - during the start event\nOnStartOrEnable - during the start event, or any subsequent OnEnable event")]
        private StartupEvent _spawnOn;

        [SerializeField()]
        private Trigger _onSpawnedObject = new Trigger(TRG_ONFINISH);

        [System.NonSerialized()]
        private GameObject _spawnedAttachment;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _spawnMechanism.Init(this);

            if(_spawnOn == StartupEvent.OnAwake)
            {
                this.Spawn();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _spawnMechanism.Active = true;
            
            switch(_spawnOn)
            {
                case StartupEvent.OnFirstEnable:
                    if (!this.started)
                    {
                        this.Spawn();
                    }
                    break;
                case StartupEvent.OnEnable:
                    this.Spawn();
                    break;
                case StartupEvent.OnStartOrEnable:
                    if(this.started)
                    {
                        this.Spawn();
                    }
                    break;
            }

        }

        protected override void Start()
        {
            base.Start();

            switch(_spawnOn)
            {
                case StartupEvent.OnStart:
                case StartupEvent.OnStartOrEnable:
                    this.Spawn();
                    break;
            }
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

        #region ISpawner Interface
        
        public int TotalCount { get { return _spawnMechanism.TotalCount; } }

        public int ActiveCount { get { return _spawnMechanism.ActiveCount; } }

        public GameObject Spawn()
        {
            if (!this.enabled) return null;
            if (_spawnedAttachment != null) this.Despawn();

            int index = SpawnPointHelper.SelectFromMultiple(this, this.PrefabCount);
            if (index < 0 || index >= this.PrefabCount) return null;

            var prefab = _prefabs[index];
            _spawnedAttachment = _spawnMechanism.Spawn(prefab, this.transform.position, this.transform.rotation, this.transform);

            if (_onSpawnedObject != null && _onSpawnedObject.Count > 0)
                _onSpawnedObject.ActivateTrigger(this, _spawnedAttachment);

            return _spawnedAttachment;
        }

        void ISpawner.Spawn()
        {
            this.Spawn();
        }

        public void Despawn()
        {
            _spawnedAttachment.Kill();
            _spawnedAttachment = null;
        }

        #endregion
        
        #region ITriggerable Interface

        public override bool CanTrigger
        {
            get { return base.CanTrigger && _prefabs != null && _prefabs.Length > 0; }
        }

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            return this.Spawn() != null;
        }

        #endregion

        #region IObserverableTarget Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onSpawnedObject };
        }

        #endregion

    }

    [System.Obsolete("Use i_AttachmentSpawnPoint instead.")]
    public class AttachmentSpawnPoint : i_AttachmentSpawnPoint
    {

    }

}