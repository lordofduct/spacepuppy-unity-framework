using UnityEngine;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Accessors
{

    [CustomTweenMemberAccessor(typeof(GameObject), "*relativePosition")]
    [CustomTweenMemberAccessor(typeof(Transform), "*relativePosition")]
    [CustomTweenMemberAccessor(typeof(IGameObjectSource), "*relativePosition")]
    public class TransformRelativePositionAccessor : ITweenMemberAccessor
    {

        private Trans _initialTrans;

        #region ITweenMemberAccessor Interface

        public System.Type GetMemberType()
        {
            return typeof(Vector3);
        }

        public System.Type Init(object target, string propName, string args)
        {
            var trans = GameObjectUtil.GetTransformFromSource(target);
            if (trans != null)
            {
                _initialTrans = Trans.GetGlobal(trans);
            }
            else
            {
                _initialTrans = Trans.Identity;
            }

            return typeof(Vector3);
        }

        public object Get(object target)
        {
            var trans = GameObjectUtil.GetTransformFromSource(target);
            if (trans != null)
            {
                return _initialTrans.InverseTransformPoint(trans.position);
            }

            return Vector3.zero;
        }

        public void Set(object target, object value)
        {
            if (!(value is Vector3)) return;

            var trans = GameObjectUtil.GetTransformFromSource(target);
            if (trans != null)
            {
                trans.position = _initialTrans.TransformPoint((Vector3)value);
            }
        }

        #endregion

    }
}
