using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Rect))]
    public class RectMemberCurve : MemberCurve, ISupportRedirectToMemberCurve
    {

        #region Fields

        private Rect _start;
        private Rect _end;

        #endregion

        #region CONSTRUCTOR

        protected RectMemberCurve()
        {

        }

        public RectMemberCurve(string propName, float dur, Rect start, Rect end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
        }

        public RectMemberCurve(string propName, float dur, Rect start, Rect end, bool slerp)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
        }

        public RectMemberCurve(string propName, Ease ease, float dur, Rect start, Rect end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void ReflectiveInit(System.Type memberType, object start, object end, object option)
        {
            _start = (start is Rect) ? (Rect)start : new Rect();
            _end = (end is Rect) ? (Rect)end : new Rect();
        }

        void ISupportRedirectToMemberCurve.ConfigureAsRedirectTo(System.Type memberType, float totalDur, object current, object start, object end, object option)
        {
            //TODO - determine mid point
            this.ReflectiveInit(memberType, current, end, option);
            this.Duration = totalDur;
        }

        #endregion

        #region Properties

        public Rect Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Rect End
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

            return new Rect(Mathf.LerpUnclamped(_start.xMin, _end.xMin, t),
                             Mathf.LerpUnclamped(_start.yMin, _end.yMin, t),
                             Mathf.LerpUnclamped(_start.width, _end.width, t),
                             Mathf.LerpUnclamped(_start.height, _end.height, t));
        }

        #endregion

    }
}
