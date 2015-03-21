using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Tween
{
    public class ObjectTweener : Tweener
    {

        #region Fields

        private object _target;
        private Curve _curve;

        #endregion
        
        #region CONSTRUCTOR

        public ObjectTweener(object targ, Curve curve)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            if (curve == null) throw new System.ArgumentNullException("curve");
            if (curve.Tween != null) throw new System.ArgumentException("Tweener can only be created with an unregistered Curve.", "curve");

            _target = targ;
            _curve = curve;
            _curve.Init(this);
        }

        #endregion

        #region Properties

        public object Target { get { return _target; } }

        #endregion

        #region Tweener Interface

        protected override float GetPlayHeadLength()
        {
            return _curve.TotalTime;
        }

        protected override void DoUpdate(float dt, float t)
        {
            _curve.Update(_target, dt, t);
        }

        #endregion

    }
}
