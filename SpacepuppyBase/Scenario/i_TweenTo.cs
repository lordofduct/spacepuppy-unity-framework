#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;

using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_TweenTo : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(Transform))]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(Transform))]
        private TriggerableTargetObject _location = new TriggerableTargetObject();


        [SerializeField()]
        private EaseStyle _ease;
        [SerializeField()]
        private SPTimePeriod _duration;

        [SerializeField]
        private bool _orientWithLocationRotation;

        [SerializeField()]
        private bool _tweenEntireEntity;

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

            if (string.IsNullOrEmpty(_tweenToken)) _tweenToken = "i_Tween*" + this.GetInstanceID().ToString();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SPTween.KillAll(_target, _tweenToken);
        }

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public TriggerableTargetObject Location
        {
            get { return _location; }
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

        public bool OrientWithLocationRotation
        {
            get { return _orientWithLocationRotation; }
            set { _orientWithLocationRotation = value; }
        }

        public bool TweenEntireEntity
        {
            get { return _tweenEntireEntity; }
            set { _tweenEntireEntity = value; }
        }

        public Trigger OnComplete
        {
            get { return _onComplete; }
        }

        public Trigger OnTick
        {
            get { return _onTick; }
        }

        public string TweenToken
        {
            get { return _tweenToken; }
            set { _tweenToken = value; }
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this._target.GetTarget<Transform>(arg);
            if (targ == null) return false;
            if (_tweenEntireEntity) targ = GameObjectUtil.FindRoot(targ).transform;

            var loc = _location.GetTarget<Transform>(arg);
            if (targ == null || loc == null) return false;

            var twn = SPTween.Tween(targ);
            
            twn.To("position", _duration.Seconds, loc.position);
            if (_orientWithLocationRotation) twn.To("rotation", _duration.Seconds, loc.rotation);

            twn.Use(_duration.TimeSupplier);
            twn.SetId(_target);

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
