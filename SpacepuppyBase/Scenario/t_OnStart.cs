using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class t_OnStart : TriggerComponent
    {

        #region Fields

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        [TimeUnitsSelector()]
        private float _delay;
        [SerializeField]
        private SPTime _delayTimeSupplier;

        #endregion

        #region Properties

        //public float Delay
        //{
        //    get { return _delay; }
        //    set { _delay = value; }
        //}

        public SPTimePeriod Delay
        {
            get
            {
                return new SPTimePeriod(_delay, _delayTimeSupplier.TimeSupplierType, _delayTimeSupplier.CustomTimeSupplierName);
            }
            set
            {
                _delay = value.Seconds;
                _delayTimeSupplier = (SPTime)value;
            }
        }

        #endregion

        #region Messages

        protected override void Start()
        {
            base.Start();

            if (_delay > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    this.ActivateTrigger(this);
                }, _delay, _delayTimeSupplier.TimeSupplier);
            }
            else
            {
                this.ActivateTrigger(this);
            }
        }

        #endregion

    }

}
