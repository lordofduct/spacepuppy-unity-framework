using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{
    [RequireComponent(typeof(i_Spawner))]
    public class RestrictSpawnCount : SPComponent, ISpawnerModifier, IObservableTrigger
    {

        public const string TRG_MAXACTIVECOUNTREACHED = "OnMaxActiveCountReached";
        public const string TRG_MAXCOUNTREACHED = "OnMaxCountReached";

        #region Fields

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("order")]
        private int _order;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("MaxCount")]
        [DiscreteFloat.NonNegative()]
        private DiscreteFloat _maxCount = 1;
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("MaxActiveCount")]
        [DiscreteFloat.NonNegative()]
        private DiscreteFloat _maxActiveCount = 1;

        [SerializeField()]
        private Trigger _onMaxActiveCountReached = new Trigger(TRG_MAXACTIVECOUNTREACHED);
        [SerializeField()]
        private Trigger _onMaxCountReached = new Trigger(TRG_MAXCOUNTREACHED);

        [System.NonSerialized()]
        private bool _maxActiveReachedTriggered = false;
        [System.NonSerialized()]
        private bool _maxCountReachedTriggered = false;

        #endregion

        #region CONSTRUCTOR

        protected override void OnEnable()
        {
            base.OnEnable();

            SpawnPointHelper.RegisterModifierWithSpawners(this.gameObject, this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SpawnPointHelper.UnRegisterModifierWithSpawners(this.gameObject, this);
        }

        #endregion

        #region Properties

        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public DiscreteFloat MaxCount
        {
            get { return _maxCount; }
            set { _maxCount = (value < 0f) ? DiscreteFloat.Zero : value; }
        }

        public DiscreteFloat MaxActivateCount
        {
            get { return _maxActiveCount; }
            set { _maxActiveCount = (value < 0f) ? DiscreteFloat.Zero : value; }
        }

        #endregion

        #region ISpawnerModifier Interface

        int ISpawnerModifier.order
        {
            get { return _order; }
        }

        void ISpawnerModifier.OnBeforeSpawnNotification(SpawnPointBeforeSpawnNotification n)
        {
            if (!this.isActiveAndEnabled) return;

            if (n.SpawnPoint.TotalCount >= this._maxCount)
            {
                n.Cancelled = true;
            }
            else if (n.SpawnPoint.ActiveCount >= this._maxActiveCount)
            {
                n.Cancelled = true;
            }
            else if(_maxActiveReachedTriggered)
            {
                _maxActiveReachedTriggered = false;
            }
        }

        void ISpawnerModifier.OnSpawnedNotification(SpawnPointTriggeredNotification n)
        {
            if (!this.isActiveAndEnabled) return;

            if (!_maxActiveReachedTriggered && n.SpawnPoint.ActiveCount == this._maxActiveCount)
            {
                _maxActiveReachedTriggered = true;
                _onMaxActiveCountReached.ActivateTrigger(this, null);
            }
            if (!_maxCountReachedTriggered && n.SpawnPoint.TotalCount == this._maxCount)
            {
                _maxCountReachedTriggered = true;
                _onMaxCountReached.ActivateTrigger(this, null);
            }
        }

        #endregion

        #region IObservableTrigger Interface
        
        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onMaxActiveCountReached, _onMaxCountReached };
        }

        #endregion

    }
}
