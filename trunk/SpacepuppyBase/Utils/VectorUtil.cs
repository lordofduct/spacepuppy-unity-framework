﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        public static string Stringify(Vector2 v)
        {
            return v.x.ToString() + "," + v.y.ToString();
        }

        public static string Stringify(Vector3 v)
        {
            return v.x.ToString() + "," + v.y.ToString() + "," + v.z.ToString();
        }

        /// <summary>
        /// Get Vector2 from angle
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2 AngleToVector2(float a, bool useRadians = false, bool yDominant = false)
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

        /// <summary>
        /// Angle in degrees off some axis in the counter-clockwise direction. Think of like 'Angle' or 'Atan2' where you get to control 
        /// which axis as opposed to only measuring off of <1,0>. 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float AngleOff(Vector2 v, Vector2 axis)
        {
            if (axis.sqrMagnitude < 0.0001f) return float.NaN;
            axis.Normalize();
            var tang = new Vector2(-axis.y, axis.x);
            return AngleBetween(v, axis) * Mathf.Sign(Vector2.Dot(v, tang));
        }

        public static void Reflect(ref Vector2 v, Vector2 normal)
        {
            var dp = 2f * Vector2.Dot(v, normal);
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

        public static void Mirror(ref Vector2 v, Vector2 axis)
        {
            v = (2 * (Vector2.Dot(v, axis) / Vector2.Dot(axis, axis)) * axis) - v;
        }

        public static Vector2 Mirror(Vector2 v, Vector2 axis)
        {
            return (2 * (Vector2.Dot(v, axis) / Vector2.Dot(axis, axis)) * axis) - v;
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
            var ca = System.Math.Cos(a);
            var sa = System.Math.Sin(a);
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
            var ca = System.Math.Cos(a);
            var sa = System.Math.Sin(a);
            var rx = v.x * ca - v.y * sa;

            v.x = (float)rx;
            v.y = (float)(v.x * sa + v.y * ca);
        }

        /// <summary>
        /// Rotates a vector toward another. Magnitude of the from vector is maintained.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="a"></param>
        /// <param name="bUseRadians"></param>
        /// <returns></returns>
        public static Vector2 RotateToward(Vector2 from, Vector2 to, float a, bool bUseRadians = false)
        {
            //var angleBetween = Mathf.Acos(Vector2.Dot(from, to) / (from.magnitude * to.magnitude));
            //if (!bUseRadians) a *= Mathf.Deg2Rad;
            //var t = angleBetween / a;
            //return Slerp(from, to, t);

            if (!bUseRadians) a *= Mathf.Deg2Rad;
            var a1 = Mathf.Atan2(from.y, from.x);
            var a2 = Mathf.Atan2(to.y, to.x);
            a2 = MathUtil.NormalizeAngleToAnother(a2, a1, true);
            var ra = (a2 - a1 >= 0f) ? a1 + a : a1 - a;
            var l = from.magnitude;
            return new Vector2(Mathf.Cos(ra) * l, Mathf.Sin(ra) * l);
        }

        public static Vector2 RotateTowardClamped(Vector2 from, Vector2 to, float a, bool bUseRadians = false)
        {
            if (!bUseRadians) a *= Mathf.Deg2Rad;
            var a1 = Mathf.Atan2(from.y, from.x);
            var a2 = Mathf.Atan2(to.y, to.x);
            a2 = MathUtil.NormalizeAngleToAnother(a2, a1, true);

            var da = a2 - a1;
            var ra = a1 + Mathf.Clamp(Mathf.Abs(a), 0f, Mathf.Abs(da)) * Mathf.Sign(da);

            var l = from.magnitude;
            return new Vector2(Mathf.Cos(ra) * l, Mathf.Sin(ra) * l);
        }

        /// <summary>
        /// Angular interpolates between two vectors.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns>The vectors are 2 dimensional, so technically this is not a spherical linear interpolation. The name Slerp is kept for consistency. 
        /// The result would be if you Slerped between 2 Vector3's that had a z value of 0. The direction interpolates at an angular rate, where as the 
        /// magnitude interpolates at a linear rate.</returns>
        public static Vector2 Slerp(Vector2 from, Vector2 to, float t)
        {
            var a = MathUtil.NormalizeAngle(Mathf.Lerp(Mathf.Atan2(from.y, from.x), Mathf.Atan2(to.y, to.x), t), true);
            var l = Mathf.Lerp(from.magnitude, to.magnitude, t);
            return new Vector2(Mathf.Cos(a) * l, Mathf.Sin(a) * l);
        }

        public static Vector2 Orth(Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }

        #endregion

        #region Vector3 Trig

        public static float AngleBetween(Vector3 a, Vector3 b)
        {
            return Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Returns a vector adjacent to up in the general direction of forward.
        /// </summary>
        /// <param name="up"></param>
        /// <param name="targForward"></param>
        /// <returns></returns>
        public static Vector3 GetForwardTangent(Vector3 forward, Vector3 up)
        {
            return Vector3.Cross(Vector3.Cross(up, forward), up);
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

        public static Vector2 Normalize(Vector2 v)
        {
            return v.normalized;
        }

        public static Vector2 Normalize(float x, float y)
        {
            float l = Mathf.Sqrt(x * x + y * y);
            return new Vector2(x / l, y / l);
        }

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

        #region Vector3 Mod

        public static Vector3 Normalize(float x, float y, float z)
        {
            float l = Mathf.Sqrt(x * x + y * y + z * z);
            return new Vector3(x / l, y / l, z / l);
        }

        public static Vector3 SetLengthOnAxis(Vector3 v, Vector3 axis, float len)
        {
            axis.Normalize();
            var d = len - Vector3.Dot(v, axis);
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

        public static bool NearZeroVector(Vector3 v)
        {
            return MathUtil.FuzzyEqual(v.sqrMagnitude, 0f, MathUtil.EPSILON_SQR);
        }

        public static bool NearZeroVector(Vector2 v)
        {
            return MathUtil.FuzzyEqual(v.sqrMagnitude, 0f, MathUtil.EPSILON_SQR);
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


        #region Linq

        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            int cnt = 0;
            Vector3 sum = Vector3.zero;
            foreach(var v in vectors)
            {
                cnt++;
                sum += v;
            }
            return (cnt > 0) ? sum / (float)cnt : Vector3.zero;
        }

        public static Vector3 SphericalAverage(this IEnumerable<Vector3> vectors)
        {
            int cnt = 0;
            float theta = 0f;
            float phi = 0f;
            foreach(var v in vectors)
            {
                cnt++;
                theta += Mathf.Acos(v.z / Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z));
                phi += Mathf.Atan(v.y / v.x);
            }
            if (cnt == 0 || float.IsNaN(theta)) return Vector3.zero;
            theta /= (float)cnt;
            phi /= (float)cnt;
            float st = Mathf.Sin(theta);
            return new Vector3(st * Mathf.Cos(phi), st * Mathf.Sin(phi), Mathf.Cos(theta));
        }

        public static Vector2 Average(this IEnumerable<Vector2> vectors)
        {
            int cnt = 0;
            Vector2 sum = Vector3.zero;
            foreach (var v in vectors)
            {
                cnt++;
                sum += v;
            }
            return (cnt > 0) ? sum / (float)cnt : Vector2.zero;
        }

        public static Vector2 PolarAverage(this IEnumerable<Vector2> vectors)
        {
            int cnt = 0;
            float sum = 0f;
            foreach(var v in vectors)
            {
                cnt++;
                sum += Mathf.Atan2(v.y, v.x);
            }
            if (cnt == 0 || float.IsNaN(sum)) return Vector2.zero;
            sum /= (float)cnt;
            return new Vector2(Mathf.Cos(sum), Mathf.Sin(sum));
        }

        #endregion




        public static string ToDetailedString(this Vector2 v)
        {
            return System.String.Format("<{0}, {1}>", v.x, v.y);
        }
        public static string ToDetailedString(this Vector3 v)
        {
            return System.String.Format("<{0}, {1}, {2}>", v.x, v.y, v.z);
        }

    }
}

