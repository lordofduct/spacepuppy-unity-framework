using UnityEngine;

using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Scenario
{
    public class i_SignalEvent : AutoTriggerableMechanism
    {

        public event System.EventHandler OnSignal;
        public event System.EventHandler OneUseOnSignal;
        
        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            if (this.OnSignal != null) this.OnSignal(this, System.EventArgs.Empty);
            if (this.OneUseOnSignal != null)
            {
                var ev = this.OneUseOnSignal;
                this.OneUseOnSignal = null;
                ev(this, System.EventArgs.Empty);
            }
            return true;
        }

    }
}
