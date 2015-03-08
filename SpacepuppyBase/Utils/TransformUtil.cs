using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{
    public static class TransformUtil
    {

        public static Vector3 GetAxis(CartesianAxis axis)
        {
            switch (axis)
            {
                case CartesianAxis.X:
                    return Vector3.right;
                case CartesianAxis.Y:
                    return Vector3.up;
                case CartesianAxis.Z:
                    return Vector3.forward;
            }

            return Vector3.zero;
        }

        public static Vector3 GetAxis(this Transform trans, CartesianAxis axis)
        {
            if (trans == null) throw new System.ArgumentNullException("trans");

            switch(axis)
            {
                case CartesianAxis.X:
                    return trans.right;
                case CartesianAxis.Y:
                    return trans.up;
                case CartesianAxis.Z:
                    return trans.forward;
            }

            return Vector3.zero;
        }

        public static Vector3 GetAxis(this Transform trans, CartesianAxis axis, bool inLocalSpace)
        {
            if (trans == null) throw new System.ArgumentNullException("trans");

            Vector3 v = Vector3.zero;
            switch (axis)
            {
                case CartesianAxis.X:
                    v = trans.right;
                    break;
                case CartesianAxis.Y:
                    v = trans.up;
                    break;
                case CartesianAxis.Z:
                    v = trans.forward;
                    break;
            }

            return (inLocalSpace) ? trans.InverseTransformDirection(v) : v;
        }

    }
}
