using UnityEngine;

using com.spacepuppy.Tween.Accessors;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(FollowTargetPositionAccessor))]
    public class FollowTargetPositionCurve : MemberCurve
    {

        #region Fields

        private Vector3 _start;
        private Transform _target;

        #endregion

        #region CONSTRUCTOR

        protected FollowTargetPositionCurve()
        {

        }

        public FollowTargetPositionCurve(string propName, float dur, Vector3 start, Transform target)
            : base(propName, dur)
        {
            _start = start;
            _target = target;
        }

        public FollowTargetPositionCurve(string propName, float dur, Transform start, Transform target)
            : base(propName, dur)
        {
            _start = start.position;
            _target = target;
        }

        public FollowTargetPositionCurve(string propName, Ease ease, float dur, Vector3 start, Transform target)
            : base(propName, ease, dur)
        {
            _start = start;
            _target = target;
        }

        public FollowTargetPositionCurve(string propName, Ease ease, float dur, Transform start, Transform target)
            : base(propName, ease, dur)
        {
            _start = start.position;
            _target = target;
        }

        protected override void ReflectiveInit(System.Type memberType, object start, object end, object option)
        {
            var trans = GameObjectUtil.GetTransformFromSource(start);
            if (trans != null)
                _start = trans.position;
            else
                _start = ConvertUtil.ToVector3(start);

            _target = GameObjectUtil.GetTransformFromSource(end);
        }

        #endregion

        #region Methods

        protected override object GetValueAt(float dt, float t)
        {
            return VectorUtil.Lerp(_start, _target.position, EaseMethods.LinearEaseNone(t, 0f, 1f, this.Duration));
        }

        #endregion
    }
}
