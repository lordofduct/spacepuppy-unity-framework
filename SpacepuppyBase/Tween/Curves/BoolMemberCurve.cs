
using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    /// <summary>
    /// The BoolMemberCurve favors 'true'.
    /// </summary>
    [CustomMemberCurve(typeof(bool))]
    public class BoolMemberCurve : MemberCurve
    {

        #region Fields

        private bool _start;
        private bool _end;

        #endregion

        #region CONSTRUCTOR

        protected BoolMemberCurve()
        {

        }

        public BoolMemberCurve(string propName, float dur, bool start, bool end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
        }

        public BoolMemberCurve(string propName, Ease ease, float dur, bool start, bool end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void ReflectiveInit(object start, object end, object option)
        {
            _start = ConvertUtil.ToBool(start);
            _end = ConvertUtil.ToBool(end);
        }

        #endregion

        #region Properties

        public bool Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public bool End
        {
            get { return _end; }
            set { _end = value; }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValueAt(float dt, float t)
        {
            if (this.Duration == 0f) return _end;
            //return this.Ease(t, _start, _end - _start, this.Duration);

            if (_end)
                return t > 0f;
            else
                return t < this.Duration;
        }

        #endregion

    }
}
