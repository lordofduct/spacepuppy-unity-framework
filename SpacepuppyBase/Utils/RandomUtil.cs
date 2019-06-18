using UnityEngine;
using BitConverter = System.BitConverter;

namespace com.spacepuppy.Utils
{
    public static class RandomUtil
    {

        #region Static Fields

        private static UnityRNG _unityRNG = new UnityRNG();

        public static IRandom Standard { get { return _unityRNG; } }

        public static IRandom CreateRNG(int seed)
        {
            return new MicrosoftRNG(seed);
        }

        public static IRandom CreateRNG()
        {
            return new MicrosoftRNG();
        }

        /// <summary>
        /// Create an rng that is deterministic to that 'seed' across all platforms.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static IRandom CreateDeterministicRNG(int seed)
        {
            return LinearCongruentialRNG.CreateMMIXKnuth(seed);
        }
        
        #endregion

        #region Static Properties

        public static float Angle(this IRandom rng)
        {
            return rng.Next() * 360f;
        }

        public static float Radian(this IRandom rng)
        {
            return rng.Next() * MathUtil.TWO_PI;
        }

        /// <summary>
        /// Return 0 or 1. Numeric version of Bool.
        /// </summary>
        /// <returns></returns>
        public static int Pop(this IRandom rng)
        {
            return rng.Next(1000) % 2;
        }

        public static int Sign(this IRandom rng)
        {
            int n = rng.Next(1000) % 2;
            return n + n - 1;
        }

        /// <summary>
        /// Return a true randomly.
        /// </summary>
        /// <returns></returns>
        public static bool Bool(this IRandom rng)
        {
            return (rng.Next(1000) % 2 != 0);
        }

        public static bool Bool(this IRandom rng, float oddsOfTrue)
        {
            int i = rng.Next(100000);
            int m = (int)(oddsOfTrue * 100000);
            return i < m;
        }

        /// <summary>
        /// Return -1, 0, 1 randomly. This can be used for bizarre things like randomizing an array.
        /// </summary>
        /// <returns></returns>
        public static int Shift(this IRandom rng)
        {
            return (rng.Next(999) % 3) - 1;
        }

        public static UnityEngine.Vector3 OnUnitSphere(this IRandom rng)
        {
            //uniform, using angles
            var a = rng.Next() * MathUtil.TWO_PI;
            var b = rng.Next() * MathUtil.TWO_PI;
            var sa = Mathf.Sin(a);
            return new Vector3(sa * Mathf.Cos(b), sa * Mathf.Sin(b), Mathf.Cos(a));

            //non-uniform, needs to test for 0 vector
            /*
            var v = new UnityEngine.Vector3(Value, Value, Value);
            return (v == UnityEngine.Vector3.zero) ? UnityEngine.Vector3.right : v.normalized;
                */
        }

        public static UnityEngine.Vector2 OnUnitCircle(this IRandom rng)
        {
            //uniform, using angles
            var a = rng.Next() * MathUtil.TWO_PI;
            return new Vector2(Mathf.Sin(a), Mathf.Cos(a));
        }

        public static UnityEngine.Vector3 InsideUnitSphere(this IRandom rng)
        {
            return rng.OnUnitSphere() * rng.Next();
        }

        public static UnityEngine.Vector2 InsideUnitCircle(this IRandom rng)
        {
            return rng.OnUnitCircle() * rng.Next();
        }

        public static UnityEngine.Vector3 AroundAxis(this IRandom rng, Vector3 axis)
        {
            var a = rng.Angle();
            if(VectorUtil.NearSameAxis(axis, Vector3.forward))
            {
                return Quaternion.AngleAxis(a, axis) * VectorUtil.GetForwardTangent(Vector3.up, axis);
            }
            else
            {
                return Quaternion.AngleAxis(a, axis) * VectorUtil.GetForwardTangent(Vector3.forward, axis);
            }
        }

        public static UnityEngine.Quaternion Rotation(this IRandom rng)
        {
            return UnityEngine.Quaternion.AngleAxis(rng.Angle(), rng.OnUnitSphere());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Select between min and max, exclussive of max.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static float Range(this IRandom rng, float max, float min = 0.0f)
        {
            return (float)(rng.NextDouble() * (max - min)) + min;
        }

        /// <summary>
        /// Select between min and max, exclussive of max.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static int Range(this IRandom rng, int max, int min = 0)
        {
            return rng.Next(min, max);
        }

        /// <summary>
        /// Select an weighted index from 0 to length of weights.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int Range(this IRandom rng, params float[] weights)
        {
            int i;
            float w;
            float total = 0f;
            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsPositiveInfinity(w)) return i;
                else if (w >= 0f && !float.IsNaN(w)) total += w;
            }

            if (rng == null) rng = RandomUtil.Standard;
            if (total == 0f) return rng.Next(weights.Length);

            float r = rng.Next();
            float s = 0f;

            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0f) continue;

