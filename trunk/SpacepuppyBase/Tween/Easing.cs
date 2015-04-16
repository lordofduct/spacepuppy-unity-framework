using UnityEngine;

using System;
using System.Collections.Generic;

namespace com.spacepuppy.Tween
{
    /// <summary>
    /// Represents an eased interpolation w/ respect to time.
    /// 
    /// float t, float b, float c, float d
    /// </summary>
    /// <param name="current">how long into the ease are we</param>
    /// <param name="initialValue">starting value if current were 0</param>
    /// <param name="totalChange">total change in the value (not the end value, but the end - start)</param>
    /// <param name="duration">the total amount of time (when current == duration, the returned value will == initial + totalChange)</param>
    /// <returns></returns>
    public delegate float Ease(float current, float initialValue, float totalChange, float duration);

    public enum EaseStyle
    {
        None = 0,
        BackEaseIn = 1,
        BackEaseOut = 2,
        BackEaseInOut = 3,
        BounceEaseIn = 4,
        BounceEaseOut = 5,
        BounceEaseInOut = 6,
        CircleEaseIn = 7,
        CircleEaseOut = 8,
        CircleEaseInOut = 9,
        CubicEaseIn = 10,
        CubicEaseOut = 11,
        CubicEaseInOut = 12,
        ElasticEaseIn = 13,
        ElasticEaseOut = 14,
        ElasticEaseInOut = 15,
        ExpoEaseIn = 16,
        ExpoEaseOut = 17,
        ExpoEaseInOut = 18,
        LinearEaseNone = 19,
        LinearEaseIn = 20,
        LinearEaseOut = 21,
        LinearEaseInOut = 22,
        QuadEaseIn = 23,
        QuadEaseOut = 24,
        QuadEaseInOut = 25,
        QuartEaseIn = 26,
        QuartEaseOut = 27,
        QuartEaseInOut = 28,
        QuintEaseIn = 29,
        QuintEaseOut = 30,
        QuintEaseInOut = 31,
        SineEaseIn = 32,
        SineEaseOut = 33,
        SineEaseInOut = 34,
        StrongEaseIn = 35,
        StrongEaseOut = 36,
        StrongEaseInOut = 37,
    }

    public static class EaseMethods
    {

        public static Ease GetEase(EaseStyle style)
        {
            switch (style)
            {
                case EaseStyle.BackEaseIn : return EaseMethods.BackEaseIn;
                case EaseStyle.BackEaseOut: return EaseMethods.BackEaseOut;
                case EaseStyle.BackEaseInOut: return EaseMethods.BackEaseInOut;

                case EaseStyle.BounceEaseIn: return EaseMethods.BounceEaseIn;
                case EaseStyle.BounceEaseOut: return EaseMethods.BounceEaseOut;
                case EaseStyle.BounceEaseInOut: return EaseMethods.BounceEaseInOut;

                case EaseStyle.CircleEaseIn: return EaseMethods.CircleEaseIn;
                case EaseStyle.CircleEaseOut: return EaseMethods.CircleEaseOut;
                case EaseStyle.CircleEaseInOut: return EaseMethods.CircleEaseInOut;

                case EaseStyle.CubicEaseIn: return EaseMethods.CubicEaseIn;
                case EaseStyle.CubicEaseOut: return EaseMethods.CubicEaseOut;
                case EaseStyle.CubicEaseInOut: return EaseMethods.CubicEaseInOut;

                case EaseStyle.ElasticEaseIn: return EaseMethods.ElasticEaseIn;
                case EaseStyle.ElasticEaseOut: return EaseMethods.ElasticEaseOut;
                case EaseStyle.ElasticEaseInOut: return EaseMethods.ElasticEaseInOut;

                case EaseStyle.ExpoEaseIn: return EaseMethods.ExpoEaseIn;
                case EaseStyle.ExpoEaseOut: return EaseMethods.ExpoEaseOut;
                case EaseStyle.ExpoEaseInOut: return EaseMethods.ExpoEaseInOut;

                case EaseStyle.LinearEaseNone: return EaseMethods.LinearEaseNone;
                case EaseStyle.LinearEaseIn: return EaseMethods.LinearEaseIn;
                case EaseStyle.LinearEaseOut: return EaseMethods.LinearEaseOut;
                case EaseStyle.LinearEaseInOut: return EaseMethods.LinearEaseInOut;

                case EaseStyle.QuadEaseIn: return EaseMethods.QuadEaseIn;
                case EaseStyle.QuadEaseOut: return EaseMethods.QuadEaseOut;
                case EaseStyle.QuadEaseInOut: return EaseMethods.QuadEaseInOut;

                case EaseStyle.QuartEaseIn: return EaseMethods.QuartEaseIn;
                case EaseStyle.QuartEaseOut: return EaseMethods.QuartEaseOut;
                case EaseStyle.QuartEaseInOut: return EaseMethods.QuartEaseInOut;

                case EaseStyle.QuintEaseIn: return EaseMethods.QuintEaseIn;
                case EaseStyle.QuintEaseOut: return EaseMethods.QuintEaseOut;
                case EaseStyle.QuintEaseInOut: return EaseMethods.QuintEaseInOut;

                case EaseStyle.SineEaseIn: return EaseMethods.SineEaseIn;
                case EaseStyle.SineEaseOut: return EaseMethods.SineEaseOut;
                case EaseStyle.SineEaseInOut: return EaseMethods.SineEaseInOut;

                case EaseStyle.StrongEaseIn: return EaseMethods.StrongEaseIn;
                case EaseStyle.StrongEaseOut: return EaseMethods.StrongEaseOut;
                case EaseStyle.StrongEaseInOut: return EaseMethods.StrongEaseInOut;
            }

            return null;
        }

