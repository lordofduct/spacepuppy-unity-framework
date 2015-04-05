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

        public ColorMemberCurve(string propName, float dur, Color start, Color end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
        }

        public ColorMemberCurve(string propName, Ease ease, float dur, Color start, Color end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void ReflectiveInit(object start, object end, object option)
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
