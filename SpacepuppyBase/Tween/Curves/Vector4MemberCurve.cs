using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Vector4))]
    public class Vector4MemberCurve : MemberCurve
    {
        
        #region Fields

        private Vector4 _start;
        private Vector4 _end;

        #endregion

        #region CONSTRUCTOR

        protected Vector4MemberCurve()
        {

        }

        public Vector4MemberCurve(string propName, float dur, Vector4 start, Vector4 end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
        }

        public Vector4MemberCurve(string propName, Ease ease, float dur, Vector4 start, Vector4 end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void ReflectiveInit(object start, object end, object option)
        {
            _start = ConvertUtil.ToVector4(start);
            _end = ConvertUtil.ToVector4(end);
        }

        #endregion

        #region Properties

        public Vector4 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector4 End
        {
            get { return _end; }
            set { _end = value; }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValueAt(float dt, float t)
        {
            t = this.Ease(t, 0f, 1f, this.Duration);
            return Vector4.Lerp(_start, _end, t);
        }

        #endregion

    }
}
