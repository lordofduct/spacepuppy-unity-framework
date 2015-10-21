using System;

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

        public NumericMemberCurve(string propName, float dur, double start, double end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
        }

        public NumericMemberCurve(string propName, Ease ease, float dur, double start, double end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
        }

        protected override void ReflectiveInit(object start, object end, object option)
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

        protected override object GetValueAt(float dt, float t)
        {
            if (this.Duration == 0) return ConvertUtil.ToPrim(_end, _numericType);
            return ConvertUtil.ToPrim(this.Ease(t, (float)_start, (float)_end - (float)_start, this.Duration), _numericType);
        }

        #endregion

    }
}
