#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_TriggerOnIfThen : AutoTriggerableMechanism
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

        #endregion

        #region Properties

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (_conditions == null || _conditions.Length == 0) return false;

            if (!this._passAlongTriggerArg) arg = null;
            foreach(var c in _conditions)
            {
                if (c.Condition.BoolValue)
                {
                    if (_delay > 0f)
                    {
                        this.Invoke(() =>
                        {
                            c.Trigger.ActivateTrigger(arg);
                        }, _delay);
                    }
                    else
                    {
                        c.Trigger.ActivateTrigger(arg);
                    }
                    return true;
                }
            }

            return false;
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
