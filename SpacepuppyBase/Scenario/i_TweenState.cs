#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_TweenState : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Object))]
        private TriggerableTargetObject _target;

        [SerializeField]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Object))]
        private TriggerableTargetObject _source;

        [SerializeField()]
        private EaseStyle _ease;
        [SerializeField()]
        public SPTimePeriod _duration;

        [SerializeField()]
        private Trigger _onComplete;

        [SerializeField()]
        private Trigger _onTick;

        [SerializeField()]
        [Tooltip("Leave blank for tweens to be unique to this component.")]
        private string _tweenToken;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if(string.IsNullOrEmpty(_tweenToken)) _tweenToken = "i_Tween*" + this.GetInstanceID().ToString();
        }

        /*
         * TODO - if want to kill these tweens, need to store each tween that was started. Can't kill all on '_target' since we changed over to TriggerableTargetObject.
         * 
        protected override void OnDisable()
        {
            base.OnDisable();

            SPTween.KillAll(_target, _tweenToken);
        }
        */

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public TriggerableTargetObject Source
        {
            get { return _source; }
        }

        #endregion

        #region Triggerable Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = _target.GetTarget<object>(arg);
            var source = _source.GetTarget<object>(arg);

            var twn = SPTween.Tween(targ)
                             .TweenToToken(source, EaseMethods.GetEase(_ease), _duration.Seconds)
                             .Use(_duration.TimeSupplier)
                             .SetId(targ);
            if (_onComplete.Count > 0)
                twn.OnFinish((t) => _onComplete.ActivateTrigger(this, null));

            if (_onTick.Count > 0)
                twn.OnStep((t) => _onTick.ActivateTrigger(this, null));
            
            twn.Play(true, _tweenToken);
            return true;
        }

        #endregion

    }

}
