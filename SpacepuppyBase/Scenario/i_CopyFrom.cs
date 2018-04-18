#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    
    public class i_CopyFrom : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField]
        private TriggerableTargetObject _target;

        [SerializeField]
        private TriggerableTargetObject _source;

        #endregion

        #region Triggerable Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = _target.GetTarget<object>(arg);
            var source = _source.GetTarget<object>(arg);

            DynamicUtil.CopyState(targ, source);

            return true;
        }

        #endregion

    }

}
