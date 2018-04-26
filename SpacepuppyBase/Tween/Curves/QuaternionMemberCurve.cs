using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Quaternion))]
    public class QuaternionMemberCurve : MemberCurve, ISupportRedirectToMemberCurve
    {
        
        #region Fields

        private Quaternion _start;
        private Quaternion _end;
        private Vector3 _startLong;
        private Vector3 _endLong;
        private QuaternionTweenOption _option;

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
            _startLong = start.eulerAngles;
            _endLong = end.eulerAngles;
            _option = QuaternionTweenOption.Spherical;
        }

        public QuaternionMemberCurve(string propName, float dur, Quaternion start, Quaternion end, QuaternionTweenOption mode)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _startLong = start.eulerAngles;
            _endLong = end.eulerAngles;
            _option = mode == QuaternionTweenOption.Long ? QuaternionTweenOption.Spherical : mode;
        }

        public QuaternionMemberCurve(string propName, Ease ease, float dur, Quaternion start, Quaternion end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _startLong = start.eulerAngles;
            _endLong = end.eulerAngles;
            _option = QuaternionTweenOption.Spherical;
        }

        public QuaternionMemberCurve(string propName, Ease ease, float dur, Quaternion start, Quaternion end, QuaternionTweenOption mode)
            : base( propName, ease, dur)
        {
            _start = start;
            _end = end;
            _startLong = start.eulerAngles;
            _endLong = end.eulerAngles;
            _option = mode == QuaternionTweenOption.Long ? QuaternionTweenOption.Spherical : mode;
        }

        public QuaternionMemberCurve(string propName, Ease ease, float dur, Vector3 eulerStart, Vector3 eulerEnd, QuaternionTweenOption mode)
            : base(propName, ease, dur)
        {
            _option = mode;
            _start = Quaternion.Euler(eulerStart);
            _end = Quaternion.Euler(eulerEnd);
            _startLong = eulerStart;
            _endLong = eulerEnd;
        }

        protected override void ReflectiveInit(System.Type memberType, object start, object end, object option)
        {
            _option = ConvertUtil.ToEnum<QuaternionTweenOption>(option);
            if(_option == QuaternionTweenOption.Long)
            {
                _startLong = QuaternionUtil.MassageAsEuler(start);
                _endLong = QuaternionUtil.MassageAsEuler(end);
                _start = Quaternion.Euler(_startLong);
                _end = Quaternion.Euler(_endLong);
            }
            else
            {
                _start = QuaternionUtil.MassageAsQuaternion(start);
                _end = QuaternionUtil.MassageAsQuaternion(end);
                _startLong = _start.eulerAngles;
                _endLong = _end.eulerAngles;
            }
        }

        void ISupportRedirectToMemberCurve.ConfigureAsRedirectTo(System.Type memberType, float totalDur, object current, object start, object end, object option)
        {
            _option = ConvertUtil.ToEnum<QuaternionTweenOption>(option);
            if (_option == QuaternionTweenOption.Long)
            {
                var c = QuaternionUtil.MassageAsEuler(current);
                var s = QuaternionUtil.MassageAsEuler(start);
                var e = QuaternionUtil.MassageAsEuler(end);

                c.x = MathUtil.NormalizeAngleToRange(c.x, s.x, e.x, false);
                c.y = MathUtil.NormalizeAngleToRange(c.y, s.y, e.y, false);
                c.z = MathUtil.NormalizeAngleToRange(c.z, s.z, e.z, false);

                _startLong = c;
                _endLong = e;
                _start = Quaternion.Euler(_startLong);
                _end = Quaternion.Euler(_endLong);
                
                c -= s;
                e -= s;
                this.Duration = totalDur * (VectorUtil.NearZeroVector(e) ? 0f : 1f - c.magnitude / e.magnitude);
            }
            else
            {
                //treat as quat
                var c = QuaternionUtil.MassageAsQuaternion(current);
                var s = QuaternionUtil.MassageAsQuaternion(start);
                var e = QuaternionUtil.MassageAsQuaternion(end);
                _start = c;
                _end = e;
                _startLong = _start.eulerAngles;
                _endLong = _end.eulerAngles;

                var at = Quaternion.Angle(s, e);
                if ((System.Math.Abs(at) < MathUtil.EPSILON))
                {
                    this.Duration = 0f;
                }
                else
                {
                    var ap = Quaternion.Angle(s, c);
                    this.Duration = (1f - ap / at) * totalDur;
                }
            }
        }

        #endregion

        #region Properties

        public Quaternion Start
        {
            get { return _start; }
            set
            {
                _start = value;
                _startLong = value.eulerAngles;
            }
        }

        public Quaternion End
        {
            get { return _end; }
            set
            {
                _end = value;
                _endLong = value.eulerAngles;
            }
        }

        public Vector3 StartEuler
        {
            get { return _startLong; }
            set
            {
                _startLong = value;
                _start = Quaternion.Euler(value);
            }
        }

        public Vector3 EndEuler
        {
            get { return _endLong; }
            set
            {
                _endLong = value;
                _end = Quaternion.Euler(value);
            }
        }

        public QuaternionTweenOption Option
        {
            get { return _option; }
            set { _option = value; }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValueAt(float dt, float t)
        {
            if (this.Duration == 0f) return _end;
            t = this.Ease(t, 0f, 1f, this.Duration);
            switch(_option)
            {
                case QuaternionTweenOption.Spherical:
                    return Quaternion.SlerpUnclamped(_start, _end, t);
                case QuaternionTweenOption.Linear:
                    return Quaternion.LerpUnclamped(_start, _end, t);
                case QuaternionTweenOption.Long:
                    {
                        var v = Vector3.LerpUnclamped(_startLong, _endLong, t);
                        return Quaternion.Euler(v);
                    }
                default:
                    return Quaternion.identity;
            }
        }

        #endregion
        
    }

}
