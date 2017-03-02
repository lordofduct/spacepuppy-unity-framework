using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public abstract class AutoTriggerComponent : TriggerComponent
    {
        
        #region Fields

        [SerializeField()]
        private ActivateEvent _activateOn = ActivateEvent.OnStartAndEnable;
        
        #endregion

        #region CONSTRUCTOR
        
        protected override void Start()
        {
            base.Start();

            if(_activateOn.HasFlag(ActivateEvent.OnStart))
            {
                this.OnTriggerActivate();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_activateOn.HasFlag(ActivateEvent.OnStart) && !this.started) return;

            if(_activateOn.HasFlag(ActivateEvent.OnEnable))
            {
                this.OnTriggerActivate();
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

        #region Methods

        /// <summary>
        /// Override this to start the trigger sequence on start/enable, depending configuration.
        /// </summary>
        protected abstract void OnTriggerActivate();
        
        #endregion
        
    }
}
