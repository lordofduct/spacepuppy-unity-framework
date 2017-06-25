using System;
using System.Collections.Generic;

namespace com.spacepuppy.Utils
{
    /// <summary>
    /// A port of the LoDMath static member class written in AS3 under the MIT license agreement.
    /// 
    /// A collection of math functions that can be very useful for many things.
    /// 
    /// 
    /// As per the license agrrement of the lodGameBox license agreement
    /// 
    /// Copyright (c) 2009 Dylan Engelman
    ///
    ///Permission is hereby granted, free of charge, to any person obtaining a copy
    ///of this software and associated documentation files (the "Software"), to deal
    ///in the Software without restriction, including without limitation the rights
    ///to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ///copies of the Software, and to permit persons to whom the Software is
    ///furnished to do so, subject to the following conditions:
    ///
    ///The above copyright notice and this permission notice shall be included in
    ///all copies or substantial portions of the Software.
    ///
    ///THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ///IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ///FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ///AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ///LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ///OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    ///THE SOFTWARE.
    /// 
    /// http://code.google.com/p/lodgamebox/source/browse/trunk/com/lordofduct/util/LoDMath.as
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public static class MathUtil
    {

        #region "Public ReadOnly Properties"

        // Number pi
        public const float PI = 3.14159265358979f;
        // PI / 2 OR 90 deg
        public const float PI_2 = 1.5707963267949f;
        // PI / 2 OR 60 deg
        public const float PI_3 = 1.04719755119659666667f;
        // PI / 4 OR 45 deg
        public const float PI_4 = 0.785398163397448f;
        // PI / 8 OR 22.5 deg
        public const float PI_8 = 0.392699081698724f;
        // PI / 16 OR 11.25 deg
        public const float PI_16 = 0.196349540849362f;
        // 2 * PI OR 180 deg
        public const float TWO_PI = 6.28318530717959f;
        // 3 * PI_2 OR 270 deg
        public const float THREE_PI_2 = 4.71238898038469f;
        // Number e
        public const float E = 2.71828182845905f;
        // ln(10)
        public const float LN10 = 2.30258509299405f;
        // ln(2)
        public const float LN2 = 0.693147180559945f;
        // logB10(e)
        public const float LOG10E = 0.434294481903252f;
        // logB2(e)
        public const float LOG2E = 1.44269504088896f;
        // sqrt( 1 / 2 )
        public const float SQRT1_2 = 0.707106781186548f;
        // sqrt( 2 )
        public const float SQRT2 = 1.4142135623731f;
        // PI / 180
        public const float DEG_TO_RAD = 0.0174532925199433f;
        //  180.0 / PI
        public const float RAD_TO_DEG = 57.2957795130823f;

        // 2^16
        public const int B_16 = 65536;
        // 2^31
        public const long B_31 = 2147483648L;
        // 2^32
        public const long B_32 = 4294967296L;
        // 2^48
        public const long B_48 = 281474976710656L;
        // 2^53 !!NOTE!! largest accurate double floating point whole value
        public const long B_53 = 9007199254740992L;
        // 2^63
        public const ulong B_63 = 9223372036854775808;
        //18446744073709551615 or 2^64 - 1 or ULong.MaxValue...
        public const ulong B_64_m1 = ulong.MaxValue;

        //  1.0/3.0
        public const float ONE_THIRD = 0.333333333333333f;
        //  2.0/3.0
        public const float TWO_THIRDS = 0.666666666666667f;
        //  1.0/6.0
        public const float ONE_SIXTH = 0.166666666666667f;

        // COS( PI / 3 )
        public const float COS_PI_3 = 0.866025403784439f;
        //  SIN( 2*PI/3 )
        public const float SIN_2PI_3 = 0.03654595f;

        // 4*(Math.sqrt(2)-1)/3.0
        public const float CIRCLE_ALPHA = 0.552284749830793f;

        public const bool ONN = true;

        public const bool OFF = false;
        // round integer epsilon
        public const float SHORT_EPSILON = 0.1f;
        // percentage epsilon
        public const float PERC_EPSILON = 0.001f;
        // single float average epsilon
        public const float EPSILON = 0.0001f;
        public const float EPSILON_SQR = 0.0000001f;

        public static readonly float MACHINE_SNG_EPSILON = MathUtil.ComputeMachineEpsilon();

        public static float ComputeMachineEpsilon()
        {
            float fourThrids = 4.0f / 3.0f;
            float third = fourThrids - 1.0f;
            float one = third + third + third;
            return Math.Abs(1.0f - one);
        }

        #endregion

        #region "Public Shared Methods"

        public static bool IsReal(float f)
        {
            return !float.IsNaN(f) && !float.IsNegativeInfinity(f) && !float.IsPositiveInfinity(f);
        }

        public static bool IsReal(double f)
        {
            return !double.IsNaN(f) && !double.IsNegativeInfinity(f) && !double.IsPositiveInfinity(f);
        }
        
        /// <summary>
        /// This exists because Mathf.Sign returns 1 when the input value is 0. I'm personally used to the System.Math.Sign method that returns 0 on 0.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Sign(float value)
        {
            if (float.IsNaN(value)) return float.NaN;
            if (value == 0f) return 0f;
            return (value > 0f) ? 1.0f : -1.0f;
        }

        /// <summary>
        /// This is an overload of MathUtil.Sign that allows defining what is returned on 0. This is nice if you are checking if 0 was returned anyways and changing the value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueOnZero"></param>
        /// <returns></returns>
        public static float Sign(float value, float valueOnZero)
        {
            if (float.IsNaN(value)) return float.NaN;
            if (value == 0f) return valueOnZero;
            return (value > 0f) ? 1.0f : -1.0f;
        }

        /// <summary>
        /// Calculates the integral part of a float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Truncate(float value)
        {
            return Convert.ToSingle(Math.Truncate(value));
        }

        /// <summary>
        /// Returns the fractional part of a float.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Shear(float value)
        {
            return value % 1.0f;
        }

        /// <summary>
        /// Returns if the value is in between or equal to max and min
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool InRange(float value, float max, float min)
        {
            return (value >= min && value <= max);
        }

        public static bool InRange(float value, float max)
        {
            return InRange(value, max, 0);
        }

        public static bool InRange(int value, int max, int min)
        {
            return (value >= min && value <= max);
        }

        public static bool InRange(int value, int max)
        {
            return InRange(value, max, 0);
        }

        public static bool InRangeExclusive(float value, float max, float min)
        {
            return (value > min && value < max);
        }

        public static bool InRangeExclusive(float value, float max)
        {
            return InRangeExclusive(value, max, 0);
        }

        /// <summary>
        /// Returns if the value is a valid index in list of length 'bound'.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool InBounds(int value, int bound)
        {
            return (value >= 0 && value < bound);
        }

        public static bool IsPowerOfTwo(ulong value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }

        #region "series"

        /// <summary>
        /// Sums a series of numeric values passed as a param array...
        /// 
        /// MathUtil.Sum(1,2,3,4) == 10
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static short Sum(params short[] arr)
        {
            short result = 0;

            foreach (short value in arr)
            {
                result += value;
            }

            return result;
        }

        /// <summary>
        /// Sums a series of numeric values passed as a param array...
        /// 
        /// MathUtil.Sum(1,2,3,4) == 10
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int Sum(params int[] arr)
        {
            int result = 0;

            foreach (int value in arr)
            {
                result += value;
            }

            return result;
        }

        public static int Sum(int[] arr, int startIndex, int endIndex)
        {
            int result = 0;

            for (int i = startIndex; i <= Math.Min(endIndex, arr.Length - 1); i++)
            {
                result += arr[i];
            }

            return result;
        }

        public static int Sum(int[] arr, int startIndex)
        {
            return Sum(arr, startIndex, int.MaxValue);
        }

        /// <summary>
        /// Sums a series of numeric values passed as a param array...
        /// 
        /// MathUtil.Sum(1,2,3,4) == 10
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long Sum(params long[] arr)
        {
            long result = 0;

            foreach (long value in arr)
            {
                result += value;
            }

            return result;
        }

        /// <summary>
        /// Sums a series of numeric values passed as a param array...
        /// 
        /// MathUtil.Sum(1,2,3,4) == 10
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Sum(params float[] arr)
        {
            float result = 0;

            foreach (float value in arr)
            {
                result += value;
            }

            return result;
        }


        /// <summary>
        /// Multiplies a series of numeric values passed as a param array...
        /// 
        /// MathUtil.Product(2,3,4) == 24
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Product(params short[] arr)
        {
            if (arr == null || arr.Length == 0)
                return float.NaN;

            float result = 1;

            foreach (short value in arr)
            {
                result *= value;
            }

            return result;
        }

        /// <summary>
        /// Multiplies a series of numeric values passed as a param array...
        /// 
        /// MathUtil.Product(2,3,4) == 24
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Product(params int[] arr)
        {
            if (arr == null || arr.Length == 0)
                return float.NaN;

            float result = 1;

            foreach (int value in arr)
            {
                result *= value;
            }

            return result;
        }

        /// <summary>
        /// Multiplies a series of numeric values passed as a param array...
        /// 
        /// MathUtil.ProductSeries(2,3,4) == 24
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Product(params long[] arr)
        {
            if (arr == null || arr.Length == 0)
                return float.NaN;

            float result = 1;

            foreach (long value in arr)
            {
                result *= value;
            }

            return result;
        }

        /// <summary>
        /// Multiplies a series of numeric values passed as a param array...
        /// 
        /// MathUtil.ProductSeries(2,3,4) == 24
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Product(params float[] arr)
        {
            if (arr == null || arr.Length == 0)
                return float.NaN;

            float result = 1f;

            foreach (float value in arr)
            {
                result *= value;
            }

            return result;
        }

        public static float Product(this IEnumerable<float> coll)
        {
            if (coll == null) return float.NaN;

            float result = 1f;
            foreach(float value in coll)
            {
                result *= value;
            }
            return result;
        }

        #endregion

        #region "Value interpolating and warping"

        /// <summary>
        /// The average of an array of values
        /// </summary>
        /// <param name="values">An array of values</param>
        /// <returns>the average</returns>
        /// <remarks></remarks>
        public static float Average(params short[] values)
        {
            float avg = 0;

            foreach (float value in values)
            {
                avg += value;
            }

            return avg / values.Length;
        }

        public static float Average(params int[] values)
        {
            float avg = 0;

            foreach (float value in values)
            {
                avg += value;
            }

            return avg / values.Length;
        }

        public static float Average(params long[] values)
        {
            float avg = 0;

            foreach (float value in values)
            {
                avg += value;
            }

            return avg / values.Length;
        }

        public static float Average(float a, float b)
        {
            return (a + b) / 2f;
        }

        public static float Average(float a, float b, float c)
        {
            return (a + b + c) / 3f;
        }

        public static float Average(float a, float b, float c, float d)
        {
            return (a + b + c + d) / 4f;
        }

        public static float Average(params float[] values)
        {
            float avg = 0;

            foreach (float value in values)
            {
                avg += value;
            }

            return avg / values.Length;
        }

        /// <summary>
        /// a one dimensional linear interpolation of a value.
        /// </summary>
        /// <param name="a">from value</param>
        /// <param name="b">to value</param>
        /// <param name="weight">lerp value</param>
        /// <returns>the value lerped from a to b</returns>
        /// <remarks></remarks>
        public static float Interpolate(float a, float b, float weight)
        {
            return (b - a) * weight + a;
        }

        public static int Interpolate(int a, int b, float weight)
        {
            return (int)Math.Round((b - a) * weight + a);
        }

        public static float SpeedLerp(float a, float b, float speed, float dt)
        {
            var v = b - a;
            var dv = speed * dt;
            if (dv > Math.Abs(v))
                return b;
            else
                return a + Math.Sign(v) * dv;
        }

        public static float AccelerativeLerp(float a, float b, float accel, float dt)
        {
            if (b > a)
            {
                a += accel * dt;
                return (a > b) ? b : a;
            }
            else
            {
                a -= accel * dt;
                return (a < b) ? b : a;
            }
        }

        /// <summary>
        /// The percentage a value is from min to max
        /// 
        /// eg:
        /// 8 of 10 out of 0->10 would be 0.8f
        /// 
        /// Good for calculating the lerp weight
        /// </summary>
        /// <param name="value">The value to text</param>
        /// <param name="max">The max value</param>
        /// <param name="min">The min value</param>
        /// <returns>The percentage value is from min</returns>
        /// <remarks></remarks>
        public static float PercentageMinMax(float value, float max, float min)
        {
            value -= min;
            max -= min;

            if (max == 0f)
            {
                return 0f;
            }
            else
            {
                return value / max;
            }
        }

        public static float PercentageMinMax(float value, float max)
        {
            return PercentageMinMax(value, max, 0);
        }

        /// <summary>
        /// The percentage a value is from max to min
        /// 
        /// eg:
        /// 8 of 10 out of 0->10 would be 0.2f
        /// 
        /// Good for calculating a discount
        /// </summary>
        /// <param name="value">The value to text</param>
        /// <param name="max">The max value</param>
        /// <param name="min">The min value</param>
        /// <returns>The percentage value is from max</returns>
        /// <remarks></remarks>
        public static float PercentageOffMinMax(float value, float max, float min)
        {
            value -= max;
            min -= max;

            if (min == 0)
            {
                return 0;
            }
            else
            {
                return value / min;
            }
        }

        public static float PercentageOffMinMax(float value, float max)
        {
            return PercentageOffMinMax(value, max, 0);
        }

        /// <summary>
        /// Return the minimum value of several values
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Min(params float[] args)
        {
            if (args.Length == 0)
                return float.NaN;
            float value = args[0];

            for (int i = 0; i <= args.Length - 1; i++)
            {
                if (args[i] < value)
                    value = args[i];
            }

            return value;
        }

        public static short Min(params short[] args)
        {
            if (args.Length == 0)
                return 0;
            short value = args[0];

            for (int i = 0; i <= args.Length - 1; i++)
            {
                if (args[i] < value)
                    value = args[i];
            }

            return value;
        }

        public static int Min(params int[] args)
        {
            if (args.Length == 0)
                return 0;
            int value = args[0];

            for (int i = 0; i <= args.Length - 1; i++)
            {
                if (args[i] < value)
                    value = args[i];
            }

            return value;
        }

        public static long Min(params long[] args)
        {
            if (args.Length == 0)
                return 0;
            long value = args[0];

            for (int i = 0; i <= args.Length - 1; i++)
            {
                if (args[i] < value)
                    value = args[i];
            }

            return value;
        }

        /// <summary>
        /// Return the maximum of several values
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Max(params float[] args)
        {
            if (args.Length == 0)
                return float.NaN;
            float value = args[0];

            for (int i = 1; i <= args.Length - 1; i++)
            {
                if (args[i] > value)
                    value = args[i];
            }

            return value;
        }

        public static short Max(params short[] args)
        {
            if (args.Length == 0)
                return 0;
            short value = args[0];

            for (int i = 0; i <= args.Length - 1; i++)
            {
                if (args[i] > value)
                    value = args[i];
            }

            return value;
        }

        public static int Max(params int[] args)
        {
            if (args.Length == 0)
                return 0;
            int value = args[0];

            for (int i = 0; i <= args.Length - 1; i++)
            {
                if (args[i] > value)
                    value = args[i];
            }

            return value;
        }

        public static long Max(params long[] args)
        {
            if (args.Length == 0)
                return 0;
            long value = args[0];

            for (int i = 0; i <= args.Length - 1; i++)
            {
                if (args[i] > value)
                    value = args[i];
            }

            return value;
        }

        public static float Wrap01(float value)
        {
            return (float)((double)value - Math.Floor((double)value));
        }

        /// <summary>
        /// Wraps a value around some significant range.
        /// 
        /// Similar to modulo, but works in a unary direction over any range (including negative values).
        /// 
        /// ex:
        /// Wrap(8,6,2) == 4
        /// Wrap(4,2,0) == 0
        /// Wrap(4,2,-2) == -2
        /// </summary>
        /// <param name="value">value to wrap</param>
        /// <param name="max">max in range</param>
        /// <param name="min">min in range</param>
        /// <returns>A value wrapped around min to max</returns>
        /// <remarks></remarks>
        public static int Wrap(int value, int max, int min)
        {
            max -= min;
            if (max == 0)
                return min;

            return value - max * (int)Math.Floor((double)(value - min) / max);
        }
        public static int Wrap(int value, int max)
        {
            return Wrap(value, max, 0);
        }

        public static long Wrap(long value, long max, long min)
        {
            max -= min;
            if (max == 0)
                return min;

            return value - max * (long)Math.Floor((double)(value - min) / max);
        }
        public static long Wrap(long value, long max)
        {
            return Wrap(value, max, 0);
        }

        public static float Wrap(float value, float max, float min)
        {
            max -= min;
            if (max == 0)
                return min;

            return value - max * (float)Math.Floor((value - min) / max);


            // //old method using while loop, slow for large numbers
            //value -= min;
            //max -= min;
            //if (max == 0)
            //    return min;

            //value = value % max;
            //value += min;
            //while (value < min)
            //{
            //    value += max;
            //}

            //return value;

        }
        public static float Wrap(float value, float max)
        {
            return Wrap(value, max, 0);


            // //old method using while loop, slow for large numbers
            //float min = 0;

            //value -= min;
            //max -= min;
            //if (max == 0)
            //    return min;

            //value = (value % max);
            //value += min;
            //while (value < min)
            //{
            //    value += max;
            //}

            //return value;


        }

        /// <summary>
        /// Alt of Wrap, obsolete.
        /// </summary>
        /// <param name="value">value to wrap</param>
        /// <param name="max">max in range</param>
        /// <param name="min">min in range</param>
        /// <returns>A value wrapped around min to max</returns>
        /// <remarks></remarks>
        [System.Obsolete("Use MathUtil.Wrap")]
        public static float ArithWrap(float value, float max, float min)
        {
            max -= min;
            if (max == 0)
                return min;

            return value - max * (float)Math.Floor((value - min) / max);
        }
        [System.Obsolete("Use MathUtil.Wrap")]
        public static float ArithWrap(float value, float max)
        {
            return ArithWrap(value, max, 0);
        }

        /// <summary>
        /// Clamp a value into a range.
        /// 
        /// If input is LT min, min returned
        /// If input is GT max, max returned
        /// else input returned
        /// </summary>
        /// <param name="input">value to clamp</param>
        /// <param name="max">max in range</param>
        /// <param name="min">min in range</param>
        /// <returns>calmped value</returns>
        /// <remarks></remarks>
        public static short Clamp(short input, short max, short min)
        {
            return Math.Max(min, Math.Min(max, input));
        }
        public static short Clamp(short input, short max)
        {
            return Math.Max((short)0, Math.Min(max, input));
        }

        public static int Clamp(int input, int max, int min)
        {
            return Math.Max(min, Math.Min(max, input));
        }
        public static int Clamp(int input, int max)
        {
            return Math.Max(0, Math.Min(max, input));
        }

        public static long Clamp(long input, long max, long min)
        {
            return Math.Max(min, Math.Min(max, input));
        }
        public static long Clamp(long input, long max)
        {
            return Math.Max(0, Math.Min(max, input));
        }

        public static float Clamp(float input, float max, float min)
        {
            return Math.Max(min, Math.Min(max, input));
        }
        public static float Clamp(float input, float max)
        {
            return Math.Max(0, Math.Min(max, input));
        }

        public static float Clamp01(float input)
        {
            if (input < 0f) return 0f;
            else if (input > 1f) return 1f;
            else return input;
        }

        /// <summary>
        /// Ensures a value is within some range. If it doesn't fall in that range than some default value is returned.
        /// </summary>
        /// <param name="input">value to clamp</param>
        /// <param name="max">max in range</param>
        /// <param name="min">min in range</param>
        /// <param name="defaultValue">default value if not in range</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static short ClampOrDefault(short input, short max, short min, short defaultValue)
        {
            return input < min || input > max ? defaultValue : input;
        }

        public static int ClampOrDefault(int input, int max, int min, int defaultValue)
        {
            return input < min || input > max ? defaultValue : input;
        }

        public static long ClampOrDefault(long input, long max, long min, long defaultValue)
        {
            return input < min || input > max ? defaultValue : input;
        }

        public static float ClampOrDefault(float input, float max, float min, float defaultValue)
        {
            return input < min || input > max ? defaultValue : input;
        }

        public static int ClampOrDefault(int input, int defaultValue, Type enumType)
        {
            return Enum.IsDefined(enumType, input) ? input : defaultValue;
        }

        /// <summary>
        /// roundTo some place comparative to a 'base', default is 10 for decimal place
        /// 
        /// 'place' is represented by the power applied to 'base' to get that place
        /// </summary>
        /// <param name="value">the value to round</param>
        /// <param name="place">the place to round to</param>
        /// <param name="base">the base to round in... default is 10 for decimal</param>
        /// <returns>The value rounded</returns>
        /// <remarks>e.g.
        /// 
        /// 2000/7 ~= 285.714285714285714285714 ~= (bin)100011101.1011011011011011
        /// 
        /// roundTo(2000/7,-3) == 0
        /// roundTo(2000/7,-2) == 300
        /// roundTo(2000/7,-1) == 290
        /// roundTo(2000/7,0) == 286
        /// roundTo(2000/7,1) == 285.7
        /// roundTo(2000/7,2) == 285.71
        /// roundTo(2000/7,3) == 285.714
        /// roundTo(2000/7,4) == 285.7143
        /// roundTo(2000/7,5) == 285.71429
        /// 
        /// roundTo(2000/7,-3,2)  == 288       -- 100100000
        /// roundTo(2000/7,-2,2)  == 284       -- 100011100
        /// roundTo(2000/7,-1,2)  == 286       -- 100011110
        /// roundTo(2000/7,0,2)  == 286       -- 100011110
        /// roundTo(2000/7,1,2) == 285.5     -- 100011101.1
        /// roundTo(2000/7,2,2) == 285.75    -- 100011101.11
        /// roundTo(2000/7,3,2) == 285.75    -- 100011101.11
        /// roundTo(2000/7,4,2) == 285.6875  -- 100011101.1011
        /// roundTo(2000/7,5,2) == 285.71875 -- 100011101.10111
        /// 
        /// note what occurs when we round to the 3rd space (8ths place), 100100000, this is to be assumed 
        /// because we are rounding 100011.1011011011011011 which rounds up.</remarks>
        public static float RoundTo(float value, int place, uint @base)
        {
            if (place == 0)
            {
                //'if zero no reason going through the math hoops
                return (float)Math.Round(value);
            }
            else if (@base == 10 && place > 0 && place <= 15)
            {
                //'Math.Round has a rounding to decimal spaces that is very efficient
                //'only useful for base 10 if places are from 1 to 15
                return (float)Math.Round(value, place);
            }
            else
            {
                float p = (float)Math.Pow(@base, place);
                return (float)Math.Round(value * p) / p;
            }
        }

        public static float RoundTo(float value, int place)
        {
            return RoundTo(value, place, 10);
        }

        public static float RoundTo(float value)
        {
            return RoundTo(value, 0, 10);
        }

        /// <summary>
        /// FloorTo some place comparative to a 'base', default is 10 for decimal place
        /// 
        /// 'place' is represented by the power applied to 'base' to get that place
        /// </summary>
        /// <param name="value"></param>
        /// <param name="place"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float FloorTo(float value, int place, uint @base)
        {
            if (place == 0)
            {
                //'if zero no reason going through the math hoops
                return (float)Math.Floor(value);
            }
            else
            {
                float p = (float)Math.Pow(@base, place);
                return (float)Math.Floor(value * p) / p;
            }
        }

        public static float FloorTo(float value, int place)
        {
            return FloorTo(value, place, 10);
        }

        public static float FloorTo(float value)
        {
            return FloorTo(value, 0, 10);
        }

        /// <summary>
        /// CeilTo some place comparative to a 'base', default is 10 for decimal place
        /// 
        /// 'place' is represented by the power applied to 'base' to get that place
        /// </summary>
        /// <param name="value"></param>
        /// <param name="place"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float CeilTo(float value, int place, uint @base)
        {
            if (place == 0)
            {
                //'if zero no reason going through the math hoops
                return (float)Math.Ceiling(value);
            }
            else
            {
                float p = (float)Math.Pow(@base, place);
                return (float)Math.Ceiling(value * p) / p;
            }
        }

        public static float CeilTo(float value, int place)
        {
            return CeilTo(value, place, 10);
        }

        public static float CeilTo(float value)
        {
            return CeilTo(value, 0, 10);
        }


        /// <summary>
        /// Rounds to the nearest interval. This can allow you to round to the nearest repeating range, for instance every 45 degrees for an angle.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static float RoundToInterval(float value, float interval)
        {
            return (float)Math.Round(value / interval) * interval;
        }

        /// <summary>
        /// Rounds to the nearest interval offset by mean. A value of 7, for interval 4, with mean 2 would be 6.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="interval"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static float RoundToInterval(float value, float interval, float mean)
        {
            return (float)Math.Round((value - mean) / interval) * interval + mean;
        }

        public static float FloorToInterval(float value, float interval)
        {
            return (float)Math.Floor(value / interval) * interval;
        }

        public static float FloorToInterval(float value, float interval, float mean)
        {
            return (float)Math.Floor((value - mean) / interval) * interval + mean;
        }

        public static float CeilToInterval(float value, float interval)
        {
            return (float)Math.Ceiling(value / interval) * interval;
        }

        public static float CeilToInterval(float value, float interval, float mean)
        {
            return (float)Math.Ceiling((value - mean) / interval) * interval + mean;
        }

        #endregion

        #region "Simple fuzzy arithmetic"

        /// <summary>
        /// Test if float is kind of equal to some other value by some epsilon.
        /// 
        /// Due to float error, two values may be considered similar... but the computer considers them different. 
        /// By using some epsilon (degree of error) once can test if the two values are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool FuzzyEqual(float a, float b, float epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        public static bool FuzzyEqual(float a, float b)
        {
            return FuzzyEqual(a, b, EPSILON);
        }

        /// <summary>
        /// Test if float is less than some other value by some degree of error in epsilon.
        /// 
        /// Due to float error, two values may be considered similar... but the computer considers them different. 
        /// By using some epsilon (degree of error) once can test if the two values are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <param name="exclusive"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool FuzzyLessThan(float a, float b, float epsilon, bool exclusive)
        {
            //exclusive means we prefer to easily exclude a true result
            if (exclusive)
                return a < b - epsilon;
            else
                return a < b + epsilon;
        }

        public static bool FuzzyLessThan(float a, float b, bool exclusive)
        {
            return FuzzyLessThan(a, b, EPSILON, exclusive);
        }

        /// <summary>
        /// Test if float is less than some other value by some degree of error in epsilon.
        /// 
        /// Due to float error, two values may be considered similar... but the computer considers them different. 
        /// By using some epsilon (degree of error) once can test if the two values are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <param name="exclusive"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool FuzzyGreaterThan(float a, float b, float epsilon, bool exclusive)
        {
            //exclusive means we prefer to easily exclude a true result
            if (exclusive)
                return a > b + epsilon;
            else
                return a > b - epsilon;
        }

        public static bool FuzzyGreaterThan(float a, float b, bool exclusive)
        {
            return FuzzyGreaterThan(a, b, EPSILON, exclusive);
        }

        /// <summary>
        /// Test if a value is near some target value, if with in some range of 'epsilon', the target is returned.
        /// 
        /// eg:
        /// Slam(1.52,2,0.1) == 1.52
        /// Slam(1.62,2,0.1) == 1.62
        /// Slam(1.72,2,0.1) == 1.72
        /// Slam(1.82,2,0.1) == 1.82
        /// Slam(1.92,2,0.1) == 2
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Slam(float value, float target, float epsilon)
        {
            if (Math.Abs(value - target) < epsilon)
            {
                return target;
            }
            else
            {
                return value;
            }
        }

        public static float Slam(float value, float target)
        {
            return Slam(value, target, EPSILON);
        }
        #endregion

        #region "Angular Math"

        /// <summary>
        /// convert radians to degrees
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float RadiansToDegrees(float angle)
        {
            return angle * RAD_TO_DEG;
        }

        /// <summary>
        /// convert degrees to radians
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float DegreesToRadians(float angle)
        {
            return angle * DEG_TO_RAD;
        }

        /// <summary>
        /// Find the angle of a segment from (x1, y1) -> (x2, y2 )
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float AngleBetween(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Atan2(y2 - y1, x2 - x1);
        }

        /// <summary>
        /// set an angle with in the bounds of -PI to PI
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float NormalizeAngle(float angle, bool useRadians)
        {
            float rd = (useRadians ? PI : 180);
            return Wrap(angle, rd, -rd);
        }

        /// <summary>
        /// closest angle from a1 to a2
        /// absolute value the return for exact angle
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float NearestAngleBetween(float a1, float a2, bool useRadians)
        {
            var rd = useRadians ? PI : 180f;
            var ra = Wrap(a2 - a1, rd * 2f);
            if (ra > rd) ra -= (rd * 2f);
            return ra;
        }

        /// <summary>
        /// Returns a value for dependant that is a value that is the shortest angle between dep and ind from ind.
        /// 
        /// 
        /// for instance if dep=-170 degrees and ind=170 degrees then 190 degrees will be returned as an alternative to -170 degrees
        /// note: angle is passed in radians, this written example is in degrees for ease of reading
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float ShortenAngleToAnother(float dep, float ind, bool useRadians)
        {
            return ind + NearestAngleBetween(ind, dep, useRadians);
        }

        /// <summary>
        /// Returns a value for dependant that is the shortest angle counter-clockwise from ind.
        /// 
        /// for instance if dep=-170 degrees, and ind=10 degrees, then 200 degrees will be returned as an alternative to -160. The shortest 
        /// path from 10 to -160 moving counter-clockwise is 190 degrees away.
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="ind"></param>
        /// <param name="useRadians"></param>
        /// <returns></returns>
        public static float NormalizeAngleToAnother(float dep, float ind, bool useRadians)
        {
            if(useRadians)
            {
                if(dep < ind)
                {
                    while (dep < ind) dep += MathUtil.TWO_PI;
                }
                else if(dep - ind > MathUtil.TWO_PI)
                {
                    while(dep - ind > MathUtil.TWO_PI) dep -= MathUtil.TWO_PI;
                }
            }
            else
            {
                if (dep < ind)
                {
                    while (dep < ind) dep += 360f;
                }
                else if (dep - ind > 360f)
                {
                    while (dep - ind > MathUtil.TWO_PI) dep -= 360f;
                }
            }
            return dep;
        }

        /// <summary>
        /// interpolate across the shortest arc between two angles
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float InterpolateAngle(float a, float b, float weight, bool useRadians)
        {
            var rd = (useRadians) ? PI : 180f;
            var delta = (b - a) % (rd * 2f);
            return Wrap(a + delta * weight, rd, -rd);
        }

        //public static float ClampAngle(float a, float max, float min, bool bUseRadians)
        //{
        //    a = NormalizeAngle(a, bUseRadians);
        //    max = NormalizeAngleToAnother(max, a, bUseRadians);
        //    min = NormalizeAngleToAnother(min, a, bUseRadians);
        //    return MathUtil.Clamp(a, max, min);
        //}

        public static float ClampIn180(float a, bool bUseRadians = false)
        {
            return NormalizeAngle(a, bUseRadians);
        }

        public static float ClampIn360(float a, bool bUseRadians = false)
        {
            a = NormalizeAngle(a, bUseRadians);
            if (a < 0f) a += (bUseRadians) ? TWO_PI : 360.0f;
            return a;
        }

        #endregion

        #region "Advanced Math"

        /// <summary>
        /// Compute the logarithm of any value of any base
        /// </summary>
        /// <param name="value"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        /// <remarks>
        /// a logarithm is the exponent that some constant (base) would have to be raised to 
        /// to be equal to value.
        /// 
        /// i.e.
        /// 4 ^ x = 16
        /// can be rewritten as to solve for x
        /// logB4(16) = x
        /// which with this function would be 
        /// LoDMath.logBaseOf(16,4)
        /// 
        /// which would return 2, because 4^2 = 16
        /// </remarks>
        public static float LogBaseOf(float value, float @base)
        {
            return (float)(Math.Log(value) / Math.Log(@base));
        }

        /// <summary>
        /// Check if a value is prime.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// In this method to increase speed we first check if the value is ltOReq 1, because values ltOReq 1 are not prime by definition. 
        /// Then we check if the value is even but not equal to 2. If so the value is most certainly not prime. 
        /// Lastly we loop through all odd divisors. No point in checking 1 or even divisors, because if it were divisible by an even 
        /// number it would be divisible by 2. If any divisor existed when i > value / i then its compliment would have already 
        /// been located. And lastly the loop will never reach i == val because i will never be > sqrt(val).
        /// 
        /// proof of validity for algorithm:
        /// 
        /// all trivial values are thrown out immediately by checking if even or less then 2
        /// 
        /// all remaining possibilities MUST be odd, an odd is resolved as the multiplication of 2 odd values only. (even * anyValue == even)
        /// 
        /// in resolution a * b = val, a = val / b. As every compliment a for b, b and a can be swapped resulting in b being ltOReq a. If a compliment for b 
        /// exists then that compliment would have already occured (as it is odd) in the swapped addition at the even split.
        /// 
        /// Example...
        /// 
        /// 16
        /// 1 * 16
        /// 2 * 8
        /// 4 * 4
        /// 8 * 2
        /// 16 * 1
        /// 
        /// checks for 1, 2, and 4 would have already checked the validity of 8 and 16.
        /// 
        /// Thusly we would only have to loop as long as i ltOReq val / i. Once we've reached the middle compliment, all subsequent factors have been resolved.
        /// 
        /// This shrinks the number of loops for odd values from [ floor(val / 2) - 1 ] down to [ ceil(sqrt(val) / 2) - 1 ]
        /// 
        /// example, if we checked EVERY odd number for the validity of the prime 7927, we'd loop 3962 times
        /// 
        /// but by this algorithm we loop only 43 times. Significant improvement!
        /// </remarks>
        public static bool IsPrime(long value)
        {
            // check if value is in prime number range
            if (value < 2)
                return false;

            // check if even, but not equal to 2
            if ((value % 2) == 0 & value != 2)
                return false;

            // if 2 or odd, check if any non-trivial divisors exist
            long sqrrt = (long)Math.Floor(Math.Sqrt(value));
            for (long i = 3; i <= sqrrt; i += 2)
            {
                if ((value % i) == 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Relative Primality between two integers
        /// 
        /// By definition two integers are considered relatively prime if their 
        /// 'greatest common divisor' is 1. So thusly we simply just check if 
        /// the GCD of m and n is 1.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsRelativelyPrime(short m, short n)
        {
            return GCD(m, n) == 1;
        }

        public static bool IsRelativelyPrime(int m, int n)
        {
            return GCD(m, n) == 1;
        }

        public static bool IsRelativelyPrime(long m, long n)
        {
            return GCD(m, n) == 1;
        }

        public static int[] FactorsOf(int value)
        {
            value = Math.Abs(value);
            List<int> arr = new List<int>();
            int sqrrt = (int)Math.Sqrt(value);
            int c = 0;

            for (int i = 1; i <= sqrrt; i++)
            {
                if ((value % i) == 0)
                {
                    arr.Add(i);
                    c = value / i;
                    if (c != i)
                        arr.Add(c);
                }
            }

            arr.Sort();

            return arr.ToArray();
        }
        
        public static int[] CommonFactorsOf(int m, int n)
        {
            int i = 0;
            int j = 0;
            if (m < 0) m = -m;
            if (n < 0) n = -n;

            if(m > n)
            {
                i = m;
                m = n;
                n = i;
            }

            var set = new HashSet<int>(); //ensures no duplicates

            int r = (int)Math.Sqrt(m);
            for(i = 1; i <= r; i++)
            {
                if((m % i) == 0 && (n % i) == 0)
                {
                    set.Add(i);
                    j = m / i;
                    if ((n % j) == 0) set.Add(j);
                    j = n / i;
                    if ((m % j) == 0) set.Add(j);
                }
            }

            int[] arr = System.Linq.Enumerable.ToArray(set);
            System.Array.Sort(arr);
            return arr;



            //more loops
            /*
            List<int> arr = new List<int>();

            int i = 0;
            if (m < 0) m = -m;
            if (n < 0) n = -n;

            //make sure m is < n
            if (m > n)
            {
                i = m;
                m = n;
                n = i;
            }

            //could be sped up by looping to sqrt(m), but then would have to do extra work to make sure duplicates don't occur
            for (i = 1; i <= m; i++)
            {
                if ((m % i) == 0 && (n % i) == 0)
                {
                    arr.Add(i);
                }
            }

            return arr.ToArray();
            */
        }

        /// <summary>
        /// Greatest Common Divisor using Euclid's algorithm
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int GCD(int m, int n)
        {
            int r = 0;

            // make sure positive, GCD is always positive
            if (m < 0) m = -m;
            if (n < 0) n = -n;

            // m must be >= n
            if (m < n)
            {
                r = m;
                m = n;
                n = r;
            }

            // now start loop, loop is infinite... we will cancel out sooner or later
            while (true)
            {
                r = m % n;
                if (r == 0)
                    return n;
                m = n;
                n = r;
            }

            // fail safe
            //return 1;
        }

        public static long GCD(long m, long n)
        {
            long r = 0;

            // make sure positive, GCD is always positive
            if (m < 0) m = -m;
            if (n < 0) n = -n;

            // m must be >= n
            if (m < n)
            {
                r = m;
                m = n;
                n = r;
            }

            // now start loop, loop is infinite... we will cancel out sooner or later
            while (true)
            {
                r = m % n;
                if (r == 0)
                    return n;
                m = n;
                n = r;
            }

            // fail safe
            //return 1;
        }

        public static int LCM(int m, int n)
        {
            return (m * n) / GCD(m, n);
        }

        /// <summary>
        /// Factorial - N!
        /// 
        /// Simple product series
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// By definition 0! == 1
        /// 
        /// Factorial assumes the idea that the value is an integer >= 0... thusly UInteger is used
        /// </remarks>
        public static long Factorial(uint value)
        {
            if (value <= 0)
                return 1;

            long res = value;

            while (--value != 0)
            {
                res *= value;
            }

            return res;
        }

        /// <summary>
        /// Falling facotiral
        /// 
        /// defined: (N)! / (N - x)!
        /// 
        /// written subscript: (N)x OR (base)exp
        /// </summary>
        /// <param name="base"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long FallingFactorial(uint @base, uint exp)
        {
            return Factorial(@base) / Factorial(@base - exp);
        }

        /// <summary>
        /// rising factorial
        /// 
        /// defined: (N + x - 1)! / (N - 1)!
        /// 
        /// written superscript N^(x) OR base^(exp)
        /// </summary>
        /// <param name="base"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long RisingFactorial(uint @base, uint exp)
        {
            return Factorial(@base + exp - 1) / Factorial(@base - 1);
        }

        /// <summary>
        /// binomial coefficient
        /// 
        /// defined: N! / (k!(N-k)!)
        /// reduced: N! / (N-k)! == (N)k (fallingfactorial)
        /// reduced: (N)k / k!
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long BinCoef(uint n, uint k)
        {
            return FallingFactorial(n, k) / Factorial(k);
        }

        /// <summary>
        /// rising binomial coefficient
        /// 
        /// as one can notice in the analysis of binCoef(...) that 
        /// binCoef is the (N)k divided by k!. Similarly rising binCoef 
        /// is merely N^(k) / k! 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long RisingBinCoef(uint n, uint k)
        {
            return RisingFactorial(n, k) / Factorial(k);
        }
        #endregion



        #region Geometric Calculations

        public static float ApproxCircumOfEllipse(float a, float b)
        {
            return (float)(PI * Math.Sqrt((a * a + b * b) / 2));
        }

        #endregion

        #endregion

    }
}

