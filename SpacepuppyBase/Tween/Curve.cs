using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween
{

    public abstract class Curve
    {

        #region Fields

        private Tweener _tween;

        #endregion

        #region CONSTRUCTOR

        public Curve()
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

        public static Curve CreateFromTo(object target, string propName, Ease ease, object start, object end, float dur, object option = null)
        {
            try
            {
                var curve = ImplicitCurve.CreateFromTo(target, propName, ease, start, end, dur, option);
                if (curve != null) return curve;
            }
            catch(System.Exception ex)
            {
                Debug.LogWarning("Failed to create ImplicitCurve, see following exception for more details.");
                Debug.LogException(ex);
            }

            return MemberCurve.CreateFromTo(target, propName, ease, start, end, dur, option);
        }

        public static Curve CreateTo(object target, string propName, Ease ease, object end, float dur, object option = null)
        {
            try
            {
                var curve = ImplicitCurve.CreateTo(target, propName, ease, end, dur, option);
                if (curve != null) return curve;
            }
            catch(System.Exception ex)
            {
                Debug.LogWarning("Failed to create ImplicitCurve, see following exception for more details.");
                Debug.LogException(ex);
            }

            return MemberCurve.CreateTo(target, propName, ease, end, dur, option);
        }

        public static Curve CreateFrom(object target, string propName, Ease ease, object start, float dur, object option = null)
        {
            try
            {
                var curve = ImplicitCurve.CreateFrom(target, propName, ease, start, dur, option);
                if (curve != null) return curve;
            }
            catch(System.Exception ex)
            {
                Debug.LogWarning("Failed to create ImplicitCurve, see following exception for more details.");
                Debug.LogException(ex);
            }

            return MemberCurve.CreateFrom(target, propName, ease, start, dur, option);
        }

        public static Curve CreateBy(object target, string propName, Ease ease, object amt, float dur, object option = null)
        {
            try
            {
                var curve = ImplicitCurve.CreateBy(target, propName, ease, amt, dur, option);
                if (curve != null) return curve;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Failed to create ImplicitCurve, see following exception for more details.");
                Debug.LogException(ex);
            }

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
        public static Curve CreateRedirectTo(object target, string propName, Ease ease, float start, float end, float dur, object option = null)
        {
            try
            {
                var curve = ImplicitCurve.CreateRedirectTo(target, propName, ease, start, end, dur, option);
                if (curve != null) return curve;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Failed to create ImplicitCurve, see following exception for more details.");
                Debug.LogException(ex);
            }

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

    }

}
