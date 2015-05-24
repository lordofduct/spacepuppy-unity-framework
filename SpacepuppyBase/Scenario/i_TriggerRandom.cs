using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_TriggerRandom : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [Trigger.Config(true)]
        private Trigger _targets;

        public bool PassAlongTriggerArg;
        public float Delay = 0f;

        #endregion

        #region Properties

        #endregion

        #region Methdos

        private void DoTriggerNext(object arg)
        {
            if (this.PassAlongTriggerArg)
                _targets.ActivateRandomTrigger(arg, true);
            else
                _targets.ActivateRandomTrigger(true);
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override object Trigger(object arg)
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

        #endregion

    }
}
