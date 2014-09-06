using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{
    public static class QuaternionUtil
    {

        /// <summary>
        /// Get the rotation that would be applied to 'start' to end up at 'end'.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Quaternion FromToRotation(Quaternion start, Quaternion end)
        {
            return end * Quaternion.Inverse(start);
        }

        public static Quaternion SpeedSlerp(Quaternion from, Quaternion to, float angularSpeed, float dt, bool bUseRadians = false)
        {
            if (bUseRadians) angularSpeed *= Mathf.Rad2Deg;
            var da = angularSpeed * dt;
            return Quaternion.RotateTowards(from, to, da);
        }

        #region Transform

        /// <summary>
        /// Transforms rotation from local space to world space.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="local"></param>
        /// <returns></returns>
        public static Quaternion TransformRotation(this Transform t, Quaternion local)
        {
            if (t == null) throw new System.ArgumentNullException("transform");
            return t.rotation * local;
        }

        /// <summary>
        /// Transforms rotation from world space to local space.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="global"></param>
        /// <returns></returns>
        public static Quaternion InverseTransformRotation(this Transform t, Quaternion global)
        {
            if (t == null) throw new System.ArgumentNullException("transform");
            return Quaternion.Inverse(t.rotation) * global;
        }

        #endregion

    }
}