                s += w / total;
                if (s > r)
                {
                    return i;
                }
            }

            //should only get here if last element had a zero weight, and the r was large
            i = weights.Length - 1;
            while (i > 0 && weights[i] <= 0f) i--;
            return i;
        }

        /// <summary>
        /// Select an weighted index from 0 to length of weights.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int Range(this IRandom rng, float[] weights, int startIndex, int count = -1)
        {
            int i;
            float w;
            float total = 0f;
            int last = count < 0 ? weights.Length : System.Math.Min(startIndex + count, weights.Length);
            for (i = startIndex; i < last; i++)
            {
                w = weights[i];
                if (float.IsPositiveInfinity(w)) return i;
                else if (w >= 0f && !float.IsNaN(w)) total += w;
            }

            if (rng == null) rng = RandomUtil.Standard;
            if (total == 0f) return rng.Next(weights.Length);

            float r = rng.Next();
            float s = 0f;

            for (i = startIndex; i < last; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0f) continue;

                s += w / total;
                if (s > r)
                {
                    return i;
                }
            }

            //should only get here if last element had a zero weight, and the r was large
            i = last - 1;
            while (i > 0 && weights[i] <= 0f) i--;
            return i;
        }

        #endregion




        #region Special Types

        private class UnityRNG : IRandom
        {

            public float Next()
            {
                //return Random.value;
                //because unity's Random returns in range 0->1, which is dumb
                //why you might say? Well it means that the 1 is the least likely value to generate, so for generating indices you get uneven results
                return Random.value * 0.9999f;
            }

            public double NextDouble()
            {
                //return (double)Random.value;
                //because unity's Random returns in range 0->1, which is dumb
                //why you might say? Well it means that the 1 is the least likely value to generate, so for generating indices you get uneven results
                return (double)Random.value * 0.99999999d;
            }

            public int Next(int size)
            {
                return (int)((double)size * NextDouble());
            }


            public int Next(int low, int high)
            {
                return (int)(NextDouble() * (high - low)) + low;
            }
        }

        private class MicrosoftRNG : System.Random, IRandom
        {

            public MicrosoftRNG() : base()
            {

            }

            public MicrosoftRNG(int seed) : base(seed)
            {

            }


            float IRandom.Next()
            {
                return (float)this.NextDouble();
            }

            double IRandom.NextDouble()
            {
                return this.NextDouble();
            }

            int IRandom.Next(int size)
            {
                return this.Next(size);
            }

            int IRandom.Next(int low, int high)
            {
                return this.Next(low, high);
            }
        }
        
        /// <summary>
        /// A simple deterministic rng using a linear congruential algorithm. 
        /// Not the best, but fast and effective for deterministic rng for games.
        /// 
        /// Various known parameter configurations are included as static factory methods for ease of creating known long-period generators.
        /// See the wiki article for a list of more known long period parameters: https://en.wikipedia.org/wiki/Linear_congruential_generator
        /// </summary>
        public class LinearCongruentialRNG : IRandom
        {

            #region Fields

            private ulong _mode;
            private ulong _mult;
            private ulong _incr;
            private ulong _seed;

            private System.Func<double> _getNext;

            #endregion

            #region CONSTRUCTOR

            public LinearCongruentialRNG(long seed, ulong increment, ulong mult, ulong mode)
            {
                _mode = mode;
                _mult = System.Math.Max(1, System.Math.Min(mode - 1, mult));
                _incr = System.Math.Max(0, System.Math.Min(mode - 1, increment));
                if (seed < 0)
                {
                    seed = System.DateTime.Now.Millisecond;
                }
                _seed = (ulong)seed % _mode;

                if(_mode == 0)
                {
                    //this counts as using 2^64 as the mode
                    _getNext = () =>
                    {
                        _seed = _mult * _seed + _incr;
                        return (double)((decimal)_seed / 18446744073709551616m); //use decimal for larger sig range
                    };
                }
                else if(_mode > 0x10000000000000)
                {
                    //double doesn't have the sig range to handle these, so we'll use decimal
                    _getNext = () =>
                    {
                        _seed = (_mult * _seed + _incr) % _mode;
                        return (double)((decimal)_seed / 18446744073709551616m); //use decimal for larger sig range
                    };
                }
                else
                {
                    //just do the maths
                    _getNext = () => (double)(_seed = (_mult * _seed + _incr) % _mode) / (double)(_mode);
                }
            }

            #endregion

            #region IRandom Interface

            public double NextDouble()
            {
                return _getNext();
            }

            public float Next()
            {
                return (float)_getNext();
            }

            public int Next(int size)
            {
                return (int)(size * _getNext());
            }

            public int Next(int low, int high)
            {
                return (int)((high - low) * _getNext() + low);
            }

            #endregion

            #region Static Factory

            public static LinearCongruentialRNG CreateMMIXKnuth(long seed = -1)
            {
                return new LinearCongruentialRNG(seed, 1442695040888963407, 6364136223846793005, 0);
            }

            public static LinearCongruentialRNG CreateAppleCarbonLib(int seed = -1)
            {
                return new LinearCongruentialRNG(seed, 0, 16807, 16807);
            }

            public static LinearCongruentialRNG CreateGLibc(int seed = -1)
            {
                return new LinearCongruentialRNG(seed, 12345, 1103515245, 2147483648);
            }

            public static LinearCongruentialRNG CreateVB6(int seed = -1)
            {
                return new LinearCongruentialRNG(seed, 12820163, 1140671485, 16777216);
            }

            #endregion

        }

        public class PCG : IRandom
        {

            #region Fields

            private ulong _seed;
            private ulong _incr;

            #endregion

            #region CONSTRUCTOR

            public PCG(long seed = -1, ulong stream = 1)
            {
                if(seed < 0)
                {
                    seed = System.DateTime.Now.Ticks;
                }
                _seed = (ulong)seed;
                _incr = stream | 1;
            }

            #endregion

            #region IRandom Interface

            public double NextDouble()
            {
                ulong old = _seed;
                _seed = old * 6364136223846793005 + _incr;
                uint xor = (uint)(((old >> 18) ^ old) >> 27);
                int rot = (int)(old >> 59);

                uint result = (xor >> rot) | (xor << (64 - rot));
                return (double)result / (double)(0x100000000);
            }

            public float Next()
            {
                return (float)this.NextDouble();
            }

            public int Next(int size)
            {
                return (int)(this.NextDouble() * size);
            }

            public int Next(int low, int high)
            {
                return (int)((high - low) * this.NextDouble()) + low;
            }

            #endregion
            
        }

        #endregion

    }
}
