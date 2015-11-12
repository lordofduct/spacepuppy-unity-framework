using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public abstract class AutoTriggerComponent : TriggerComponent
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
        private ActivateEventSettings _activateOn = ActivateEventSettings.OnStartAndEnable;
        
        #endregion

        #region CONSTRUCTOR
        
        protected override void Start()
        {
            base.Start();

            if(_activateOn.HasFlag(ActivateEventSettings.OnStart))
            {
                this.OnTriggerActivate();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_activateOn.HasFlag(ActivateEventSettings.OnStart) && !this.started) return;

            if(_activateOn.HasFlag(ActivateEventSettings.OnEnable))
            {
                this.OnTriggerActivate();
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

        #region Methods

        /// <summary>
        /// Override this to start the trigger sequence on start/enable, depending configuration.
        /// </summary>
        protected abstract void OnTriggerActivate();
        
        #endregion
        
    }
}
