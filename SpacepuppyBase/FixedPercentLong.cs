using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace com.spacepuppy
{

    [System.Serializable()]
    public struct FixedPercentLong : IConvertible, IEquatable<FixedPercentLong>, IFormattable, UnityEngine.ISerializationCallbackReceiver
    {

        public const decimal MAX_VALUE = decimal.MaxValue;
        public const decimal MIN_VALUE = decimal.MinValue;

        #region Fields

        private decimal _value;
        [SerializeField()]
        private int[] _bits;

        #endregion

        #region CONSTRUCTOR

        public FixedPercentLong(float value)
        {
            _value = System.Convert.ToDecimal(value);
            _bits = null;
        }

        public FixedPercentLong(double value)
        {
            _value = System.Convert.ToDecimal(value);
            _bits = null;
        }

        public FixedPercentLong(decimal value)
        {
            _value = value;
            _bits = null;
        }

        public FixedPercentLong(int value)
        {
            _value = (decimal)value;
            _bits = null;
        }

        #endregion

        #region Properties

        public decimal Value
        {
            get { return _value; }
            set { _value = value; }
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
            return new FixedPercentLong(a._value + 1M);
        }

        public static FixedPercentLong operator +(FixedPercentLong a, FixedPercentLong b)
        {
            return new FixedPercentLong(a._value + b._value);
        }

        public static FixedPercentLong operator -(FixedPercentLong a)
        {
            a._value = -a._value;
            return a;
        }

        public static FixedPercentLong operator --(FixedPercentLong a)
        {
            return new FixedPercentLong(a._value - 1M);
        }

        public static FixedPercentLong operator -(FixedPercentLong a, FixedPercentLong b)
        {
            return new FixedPercentLong(a._value - b._value);
        }

        public static FixedPercentLong operator *(FixedPercentLong a, FixedPercentLong b)
        {
            return new FixedPercentLong(a._value * b._value);
        }

        public static FixedPercentLong operator /(FixedPercentLong a, FixedPercentLong b)
        {
            return new FixedPercentLong(a._value / b._value);
        }

        public static bool operator >(FixedPercentLong a, FixedPercentLong b)
        {
            return a._value > b._value;
        }

        public static bool operator <(FixedPercentLong a, FixedPercentLong b)
        {
            return a._value < b._value;
        }

        public static bool operator >=(FixedPercentLong a, FixedPercentLong b)
        {
            return a._value >= b._value;
        }

        public static bool operator <=(FixedPercentLong a, FixedPercentLong b)
        {
            return a._value <= b._value;
        }

        public static bool operator ==(FixedPercentLong a, FixedPercentLong b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(FixedPercentLong a, FixedPercentLong b)
        {
            return a._value != b._value;
        }

        #endregion

        #region IConvertible Interface

        public TypeCode GetTypeCode()
        {
            return TypeCode.Decimal;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return _value != 0;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this._value);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this._value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this._value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return this._value;
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this._value);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this._value);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this._value);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this._value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this._value);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this._value);
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
            return Convert.ToUInt16(this._value);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this._value);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this._value);
        }

        #endregion

        #region IFormattable Interface

        public string ToString(string format)
        {
            return this._value.ToString(format);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return this._value.ToString(format, provider);
        }

        #endregion

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _bits = decimal.GetBits(_value);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if(_bits != null && _bits.Length == 4)
            {
                bool sign = (_bits[3] & 0x80000000) != 0;
                byte scale = (byte)((_bits[3] >> 16) & 0x7F);
                _value = new System.Decimal(_bits[0], _bits[1], _bits[2], sign, scale);
            }
            _bits = null;
        }

        #endregion

    }
}
