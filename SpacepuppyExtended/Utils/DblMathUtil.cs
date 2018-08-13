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
	public static class DblMathUtil
    {

        #region "Public ReadOnly Properties"
        // Number pi
        public const double PI = 3.14159265358979;
        // PI / 2 OR 90 deg
        public const double PI_2 = 1.5707963267949;
        // PI / 4 OR 45 deg
        public const double PI_4 = 0.785398163397448;
        // PI / 8 OR 22.5 deg
        public const double PI_8 = 0.392699081698724;
        // PI / 16 OR 11.25 deg
        public const double PI_16 = 0.196349540849362;
        // 2 * PI OR 180 deg
        public const double TWO_PI = 6.28318530717959;
        // 3 * PI_2 OR 270 deg
        public const double THREE_PI_2 = 4.71238898038469;
        // Number e
        public const double E = 2.71828182845905;
        // ln(10)
        public const double LN10 = 2.30258509299405;
        // ln(2)
        public const double LN2 = 0.693147180559945;
        // logB10(e)
        public const double LOG10E = 0.434294481903252;
        // logB2(e)
        public const double LOG2E = 1.44269504088896;
        // sqrt( 1 / 2 )
        public const double SQRT1_2 = 0.707106781186548;
        // sqrt( 2 )
        public const double SQRT2 = 1.4142135623731;
        // PI / 180
        public const double DEG_TO_RAD = 0.0174532925199433;
        //  180.0 / PI
        public const double RAD_TO_DEG = 57.2957795130823;

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
        public const double ONE_THIRD = 0.333333333333333;
        //  2.0/3.0
        public const double TWO_THIRDS = 0.666666666666667;
        //  1.0/6.0
        public const double ONE_SIXTH = 0.166666666666667;

        // COS( PI / 3 )
        public const double COS_PI_3 = 0.866025403784439;
        //  SIN( 2*PI/3 )
        public const double SIN_2PI_3 = 0.03654595;

        // 4*(Math.sqrt(2)-1)/3.0
        public const double CIRCLE_ALPHA = 0.552284749830793;

        public const bool ONN = true;

        public const bool OFF = false;
        // round integer epsilon
        public const double SHORT_EPSILON = 0.1;
        // percentage epsilon
        public const double PERC_EPSILON = 0.001;
        // single float average epsilon
        public const double EPSILON = 0.0001;
        // arbitrary 8 digit epsilon
        public const double LONG_EPSILON = 1E-08;

        public static readonly double MACHINE_EPSILON = DblMathUtil.ComputeMachineEpsilon();

        public static double ComputeMachineEpsilon()
        {
            double fourThirds = 4.0 / 3.0;
            double third = fourThirds - 1.0;
            double one = third + third + third;
            return Math.Abs(1.0 - one);
        }
        #endregion

        #region "Public Shared Methods"

        /// <summary>
        /// Calculates the integral part of a float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double Truncate(double value)
        {
            return Math.Truncate(value);
        }

        /// <summary>
        /// Shears off the fractional part of a float.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double Shear(double value)
        {
            return value % 1;
        }

        /// <summary>
        /// Returns if the value is in between or equal to max and min
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool InRange(double value, double max, double min)
        {
            return (value >= min && value <= max);
        }

        public static bool InRange(double value, double max)
        {
            return InRange(value, max, 0);
        }

        #region "series"
        /// <summary>
        /// Sums a series of numeric values passed as a param array...
        /// 
        /// MathUtil.Summation(1,2,3,4) == 10
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double Summation(params double[] arr)
        {
            double result = 0;

            foreach (double value in arr)
            {
                result += value;
            }

            return result;
        }

        public static double Summation(double[] arr, int startIndex, int endIndex)
        {
            double result = 0;

            for (int i = startIndex; i <= Math.Min(endIndex, arr.Length - 1); i++)
            {
                result += arr[i];
            }

            return result;
        }

        public static double Summation(double[] arr, int startIndex)
        {
            return Summation(arr, startIndex, int.MaxValue);
        }

        /// <summary>
        /// Multiplies a series of numeric values passed as a param array...
        /// 
        /// MathUtil.ProductSeries(2,3,4) == 24
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double ProductSeries(params double[] arr)
        {
            if (arr == null || arr.Length == 0)
                return double.NaN;

            double result = 1;

            foreach (double value in arr)
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
        public static double Average(params double[] values)
        {
            double avg = 0;

            foreach (double value in values)
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
        public static double Interpolate(double a, double b, double weight)
        {
            return (b - a) * weight + a;
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
        public static double PercentageMinMax(double value, double max, double min)
        {
            value -= min;
            max -= min;

            if (max == 0)
            {
                return 0;
            }
            else
            {
                return value / max;
            }
        }
		public static double PercentageMinMax(double value, double max)
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
        public static double PercentageOffMinMax(double value, double max, double min)
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
		public static double PercentageOffMinMax(double value, double max)
        {
            return PercentageOffMinMax(value, max);
        }

        /// <summary>
        /// Return the minimum value of several values
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double Min(params double[] args)
        {
            if (args.Length == 0)
                return double.NaN;
            double value = args[0];

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
        public static double Max(params double[] args)
        {
            if (args.Length == 0)
                return double.NaN;
            double value = args[0];

            for (int i = 1; i <= args.Length - 1; i++)
            {
                if (args[i] > value)
                    value = args[i];
            }

            return value;
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
        public static double Wrap(double value, double max, double min)
        {
            value -= min;
            max -= min;
            if (max == 0)
                return min;

            value = value % max;
            value += min;
            while (value < min)
            {
                value += max;
            }

            return value;

        }
		public static double Wrap(double value, double max)
        {
            return Wrap(value, max, 0);
        }

        /// <summary>
        /// Arithmetic version of Wrap... unsure of which is more efficient.
        /// 
        /// Here for demo purposes
        /// </summary>
        /// <param name="value">value to wrap</param>
        /// <param name="max">max in range</param>
        /// <param name="min">min in range</param>
        /// <returns>A value wrapped around min to max</returns>
        /// <remarks></remarks>
        public static double ArithWrap(double value, double max, double min)
        {
            max -= min;
            if (max == 0)
                return min;

            return value - max * Math.Floor((value - min) / max);
        }
		public static double ArithWrap(double value, double max)
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
        public static double Clamp(double input, double max, double min)
        {
            return Math.Max(min, Math.Min(max, input));
        }
		
		public static double Clamp(double input, double max)
        {
            return Math.Max(0, Math.Min(max, input));
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
        public static double ClampOrDefault(double input, double max, double min, double defaultValue)
        {
            return input < min || input > max ? defaultValue : input;
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
        public static double RoundTo(double value, int place, uint @base)
        {
            if (place == 0)
            {
                //'if zero no reason going through the math hoops
                return Math.Round(value);
            }
            else if (@base == 10 && place > 0 && place <= 15)
            {
                //'Math.Round has a rounding to decimal spaces that is very efficient
                //'only useful for base 10 if places are from 1 to 15
                return Math.Round(value, place);
            }
            else
            {
                double p = Math.Pow(@base, place);
                return Math.Round(value * p) / p;
            }
        }

        public static double RoundTo(double value, int place)
        {
            return RoundTo(value, place, 10);
        }

        public static double RoundTo(double value)
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
        public static double FloorTo(double value, int place, uint @base)
        {
            if (place == 0)
            {
                //'if zero no reason going through the math hoops
                return Math.Floor(value);
            }
            else
            {
                double p = Math.Pow(@base, place);
                return Math.Floor(value * p) / p;
            }
        }

        public static double FloorTo(double value, int place)
        {
            return FloorTo(value, place, 10);
        }

        public static double FloorTo(double value)
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
        public static double CeilTo(double value, int place, uint @base)
        {
            if (place == 0)
            {
                //'if zero no reason going through the math hoops
                return Math.Ceiling(value);
            }
            else
            {
                double p = Math.Pow(@base, place);
                return Math.Ceiling(value * p) / p;
            }
        }

        public static double CeilTo(double value, int place)
        {
            return CeilTo(value, place, 10);
        }

        public static double CeilTo(double value)
        {
            return CeilTo(value, 0, 10);
        }
        #endregion

        #region "Simple fuzzy arithmetic"

        /// <summary>
        /// Test if Double is kind of equal to some other value by some epsilon.
        /// 
        /// Due to float error, two values may be considered similar... but the computer considers them different. 
        /// By using some epsilon (degree of error) once can test if the two values are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool FuzzyEqual(double a, double b, double epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        public static bool FuzzyEqual(double a, double b)
        {
            return FuzzyEqual(a, b, EPSILON);
        }

        /// <summary>
        /// Test if Double is greater than some other value by some degree of error in epsilon.
        /// 
        /// Due to float error, two values may be considered similar... but the computer considers them different. 
        /// By using some epsilon (degree of error) once can test if the two values are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool FuzzyLessThan(double a, double b, double epsilon)
        {
            return a < b + epsilon;
        }

        public static bool FuzzyLessThan(double a, double b)
        {
            return FuzzyLessThan(a, b, EPSILON);
        }

        /// <summary>
        /// Test if Double is less than some other value by some degree of error in epsilon.
        /// 
        /// Due to float error, two values may be considered similar... but the computer considers them different. 
        /// By using some epsilon (degree of error) once can test if the two values are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool FuzzyGreaterThan(double a, double b, double epsilon)
        {
            return a > b - epsilon;
        }

        public static bool FuzzyGreaterThan(double a, double b)
        {
            return FuzzyGreaterThan(a, b, EPSILON);
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
        public static double Slam(double value, double target, double epsilon)
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

        public static double Slam(double value, double target)
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
        public static double RadiansToDegrees(double angle)
        {
            return angle * RAD_TO_DEG;
        }

        /// <summary>
        /// convert degrees to radians
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double DegreesToRadians(double angle)
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
        public static double AngleBetween(double x1, double y1, double x2, double y2)
        {
            return Math.Atan2(y2 - y1, x2 - x1);
        }

        /// <summary>
        /// set an angle with in the bounds of -PI to PI
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double NormalizeAngle(double angle, bool useRadians)
        {
            double rd = (useRadians ? PI : 180);
            return Wrap(angle, rd, -rd);
        }

        public static double NormalizeAngle(double angle)
        {
            return NormalizeAngle(angle, true);
        }

        /// <summary>
        /// closest angle between two angles from a1 to a2
        /// absolute value the return for exact angle
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double NearestAngleBetween(double a1, double a2, bool useRadians)
        {
            double rd_2 = (useRadians ? PI_2 : 90);
            double two_rd = (useRadians ? TWO_PI : 360);

            a1 = NormalizeAngle(a1);
            a2 = NormalizeAngle(a2);

            if (a1 < -rd_2 & a2 > rd_2)
                a1 += two_rd;
            if (a2 < -rd_2 & a1 > rd_2)
                a2 += two_rd;

            return a2 - a1;
        }

        public static double NearestAngleBetween(double a1, double a2)
        {
            return NearestAngleBetween(a1, a2, true);
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
        public static double ShortenAngleToAnother(double dep, double ind, bool useRadians)
        {
            return ind + NearestAngleBetween(ind, dep, useRadians);
        }

        public static double ShortenAngleToAnother(double dep, double ind)
        {
            return ShortenAngleToAnother(dep, ind, true);
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
        public static double NormalizeAngleToAnother(double dep, double ind, bool useRadians)
        {
            if (useRadians)
            {
                if (dep < ind)
                {
                    while (dep < ind) dep += DblMathUtil.TWO_PI;
                }
                else if (dep - ind > DblMathUtil.TWO_PI)
                {
                    while (dep - ind > DblMathUtil.TWO_PI) dep -= DblMathUtil.TWO_PI;
                }
            }
            else
            {
                if (dep < ind)
                {
                    while (dep < ind) dep += 360d;
                }
                else if (dep - ind > 360d)
                {
                    while (dep - ind > MathUtil.TWO_PI) dep -= 360d;
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
        public static double InterpolateAngle(double a1, double a2, double weight, bool useRadians)
        {
            a1 = NormalizeAngle(a1, useRadians);
            a2 = ShortenAngleToAnother(a2, a1, useRadians);

            return Interpolate(a1, a2, weight);
        }

        public static double InterpolateAngle(double a1, double a2, double weight)
        {
            return InterpolateAngle(a1, a2, weight, true);
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
        public static double LogBaseOf(double value, double @base)
        {
            return Math.Log(value) / Math.Log(@base);
        }
        #endregion



        #region Geometric Calculations

        public static double ApproxCircumOfEllipse(double a, double b)
        {
            return PI * Math.Sqrt((a * a + b * b) / 2);
        }

        #endregion

        #endregion
    }
}

