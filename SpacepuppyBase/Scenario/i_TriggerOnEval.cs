#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_TriggerOnEval : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private ConditionBlock[] _conditions;

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

        #region ITriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (_conditions == null || _conditions.Length == 0) return false;

            bool result = false;
            if (!this._passAlongTriggerArg) arg = null;
            foreach (var c in _conditions)
            {
                if (c.Condition.BoolValue)
                {
                    if (_delay > 0f)
                    {
                        this.InvokeGuaranteed(() =>
                        {
                            c.Trigger.ActivateTrigger(this, arg);
                        }, _delay, _delayTimeSupplier.TimeSupplier);
                    }
                    else
                    {
                        c.Trigger.ActivateTrigger(this, arg);
                    }

                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        public class ConditionBlock
        {

            [SerializeField()]
            private VariantReference _condition = new VariantReference();
            [SerializeField()]
            private Trigger _trigger = new Trigger();


            public VariantReference Condition
            {
                get { return _condition; }
            }

            public Trigger Trigger
            {
                get { return _trigger; }
            }

        }

        #endregion

    }
}
