using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Accessors
{

    [CustomTweenMemberAccessor(typeof(Rigidbody), "*Move")]
    [CustomTweenMemberAccessor(typeof(GameObject), "*Move")]
    [CustomTweenMemberAccessor(typeof(Transform), "*Move")]
    [CustomTweenMemberAccessor(typeof(IGameObjectSource), "*Move")]
    public class GeneralMoveAccessor : ITweenMemberAccessor
    {


        #region ImplicitCurve Interface

        public System.Type Init(string propName, string args)
        {
            return typeof(Vector3);
        }

        public object Get(object target)
        {
            var t = GameObjectUtil.GetTransformFromSource(target);
            if (t != null)
            {
                return t.position;
            }
            return Vector3.zero;
        }

        public void Set(object targ, object valueObj)
        {
            var value = ConvertUtil.ToVector3(valueObj);

            if (targ is Rigidbody)
            {
                var rb = targ as Rigidbody;
                rb.MovePosition(value - rb.position);
            }
            else
            {
                var trans = GameObjectUtil.GetTransformFromSource(targ);
                if (trans == null) return;

                var rb = trans.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    var dp = value - rb.position;
                    rb.velocity = dp / Time.fixedDeltaTime;
                    return;
                }

                //just update the position
                trans.position = value;
            }
        }

        #endregion


    }
}
