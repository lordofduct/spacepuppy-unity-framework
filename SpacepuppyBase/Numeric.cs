using System;
using UnityEngine;

namespace com.spacepuppy
{

    /// <summary>
    /// General interface for custom numeric types like fixedpoint, discrete float, etc. 
    /// Numerics should usually be a struct type, but is not required.
    /// </summary>
    public interface INumeric : IConvertible
    {

        /// <summary>
        /// Returns the byte reprsentation of the numeric value.
        /// When implementing this methods, if you need to convert C# built-in numeric types make sure they're in big-endian. 
        /// Use the 'Numerics' static class as a helper tool to do this.
        /// </summary>
        /// <returns></returns>
        byte[] ToByteArray();
        /// <summary>
        /// Sets the numeric value based on some byte array.
        /// When implementing this methods, if you need to convert C# built-in numeric types make sure they're in big-endian. 
        /// Use the 'Numerics' static class as a helper tool to do this.
        /// </summary>
        /// <param name="arr"></param>
        void FromByteArray(byte[] arr);

        /// <summary>
        /// Get the type code the underlying data can losslessly be converted to for easy storing.
        /// </summary>
        /// <returns></returns>
        TypeCode GetUnderlyingTypeCode();

        /// <summary>
        /// Set value based on a long.
        /// </summary>
        /// <param name="value"></param>
        void FromLong(long value);

        /// <summary>
        /// Set value based on a double.
        /// </summary>
        /// <param name="value"></param>
        void FromDouble(double value);

    }

    public static class Numerics
    {
        
        public static INumeric CreateNumeric<T>(byte[] data) where T : INumeric
        {
            var value = System.Activator.CreateInstance<T>();
            if (value != null) value.FromByteArray(data);
            return value;
        }

        public static INumeric CreateNumeric<T>(long data) where T : INumeric
        {
            var value = System.Activator.CreateInstance<T>();
            if (value != null) value.FromLong(data);
            return value;
        }

        public static INumeric CreateNumeric<T>(double data) where T : INumeric
        {
            var value = System.Activator.CreateInstance<T>();
            if (value != null) value.FromDouble(data);
            return value;
        }
        
        public static INumeric CreateNumeric(System.Type tp, byte[] data)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(INumeric).IsAssignableFrom(tp) && !tp.IsAbstract) throw new System.ArgumentException("Type must implement INumeric.");

