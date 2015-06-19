using UnityEngine;
using System.Collections.Generic;

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

        #endregion

        #region Static Properties

        public static float Angle(this IRandom rng)
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

        public static UnityEngine.Quaternion Rotation(this IRandom rng)
        {
            return UnityEngine.Quaternion.AngleAxis(rng.Angle(), rng.OnUnitSphere());
        }

        #endregion

        #region Methods

        public static float Range(this IRandom rng, float max, float min = 0.0f)
        {
            return (float)(rng.NextDouble() * (max - min)) + min;
        }

        public static int Range(this IRandom rng, int max, int min = 0)
        {
            return rng.Next(max, min);
        }

        #endregion




        #region Special Types

        private class UnityRNG : IRandom
        {

            public float Next()
            {
                return Random.value;
            }

            public double NextDouble()
            {
                return (double)Random.value;
            }

            public int Next(int size)
            {
                return (int)((float)size * Random.value);
            }


            public int Next(int low, int high)
            {
                return (int)(Random.value * (high - low)) + low;
            }
        }

        private class MicrosoftRNG : System.Random, IRandom
        {

            public MicrosoftRNG(int seed) : base(seed)
            {

            }


            float IRandom.Next()
            {
                return (float)this.Next();
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

        #endregion

    }
}
