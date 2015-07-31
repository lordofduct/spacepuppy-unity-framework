using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Geom
{

    /// <summary>
    /// In this class 'ideal conditions' is defined as a frictionless system with no air resistance and constant gravitational acceleration.
    /// </summary>
    /// <notes>
    /// v2 − u2 = 2as
    /// 
    /// v = final velocity
    /// u = initial velocity
    /// a = acceleration force
    /// s = distance covered
    /// </notes>
    public static class TrajectoryUtil
    {

        /// <summary>
        /// Returns the farthest range of a projectile thrown in ideal conditions over flat ground.
        /// </summary>
        /// <param name="initialSpeed"></param>
        /// <returns></returns>
        public static float GetIdealRangeOfProjectile(float initialSpeed, float gravity)
        {
            //R = speed^2 * sin(2*theta) / gravity
            //max range in an ideal conditions will be 45 degrees, sin(2 *45) = 1
            //reduces to: R = s^2 / gravity
            return initialSpeed * initialSpeed / gravity;
        }

        public static float GetIdealMaxHeightOfProjectile(Vector2 initialVelocity, float gravity)
        {
            return GetIdealMaxHeightOfProjectile(initialVelocity.y, gravity);
        }

        public static float GetIdealMaxHeightOfProjectile(float initialSpeed, float angle, float gravity, bool useRadians = false)
        {
            if (!useRadians) angle *= Mathf.Deg2Rad;
            var s = Mathf.Cos(angle) * initialSpeed;
            return GetIdealMaxHeightOfProjectile(s, gravity);
        }

        public static float GetIdealMaxHeightOfProjectile(float initialSpeed, float gravity)
        {
            return Mathf.Max(initialSpeed * initialSpeed / (2 * gravity), 0f);
        }

        /// <summary>
        /// Finds the best launch velocity to travel from 'from' to 'to' with some initialSpeed and constant gravity. 
        /// Two probably paths can potentially be calculated, the shorter of which is chosen.
        /// If the target position 'to' is out of range than velocity is set to a velocity straight at the target and false is returned.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="initialSpeed"></param>
        /// <param name="gravity"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public static bool CalculateLaunchVelocity(Vector2 from, Vector2 to, float initialSpeed, float gravity, out Vector2 velocity)
        {
            var dv = to - from;
            velocity = new Vector2(0f, initialSpeed * MathUtil.Sign(dv.y, 1f));

            if (Mathf.Abs(dv.x) < 0.001f) return false;

            var dx = Mathf.Abs(dv.x);
            var dy = dv.y;

            //theta = atan( (v^2 +/- Sqrt(v^4 - g(gx^2 + 2yv^2))) / (gx) )
            var v2 = initialSpeed * initialSpeed;
            var v4 = v2 * v2;
            var gx = gravity * dx;
            var root = Mathf.Sqrt(v4 - gravity * (gx * dx + 2f * dy * v2));
            var theta1 = Mathf.Atan((v2 + root) / gx);
            var theta2 = Mathf.Atan((v2 - root) / gx);

            float a;
            if (float.IsNaN(theta1))
            {
                if (float.IsNaN(theta2)) return false;
                else a = theta2;
            }
            else
            {
                if (!float.IsNaN(theta2)) a = Mathf.Min(theta1, theta2);
                else a = theta1;
            }

            velocity = new Vector2(Mathf.Sin(a) * Mathf.Sign(dv.x), Mathf.Cos(a));
            velocity *= initialSpeed;
            return true;
        }

        /// <summary>
        /// Finds the best launch velocity to travel from 'from' to 'to' with some initialSpeed and constant gravity. 
        /// Two probably paths can potentially be calculated, the shorter of which is chosen.
        /// If the target position 'to' is out of range than velocity is set to a velocity straight at the target and false is returned.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="initialSpeed"></param>
        /// <param name="gravity"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public static bool CalculateLaunchVelocity(Vector3 from, Vector3 to, float initialSpeed, float gravity, out Vector3 velocity)
        {
            var dv = to - from;
            velocity = dv.normalized * initialSpeed;

            var a = Vector3.Angle(dv, Vector3.up);
            if (a < 0.001f || (180f - a) < 0.001f) return false;

            var dir = new Vector3(dv.x, 0f, dv.z);
            var dx = dir.magnitude;
            var dy = dv.y;

            //theta = atan( (v^2 +/- Sqrt(v^4 - g(gx^2 + 2yv^2))) / (gx) )
            var v2 = initialSpeed * initialSpeed;
            var v4 = v2 * v2;
            var gx = gravity * dx;
            var root = Mathf.Sqrt(v4 - gravity * (gx * dx + 2f * dy * v2));
            var theta1 = Mathf.Atan((v2 + root) / gx);
            var theta2 = Mathf.Atan((v2 - root) / gx);

            if (float.IsNaN(theta1))
            {
                if (float.IsNaN(theta2)) return false;
                else a = theta2;
            }
            else
            {
                if (!float.IsNaN(theta2)) a = Mathf.Min(theta1, theta2);
                else a = theta1;
            }

            dir.Normalize();
            var right = Vector3.Cross(dir, Vector3.up);
            velocity = Quaternion.AngleAxis(a * Mathf.Rad2Deg, right) * dir * initialSpeed;
            return true;
        }

    }
}
