#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Tween;

namespace com.spacepuppy.Scenario
{
    public class i_TweenValue : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private SPTime _timeSupplier;

        [SerializeField()]
        [SelectableObject()]
        private UnityEngine.Object _target;

        [SerializeField()]
        private TweenData[] _data;

        [SerializeField()]
        private Trigger _onComplete;

        [SerializeField()]
        private Trigger _onTick;

        [SerializeField()]
        [Tooltip("Leave blank for tweens to be unique to this component.")]
        private string _tweenToken;

        [SerializeField]
        private bool _killOnDisable;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if(string.IsNullOrEmpty(_tweenToken)) _tweenToken = "i_Tween*" + this.GetInstanceID().ToString();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (_killOnDisable)
                SPTween.KillAll(_target, _tweenToken);
        }

        #endregion

        #region Properties

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

        #region Methods

        #endregion

        #region ITriggerable Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _target != null && _data.Length > 0;
            }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var twn = SPTween.Tween(_target);
            for (int i = 0; i < _data.Length; i++)
            {
                twn.ByAnimMode(_data[i].Mode, _data[i].MemberName, EaseMethods.GetEase(_data[i].Ease), _data[i].ValueS.Value, _data[i].Duration, _data[i].ValueE.Value, _data[i].Option);
            }
            twn.Use(_timeSupplier.TimeSupplier);
            twn.SetId(_target);

            if (_onComplete.Count > 0)
                twn.OnFinish((t) => _onComplete.ActivateTrigger(this, null));

            if (_onTick.Count > 0)
                twn.OnStep((t) => _onTick.ActivateTrigger(this, null));

            twn.Play(true, _tweenToken);
            return true;
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        public class TweenData
        {
            [SerializeField()]
            [EnumPopupExcluding((int)TweenHash.AnimMode.AnimCurve, (int)TweenHash.AnimMode.Curve)]
            public TweenHash.AnimMode Mode;
            [SerializeField()]
            public string MemberName;
            [SerializeField()]
            public EaseStyle Ease;
            [SerializeField()]
            public VariantReference ValueS;
            [SerializeField()]
            public VariantReference ValueE;
            [SerializeField()]
            [TimeUnitsSelector()]
            public float Duration;
            [SerializeField]
            public int Option;
        }

        #endregion

    }
}
