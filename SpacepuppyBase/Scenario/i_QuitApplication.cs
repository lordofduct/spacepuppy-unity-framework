using System;
using UnityEngine;

namespace com.spacepuppy.Scenario
{
    public class i_QuitApplication : TriggerableMechanism
    {
        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            GameLoopEntry.QuitApplication();
            return GameLoopEntry.QuitState > QuitState.None;
        }
    }
}
