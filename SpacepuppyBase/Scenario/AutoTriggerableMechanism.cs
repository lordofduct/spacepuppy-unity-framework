using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public abstract class AutoTriggerableMechanism : TriggerableMechanism
    {
        
        #region Fields

        [SerializeField()]
        private ActivateEvent _activateOn = ActivateEvent.None;

        #endregion

        #region CONSTRUCTOR

        protected override void Start()
        {
            base.Start();

            if (_activateOn.HasFlag(ActivateEvent.OnStart))
            {
                this.Trigger(null);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_activateOn.HasFlag(ActivateEvent.OnStart) && !this.started) return;

            if (_activateOn.HasFlag(ActivateEvent.OnEnable))
            {
                this.Trigger(null);
            }
        }

        #endregion

        #region Properties

        public ActivateEvent ActivateOn
        {
            get { return _activateOn; }
            set { _activateOn = value; }
        }

        #endregion


    }
}
