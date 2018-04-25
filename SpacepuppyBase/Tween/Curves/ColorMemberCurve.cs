using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Color))]
    public class ColorMemberCurve : MemberCurve
    {

        #region Fields

        private Color _start;
        private Color _end;
        private bool _useSlerp;

        #endregion

        #region CONSTRUCTOR

        protected ColorMemberCurve()
        {

        }

        public ColorMemberCurve(string propName, float dur, Color start, Color end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public ColorMemberCurve(string propName, Ease ease, float dur, Color start, Color end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public ColorMemberCurve(string propName, float dur, Color start, Color end, bool useSlerp)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = useSlerp;
        }

        public ColorMemberCurve(string propName, Ease ease, float dur, Color start, Color end, bool useSlerp)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = useSlerp;
        }

        protected override void ReflectiveInit(object start, object end, object option)
        {
            _start = ConvertUtil.ToColor(start);
            _end = ConvertUtil.ToColor(end);
            _useSlerp = ConvertUtil.ToBool(option);
        }

        #endregion

        #region Properties

        public Color Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Color End
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
            return _useSlerp ? ColorUtil.Slerp(_start, _end, t) : ColorUtil.Lerp(_start, _end, t);
        }

        #endregion

    }

}