        public static float EasedLerp(Ease ease, float from, float to, float t)
        {
            return ease(t, from, to - from, 1f);
        }

        private const float _2PI = 6.28318530717959f;
        private const float _HALF_PI = 1.5707963267949f;

        #region Back Ease
        public static float BackEaseIn(float t, float b, float c, float d)
        {
            return BackEaseInFull(t, b, c, d);
        }

        public static float BackEaseOut(float t, float b, float c, float d)
        {
            return BackEaseOutFull(t, b, c, d);
        }
        public static float BackEaseInOut(float t, float b, float c, float d)
        {
            return BackEaseInOutFull(t, b, c, d);
        }

        public static float BackEaseInFull(float t, float b, float c, float d, float s = 1.70158f)
        {
            return c * (t /= d) * t * ((s + 1) * t - s) + b;
        }

        public static float BackEaseOutFull(float t, float b, float c, float d, float s = 1.70158f)
        {
            return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
        }
        public static float BackEaseInOutFull(float t, float b, float c, float d, float s = 1.70158f)
        {
            if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
            return c / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
        }
        #endregion

        #region Bounce Ease
        public static float BounceEaseOut(float t, float b, float c, float d)
        {
            if ((t /= d) < (1 / 2.75f))
            {
                return c * (7.5625f * t * t) + b;
            }
            else if (t < (2 / 2.75))
            {
                return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + b;
            }
            else if (t < (2.5f / 2.75f))
            {
                return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + b;
            }
            else
            {
                return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + b;
            }
        }
        public static float BounceEaseIn(float t, float b, float c, float d)
        {
            return c - BounceEaseOut(d - t, 0, c, d) + b;
        }
        public static float BounceEaseInOut(float t, float b, float c, float d)
        {
            if (t < d / 2) return BounceEaseIn(t * 2, 0, c, d) * .5f + b;
            else return BounceEaseOut(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
        }
        #endregion

        #region Circle Ease
        public static float CircleEaseIn(float t, float b, float c, float d)
        {
            return -c * ((float)Math.Sqrt(1 - (t /= d) * t) - 1) + b;
        }
        public static float CircleEaseOut(float t, float b, float c, float d)
        {
            return c * (float)Math.Sqrt(1 - (t = t / d - 1) * t) + b;
        }
        public static float CircleEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return -c / 2 * ((float)Math.Sqrt(1 - t * t) - 1) + b;
            return c / 2 * ((float)Math.Sqrt(1 - (t -= 2) * t) + 1) + b;
        }
        #endregion

