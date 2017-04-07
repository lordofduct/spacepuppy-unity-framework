using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Accessors
{

    [CustomTweenMemberAccessor(typeof(GameObject), "*Follow")]
    [CustomTweenMemberAccessor(typeof(Component), "*Follow")]
    [CustomTweenMemberAccessor(typeof(IGameObjectSource), "*Follow")]
    public class FollowTargetPositionAccessor : ITweenMemberAccessor
    {



        public System.Type GetMemberType()
        {
            return typeof(Vector3);
        }

        public System.Type Init(object target, string propName, string args)
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


    }

}
