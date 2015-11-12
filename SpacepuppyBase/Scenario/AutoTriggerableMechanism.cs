using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public abstract class AutoTriggerableMechanism : TriggerableMechanism
    {

        [System.Flags()]
        public enum ActivateEventSettings
        {

            None = 0,
            OnStart = 1,
            OnEnable = 2,
            OnStartAndEnable = 3

        }

        #region Fields

        [SerializeField()]
        private ActivateEventSettings _activateOn = ActivateEventSettings.None;

        #endregion

        #region CONSTRUCTOR

        protected override void Start()
        {
            base.Start();

            if (_activateOn.HasFlag(ActivateEventSettings.OnStart))
            {
                this.Trigger();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_activateOn.HasFlag(ActivateEventSettings.OnStart) && !this.started) return;

            if (_activateOn.HasFlag(ActivateEventSettings.OnEnable))
            {
                this.Trigger();
            }
        }

        #endregion

        #region Properties

        public ActivateEventSettings ActivateOn
        {
            get { return _activateOn; }
            set { _activateOn = value; }
        }

        #endregion


    }
}
