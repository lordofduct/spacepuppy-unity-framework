#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_TriggerRandom : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [Trigger.Config(Weighted=true)]
        private Trigger _targets;

        [SerializeField]
        private bool _selectOnlyActiveTargets;

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

        #region Properties

        public bool SelectOnlyActiveTargets
        {
            get { return _selectOnlyActiveTargets; }
            set { _selectOnlyActiveTargets = value; }
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

            if (_delay > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    if (this._passAlongTriggerArg)
                        _targets.ActivateRandomTrigger(this, arg, true, _selectOnlyActiveTargets);
                    else
                        _targets.ActivateRandomTrigger(this, null, true, _selectOnlyActiveTargets);
                }, _delay, _delayTimeSupplier.TimeSupplier);
            }
            else
            {
                if (this._passAlongTriggerArg)
                    _targets.ActivateRandomTrigger(this, arg, true, _selectOnlyActiveTargets);
                else
                    _targets.ActivateRandomTrigger(this, null, true, _selectOnlyActiveTargets);
            }

            return true;
        }

        #endregion

    }
}
