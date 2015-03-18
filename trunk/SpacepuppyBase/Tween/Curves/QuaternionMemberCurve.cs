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

        public QuaternionMemberCurve(float dur, Quaternion start, Quaternion end, bool slerp = false)
            : base(dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        public QuaternionMemberCurve(Ease ease, float dur, Quaternion start, Quaternion end, bool slerp = false) : base(ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        protected override void Init(object start, object end, bool slerp)
        {
            _start = ConvertUtil.ToQuaternion(start);
            _end = ConvertUtil.ToQuaternion(end);
            _useSlerp = slerp;
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

        protected override object GetValue(float t)
        {
            return (_useSlerp) ? Quaternion.Slerp(_start, _end, t) : Quaternion.Lerp(_start, _end, t);
        }

        #endregion

    }
}
