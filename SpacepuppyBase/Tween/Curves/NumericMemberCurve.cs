using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(float), priority = int.MinValue)]
    [CustomMemberCurve(typeof(double), priority=int.MinValue)]
    [CustomMemberCurve(typeof(decimal), priority = int.MinValue)]
    [CustomMemberCurve(typeof(sbyte), priority = int.MinValue)]
    [CustomMemberCurve(typeof(int), priority = int.MinValue)]
    [CustomMemberCurve(typeof(long), priority = int.MinValue)]
    [CustomMemberCurve(typeof(byte), priority = int.MinValue)]
    [CustomMemberCurve(typeof(uint), priority = int.MinValue)]
    [CustomMemberCurve(typeof(ulong), priority = int.MinValue)]
    public class NumericMemberCurve : MemberCurve
    {

        #region Fields

        private double _start;
        private double _end;
        private TypeCode _numericType = TypeCode.Double;

        #endregion

        #region CONSTRUCTOR

        protected NumericMemberCurve()
        {

        }

        public NumericMemberCurve(float dur, double start, double end)
            : base(dur)
        {
            _start = start;
            _end = end;
        }

        public NumericMemberCurve(Ease ease, float dur, double start, double end) : base(ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void Init(object start, object end, bool slerp)
        {
            _start = ConvertUtil.ToDouble(start);
            _end = ConvertUtil.ToDouble(end);
        }

        #endregion

        #region Properties

        public double Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public double End
        {
            get { return _end; }
            set { _end = value; }
        }

        internal TypeCode NumericType
        {
            get { return _numericType; }
            set
            {
                if (!ConvertUtil.IsNumericType(value)) throw new System.ArgumentException("TypeCode must be a numeric type.", "value");
                _numericType = value;
            }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValue(float t)
        {
            return ConvertUtil.ToPrim((double)t * (_end - _start) + _start, _numericType);
        }

        #endregion

    }
}
