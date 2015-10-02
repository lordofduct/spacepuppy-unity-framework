using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{

    /// <summary>
    /// Represents a base 10 value with range -2147 to 2147 with 10 digits of precision (4 whole, 6 fractional). 
    /// </summary>
    /// <remarks>
    /// Unity lacks support for serializing decimal, so this is a base 10 precise number type for use in unity. 
    /// Int64 is also not serializable, so we were restricted to the Int32. I find that when I need precise base 10 
    /// it's in the fractional ranges, so I dedicated most of the value to the fraction rather than the whole. 
    /// 6 digits of fractional precision was the value chosen because conversion to a float of a 0->1 'percent' 
    /// value has enough precision to house the FixedPercent value in the same range.
    /// </remarks>
    [System.Serializable()]
    public struct FixedPercent : IConvertible, IEquatable<FixedPercent>
    {

        public const decimal MAX_VALUE = 2147M;
        public const decimal MIN_VALUE = -2147M;
        private const decimal MAX_VALUE_R = 2147.483647M;
        private const decimal MIN_VALUE_R = -2147.483648M;
        public const int PRECISION = 1000000;
        private const decimal PRECISION_M = 1000000M;

        [UnityEngine.SerializeField()]
        private int _value;

        #region CONSTRUCTOR

        public FixedPercent(float value)
        {
            if (value < (float)MIN_VALUE) _value = Int32.MinValue;
            else if (value > (float)MAX_VALUE) _value = Int32.MaxValue;
            else _value = (int)(value * (float)PRECISION);
        }

        public FixedPercent(double value)
        {
            if (value < (double)MIN_VALUE) _value = Int32.MinValue;
            else if (value > (double)MAX_VALUE) _value = Int32.MaxValue;
            else _value = (int)(value * (double)PRECISION);
        }

        public FixedPercent(decimal value)
        {
            if (value < MIN_VALUE) _value = Int32.MinValue;
            else if (value > MAX_VALUE) _value = Int32.MaxValue;
            else _value = (int)(value * PRECISION_M);
        }

        public FixedPercent(int value)
        {
            if (value < MIN_VALUE) _value = Int32.MinValue;
            else if (value > MAX_VALUE) _value = Int32.MaxValue;
            else _value = (int)(value * PRECISION_M);
        }

        #endregion

        #region Property

        public int RawValue
        {
            get { return _value; }
            set { _value = value; }
        }

        public decimal Value
        {
            get { return (decimal)_value / PRECISION_M; }
            set
            {
                if (value < MIN_VALUE) _value = Int32.MinValue;
                else if (value > MAX_VALUE) _value = Int32.MaxValue;
                else _value = (int)(value * PRECISION_M);
            }
        }

        //public double ValueDouble
        //{
        //    get { return (double)this.Value; }
        //    set
        //    {
        //        if (value < (double)MIN_VALUE) _value = Int32.MinValue;
        //        else if (value > (double)MAX_VALUE) _value = Int32.MaxValue;
        //        else _value = (int)(value * (double)PRECISION);
        //    }
        //}

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj is FixedPercent)
                return _value == ((FixedPercent)obj)._value;

            var d = com.spacepuppy.Utils.ConvertUtil.ToDecimal(obj);
            return d == this.Value;
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public bool Equals(FixedPercent other)
        {
            return _value == other._value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        #endregion

        #region Static Utils

        public static FixedPercent Clamp01(FixedPercent p)
        {
            if (p._value < 0) p._value = 0;
            else if (p._value > PRECISION) p._value = PRECISION;
            return p;
        }

        #endregion


        #region Conversion

        public static implicit operator FixedPercent(float f)
        {
            return new FixedPercent(f);
        }

        public static implicit operator FixedPercent(double f)
        {
            return new FixedPercent(f);
        }

        public static implicit operator FixedPercent(decimal d)
        {
            return new FixedPercent(d);
        }

        public static implicit operator FixedPercent(int i)
        {
            return new FixedPercent(i);
        }

        public static explicit operator float (FixedPercent p)
        {
            return Convert.ToSingle(p.Value);
        }

        public static explicit operator double (FixedPercent p)
        {
            return Convert.ToDouble(p.Value);
        }

        public static explicit operator decimal (FixedPercent p)
        {
            return p.Value;
        }

        public static explicit operator int (FixedPercent p)
        {
            return (int)p.Value;
        }

        #endregion

        #region Operators

        public static FixedPercent operator +(FixedPercent a)
        {
            return a;
        }

        public static FixedPercent operator ++(FixedPercent a)
        {
            return new FixedPercent(a.Value + 1M);
        }

        public static FixedPercent operator +(FixedPercent a, FixedPercent b)
        {
            return new FixedPercent(a.Value + b.Value);
        }

        public static FixedPercent operator -(FixedPercent a)
        {
            a._value = -a._value;
            return a;
        }

        public static FixedPercent operator --(FixedPercent a)
        {
            return new FixedPercent(a.Value - 1M);
        }

        public static FixedPercent operator -(FixedPercent a, FixedPercent b)
        {
            return new FixedPercent(a.Value - b.Value);
        }

        public static FixedPercent operator *(FixedPercent a, FixedPercent b)
        {
            return new FixedPercent(a.Value * b.Value);
        }
        
        public static FixedPercent operator /(FixedPercent a, FixedPercent b)
        {
            return new FixedPercent(a.Value / b.Value);
        }
        
        public static bool operator >(FixedPercent a, FixedPercent b)
        {
            return a._value > b._value;
        }
        
        public static bool operator <(FixedPercent a, FixedPercent b)
        {
            return a._value < b._value;
        }
        
        public static bool operator >=(FixedPercent a, FixedPercent b)
        {
            return a._value >= b._value;
        }
        
        public static bool operator <=(FixedPercent a, FixedPercent b)
        {
            return a._value <= b._value;
        }
        
        public static bool operator ==(FixedPercent a, FixedPercent b)
        {
            return a._value == b._value;
        }
        
        public static bool operator !=(FixedPercent a, FixedPercent b)
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
            return Convert.ToString(this.Value);
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


        #region Special Types

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class ConfigAttribute : Attribute
        {

            public bool displayAsPercent;
            public bool displayAsRange;
            public float min = (float)FixedPercent.MIN_VALUE;
            public float max = (float)FixedPercent.MAX_VALUE;

            public ConfigAttribute(bool displayAsPercent)
            {
                this.displayAsPercent = displayAsPercent;
            }

            public ConfigAttribute(float min, float max)
            {
                this.displayAsPercent = false;
                this.displayAsRange = true;
                this.min = min;
                this.max = max;
            }

            public ConfigAttribute(float min, float max, bool displayAsPercent)
            {
                this.displayAsRange = true;
                this.min = min;
                this.max = max;
                this.displayAsPercent = displayAsPercent;
            }

        }

        #endregion

    }

}
