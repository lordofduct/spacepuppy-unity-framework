
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Trans))]
    public class TransMemberCurve : MemberCurve, ISupportRedirectToMemberCurve
    {

        #region Fields

        private Trans _start;
        private Trans _end;
        private bool _useSlerp;

        #endregion

        #region CONSTRUCTOR

        protected TransMemberCurve()
        {

        }

        public TransMemberCurve(string propName, float dur, Trans start, Trans end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public TransMemberCurve(string propName, float dur, Trans start, Trans end, bool slerp)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        public TransMemberCurve(string propName, Ease ease, float dur, Trans start, Trans end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public TransMemberCurve(string propName, Ease ease, float dur, Trans start, Trans end, bool slerp)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        protected override void ReflectiveInit(System.Type memberType, object start, object end, object option)
        {
            _start = (start is Trans) ? (Trans)start : Trans.Identity;
            if (end is Trans)
                _end = (Trans)end;
            else if (end is UnityEngine.Transform)
                _end = Trans.GetGlobal((UnityEngine.Transform)end);
            else if (GameObjectUtil.IsGameObjectSource(end))
                _end = Trans.GetGlobal(GameObjectUtil.GetGameObjectFromSource(end).transform);
            else
                _end = Trans.Identity;
            _useSlerp = ConvertUtil.ToBool(option);
        }

        void ISupportRedirectToMemberCurve.ConfigureAsRedirectTo(System.Type memberType, float totalDur, object current, object start, object end, object option)
        {
            //TODO - determine mid point
            this.ReflectiveInit(memberType, current, end, option);
            this.Duration = totalDur;
        }

        #endregion

        #region Properties

        public Trans Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Trans End
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
            return (_useSlerp) ? Trans.Slerp(_start, _end, t) : Trans.Lerp(_start, _end, t);
        }

        #endregion

    }
}
