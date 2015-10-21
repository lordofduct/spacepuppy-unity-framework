using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Accessors
{

    [CustomTweenMemberAccessor(typeof(Rigidbody), "MovePosition")]
    public class RigidbodyMovePositionAccessor : ITweenMemberAccessor
    {

        #region ITweenMemberAccessor Interface

        public System.Type Init(string propName, string args)
        {
            return typeof(Vector3);
        }

        public object Get(object target)
        {
            var rb = target as Rigidbody;
            if (rb != null)
            {
                return rb.position;
            }
            return Vector3.zero;
        }

        public void Set(object target, object value)
        {
            var rb = target as Rigidbody;
            if (rb != null)
            {
                rb.MovePosition(ConvertUtil.ToVector3(value) - rb.position);
            }
        }

        #endregion

    }
}
