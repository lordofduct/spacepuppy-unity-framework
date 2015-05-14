using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_Trigger : TriggerComponent, ITriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private int _order;

        public bool PassAlongTriggerArg;
        public float Delay = 0f;

        #endregion

        #region Methods

        private void DoTriggerNext(object arg)
        {
            if (this.PassAlongTriggerArg)
                this.ActivateTrigger(arg);
            else
                this.ActivateTrigger();
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

        public new object Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            if(this.Delay > 0f)
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
