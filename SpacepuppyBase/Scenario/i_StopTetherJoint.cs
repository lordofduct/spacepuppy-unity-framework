using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_StopTetherJoint : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField]
        [TriggerableTargetObject.Config(typeof(Transform))]
        [Tooltip("Target that gets tethered to 'Link Target'.")]
        private TriggerableTargetObject _target;

        [SerializeField]
        [Tooltip("Destroy the tether instead of just disabling it.")]
        private bool _destroyTether;

        #endregion

        #region Triggerable Mechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            
            var targ = _target.GetTarget<Transform>(arg);
            if (targ == null) return false;

            var tether = targ.GetComponent<TetherJoint>();
            if (tether == null) return false;

            if (_destroyTether)
                ObjUtil.SmartDestroy(tether);
            else
                tether.enabled = false;

            return true;
        }

        #endregion

    }
}
