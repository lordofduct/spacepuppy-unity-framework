using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_SignalEvent : AutoTriggerableMechanism
    {

        private System.Action<string> _signal;

        public void WaitOne(System.Action<string> callback)
        {
            _signal += callback;
        }

        public void Signal(string token)
        {
            var d = _signal;
            _signal = null;
            if (d != null) d(token);
        }
        
        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            this.Signal(null);
            return true;
        }

    }

}
