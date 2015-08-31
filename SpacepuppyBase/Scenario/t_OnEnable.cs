using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_OnEnable : TriggerComponent
    {

        #region Fields

        public float Delay;

        #endregion

        #region Messages

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            if (this.Delay > 0f)
            {
                this.Invoke(() =>
                {
                    this.ActivateTrigger(this);
                }, this.Delay);
            }
            else
            {
                this.ActivateTrigger(this);
            }

        }

        #endregion


    }
}
