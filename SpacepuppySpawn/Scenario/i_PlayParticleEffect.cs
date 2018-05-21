using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Spawn;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_PlayParticleEffect : TriggerableMechanism, ISpawner, IObservableTrigger
    {

        public const string TRG_ONFINISH = "OnSpawned";

        #region Fields
        
        [SerializeField()]
        private SelfTrackingSpawnerMechanism _spawnMechanism = new SelfTrackingSpawnerMechanism();

        [SerializeField()]
        [FormerlySerializedAs("EffectPrefab")]
        private ParticleSystem _effectPrefab;

        [SerializeField()]
        [TimeUnitsSelector()]
        [Tooltip("Delete particle effect after a duration. Leave 0 to use the 'duration' of the particle effect, or use negative value (-1) to never delete.")]
        [FormerlySerializedAs("Duration")]
        private float _duration;

        [SerializeField()]
        private bool _spawnAsChild = true;

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

        public ParticleSystem EffectsPrefab
        {
            get { return _effectPrefab; }
            set { _effectPrefab = value; }
        }

        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public bool SpawnAsChild
        {
            get { return _spawnAsChild; }
            set { _spawnAsChild = value; }
        }

        #endregion

        #region ISpawner Interface

        public SelfTrackingSpawnerMechanism Mechanism { get { return _spawnMechanism; } }

        public int TotalCount { get { return _spawnMechanism.TotalCount; } }

        public int ActiveCount { get { return _spawnMechanism.ActiveCount; } }

        public GameObject Spawn()
        {
            if (!this.enabled) return null;
            if (_effectPrefab == null) return null;

            var go = _spawnMechanism.Spawn(_effectPrefab.gameObject, this.transform.position, this.transform.rotation, (_spawnAsChild) ? this.transform : null);
            if (go == null) return null;

            var dur = (_duration == 0f) ? _effectPrefab.main.duration : _duration;
            if (dur > 0f && dur != float.PositiveInfinity)
            {
                GameLoopEntry.Hook.Invoke(go.Kill, dur);
            }

            if (_onSpawnedObject != null && _onSpawnedObject.Count > 0)
                _onSpawnedObject.ActivateTrigger(this, go);

            return go;
        }
        
        void ISpawner.Spawn()
        {
            this.Spawn();
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool CanTrigger
        {
            get { return base.CanTrigger && _effectPrefab != null; }
        }

        public override bool Trigger(object sender, object arg)
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

}