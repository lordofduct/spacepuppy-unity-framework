using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Quaternion))]
    public class QuaternionMemberCurve : MemberCurve
    {
        
        #region Fields

        private Quaternion _start;
        private Quaternion _end;
        private bool _useSlerp;

        #endregion

        #region CONSTRUCTOR

        protected QuaternionMemberCurve()
        {

        }

        public QuaternionMemberCurve(string propName, float dur, Quaternion start, Quaternion end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public QuaternionMemberCurve(string propName, float dur, Quaternion start, Quaternion end, bool slerp)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        public QuaternionMemberCurve(string propName, Ease ease, float dur, Quaternion start, Quaternion end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public QuaternionMemberCurve(string propName, Ease ease, float dur, Quaternion start, Quaternion end, bool slerp)
            : base( propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        protected override void ReflectiveInit(object start, object end, object option)
        {
            _start = ConvertUtil.ToQuaternion(start);
            _end = ConvertUtil.ToQuaternion(end);
            _useSlerp = ConvertUtil.ToBool(option);
        }

        #endregion

        #region Properties

        public Quaternion Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Quaternion End
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
            t = this.Ease(t, 0f, 1f, this.Duration);
            return (_useSlerp) ? Quaternion.Slerp(_start, _end, t) : Quaternion.Lerp(_start, _end, t);
        }

        #endregion

    }
}
