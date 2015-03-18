using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public FloatMemberCurve(float dur, float start, float end)
            : base(dur)
        {
            _start = start;
            _end = end;
        }

        public FloatMemberCurve(Ease ease, float dur, float start, float end)
            : base(ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void Init(object start, object end, bool slerp)
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

        protected override object GetValue(float t)
        {
            return Mathf.Lerp(_start, _end, t);
        }

        #endregion

    }
}
