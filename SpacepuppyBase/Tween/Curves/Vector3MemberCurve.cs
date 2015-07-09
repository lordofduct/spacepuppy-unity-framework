using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Vector3))]
    public class Vector3MemberCurve : MemberCurve
    {

        #region Fields

        private Vector3 _start;
        private Vector3 _end;
        private bool _useSlerp;

        #endregion

        #region CONSTRUCTOR

        protected Vector3MemberCurve()
        {

        }

        public Vector3MemberCurve(string propName, float dur, Vector3 start, Vector3 end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public Vector3MemberCurve(string propName, float dur, Vector3 start, Vector3 end, bool slerp)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        public Vector3MemberCurve(string propName, Ease ease, float dur, Vector3 start, Vector3 end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public Vector3MemberCurve(string propName, Ease ease, float dur, Vector3 start, Vector3 end, bool slerp)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        protected override void ReflectiveInit(object start, object end, object option)
        {
            _start = ConvertUtil.ToVector3(start);
            _end = ConvertUtil.ToVector3(end);
            _useSlerp = ConvertUtil.ToBool(option);
        }

        #endregion

        #region Properties

        public Vector3 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector3 End
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
            return (_useSlerp) ? Vector3.Slerp(_start, _end, t) : Vector3.Lerp(_start, _end, t);
        }

        #endregion

    }
}
