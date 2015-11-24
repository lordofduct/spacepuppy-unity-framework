using UnityEngine;

using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Scenario
{

    public class i_ModifyCollisionBetweenEntities : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(SPEntity))]
        private TriggerableTargetObject _targetA;

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(SPEntity))]
        private TriggerableTargetObject _targetB;

        [SerializeField()]
        private bool _ignoreCollision;

        #endregion
        
        #region ITriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var a = _targetA.GetTarget<SPEntity>(arg);
            var b = _targetB.GetTarget<SPEntity>(arg);

            if (a == null || b == null || a == b) return false;

            a.IgnoreCollision(b, _ignoreCollision);

            return true;
        }

        #endregion

    }

}
