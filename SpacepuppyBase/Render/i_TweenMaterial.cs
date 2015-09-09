using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Render
{
    public class i_TweenMaterial : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private SPTime _timeSupplier;

        [SerializeField()]
        [TimeUnitsSelector()]
        private float _duration;

        [SerializeField()]
        private EaseStyle _ease;

        [SerializeField()]
        [DisableOnPlay()]
        [EnumPopupExcluding((int)TweenHash.AnimMode.AnimCurve, (int)TweenHash.AnimMode.Curve, (int)TweenHash.AnimMode.By, (int)TweenHash.AnimMode.RedirectTo)]
        private TweenHash.AnimMode _mode;

        [SerializeField()]
        private MaterialTransition _transition;

        [SerializeField()]
        private Trigger _onComplete;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();


            switch (_mode)
            {
                case TweenHash.AnimMode.To:
                    _transition.Values.Insert(0, new VariantReference());
                    break;
                case TweenHash.AnimMode.From:
                    _transition.Values.Add(new VariantReference());
                    break;
            }

        }

        #endregion

        #region Tweener Update

        private void TweenUpdate(Tweener tween, float dt, float t)
        {
            _transition.Position = tween.PlayHeadTime / tween.PlayHeadLength;
        }

        #endregion

        #region ITriggerable Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _transition.Material != null && _transition.Values.Count != 0;
            }
        }

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            switch (_mode)
            {
                case TweenHash.AnimMode.To:
                    _transition.Values[0].Value = _transition.GetValue();
                    break;
                case TweenHash.AnimMode.From:
                    _transition.Values[_transition.Values.Count - 1].Value = _transition.GetValue();
                    break;
            }

            var twn = SPTween.Tween(this.TweenUpdate, _duration)
                              .Use(_timeSupplier.TimeSupplier)
                              .Ease(EaseMethods.GetEase(_ease))
                              .AutoKill(this.GetInstanceID());

            if (_onComplete.Count > 0)
                twn.OnFinish((t) => _onComplete.ActivateTrigger());

            twn.Play();

            return true;
        }

        #endregion

    }
}
