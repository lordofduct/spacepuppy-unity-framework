using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Color32))]
    public class Color32MemberCurve : MemberCurve
    {

        #region Fields

        private Color32 _start;
        private Color32 _end;

        #endregion

        #region CONSTRUCTOR

        protected Color32MemberCurve()
        {

        }

        public Color32MemberCurve(float dur, Color32 start, Color32 end)
            : base(dur)
        {
            _start = start;
            _end = end;
        }

        public Color32MemberCurve(Ease ease, float dur, Color32 start, Color32 end)
            : base(ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void Init(object start, object end, bool slerp)
        {
            _start = ConvertUtil.ToColor32(start);
            _end = ConvertUtil.ToColor32(end);
        }

        #endregion

        #region Properties

        public Color32 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Color32 End
        {
            get { return _end; }
            set { _end = value; }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValue(float t)
        {
            return Color32.Lerp(_start, _end, t);
        }

        #endregion

    }
}
