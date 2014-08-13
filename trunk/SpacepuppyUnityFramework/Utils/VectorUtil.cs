using System;
using UnityEngine;

namespace com.spacepuppy.Utils
{
    public static class VectorUtil
    {

        public static Vector2 NaNVector2 { get { return new Vector2(float.NaN, float.NaN); } }
        public static Vector3 NaNVector3 { get { return new Vector3(float.NaN, float.NaN, float.NaN); } }
        public static Vector2 PosInfVector2 { get { return new Vector2(float.PositiveInfinity, float.PositiveInfinity); } }
        public static Vector3 PosInfVector3 { get { return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity); } }
        public static Vector2 NegInfVector2 { get { return new Vector2(float.NegativeInfinity, float.NegativeInfinity); } }
        public static Vector3 NegInfVector3 { get { return new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); } }

        #region Vector Convert

        /// <summary>
        /// Get Vector2 from angle
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(float a, bool useRadians = false, bool yDominant = false)
        {
            if (!useRadians) a *= Mathf.Deg2Rad;
            if (yDominant)
            {
                return new Vector2(Mathf.Sin(a), Mathf.Cos(a));
            }
            else
            {
                return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            }
        }

        public static Vector2 ToVector2(string sval)
        {
            if (String.IsNullOrEmpty(sval)) return Vector2.zero;

            var arr = StringUtil.SplitFixedLength(sval, ',', 2);

            return new Vector2(ConvertUtil.ToSingle(arr[0]), ConvertUtil.ToSingle(arr[1]));
        }

        /// <summary>
        /// Creates Vector2 from X and Y values of a Vector3
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public static Vector3 ToVector3(Vector2 vec)
        {
            return new Vector3(vec.x, vec.y, 0);
        }

        public static Vector3 ToVector3(string sval)
        {
            if (String.IsNullOrEmpty(sval)) return Vector3.zero;

            var arr = StringUtil.SplitFixedLength(sval, ',', 3);

            return new Vector3(ConvertUtil.ToSingle(arr[0]), ConvertUtil.ToSingle(arr[1]), ConvertUtil.ToSingle(arr[2]));
        }

        public static Vector3 RotateAroundAxis(Vector3 v, float a, Vector3 axis, bool bUseRadians = false)
        {
            if (bUseRadians) a *= MathUtil.RAD_TO_DEG;
            var q = Quaternion.AngleAxis(a, axis);
            return q * v;
        }

        public static Vector3 Clamp(Vector3 input, Vector3 max, Vector3 min)
        {
            input.x = MathUtil.Clamp(input.x, max.x, min.x);
            input.y = MathUtil.Clamp(input.y, max.y, min.y);
            input.z = MathUtil.Clamp(input.z, max.z, min.z);
            return input;
        }

        public static Vector3 Clamp(Vector2 input, Vector2 max, Vector2 min)
        {
            input.x = MathUtil.Clamp(input.x, max.x, min.x);
            input.y = MathUtil.Clamp(input.y, max.y, min.y);
            return input;
        }

        #endregion

        #region Vector2 Trig

        public static Vector2 Slerp(Vector2 a, Vector2 b, float t)
        {
            var a3 = new Vector3(a.x, a.y, 0);
            var b3 = new Vector3(b.x, b.y, 0);
            var r3 = Vector3.Slerp(a3, b3, t);
            return new Vector2(r3.x, r3.y);
        }

        /// <summary>
        /// Get the angle in degrees off the forward defined by x.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float Angle(Vector2 v)
        {
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Get the angle in degrees off the forward defined by x.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float Angle(float x, float y)
        {
            return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        }

        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            return Mathf.Acos(Vector2.Dot(a, b) / (a.magnitude * b.magnitude)) * Mathf.Rad2Deg;
        }

        public static void Reflect(ref Vector2 v, Vector2 normal)
        {
            var dp = 2 * Vector2.Dot(v, normal);
            var ix = v.x - normal.x * dp;
            var iy = v.y - normal.y * dp;
            v.x = ix;
            v.y = iy;
        }

        public static Vector2 Reflect(Vector2 v, Vector2 normal)
        {
            var dp = 2 * Vector2.Dot(v, normal);
            return new Vector2(v.x - normal.x * dp, v.y - normal.y * dp);
        }

        /// <summary>
        /// Rotate Vector2 clockwise by 'a'
        /// </summary>
        /// <param name="v"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2 RotateBy(Vector2 v, float a, bool bUseRadians = false)
        {
            if (!bUseRadians) a *= Mathf.Deg2Rad;
            var ca = Math.Cos(a);
            var sa = Math.Sin(a);
            var rx = v.x * ca - v.y * sa;

            return new Vector2((float)rx, (float)(v.x * sa + v.y * ca));
        }

        /// <summary>
        /// Rotate Vector2 clockwise by 'a'
        /// </summary>
        /// <param name="v"></param>
        /// <param name="a"></param>
        public static void RotateBy(ref Vector2 v, float a, bool bUseRadians = false)
        {
            if (!bUseRadians) a *= Mathf.Deg2Rad;
            var ca = Math.Cos(a);
            var sa = Math.Sin(a);
            var rx = v.x * ca - v.y * sa;

            v.x = (float)rx;
            v.y = (float)(v.x * sa + v.y * ca);
        }

        public static Vector2 RotateToward(Vector2 from, Vector2 to, float a, bool bUseRadians = false)
        {
            var angleBetween = AngleBetween(from, to);
            if (bUseRadians) angleBetween *= Mathf.Deg2Rad;
            var t = angleBetween / a;
            return Slerp(from, to, t);
        }

        #endregion

        #region Vector3 Trig

        public static float AngleBetween(Vector3 a, Vector3 b)
        {
            return Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude)) * Mathf.Rad2Deg;
        }

        public static Vector3 GetForwardTangent(Vector3 mv, Vector3 up)
        {
            return Vector3.Cross(Vector3.Cross(up, mv), up);
        }

        /// <summary>
        /// Find some projected angle measure off some forward around some axis.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="forward"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis)
        {
            Vector3 right = Vector3.Cross(axis, forward);
            forward = Vector3.Cross(right, axis);
            Vector2 v2 = new Vector2(Vector3.Dot(v, forward), Vector3.Dot(v, right));
            v2.Normalize();
            return VectorUtil.Angle(v2);
        }

        #endregion

        #region Vector2 Mod

        public static Vector2 ClampToAxis(Vector2 v, Vector2 axis)
        {
            var n = new Vector2(-axis.y, axis.x);
            n.Normalize();
            return v - n * Vector2.Dot(v, n);
        }

        public static Vector2 SetLengthOnAxis(Vector2 v, Vector2 axis, float len)
        {
            //var n = new Vector2(-axis.y, axis.x);
            //n.Normalize();

            //var d = Vector2.Dot(v, n);
            //v -= n * d;
            //v = v.normalized * len;
            //v += n * d;
            //return v;

            axis.Normalize();
            var d = len - Vector2.Dot(v, axis);
            return v + axis * d;
        }

        #endregion

        #region Scale Vector

        public static float GetMaxScalar(Vector2 v)
        {
            return Mathf.Max(v.x, v.y);
        }

        public static float GetMaxScalar(Vector3 v)
        {
            return Mathf.Max(v.x, v.y, v.z);
        }

        public static float GetMaxScalar(Vector4 v)
        {
            return Mathf.Max(v.x, v.y, v.z, v.z);
        }

        public static float GetMinScalar(Vector2 v)
        {
            return Mathf.Min(v.x, v.y);
        }

        public static float GetMinScalar(Vector3 v)
        {
            return Mathf.Min(v.x, v.y, v.z);
        }

        public static float GetMinScalar(Vector4 v)
        {
            return Mathf.Min(v.x, v.y, v.z, v.z);
        }

        #endregion

        #region Compare Vector

        /// <summary>
        /// Compares if a and b are nearly on the same axis and will probably return a zero vector from a cross product
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool NearSameAxis(Vector3 a, Vector3 b, float epsilon = MathUtil.EPSILON)
        {
            return MathUtil.FuzzyEqual(Mathf.Abs(Vector3.Dot(a.normalized, b.normalized)), 1.0f, epsilon);
        }

        #endregion

        #region Lerp Like

        /// <summary>
        /// Moves from a to b at some speed dependent of a delta time with out passing b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="speed"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Vector2 SpeedLerp(Vector2 a, Vector2 b, float speed, float dt)
        {
            var v = b - a;
            var dv = speed * dt;
            if (dv > v.magnitude)
                return b;
            else
                return a + v.normalized * dv;
        }

        /// <summary>
        /// Moves from a to b at some speed dependent of a delta time with out passing b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="speed"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Vector3 SpeedLerp(Vector3 a, Vector3 b, float speed, float dt)
        {
            var v = b - a;
            var dv = speed * dt;
            if (dv > v.magnitude)
                return b;
            else
                return a + v.normalized * dv;
        }

        #endregion

        public static string ToDetailedString(this Vector2 v)
        {
            return String.Format("<{0}, {1}>", v.x, v.y);
        }
        public static string ToDetailedString(this Vector3 v)
        {
            return String.Format("<{0}, {1}, {2}>", v.x, v.y, v.z);
        }
    }
}

