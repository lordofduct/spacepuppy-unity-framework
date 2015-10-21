using UnityEngine;

using com.spacepuppy.Geom;

namespace com.spacepuppy.Tween.Accessors
{

    [CustomTweenMemberAccessor(typeof(Transform), "GlobalTrans")]
    public class TransformGlobalTransAccessor : ITweenMemberAccessor
    {

        #region ITweenMemberAccessor Interface

        public System.Type Init(string propName, string args)
        {
            return typeof(Trans);
        }

        public object Get(object target)
        {
            var trans = target as Transform;
            if (trans != null)
            {
                return Trans.GetGlobal(trans);
            }
            return Trans.Identity;
        }

        public void Set(object target, object value)
        {
            if (!(value is Trans)) return;
            var trans = target as Transform;
            if (trans != null)
            {
                ((Trans)value).SetToGlobal(trans, true);
            }
        }

        #endregion

    }
}
