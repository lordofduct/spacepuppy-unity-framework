#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_Trigger : AutoTriggerableMechanism, IObservableTrigger
    {

        #region Fields
        
        [SerializeField()]
        private Trigger _trigger;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("PassAlongTriggerArg")]
        private bool _passAlongTriggerArg;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        [TimeUnitsSelector()]
        private float _delay = 0f;
        [SerializeField]
        private SPTime _delayTimeSupplier;
        
        #endregion

        #region CONSTRUCTOR
        
        #endregion

        #region Properties

        public Trigger TriggerEvent
        {
            get
            {
                return _trigger;
            }
        }

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        //public float Delay
        //{
        //    get { return _delay; }
        //    set { _delay = value; }
        //}

        public SPTimePeriod Delay
        {
            get
            {
                return new SPTimePeriod(_delay, _delayTimeSupplier);
            }
            set
            {
                _delay = value.Seconds;
                _delayTimeSupplier = (SPTime)value;
            }
        }
        
        #endregion

        #region Methods

        private void DoTriggerNext(object arg)
        {
            if (this._passAlongTriggerArg)
                _trigger.ActivateTrigger(this, arg);
            else
                _trigger.ActivateTrigger(this, null);
        }

        #endregion

        #region ITriggerableMechanism Interface
        
        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            if (_delay > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    this.DoTriggerNext(arg);
                }, _delay, _delayTimeSupplier.TimeSupplier);
            }
            else
            {
                this.DoTriggerNext(arg);
            }

            return true;
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