        #region Cubic Ease
        public static float CubicEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t + b;
        }
        public static float CubicEaseOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }
        public static float CubicEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }
        #endregion

        #region Elastic Ease
        public static float ElasticEaseIn(float t, float b, float c, float d)
        {
            return ElasticEaseInFull(t, b, c, d, 0, 0);
        }

        public static float ElasticEaseOut(float t, float b, float c, float d)
        {
            return ElasticEaseOutFull(t, b, c, d, 0, 0);
        }

        public static float ElasticEaseInOut(float t, float b, float c, float d)
        {
            return ElasticEaseInOutFull(t, b, c, d, 0, 0);
        }

        public static float ElasticEaseInFull(float t, float b, float c, float d, float a, float p)
        {
            float s;
            if (t == 0) return b; if ((t /= d) == 1) return b + c;
            if (p != 0) p = d * 0.3f;
            if (a != 0 || a < Math.Abs(c)) { a = c; s = p / 4; }
            else s = p / _2PI * (float)Math.Asin(c / a);
            return -(a * (float)Math.Pow(2, 10 * (t -= 1)) * (float)Math.Sin((t * d - s) * _2PI / p)) + b;
        }
        public static float ElasticEaseOutFull(float t, float b, float c, float d, float a = 0, float p = 0)
        {
            float s;
            if (t == 0) return b;
            if ((t /= d) == 1) return b + c;
            if (p != 0) p = d * 0.3f;
            if (a != 0 || a < Math.Abs(c)) { a = c; s = p / 4; }
            else s = p / _2PI * (float)Math.Asin(c / a);
            return (a * (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * d - s) * _2PI / p) + c + b);
        }
        public static float ElasticEaseInOutFull(float t, float b, float c, float d, float a = 0, float p = 0)
        {
            float s;
            if (t == 0) return b; if ((t /= d / 2) == 2) return b + c;
            if (p != 0) p = d * (0.3f * 1.5f);
            if (a != 0 || a < Math.Abs(c)) { a = c; s = p / 4; }
            else s = p / _2PI * (float)Math.Asin(c / a);
            if (t < 1) return -.5f * (a * (float)Math.Pow(2, 10 * (t -= 1)) * (float)Math.Sin((t * d - s) * _2PI / p)) + b;
            return a * (float)Math.Pow(2, -10 * (t -= 1)) * (float)Math.Sin((t * d - s) * _2PI / p) * .5f + c + b;
        }
        #endregion

        #region Expo Ease
        public static float ExpoEaseIn(float t, float b, float c, float d)
        {
            return (t == 0) ? b : c * (float)Math.Pow(2, 10 * (t / d - 1)) + b - c * 0.001f;
        }
        public static float ExpoEaseOut(float t, float b, float c, float d)
        {
            return (t == d) ? b + c : c * (-(float)Math.Pow(2, -10 * t / d) + 1) + b;
        }
        public static float ExpoEaseInOut(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            if (t == d) return b + c;
            if ((t /= d / 2) < 1) return c / 2 * (float)Math.Pow(2, 10 * (t - 1)) + b;
            return c / 2 * (-(float)Math.Pow(2, -10 * --t) + 2) + b;
        }
        #endregion

        #region Linear Ease
        public static float LinearEaseNone(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        public static float LinearEaseIn(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        public static float LinearEaseOut(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        public static float LinearEaseInOut(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        #endregion

        #region Quad Ease
        public static float QuadEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t + b;
        }
        public static float QuadEaseOut(float t, float b, float c, float d)
        {
            return -c * (t /= d) * (t - 2) + b;
        }
        public static float QuadEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t + b;
            return -c / 2 * ((--t) * (t - 2) - 1) + b;
        }
        #endregion

        #region Quart Ease
        public static float QuartEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t + b;
        }
        public static float QuartEaseOut(float t, float b, float c, float d)
        {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }
        public static float QuartEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }
        #endregion

        #region Quint Ease
        public static float QuintEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t * t + b;
        }
        public static float QuintEaseOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }
        public static float QuintEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }
        #endregion

        #region Sine Ease
        public static float SineEaseIn(float t, float b, float c, float d)
        {
            return -c * (float)Math.Cos(t / d * _HALF_PI) + c + b;
        }
        public static float SineEaseOut(float t, float b, float c, float d)
        {
            return c * (float)Math.Sin(t / d * _HALF_PI) + b;
        }
        public static float SineEaseInOut(float t, float b, float c, float d)
        {
            return -c / 2 * ((float)Math.Cos(Math.PI * t / d) - 1) + b;
        }
        #endregion

        #region Strong Ease
        public static float StrongEaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t * t + b;
        }
        public static float StrongEaseOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }
        public static float StrongEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }
        #endregion



        public static Vector2 EaseVector2(Ease ease, Vector2 start, Vector2 end, float t, float dur)
        {
            return (ease(t, 0, 1,dur) * (end - start)) + start;

            //return new Vector2(ease(t, start.x, end.x - start.x, dur), ease(t, start.y, end.y - start.y, dur));
        }

        public static Vector3 EaseVector3(Ease ease, Vector3 start, Vector3 end, float t, float dur)
        {
            return (ease(t, 0, 1, dur) * (end - start)) + start;

            //return new Vector3(ease(t, start.x, end.x - start.x, dur), ease(t, start.y, end.y - start.y, dur), ease(t, start.z, end.z - start.z, dur));
        }

        public static Vector4 EaseVector3(Ease ease, Vector4 start, Vector4 end, float t, float dur)
        {
            return (ease(t, 0, 1, dur) * (end - start)) + start;

            //return new Vector4(ease(t, start.x, end.x - start.x, dur), ease(t, start.y, end.y - start.y, dur), ease(t, start.z, end.z - start.z, dur), ease(t, start.w, end.w - start.w, dur));
        }

        public static Quaternion EaseQuaternion(Ease ease, Quaternion start, Quaternion end, float t, float dur)
        {
            return Quaternion.Slerp(start, end, ease(t, 0, 1, dur));
        }
    }
}
