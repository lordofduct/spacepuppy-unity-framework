using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Anim
{
    public class IKSolver
    {
        

        public static void CalculateIK(Transform upperArm, Transform foreArm, Transform hand, Vector3 target, float lerpT = 1f, float elbowAngle = 0f)
        {
            Vector3 dir = target - upperArm.position;
            Quaternion r;

            //upper arm looks target
            r = Quaternion.LookRotation(foreArm.position - upperArm.position);
            r = Quaternion.Inverse(r) * upperArm.rotation;
            var upperArmRot = Quaternion.LookRotation(dir) * r;

            //upper arm IK angle
            var upperArmLength = Vector3.Distance(upperArm.position, foreArm.position);
            var forearmLength = Vector3.Distance(foreArm.position, hand.position);
            float dist = Mathf.Min(dir.magnitude, upperArmLength + forearmLength - 0.00001f);
            float adj = (upperArmLength * upperArmLength - forearmLength * forearmLength + dist * dist) / (2 * dist);
            float angle = Mathf.Acos(adj / upperArmLength) * Mathf.Rad2Deg;
            upperArmRot *= Quaternion.AngleAxis(-angle, upperArmRot * Vector3.forward);


            //forearm looks target
            r = Quaternion.LookRotation(hand.position - foreArm.position);
            r = Quaternion.Inverse(r) * foreArm.rotation;
            var foreArmRot = Quaternion.LookRotation(target - foreArm.position) * r;

            //elbow angle
            if (elbowAngle != 0f)
                upperArmRot *= Quaternion.AngleAxis(elbowAngle, dir);

            if (lerpT < 1f)
            {
                upperArm.rotation = Quaternion.Slerp(upperArm.rotation, upperArmRot, lerpT);
                foreArm.rotation = Quaternion.Slerp(foreArm.rotation, foreArmRot, lerpT);
            }
            else
            {
                upperArm.rotation = upperArmRot;
                foreArm.rotation = foreArmRot;
            }
        }

        public static void CalculateIK(Transform upperArm, Transform foreArm, Transform hand, Vector3 target, Vector3 forwardAxis, float lerpT = 1f, float elbowAngle = 0f)
        {
            Vector3 dir = target - upperArm.position;
            Quaternion r;

            //upper arm looks target
            r = Quaternion.LookRotation(foreArm.position - upperArm.position);
            r = Quaternion.Inverse(r) * upperArm.rotation;
            var upperArmRot = Quaternion.LookRotation(dir) * r;

            //upper arm IK angle
            var upperArmLength = Vector3.Distance(upperArm.position, foreArm.position);
            var forearmLength = Vector3.Distance(foreArm.position, hand.position);
            float dist = Mathf.Min(dir.magnitude, upperArmLength + forearmLength - 0.00001f);
            float adj = (upperArmLength * upperArmLength - forearmLength * forearmLength + dist * dist) / (2 * dist);
            float angle = Mathf.Acos(adj / upperArmLength) * Mathf.Rad2Deg;
            upperArmRot *= Quaternion.AngleAxis(-angle, upperArmRot * Vector3.forward);


            //forearm looks target
            r = Quaternion.LookRotation(hand.position - foreArm.position);
            r = Quaternion.Inverse(r) * foreArm.rotation;
            var foreArmRot = Quaternion.LookRotation(target - foreArm.position) * r;

            //elbow angle
            if (elbowAngle != 0f)
                upperArmRot *= Quaternion.AngleAxis(elbowAngle, dir);


            Quaternion adjust = Quaternion.FromToRotation(forwardAxis, Vector3.forward);
            upperArmRot *= adjust;
            foreArmRot *= adjust;

            if (lerpT < 1f)
            {
                upperArm.rotation = Quaternion.Slerp(upperArm.rotation, upperArmRot, lerpT);
                foreArm.rotation = Quaternion.Slerp(foreArm.rotation, foreArmRot, lerpT);
            }
            else
            {
                upperArm.rotation = upperArmRot;
                foreArm.rotation = foreArmRot;
            }
        }

    }
}
