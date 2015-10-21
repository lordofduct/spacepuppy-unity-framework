using UnityEngine;

namespace com.spacepuppy.Utils
{
    public static class QuaternionUtil
    {

        public static string Stringify(Quaternion q)
        {
            return q.x.ToString() + "," + q.y.ToString() + "," + q.z.ToString() + q.w.ToString();
        }

        public static string ToDetailedString(this Quaternion v)
        {
            return System.String.Format("<{0}, {1}, {2}, {3}>", v.x, v.y, v.z, v.w);
        }

        public static Quaternion Normalize(Quaternion q)
        {
            var mag = System.Math.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
            q.w = (float)((double)q.w / mag);
            q.x = (float)((double)q.x / mag);
            q.y = (float)((double)q.y / mag);
            q.z = (float)((double)q.z / mag);
            return q;
        }
        
        /// <summary>
        /// A cleaner version of FromToRotation, Quaternion.FromToRotation for some reason can only handle down to #.## precision.
        /// This will result in true 7 digits of precision down to depths of 0.00000# (depth tested so far).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Quaternion FromToRotation(Vector3 v1, Vector3 v2)
        {
            var a = Vector3.Cross(v1, v2);
            var w = Mathf.Sqrt(v1.sqrMagnitude * v2.sqrMagnitude) + Vector3.Dot(v1, v2);
            return new Quaternion(a.x, a.y, a.z, w);
        }

        /// <summary>
        /// Get the rotation that would be applied to 'start' to end up at 'end'.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Quaternion FromToRotation(Quaternion start, Quaternion end)
        {
            return Quaternion.Inverse(start) * end;
        }
        
        public static Quaternion SpeedSlerp(Quaternion from, Quaternion to, float angularSpeed, float dt, bool bUseRadians = false)
        {
            if (bUseRadians) angularSpeed *= Mathf.Rad2Deg;
            var da = angularSpeed * dt;
            return Quaternion.RotateTowards(from, to, da);
        }

        #region Transform

        public static Quaternion IndependentLookRotation(Vector3 dir, Vector3 forwardAxis)
        {
            return Quaternion.LookRotation(dir) * Quaternion.FromToRotation(Vector3.forward, forwardAxis);
        }

        //DEPRECATED - moved to TransformUtil

        ///// <summary>
        ///// Transforms rotation from local space to world space.
        ///// </summary>
        ///// <param name="t"></param>
        ///// <param name="local"></param>
        ///// <returns></returns>
        //public static Quaternion TransformRotation(this Transform t, Quaternion local)
        //{
        //    if (t == null) throw new System.ArgumentNullException("transform");
        //    return t.rotation * local;
        //}

        ///// <summary>
        ///// Transforms rotation from world space to local space.
        ///// </summary>
        ///// <param name="t"></param>
        ///// <param name="global"></param>
        ///// <returns></returns>
        //public static Quaternion InverseTransformRotation(this Transform t, Quaternion global)
        //{
        //    if (t == null) throw new System.ArgumentNullException("transform");
        //    return Quaternion.Inverse(t.rotation) * global;
        //}


        /// <summary>
        /// Returns a rotation of up attempting to face in the general direction of forward.
        /// </summary>
        /// <param name="up"></param>
        /// <param name="targForward"></param>
        /// <returns></returns>
        public static Quaternion FaceRotation(Vector3 forward, Vector3 up)
        {
            forward = VectorUtil.GetForwardTangent(forward, up);
            return Quaternion.LookRotation(forward, up);
        }

        public static void GetAngleAxis(this Quaternion q, out Vector3 axis, out float angle)
        {
            if (q.w > 1) q = QuaternionUtil.Normalize(q);

            //get as doubles for precision
            var qw = (double)q.w;
            var qx = (double)q.x;
            var qy = (double)q.y;
            var qz = (double)q.z;
            var ratio = System.Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * System.Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }

        public static void GetShortestAngleAxisBetween(Quaternion a, Quaternion b, out Vector3 axis, out float angle)
        {
            var dq = Quaternion.Inverse(a) * b;
            if (dq.w > 1) dq = QuaternionUtil.Normalize(dq);

            //get as doubles for precision
            var qw = (double)dq.w;
            var qx = (double)dq.x;
            var qy = (double)dq.y;
            var qz = (double)dq.z;
            var ratio = System.Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * System.Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }

        #endregion

    }
}
