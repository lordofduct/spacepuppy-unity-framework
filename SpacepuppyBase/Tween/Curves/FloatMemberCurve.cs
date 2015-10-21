
using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(float))]
    public class FloatMemberCurve : MemberCurve
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

        protected override void ReflectiveInit(object start, object end, object option)
        {
            _start = ConvertUtil.ToSingle(start);
            _end = ConvertUtil.ToSingle(end);
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
