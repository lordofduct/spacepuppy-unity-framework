using UnityEngine;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Accessors
{

    [CustomTweenMemberAccessor(typeof(GameObject), typeof(Trans), "*LocalTrans")]
    [CustomTweenMemberAccessor(typeof(Component), typeof(Trans), "*LocalTrans")]
    [CustomTweenMemberAccessor(typeof(IGameObjectSource), typeof(Trans), "*LocalTrans")]
    public class TransformLocalTransAccessor : ITweenMemberAccessor
    {

        private bool _includeScale;

        #region ITweenMemberAccessor Interface

        public System.Type GetMemberType()
        {
            return typeof(Trans);
        }

        public System.Type Init(object target, string propName, string args)
        {
            _includeScale = ConvertUtil.ToBool(args);

            return typeof(Trans);
        }

        public object Get(object target)
        {
            var trans = GameObjectUtil.GetTransformFromSource(target);
            if (trans != null)
            {
                return Trans.GetLocal(trans);
            }
            return Trans.Identity;
        }

        public void Set(object target, object value)
        {
            if (!(value is Trans)) return;
            var trans = GameObjectUtil.GetTransformFromSource(target);
            if (trans != null)
            {
                ((Trans)value).SetToLocal(trans, _includeScale);
            }
        }

        #endregion

    }
}
