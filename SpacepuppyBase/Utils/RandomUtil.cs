using System;
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

        public static VB_RNG CreateVB_RNG()
        {
            return new VB_RNG();
        }

        public static VB_RNG CreateVB_RNG(double seed)
        {
            return new VB_RNG(seed);
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

        public static float Range(this IRandom rng, float max, float min = 0.0f)
        {
            return (float)(rng.NextDouble() * (max - min)) + min;
        }

        public static int Range(this IRandom rng, int max, int min = 0)
        {
            return rng.Next(min, max);
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

        public class VB_RNG : IRandom
        {

            #region Fields

            private int _seed;

            #endregion

            #region Constructor

            public VB_RNG()
            {
                this.Randomize();
            }

            public VB_RNG(double seed)
            {
                this.Randomize(seed);
            }

            #endregion

            #region Methods
            
            public float Next()
            {
                return this.VBNext(1f);
            }

            public int Next(int size)
            {
                return (int)(this.Next() * size);
            }

            public int Next(int low, int high)
            {
                return (int)(this.Next() * (high - low)) + low;
            }

            public double NextDouble()
            {
                return (double)this.Next();
            }

            public float VBNext(float num)
            {
                int num1 = _seed;
                if ((double)num != 0.0)
                {
                    if ((double)num < 0.0)
                    {
                        long num2 = (long)BitConverter.ToInt32(BitConverter.GetBytes(num), 0) & (long)uint.MaxValue;
                        num1 = checked((int)(num2 + (num2 >> 24) & 16777215L));
                    }
                    num1 = checked((int)((long)num1 * 1140671485L + 12820163L & 16777215L));
                }
                _seed = num1;
                return (float)num1 / 1.677722E+07f;
            }

            public void Randomize()
            {
                DateTime now = DateTime.Now;
                float timer = (float)checked((60 * now.Hour + now.Minute) * 60 + now.Second) + (float)now.Millisecond / 1000f;
                int num1 = _seed;
                int num2 = BitConverter.ToInt32(BitConverter.GetBytes(timer), 0);
                int num3 = (num2 & (int)ushort.MaxValue ^ num2 >> 16) << 8;
                int num4 = num1 & -16776961 | num3;
                _seed = num4;
            }

            public void Randomize(double num)
            {
                int num1 = _seed;
                int num2 = !BitConverter.IsLittleEndian ? BitConverter.ToInt32(BitConverter.GetBytes(num), 0) : BitConverter.ToInt32(BitConverter.GetBytes(num), 4);
                int num3 = (num2 & (int)ushort.MaxValue ^ num2 >> 16) << 8;
                int num4 = num1 & -16776961 | num3;
                _seed = num4;
            }

            #endregion

        }

        #endregion

    }
}
