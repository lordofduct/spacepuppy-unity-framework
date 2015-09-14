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

    public enum EaseStyle : int
    {
        Linear = 0,
        LinearEaseIn = 1,
        LinearEaseOut = 2,
        LinearEaseInOut = 3,
        BackEaseIn = 4,
        BackEaseOut = 5,
        BackEaseInOut = 6,
        BounceEaseIn = 7,
        BounceEaseOut = 8,
        BounceEaseInOut = 9,
        CircleEaseIn = 10,
        CircleEaseOut = 11,
        CircleEaseInOut = 12,
        CubicEaseIn = 13,
        CubicEaseOut = 14,
        CubicEaseInOut = 15,
        ElasticEaseIn = 16,
        ElasticEaseOut = 17,
        ElasticEaseInOut = 18,
        ExpoEaseIn = 19,
        ExpoEaseOut = 20,
        ExpoEaseInOut = 21,
        QuadEaseIn = 22,
        QuadEaseOut = 23,
        QuadEaseInOut = 24,
        QuartEaseIn = 25,
        QuartEaseOut = 26,
        QuartEaseInOut = 27,
        QuintEaseIn = 28,
        QuintEaseOut = 29,
        QuintEaseInOut = 30,
        SineEaseIn = 31,
        SineEaseOut = 32,
        SineEaseInOut = 33,
        StrongEaseIn = 34,
        StrongEaseOut = 35,
        StrongEaseInOut = 36,
    }

    /// <summary>
    /// A set of easing methods, to see a visual representation you can check out:
    /// https://msdn.microsoft.com/en-us/library/vstudio/Ee308751%28v=VS.100%29.aspx
    /// </summary>
    public static class ConcreteEaseMethods
    {

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
        
    }

    /// <summary>
    /// References to Ease delegates for use, this avoids the garbage of the ease delegate.
    /// </summary>
    public static class EaseMethods
    {

        public static Ease GetEase(EaseStyle style)
        {
            switch (style)
            {
                case EaseStyle.Linear: return EaseMethods.LinearEaseNone;
                case EaseStyle.LinearEaseIn: return EaseMethods.LinearEaseIn;
                case EaseStyle.LinearEaseOut: return EaseMethods.LinearEaseOut;
                case EaseStyle.LinearEaseInOut: return EaseMethods.LinearEaseInOut;

                case EaseStyle.BackEaseIn: return EaseMethods.BackEaseIn;
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



        #region Back Ease

        private static Ease _backEaseIn;
        public static Ease BackEaseIn
        {
            get
            {
                if (_backEaseIn == null) _backEaseIn = ConcreteEaseMethods.BackEaseIn;
                return _backEaseIn;
            }
        }

        private static Ease _backEaseOut;
        public static Ease BackEaseOut
        {
            get
            {
                if (_backEaseOut == null) _backEaseOut = ConcreteEaseMethods.BackEaseOut;
                return _backEaseOut;
            }
        }

        private static Ease _backEaseInOut;
        public static Ease BackEaseInOut
        {
            get
            {
                if (_backEaseInOut == null) _backEaseInOut = ConcreteEaseMethods.BackEaseInOut;
                return _backEaseInOut;
            }
        }

        #endregion

        #region Bounce Ease

        private static Ease _bounceEaseIn;
        public static Ease BounceEaseIn
        {
            get
            {
                if (_bounceEaseIn == null) _bounceEaseIn = ConcreteEaseMethods.BounceEaseIn;
                return _bounceEaseIn;
            }
        }

        private static Ease _bounceEaseOut;
        public static Ease BounceEaseOut
        {
            get
            {
                if (_bounceEaseOut == null) _bounceEaseOut = ConcreteEaseMethods.BounceEaseOut;
                return _bounceEaseOut;
            }
        }

        private static Ease _bounceEaseInOut;
        public static Ease BounceEaseInOut
        {
            get
            {
                if (_bounceEaseInOut == null) _bounceEaseInOut = ConcreteEaseMethods.BounceEaseInOut;
                return _bounceEaseInOut;
            }
        }

        #endregion

        #region Circle Ease

        private static Ease _circleEaseIn;
        public static Ease CircleEaseIn
        {
            get
            {
                if (_circleEaseIn == null) _circleEaseIn = ConcreteEaseMethods.CircleEaseIn;
                return _circleEaseIn;
            }
        }

        private static Ease _circleEaseOut;
        public static Ease CircleEaseOut
        {
            get
            {
                if (_circleEaseOut == null) _circleEaseOut = ConcreteEaseMethods.CircleEaseOut;
                return _circleEaseOut;
            }
        }

        private static Ease _circleEaseInOut;
        public static Ease CircleEaseInOut
        {
            get
            {
                if (_circleEaseInOut == null) _circleEaseInOut = ConcreteEaseMethods.CircleEaseInOut;
                return _circleEaseInOut;
            }
        }

        #endregion

        #region Cubic Ease

        private static Ease _cubicEaseIn;
        public static Ease CubicEaseIn
        {
            get
            {
                if (_cubicEaseIn == null) _cubicEaseIn = ConcreteEaseMethods.CubicEaseIn;
                return _cubicEaseIn;
            }
        }

        private static Ease _cubicEaseOut;
        public static Ease CubicEaseOut
        {
            get
            {
                if (_cubicEaseOut == null) _cubicEaseOut = ConcreteEaseMethods.CubicEaseOut;
                return _cubicEaseOut;
            }
        }

        private static Ease _cubicEaseInOut;
        public static Ease CubicEaseInOut
        {
            get
            {
                if (_cubicEaseInOut == null) _cubicEaseInOut = ConcreteEaseMethods.CubicEaseInOut;
                return _cubicEaseInOut;
            }
        }

        #endregion

        #region Elastic Ease

        private static Ease _elasticEaseIn;
        public static Ease ElasticEaseIn
        {
            get
            {
                if (_elasticEaseIn == null) _elasticEaseIn = ConcreteEaseMethods.ElasticEaseIn;
                return _elasticEaseIn;
            }
        }

        private static Ease _elasticEaseOut;
        public static Ease ElasticEaseOut
        {
            get
            {
                if (_elasticEaseOut == null) _elasticEaseOut = ConcreteEaseMethods.ElasticEaseOut;
                return _elasticEaseOut;
            }
        }

        private static Ease _elasticEaseInOut;
        public static Ease ElasticEaseInOut
        {
            get
            {
                if (_elasticEaseInOut == null) _elasticEaseInOut = ConcreteEaseMethods.ElasticEaseInOut;
                return _elasticEaseInOut;
            }
        }

        #endregion

        #region Expo Ease

        private static Ease _expoEaseIn;
        public static Ease ExpoEaseIn
        {
            get
            {
                if (_expoEaseIn == null) _expoEaseIn = ConcreteEaseMethods.ExpoEaseIn;
                return _expoEaseIn;
            }
        }

        private static Ease _expoEaseOut;
        public static Ease ExpoEaseOut
        {
            get
            {
                if (_expoEaseOut == null) _expoEaseOut = ConcreteEaseMethods.ExpoEaseOut;
                return _expoEaseOut;
            }
        }

        private static Ease _expoEaseInOut;
        public static Ease ExpoEaseInOut
        {
            get
            {
                if (_expoEaseInOut == null) _expoEaseInOut = ConcreteEaseMethods.ExpoEaseInOut;
                return _expoEaseInOut;
            }
        }

        #endregion

        #region Linear Ease

        private static Ease _linearEaseNone;
        public static Ease LinearEaseNone
        {
            get
            {
                if (_linearEaseNone == null) _linearEaseNone = ConcreteEaseMethods.LinearEaseNone;
                return _linearEaseNone;
            }
        }

        private static Ease _linearEaseIn;
        public static Ease LinearEaseIn
        {
            get
            {
                if (_linearEaseIn == null) _linearEaseIn = ConcreteEaseMethods.LinearEaseIn;
                return _linearEaseIn;
            }
        }

        private static Ease _linearEaseOut;
        public static Ease LinearEaseOut
        {
            get
            {
                if (_linearEaseOut == null) _linearEaseOut = ConcreteEaseMethods.LinearEaseOut;
                return _linearEaseOut;
            }
        }

        private static Ease _linearEaseInOut;
        public static Ease LinearEaseInOut
        {
            get
            {
                if (_linearEaseInOut == null) _linearEaseInOut = ConcreteEaseMethods.LinearEaseInOut;
                return _linearEaseInOut;
            }
        }

        #endregion

        #region Quad Ease

        private static Ease _quadEaseIn;
        public static Ease QuadEaseIn
        {
            get
            {
                if (_quadEaseIn == null) _quadEaseIn = ConcreteEaseMethods.QuadEaseIn;
                return _quadEaseIn;
            }
        }

        private static Ease _quadEaseOut;
        public static Ease QuadEaseOut
        {
            get
            {
                if (_quadEaseOut == null) _quadEaseOut = ConcreteEaseMethods.QuadEaseOut;
                return _quadEaseOut;
            }
        }

        private static Ease _quadEaseInOut;
        public static Ease QuadEaseInOut
        {
            get
            {
                if (_quadEaseInOut == null) _quadEaseInOut = ConcreteEaseMethods.QuadEaseInOut;
                return _quadEaseInOut;
            }
        }

        #endregion

        #region Quart Ease

        private static Ease _quartEaseIn;
        public static Ease QuartEaseIn
        {
            get
            {
                if (_quartEaseIn == null) _quartEaseIn = ConcreteEaseMethods.QuartEaseIn;
                return _quartEaseIn;
            }
        }

        private static Ease _quartEaseOut;
        public static Ease QuartEaseOut
        {
            get
            {
                if (_quartEaseOut == null) _quartEaseOut = ConcreteEaseMethods.QuartEaseOut;
                return _quartEaseOut;
            }
        }

        private static Ease _quartEaseInOut;
        public static Ease QuartEaseInOut
        {
            get
            {
                if (_quartEaseInOut == null) _quartEaseInOut = ConcreteEaseMethods.QuartEaseInOut;
                return _quartEaseInOut;
            }
        }

        #endregion

        #region Quint Ease

        private static Ease _quintEaseIn;
        public static Ease QuintEaseIn
        {
            get
            {
                if (_quintEaseIn == null) _quintEaseIn = ConcreteEaseMethods.QuintEaseIn;
                return _quintEaseIn;
            }
        }

        private static Ease _quintEaseOut;
        public static Ease QuintEaseOut
        {
            get
            {
                if (_quintEaseOut == null) _quintEaseOut = ConcreteEaseMethods.QuintEaseOut;
                return _quintEaseOut;
            }
        }

        private static Ease _quintEaseInOut;
        public static Ease QuintEaseInOut
        {
            get
            {
                if (_quintEaseInOut == null) _quintEaseInOut = ConcreteEaseMethods.QuintEaseInOut;
                return _quintEaseInOut;
            }
        }

        #endregion

        #region Sine Ease

        private static Ease _sineEaseIn;
        public static Ease SineEaseIn
        {
            get
            {
                if (_sineEaseIn == null) _sineEaseIn = ConcreteEaseMethods.SineEaseIn;
                return _sineEaseIn;
            }
        }

        private static Ease _sineEaseOut;
        public static Ease SineEaseOut
        {
            get
            {
                if (_sineEaseOut == null) _sineEaseOut = ConcreteEaseMethods.SineEaseOut;
                return _sineEaseOut;
            }
        }

        private static Ease _sineEaseInOut;
        public static Ease SineEaseInOut
        {
            get
            {
                if (_sineEaseInOut == null) _sineEaseInOut = ConcreteEaseMethods.SineEaseInOut;
                return _sineEaseInOut;
            }
        }

        #endregion

        #region Strong Ease

        private static Ease _strongEaseIn;
        public static Ease StrongEaseIn
        {
            get
            {
                if (_strongEaseIn == null) _strongEaseIn = ConcreteEaseMethods.StrongEaseIn;
                return _strongEaseIn;
            }
        }

        private static Ease _strongEaseOut;
        public static Ease StrongEaseOut
        {
            get
            {
                if (_strongEaseOut == null) _strongEaseOut = ConcreteEaseMethods.StrongEaseOut;
                return _strongEaseOut;
            }
        }

        private static Ease _strongEaseInOut;
        public static Ease StrongEaseInOut
        {
            get
            {
                if (_strongEaseInOut == null) _strongEaseInOut = ConcreteEaseMethods.StrongEaseInOut;
                return _strongEaseInOut;
            }
        }

        #endregion

        #region AnimationCurve

        public static Ease FromAnimationCurve(AnimationCurve curve)
        {
            return (c, s, e, d) =>
            {
                return curve.Evaluate(c);
            };
        }

        #endregion

        #region Configurable Cubic Bezier

        public static Ease CubicBezier(float p0, float p1, float p2, float p3)
        {
            return (c, s, e, d) =>
            {
                var t = c / d;
                var it = 1f - t;
                var r = (Mathf.Pow(it, 3f) * p0)
                      + (3f * Mathf.Pow(it, 2f) * t * p1)
                      + (3f * it * Mathf.Pow(t, 2f) * p2)
                      + (Mathf.Pow(t, 3f) * p3);
                return s + e * r;
            };
        }

        #endregion








        public static Vector2 EaseVector2(Ease ease, Vector2 start, Vector2 end, float t, float dur)
        {
            return (ease(t, 0, 1, dur) * (end - start)) + start;

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
