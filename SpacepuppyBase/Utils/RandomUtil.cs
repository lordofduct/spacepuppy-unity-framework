using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Utils
{
    public class RandomUtil
    {

        #region Static Fields

        #endregion

        #region Static Properties

        public static int Next()
        {
            return (int)(Random.value * 10000f);
        }

        public static float Value()
        {
            return Random.value;
        }

        public static float Angle()
        {
            return Random.value * MathUtil.TWO_PI;
        }

        /// <summary>
        /// Return 0 or 1. Numeric version of Bool.
        /// </summary>
        /// <returns></returns>
        public static int Pop()
        {
            return Next() % 2;
        }

        public static int Sign()
        {
            int n = Next() % 2;
            return n + n - 1;
        }

        /// <summary>
        /// Return a true randomly.
        /// </summary>
        /// <returns></returns>
        public static bool Bool()
        {
            return (Next() % 2 != 0);
        }

        /// <summary>
        /// Return -1, 0, 1 randomly. This can be used for bizarre things like randomizing an array.
        /// </summary>
        /// <returns></returns>
        public static int Shift()
        {
            return (Next() % 3) - 1;
        }

        public static UnityEngine.Vector3 OnUnitSphere()
        {
            //uniform, using angles
            var a = Random.value * MathUtil.TWO_PI;
            var b = Random.value * MathUtil.TWO_PI;
            var sa = Mathf.Sin(a);
            return new Vector3(sa * Mathf.Cos(b), sa * Mathf.Sin(b), Mathf.Cos(a));

            //non-uniform, needs to test for 0 vector
            /*
            var v = new UnityEngine.Vector3(Value, Value, Value);
            return (v == UnityEngine.Vector3.zero) ? UnityEngine.Vector3.right : v.normalized;
                */
        }

        public static UnityEngine.Vector2 OnUnitCircle()
        {
            //uniform, using angles
            var a = Random.value * MathUtil.TWO_PI;
            return new Vector2(Mathf.Sin(a), Mathf.Cos(a));
        }

        public static UnityEngine.Vector3 InsideUnitSphere()
        {
            return OnUnitSphere() * Value();
        }

        public static UnityEngine.Vector2 InsideUnitCircle()
        {
            return OnUnitCircle() * Value();
        }

        public static UnityEngine.Quaternion Rotation()
        {
            return UnityEngine.Quaternion.AngleAxis(RandomUtil.Angle(), RandomUtil.OnUnitSphere());
        }

        #endregion

        #region Methods

        public static float Range(float max, float min = 0.0f)
        {
            return Random.value * (max - min) + min;
        }

        public static int Range(int max, int min = 0)
        {
            return (int)(Random.value * (float)(max - min)) + min;
        }

        public static void Shuffle<T>(T[] arr)
        {
            System.Array.Sort(arr, delegate(T a, T b) { return Shift(); });
        }

        #endregion

    }
}
