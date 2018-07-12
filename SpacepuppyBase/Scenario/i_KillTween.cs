#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Tween;

namespace com.spacepuppy.Scenario
{

    public class i_KillTween : AutoTriggerableMechanism
    {

        #region Properties

        [SerializeField()]
        [SelectableObject()]
        private UnityEngine.Object _target;

        [SerializeField()]
        [Tooltip("Leave blank to kill all associated with target.")]
        private string _tweenToken;

        #endregion

        #region ITriggerable Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            if (string.IsNullOrEmpty(_tweenToken))
                SPTween.KillAll(_target);
            else
                SPTween.KillAll(_target, _tweenToken);

            return true;
        }

        #endregion

    }

}
