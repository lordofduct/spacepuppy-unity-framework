#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Mecanim
{
    public class MecanimSpeedScaler : SPComponent
    {

        #region Fields

        [SerializeField()]
        [DefaultFromSelf()]
        private Animator _animator;
        [SerializeField()]
        private SPTime _timeSupplier;
        [SerializeField()]
        private float _speed;

        #endregion

        #region CONSTRUCTOR

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            if (!_timeSupplier.IsCustom) return;

            var ts = _timeSupplier.TimeSupplier as IScalableTimeSupplier;
            if (ts != null || _animator != null) ts.TimeScaleChanged += this.OnTimeScaleChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            var ts = _timeSupplier.TimeSupplier as IScalableTimeSupplier;
            if (ts != null) ts.TimeScaleChanged -= this.OnTimeScaleChanged;
        }

        #endregion

        #region Callbacks

        private void OnTimeScaleChanged(object sender, System.EventArgs e)
        {
            if(_animator == null)
            {
                var ts = _timeSupplier.TimeSupplier as IScalableTimeSupplier;
                if (ts != null) ts.TimeScaleChanged -= this.OnTimeScaleChanged;
                return;
            }

            _animator.speed = _speed * SPTime.GetInverseScale(_timeSupplier.TimeSupplier as ITimeSupplier);
        }

        #endregion

    }
}
