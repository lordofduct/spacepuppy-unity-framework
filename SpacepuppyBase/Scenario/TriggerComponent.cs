using UnityEngine;

namespace com.spacepuppy.Scenario
{

    public abstract class TriggerComponent : SPNotifyingComponent, IObservableTrigger
    {

        #region Fields

        [SerializeField()]
        private Trigger _trigger = new Trigger();

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
            
            _trigger.ObservableTriggerId = this.GetType().Name;
        }

        #endregion

        #region Properties

        public Trigger Trigger
        {
            get
            {
                return _trigger;
            }
        }

        #endregion

        #region Methods

        public virtual void ActivateTrigger()
        {
            _trigger.ActivateTrigger();
        }

        public virtual void ActivateTrigger(object arg)
        {
            _trigger.ActivateTrigger(arg);
        }

        #endregion

        #region IObservableTrigger Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _trigger };
        }

        #endregion

    }

}