            var value = System.Activator.CreateInstance(tp) as INumeric;
            if (value != null) value.FromByteArray(data);
            return value;
        }

        public static INumeric CreateNumeric(System.Type tp, long data)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(INumeric).IsAssignableFrom(tp) && !tp.IsAbstract) throw new System.ArgumentException("Type must implement INumeric.");

            var value = System.Activator.CreateInstance(tp) as INumeric;
            if (value != null) value.FromLong(data);
            return value;
        }

        public static INumeric CreateNumeric(System.Type tp, double data)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(INumeric).IsAssignableFrom(tp) && !tp.IsAbstract) throw new System.ArgumentException("Type must implement INumeric.");

            var value = System.Activator.CreateInstance(tp) as INumeric;
            if (value != null) value.FromDouble(data);
            return value;
        }


        #region Bit Helpers

        /// <summary>
        /// Returns a byte representation in big-endian order.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(float value)
        {
            var result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) System.Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// Returns a byte representation in big-endian order.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(double value)
        {
            var result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) System.Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// Returns a byte representation in big-endian order.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(Int16 value)
        {
            var result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) System.Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// Returns a byte representation in big-endian order.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(Int32 value)
        {
            var result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) System.Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// Returns a byte representation in big-endian order.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(Int64 value)
        {
            var result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) System.Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// Returns a byte representation in big-endian order.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(UInt16 value)
        {
            var result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) System.Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// Returns a byte representation in big-endian order.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(UInt32 value)
        {
            var result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) System.Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// Returns a byte representation in big-endian order.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(UInt64 value)
        {
            var result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) System.Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// Converts a byte array in big-endian form to a numeric value.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static float ToSingle(byte[] arr)
        {
            if (BitConverter.IsLittleEndian)
                System.Array.Reverse(arr);
            return BitConverter.ToSingle(arr, 0);
        }

        /// <summary>
        /// Converts a byte array in big-endian form to a numeric value.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static double ToDouble(byte[] arr)
        {
            if (BitConverter.IsLittleEndian)
                System.Array.Reverse(arr);
            return BitConverter.ToDouble(arr, 0);
        }

        /// <summary>
        /// Converts a byte array in big-endian form to a numeric value.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Int16 ToInt16(byte[] arr)
        {
            if (BitConverter.IsLittleEndian)
                System.Array.Reverse(arr);
            return BitConverter.ToInt16(arr, 0);
        }

        /// <summary>
        /// Converts a byte array in big-endian form to a numeric value.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Int32 ToInt32(byte[] arr)
        {
            if (BitConverter.IsLittleEndian)
                System.Array.Reverse(arr);
            return BitConverter.ToInt32(arr, 0);
        }

        /// <summary>
        /// Converts a byte array in big-endian form to a numeric value.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Int64 ToInt64(byte[] arr)
        {
            if (BitConverter.IsLittleEndian)
                System.Array.Reverse(arr);
            return BitConverter.ToInt64(arr, 0);
        }

        /// <summary>
        /// Converts a byte array in big-endian form to a numeric value.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static UInt16 ToUInt16(byte[] arr)
        {
            if (BitConverter.IsLittleEndian)
                System.Array.Reverse(arr);
            return BitConverter.ToUInt16(arr, 0);
        }

        /// <summary>
        /// Converts a byte array in big-endian form to a numeric value.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static UInt32 ToUInt32(byte[] arr)
        {
            if (BitConverter.IsLittleEndian)
                System.Array.Reverse(arr);
            return BitConverter.ToUInt32(arr, 0);
        }

        /// <summary>
        /// Converts a byte array in big-endian form to a numeric value.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static UInt64 ToUInt64(byte[] arr)
        {
            if (BitConverter.IsLittleEndian)
                System.Array.Reverse(arr);
            return BitConverter.ToUInt64(arr, 0);
        }

        #endregion

    }

    /// <summary>
    /// Stores a whole number as a floating point value. You get the range of a float, as well as infinity representations. 
    /// Implicit conversion between float and int is defined.
    /// </summary>
    [System.Serializable()]
    public struct DiscreteFloat : INumeric, IConvertible, IComparable, IComparable<float>, IComparable<DiscreteFloat>, IEquatable<float>, IEquatable<DiscreteFloat>, IFormattable
    {

        [SerializeField()]
        private float _value;

        #region CONSTRUCTOR

        public DiscreteFloat(float f)
        {
            _value = Mathf.Round(f);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj is DiscreteFloat)
            {
                return _value == ((DiscreteFloat)obj)._value;
            }
            else if (obj is System.IConvertible)
            {
                try
                {
                    var f = System.Convert.ToSingle(obj);
                    return _value == f;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        #endregion

        #region Static/Constants

        public static DiscreteFloat Zero
        {
            get
            {
                return new DiscreteFloat()
                            {
                                _value = 0f
                            };
            }
        }

        public static DiscreteFloat PositiveInfinity { get { return new DiscreteFloat() { _value = float.PositiveInfinity }; } }
        public static DiscreteFloat NegativeInfinity { get { return new DiscreteFloat() { _value = float.NegativeInfinity }; } }

        public static bool IsNaN(DiscreteFloat value)
        {
            return float.IsNaN(value._value);
        }

        public static bool IsInfinity(DiscreteFloat value)
        {
            return float.IsInfinity(value._value);
        }

        public static bool IsPositiveInfinity(DiscreteFloat value)
        {
            return float.IsPositiveInfinity(value._value);
        }

        public static bool IsNegativeInfinity(DiscreteFloat value)
        {
            return float.IsNegativeInfinity(value._value);
        }

        public static bool IsReal(DiscreteFloat value)
        {
            return !(float.IsNaN(value._value) || float.IsInfinity(value._value));
        }

        

        #endregion

        #region Operators

        public static DiscreteFloat operator ++(DiscreteFloat df)
        {
            df._value++;
            return df;
        }

        public static DiscreteFloat operator +(DiscreteFloat df)
        {
            return df;
        }

        public static DiscreteFloat operator +(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value + b._value);
            return a;
        }

        public static DiscreteFloat operator --(DiscreteFloat df)
        {
            df._value--;
            return df;
        }

        public static DiscreteFloat operator -(DiscreteFloat df)
        {
            df._value = -df._value;
            return df;
        }

        public static DiscreteFloat operator -(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value - b._value);
            return a;
        }
        
        public static DiscreteFloat operator *(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value * b._value);
            return a;
        }
        
        public static DiscreteFloat operator /(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value / b._value);
            return a;
        }
        
        public static bool operator >(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value > b._value;
        }

        public static bool operator >=(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value >= b._value;
        }

        public static bool operator <(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value < b._value;
        }

        public static bool operator <=(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value <= b._value;
        }

        public static bool operator ==(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value != b._value;
        }
        
        #endregion

        #region Conversions

        public static implicit operator DiscreteFloat(int f)
        {
            return new DiscreteFloat((float)f);
        }

        public static implicit operator int(DiscreteFloat df)
        {
            return (int)df._value;
        }

        public static implicit operator DiscreteFloat(float f)
        {
            return new DiscreteFloat(f);
        }

        public static implicit operator float(DiscreteFloat df)
        {
            return df._value;
        }

        public static implicit operator DiscreteFloat(double d)
        {
            return new DiscreteFloat((float)d);
        }

        public static implicit operator double(DiscreteFloat df)
        {
            return (double)df._value;
        }

        public static implicit operator DiscreteFloat(decimal d)
        {
            return new DiscreteFloat((float)d);
        }

        public static implicit operator decimal(DiscreteFloat df)
        {
            return (decimal)df._value;
        }

        #endregion

        #region INumeric Interface

        TypeCode INumeric.GetUnderlyingTypeCode()
        {
            return TypeCode.Single;
        }

        public byte[] ToByteArray()
        {
            return Numerics.GetBytes(_value);
        }

        void INumeric.FromByteArray(byte[] data)
        {
            _value = Mathf.Round(Numerics.ToSingle(data));
        }

        void INumeric.FromLong(long value)
        {
            _value = Convert.ToSingle(value);
        }

        void INumeric.FromDouble(double value)
        {
            _value = (float)Math.Round(value);
        }

        public static DiscreteFloat FromByteArray(byte[] data)
        {
            var result = new DiscreteFloat();
            result._value = Mathf.Round(Numerics.ToSingle(data));
            return result;
        }

        #endregion

        #region IConvertible

        public TypeCode GetTypeCode()
        {
            return TypeCode.Single;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return _value != 0f;
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(_value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(_value);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(_value);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(_value);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(_value);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(_value);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(_value);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(_value);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return _value;
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(_value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(_value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(_value);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return _value.ToString(provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return (_value as IConvertible).ToType(conversionType, provider);
        }

        #endregion

        #region IComparable Interface

        public int CompareTo(object obj)
        {
            return _value.CompareTo(obj);
        }

        public int CompareTo(float other)
        {
            return _value.CompareTo(other);
        }

        public int CompareTo(DiscreteFloat other)
        {
            return _value.CompareTo(other._value);
        }

        #endregion

        #region IEquatable Interface

        public bool Equals(float other)
        {
            return _value.Equals(other);
        }

        public bool Equals(DiscreteFloat other)
        {
            return _value.Equals(other._value);
        }

        #endregion

        #region IFormattable Interface

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _value.ToString(format, formatProvider);
        }

        #endregion

        #region Special Types

        public abstract class ConfigAttribute : System.Attribute
        {

            public abstract float Normalize(float value);

        }

        public class NonNegative : ConfigAttribute
        {

            public override float Normalize(float value)
            {
                if (value < 0f) return 0f;
                else return value;
            }

        }

        #endregion

    }

}
