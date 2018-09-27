#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_EnableComponent : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(Component))]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Mode")]
        private EnableMode _mode;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        [TimeUnitsSelector()]
        private float _delay = 0f;
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

        #region Methods

        private void SetEnabledByMode(object arg)
        {
            var targ = _target.GetTarget<Component>(arg);

            switch (_mode)
            {
                case EnableMode.Enable:
                    targ.SetEnabled(true);
                    break;
                case EnableMode.Disable:
                    targ.SetEnabled(false);
                    break;
                case EnableMode.Toggle:
                    targ.SetEnabled(!targ.IsEnabled());
                    break;
            }
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            if (_delay > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    this.SetEnabledByMode(arg);
                }, _delay, _delayTimeSupplier.TimeSupplier);
            }
            else
            {
                this.SetEnabledByMode(arg);
            }

            return true;
        }

        #endregion

    }

}