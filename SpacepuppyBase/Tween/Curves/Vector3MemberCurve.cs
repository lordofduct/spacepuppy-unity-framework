using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Vector3))]
    public class Vector3MemberCurve : MemberCurve, ISupportRedirectToMemberCurve
    {

        #region Fields

        private Vector3 _start;
        private Vector3 _end;
        private bool _useSlerp;

        #endregion

        #region CONSTRUCTOR

        protected Vector3MemberCurve()
        {

        }

        public Vector3MemberCurve(string propName, float dur, Vector3 start, Vector3 end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public Vector3MemberCurve(string propName, float dur, Vector3 start, Vector3 end, bool slerp)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        public Vector3MemberCurve(string propName, Ease ease, float dur, Vector3 start, Vector3 end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public Vector3MemberCurve(string propName, Ease ease, float dur, Vector3 start, Vector3 end, bool slerp)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        protected override void ReflectiveInit(System.Type memberType, object start, object end, object option)
        {
            _start = ConvertUtil.ToVector3(start);
            _end = ConvertUtil.ToVector3(end);
            _useSlerp = ConvertUtil.ToBool(option);
        }

        void ISupportRedirectToMemberCurve.ConfigureAsRedirectTo(System.Type memberType, float totalDur, object current, object start, object end, object option)
        {
            _useSlerp = ConvertUtil.ToBool(option);

            if (_useSlerp)
            {
                var c = ConvertUtil.ToVector3(current);
                var s = ConvertUtil.ToVector3(start);
                var e = ConvertUtil.ToVector3(end);
                _start = c;
                _end = e;

                var at = Vector3.Angle(s, e);
                if ((System.Math.Abs(at) < MathUtil.EPSILON))
                {
                    this.Duration = 0f;
                }
                else
                {
                    var ap = Vector3.Angle(s, c);
                    this.Duration = (1f - ap / at) * totalDur;
                }
            }
            else
            {
                var c = ConvertUtil.ToVector3(current);
                var s = ConvertUtil.ToVector3(start);
                var e = ConvertUtil.ToVector3(end);
                _start = c;
                _end = e;

                c -= s;
                e -= s;
                this.Duration = totalDur * (VectorUtil.NearZeroVector(e) ? 0f : 1f - c.magnitude / e.magnitude);
            }
        }

        #endregion

        #region Properties

        public Vector3 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector3 End
        {
            get { return _end; }
            set { _end = value; }
        }

        public bool UseSlerp
        {
            get { return _useSlerp; }
            set { _useSlerp = value; }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValueAt(float dt, float t)
        {
            if (this.Duration == 0f) return _end;
            t = this.Ease(t, 0f, 1f, this.Duration);
            return (_useSlerp) ? Vector3.SlerpUnclamped(_start, _end, t) : Vector3.LerpUnclamped(_start, _end, t);
        }

        #endregion

    }
}
