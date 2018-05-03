#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;

namespace com.spacepuppy.Scenario
{

    public class i_SetParent : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField]
        private TriggerableTargetObject _child;
        [SerializeField]
        private TriggerableTargetObject _parent;

        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var child = _child.GetTarget<Transform>(arg);
            var parent = _parent.GetTarget<Transform>(arg);

            if (child == null) return false;
            child.parent = parent;
            return true;
        }

        #endregion

    }

}
