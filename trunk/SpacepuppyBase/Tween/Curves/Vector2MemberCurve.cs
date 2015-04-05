using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Vector2))]
    public class Vector2MemberCurve : MemberCurve
    {
        
        #region Fields

        private Vector2 _start;
        private Vector2 _end;
        private bool _useSlerp;

        #endregion

        #region CONSTRUCTOR

        protected Vector2MemberCurve()
        {

        }

        public Vector2MemberCurve(string propName, float dur, Vector2 start, Vector2 end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public Vector2MemberCurve(string propName, float dur, Vector2 start, Vector2 end, bool slerp)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        public Vector2MemberCurve(string propName, Ease ease, float dur, Vector2 start, Vector2 end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public Vector2MemberCurve(string propName, Ease ease, float dur, Vector2 start, Vector2 end, bool slerp)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        protected override void ReflectiveInit(object start, object end, object option)
        {
            _start = ConvertUtil.ToVector2(start);
            _end = ConvertUtil.ToVector2(end);
            _useSlerp = ConvertUtil.ToBool(option);
        }

        #endregion

        #region Properties

        public Vector2 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector2 End
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

        protected override object GetValue(float t)
        {
            return (_useSlerp) ? VectorUtil.Slerp(_start, _end, t) : Vector2.Lerp(_start, _end, t);
        }

        #endregion

    }
}
