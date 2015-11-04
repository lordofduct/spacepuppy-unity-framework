using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_Trigger : SPNotifyingComponent, ITriggerableMechanism, IObservableTrigger
    {

        #region Fields

        [SerializeField()]
        private int _order;

        [SerializeField()]
        private Trigger _trigger = new Trigger();

        public bool PassAlongTriggerArg;
        public float Delay = 0f;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _trigger.ObservableTriggerOwner = this;
            _trigger.ObservableTriggerId = "i_Trigger";
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

        private void DoTriggerNext(object arg)
        {
            if (this.PassAlongTriggerArg)
                _trigger.ActivateTrigger(arg);
            else
                _trigger.ActivateTrigger();
        }

        #endregion

        #region ITriggerableMechanism Interface

        public int Order
        {
            get { return _order; }
        }

        public bool CanTrigger
        {
            get { return this.enabled; }
        }

        public void ActivateTrigger()
        {
            this.ActivateTrigger(null);
        }
        
        public bool ActivateTrigger(object arg)
        {
            if (!this.CanTrigger) return false;
            
            if (this.Delay > 0f)
            {
                this.Invoke(() =>
                {
                    this.DoTriggerNext(arg);
                }, this.Delay);
            }
            else
            {
                this.DoTriggerNext(arg);
            }

            return true;
        }

        void ITriggerableMechanism.Trigger()
        {
            this.ActivateTrigger(null);
        }

        bool ITriggerableMechanism.Trigger(object arg)
        {
            return this.ActivateTrigger(arg);
        }

        #endregion

        #region IObservableTrigger Interface

        bool IObservableTrigger.IsComplex
        {
            get { return false; }
        }

        string[] IObservableTrigger.GetComplexIds()
        {
            return new string[] { "i_Trigger" };
        }

        #endregion

    }
}
