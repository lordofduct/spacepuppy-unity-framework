using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween
{

    public abstract class TweenCurve
    {

        #region Fields

        private Tweener _tween;

        #endregion

        #region CONSTRUCTOR

        public TweenCurve()
        {

        }

        protected internal virtual void Init(Tweener twn)
        {
            if (_tween != null) throw new System.InvalidOperationException("Curve can only be registered with one Tweener at a time, and should not be doubly nested in any Curve collections.");
            _tween = twn;
        }

        #endregion

        #region Properties

        public Tweener Tween { get { return _tween; } }

        #endregion

        #region Methods



        #endregion

        #region Curve Interface

        /// <summary>
        /// The duration of this curve from beginning to end, including any delays.
        /// </summary>
        public abstract float TotalTime { get; }

        /// <summary>
        /// Updates the targ in an appropriate manner, if the targ is of a type that can be updated by this curve.
        /// </summary>
        /// <param name="dt">The change in time since last update.</param>
        /// <param name="t">A value from 0 to TotalDuration representing the position the curve aught to be at.</param>
        protected internal abstract void Update(object targ, float dt, float t);

        #endregion

        #region Factory

        private static NullCurve _null;
        public static TweenCurve Null
        {
            get
            {
                if (_null == null) _null = new NullCurve();
                return _null;
            }
        }



        public static TweenCurve CreateFromTo(object target, string propName, Ease ease, object start, object end, float dur, object option = null)
        {
            return MemberCurve.CreateFromTo(target, propName, ease, start, end, dur, option);
        }

        public static TweenCurve CreateTo(object target, string propName, Ease ease, object end, float dur, object option = null)
        {
            return MemberCurve.CreateTo(target, propName, ease, end, dur, option);
        }

        public static TweenCurve CreateFrom(object target, string propName, Ease ease, object start, float dur, object option = null)
        {
            return MemberCurve.CreateFrom(target, propName, ease, start, dur, option);
        }

        public static TweenCurve CreateBy(object target, string propName, Ease ease, object amt, float dur, object option = null)
        {
            return MemberCurve.CreateBy(target, propName, ease, amt, dur, option);
        }

        /// <summary>
        /// Creates a curve that will animate from the current value to the end value, but will rescale the duration from how long it should have 
        /// taken from start to end, but already animated up to current.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propName"></param>
        /// <param name="ease"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dur"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static TweenCurve CreateRedirectTo(object target, string propName, Ease ease, float start, float end, float dur, object option = null)
        {
            return MemberCurve.CreateRedirectTo(target, propName, ease, start, end, dur, option);
        }


        #endregion

        #region Internal Util

        internal static object TrySum(System.Type tp, object a, object b)
        {
            if (tp == null) return b;

            if (ConvertUtil.IsNumericType(tp))
            {
                return ConvertUtil.ToPrim(ConvertUtil.ToDouble(a) + ConvertUtil.ToDouble(b), tp);
            }
            else if (tp == typeof(Vector2))
            {
                return ConvertUtil.ToVector2(a) + ConvertUtil.ToVector2(b);
            }
            else if (tp == typeof(Vector3))
            {
                return ConvertUtil.ToVector3(a) + ConvertUtil.ToVector3(b);
            }
            else if (tp == typeof(Vector4))
            {
                return ConvertUtil.ToVector4(a) + ConvertUtil.ToVector4(b);
            }
            else if (tp == typeof(Quaternion))
            {
                return ConvertUtil.ToQuaternion(a) * ConvertUtil.ToQuaternion(b);
            }
            else if (tp == typeof(Color))
            {
                return ConvertUtil.ToColor(a) + ConvertUtil.ToColor(b);
            }
            else if (tp == typeof(Color32))
            {
                return ConvertUtil.ToColor32(ConvertUtil.ToColor(a) + ConvertUtil.ToColor(b));
            }

            return b;
        }

        #endregion


        #region Special Types

        private class NullCurve : TweenCurve
        {

            protected internal override void Init(Tweener twn)
            {
                //don't init
            }

            public override float TotalTime
            {
                get { return 0f; }
            }

            protected internal override void Update(object targ, float dt, float t)
            {
            }
        }

        #endregion

    }

}
