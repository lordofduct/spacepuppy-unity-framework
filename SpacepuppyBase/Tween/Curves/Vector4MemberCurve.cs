using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Vector4))]
    public class Vector4MemberCurve : MemberCurve, ISupportRedirectToMemberCurve
    {
        
        #region Fields

        private Vector4 _start;
        private Vector4 _end;

        #endregion

        #region CONSTRUCTOR

        protected Vector4MemberCurve()
        {

        }

        public Vector4MemberCurve(string propName, float dur, Vector4 start, Vector4 end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
        }

        public Vector4MemberCurve(string propName, Ease ease, float dur, Vector4 start, Vector4 end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void ReflectiveInit(System.Type memberType, object start, object end, object option)
        {
            _start = ConvertUtil.ToVector4(start);
            _end = ConvertUtil.ToVector4(end);
        }

        void ISupportRedirectToMemberCurve.ConfigureAsRedirectTo(System.Type memberType, float totalDur, object current, object start, object end, object option)
        {
            var c = ConvertUtil.ToVector4(current);
            var s = ConvertUtil.ToVector4(start);
            var e = ConvertUtil.ToVector4(end);
            _start = c;
            _end = e;

            c -= s;
            e -= s;
            this.Duration = totalDur * (VectorUtil.NearZeroVector(e) ? 0f : 1f - c.magnitude / e.magnitude);
        }

        #endregion

        #region Properties

        public Vector4 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector4 End
        {
            get { return _end; }
            set { _end = value; }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValueAt(float dt, float t)
        {
            if (this.Duration == 0f) return _end;
            t = this.Ease(t, 0f, 1f, this.Duration);
            return Vector4.LerpUnclamped(_start, _end, t);
        }

        #endregion

    }
}
