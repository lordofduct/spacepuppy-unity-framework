using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Color))]
    public class ColorMemberCurve : MemberCurve
    {

        #region Fields

        private Color _start;
        private Color _end;

        #endregion

        #region CONSTRUCTOR

        protected ColorMemberCurve()
        {

        }

        public ColorMemberCurve(float dur, Color start, Color end)
            : base(dur)
        {
            _start = start;
            _end = end;
        }

        public ColorMemberCurve(Ease ease, float dur, Color start, Color end) : base(ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void Init(object start, object end, bool slerp)
        {
            _start = ConvertUtil.ToColor(start);
            _end = ConvertUtil.ToColor(end);
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

        #endregion

        #region MemberCurve Interface

        protected override object GetValue(float t)
        {
            return Color.Lerp(_start, _end, t);
        }

        #endregion

    }
}
