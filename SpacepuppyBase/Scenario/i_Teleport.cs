using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_Teleport : TriggerableMechanism
    {

        #region Fields

        [TriggerableTargetObject.Config(typeof(Transform))]
        public TriggerableTargetObject Target;

        [TriggerableTargetObject.Config(typeof(Transform))]
        public TriggerableTargetObject Location = new TriggerableTargetObject(TriggerableTargetObject.TargetSource.Configurable);

        public bool TeleportEntireEntity = true;

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            var targ = this.Target.GetTarget<Transform>(arg, this.TeleportEntireEntity);
            var loc = this.Location.GetTarget<Transform>(arg, false);
            if (targ == null || loc == null) return false;

            targ.position = loc.position;

            return true;
        }

        #endregion

    }

}