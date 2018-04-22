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

        [SerializeField]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Object))]
        private TriggerableTargetObject _sourceAlt;

        [SerializeField]
        [EnumPopupExcluding((int)TweenHash.AnimMode.AnimCurve, (int)TweenHash.AnimMode.Curve)]
        private TweenHash.AnimMode _mode;
        [SerializeField()]
        private EaseStyle _ease;
        [SerializeField()]
        public SPTimePeriod _duration;

        [SerializeField()]
        [Tooltip("Leave blank for tweens to be unique to this component.")]
        private string _tweenToken;

        [SerializeField()]
        private Trigger _onComplete;

        [SerializeField()]
        private Trigger _onTick;

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

        public TriggerableTargetObject SourceAlt
        {
            get { return _sourceAlt; }
        }

        public TweenHash.AnimMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public EaseStyle Ease
        {
            get { return _ease; }
            set { _ease = value; }
        }

        public SPTimePeriod Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public string TweenToken
        {
            get { return _tweenToken; }
            set { _tweenToken = value; }
        }

        public Trigger OnComplete
        {
            get { return _onComplete; }
        }

        public Trigger OnTick
        {
            get { return _onTick; }
        }

        #endregion

        #region Triggerable Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = _target.GetTarget<object>(arg);
            var source = _source.GetTarget<object>(arg);

            var twn = SPTween.Tween(targ)
                             .TweenWithToken(_mode, EaseMethods.GetEase(_ease), source, _duration.Seconds, _sourceAlt)
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
