
using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(float))]
    public class FloatMemberCurve : MemberCurve, ISupportRedirectToMemberCurve
    {
        
        #region Fields

        private float _start;
        private float _end;

        #endregion

        #region CONSTRUCTOR

        protected FloatMemberCurve()
        {

        }

        public FloatMemberCurve(string propName, float dur, float start, float end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
        }

        public FloatMemberCurve(string propName, Ease ease, float dur, float start, float end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void ReflectiveInit(System.Type memberType, object start, object end, object option)
        {
            _start = ConvertUtil.ToSingle(start);
            _end = ConvertUtil.ToSingle(end);
        }

        void ISupportRedirectToMemberCurve.ConfigureAsRedirectTo(System.Type memberType, float totalDur, object current, object start, object end, object option)
        {
            var c = ConvertUtil.ToSingle(current);
            var s = ConvertUtil.ToSingle(_start);
            var e = ConvertUtil.ToSingle(end);
            _start = c;
            _end = e;

            c -= e;
            s -= e;
            this.Duration = System.Math.Abs(s) < MathUtil.EPSILON ? 0f : totalDur * c / s;
        }

        #endregion

        #region Properties

        public float Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public float End
        {
            get { return _end; }
            set { _end = value; }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValueAt(float dt, float t)
        {
            if (this.Duration == 0f) return _end;
            return this.Ease(t, _start, _end - _start, this.Duration);
        }

        #endregion

    }
}
