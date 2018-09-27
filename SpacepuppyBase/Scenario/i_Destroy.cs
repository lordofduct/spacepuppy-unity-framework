using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_Destroy : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Target")]
        [TriggerableTargetObject.Config(typeof(UnityEngine.GameObject))]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        [TimeUnitsSelector()]
        private float _delay = 0f;
        [SerializeField]
        private SPTime _delayTimeSupplier;

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
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
                return new SPTimePeriod(_delay, _delayTimeSupplier.TimeSupplierType, _delayTimeSupplier.CustomTimeSupplierName);
            }
            set
            {
                _delay = value.Seconds;
                _delayTimeSupplier = (SPTime)value;
            }
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this._target.GetTarget<UnityEngine.Object>(arg);
            if (targ == null) return false;

            if (_delay > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    ObjUtil.SmartDestroy(targ);
                }, _delay, _delayTimeSupplier.TimeSupplier);
            }
            else
            {
                ObjUtil.SmartDestroy(targ);
            }

            return true;
        }

        #endregion

    }

}