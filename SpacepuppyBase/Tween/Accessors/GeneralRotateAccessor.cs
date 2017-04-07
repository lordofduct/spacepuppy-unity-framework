using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Accessors
{

    [CustomTweenMemberAccessor(typeof(GameObject), "*Rotate")]
    [CustomTweenMemberAccessor(typeof(Component), "*Rotate")]
    [CustomTweenMemberAccessor(typeof(IGameObjectSource), "*Rotate")]
    public class GeneralRotateAccessor : ITweenMemberAccessor
    {

        #region ImplicitCurve Interface

        public System.Type GetMemberType()
        {
            return typeof(Quaternion);
        }

        public System.Type Init(object target, string propName, string args)
        {
            return typeof(Quaternion);
        }

        public object Get(object target)
        {
            var t = GameObjectUtil.GetTransformFromSource(target);
            if (t != null)
            {
                return t.rotation;
            }
            return Quaternion.identity;
        }

        public void Set(object targ, object valueObj)
        {
            var value = ConvertUtil.ToQuaternion(valueObj);

            if (targ is Rigidbody)
            {
                var rb = targ as Rigidbody;
                rb.MoveRotation(QuaternionUtil.FromToRotation(rb.rotation, value));
            }
            else
            {
                var trans = GameObjectUtil.GetTransformFromSource(targ);
                if (trans == null) return;

                var rb = trans.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    rb.MoveRotation(QuaternionUtil.FromToRotation(rb.rotation, value));
                    return;
                }

                //just update the rotation
                trans.rotation = value;
            }
        }

        #endregion

    }
}
