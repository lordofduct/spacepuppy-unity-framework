using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{

    [System.Serializable()]
    public struct FixedPercentLong : IConvertible, IEquatable<FixedPercentLong>, IFormattable
    {

        public const decimal MAX_VALUE = (decimal)long.MaxValue + 0.999999999M;
        public const decimal MIN_VALUE = decimal.MinValue;
        public const decimal MINUS_ONE = decimal.MinusOne;
        public const decimal ONE = decimal.One;
        public const decimal RNG_FRACT = 1000000000M;

        #region Fields

        [UnityEngine.SerializeField()]
        private long _value;
        [UnityEngine.SerializeField()]
        private int _fract;

        #endregion

        #region CONSTRUCTOR

        public FixedPercentLong(float value)
        {
            _value = (long)Math.Floor(value);
            _fract = (int)((decimal)(value % 1f) * RNG_FRACT);
        }

        public FixedPercentLong(double value)
        {
            _value = (long)Math.Floor(value);
            _fract = (int)((decimal)(value % 1d) * RNG_FRACT);
        }

        public FixedPercentLong(decimal value)
        {
            _value = (long)Math.Floor(value);
            _fract = (int)((decimal)(value % 1M) * RNG_FRACT);
        }

        public FixedPercentLong(int value)
        {
            _value = value;
            _fract = 0;
        }

        #endregion

        #region Properties

        public decimal Value
        {
            get
            {
                return (decimal)_value + (decimal)_fract / RNG_FRACT;
            }
            set
            {
                _value = (long)Math.Floor(value);
                _fract = (int)((decimal)(value % 1M) * RNG_FRACT);
            }
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj is FixedPercent)
                return _value == ((FixedPercentLong)obj)._value;

            var d = com.spacepuppy.Utils.ConvertUtil.ToDecimal(obj);
            return d == this.Value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public bool Equals(FixedPercentLong other)
        {
            return _value == other._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        #endregion

        #region Conversion

        public static implicit operator FixedPercentLong(float f)
        {
            return new FixedPercentLong(f);
        }

        public static implicit operator FixedPercentLong(double f)
        {
            return new FixedPercentLong(f);
        }

        public static implicit operator FixedPercentLong(decimal d)
        {
            return new FixedPercentLong(d);
        }

        public static implicit operator FixedPercentLong(int i)
        {
            return new FixedPercentLong(i);
        }

        public static explicit operator float (FixedPercentLong p)
        {
            return Convert.ToSingle(p.Value);
        }

        public static explicit operator double (FixedPercentLong p)
        {
            return Convert.ToDouble(p.Value);
        }

        public static implicit operator decimal (FixedPercentLong p)
        {
            return p.Value;
        }

        public static explicit operator int (FixedPercentLong p)
        {
            return (int)p.Value;
        }

        #endregion

        #region Operators

        public static FixedPercentLong operator +(FixedPercentLong a)
        {
            return a;
        }

        public static FixedPercentLong operator ++(FixedPercentLong a)
        {
            return new FixedPercentLong(a.Value + 1M);
        }

        public static FixedPercentLong operator +(FixedPercentLong a, FixedPercentLong b)
        {
            return new FixedPercentLong(a.Value + b.Value);
        }

        public static FixedPercentLong operator -(FixedPercentLong a)
        {
            a.Value = -a.Value;
            return a;
        }

        public static FixedPercentLong operator --(FixedPercentLong a)
        {
            return new FixedPercentLong(a.Value - 1M);
        }

        public static FixedPercentLong operator -(FixedPercentLong a, FixedPercentLong b)
        {
            return new FixedPercentLong(a.Value - b.Value);
        }

        public static FixedPercentLong operator *(FixedPercentLong a, FixedPercentLong b)
        {
            return new FixedPercentLong(a.Value * b.Value);
        }

        public static FixedPercentLong operator /(FixedPercentLong a, FixedPercentLong b)
        {
            return new FixedPercentLong(a.Value / b.Value);
        }

        public static FixedPercentLong operator %(FixedPercentLong a, FixedPercentLong b)
        {
            return new FixedPercentLong(a.Value % b.Value);
        }

        public static bool operator >(FixedPercentLong a, FixedPercentLong b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(FixedPercentLong a, FixedPercentLong b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(FixedPercentLong a, FixedPercentLong b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(FixedPercentLong a, FixedPercentLong b)
        {
            return a.Value <= b.Value;
        }

        public static bool operator ==(FixedPercentLong a, FixedPercentLong b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(FixedPercentLong a, FixedPercentLong b)
        {
            return a.Value != b.Value;
        }

        #endregion

        #region IConvertible Interface

        public TypeCode GetTypeCode()
        {
            return TypeCode.Decimal;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return _value != 0 && _fract != 0;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.Value);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.Value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this.Value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return this.Value;
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this.Value);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.Value);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.Value);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.Value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.Value);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.Value);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return this.ToString("", provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return null;
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.Value);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.Value);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.Value);
        }

        #endregion

        #region IFormattable Interface

        public string ToString(string format)
        {
            return this.Value.ToString(format);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return this.Value.ToString(format, provider);
        }

        #endregion

    }
}
