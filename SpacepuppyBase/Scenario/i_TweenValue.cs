using UnityEngine;

using com.spacepuppy.Tween;

namespace com.spacepuppy.Scenario
{
    public class i_TweenValue : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private SPTime _timeSupplier;

        [SerializeField()]
        [SelectableComponent()]
        private Component _target;

        [SerializeField()]
        private TweenData[] _data;

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

        protected override void OnDisable()
        {
            base.OnDisable();

            SPTween.KillAll(_target, _tweenToken);
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

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var twn = SPTween.Tween(_target);
            for (int i = 0; i < _data.Length; i++)
            {
                twn.ByAnimMode(_data[i].Mode, _data[i].MemberName, EaseMethods.GetEase(_data[i].Ease), _data[i].ValueS.Value, _data[i].Duration, _data[i].ValueE.Value);
            }
            twn.Use(_timeSupplier.TimeSupplier);
            twn.SetId(_target);
            twn.AutoKill(_tweenToken);

            if (_onComplete.Count > 0)
                twn.OnFinish((t) => _onComplete.ActivateTrigger());

            if (_onTick.Count > 0)
                twn.OnStep((t) => _onTick.ActivateTrigger());

            twn.Play();
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
        }

        #endregion

    }
}
