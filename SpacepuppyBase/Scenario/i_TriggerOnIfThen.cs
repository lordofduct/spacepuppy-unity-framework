using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_TriggerOnIfThen : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private ConditionBlock[] _conditions;
        public bool PassAlongTriggerArg;
        public float Delay = 0f;

        #endregion


        #region ITriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (_conditions == null || _conditions.Length == 0) return false;

            if (!this.PassAlongTriggerArg) arg = null;
            foreach(var c in _conditions)
            {
                if (c.Condition.BoolValue)
                {
                    if (this.Delay > 0f)
                    {
                        this.Invoke(() =>
                        {
                            c.Trigger.ActivateTrigger(arg);
                        }, this.Delay);
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
