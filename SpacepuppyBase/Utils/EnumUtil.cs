using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{

    /// <summary>
    /// Readable shorthand methods for enum operations. The underlying bitwise operators are substantially faster. 
    /// These methods are intended for making code more readable, rather than more optimized. Use sparingly.
    /// </summary>
    public static class EnumUtil
    {

        public static object ToEnumsNumericType(ulong v, System.TypeCode code)
        {
            switch (code)
            {
                case System.TypeCode.Byte:
                    return System.Convert.ToByte(v);
                case System.TypeCode.SByte:
                    return System.Convert.ToSByte(v);
                case System.TypeCode.Int16:
                    return System.Convert.ToInt16(v);
                case System.TypeCode.UInt16:
                    return System.Convert.ToUInt16(v);
                case System.TypeCode.Int32:
                    return System.Convert.ToInt32(v);
                case System.TypeCode.UInt32:
                    return System.Convert.ToUInt32(v);
                case System.TypeCode.Int64:
                    return System.Convert.ToInt64(v);
                case System.TypeCode.UInt64:
                    return System.Convert.ToUInt64(v);
                default:
                    return null;
            }
        }

        public static bool EnumValueIsDefined(object value, System.Type enumType)
        {
            if (enumType == null) throw new System.ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new System.ArgumentException("Must be enum type.", "enumType");

            try
            {
                if(value is string)
                    return System.Enum.IsDefined(enumType, value);
                else if(ConvertUtil.IsNumeric(value))
                {
                    value = ToEnumsNumericType(System.Convert.ToUInt64(value), System.Type.GetTypeCode(enumType));
                    return System.Enum.IsDefined(enumType, value);
                }
            }
            catch
            {
            }

            return false;
        }

        public static bool EnumValueIsDefined<T>(object value)
        {
            return EnumValueIsDefined(value, typeof(T));
        }

        public static T AddFlag<T>(this T e, T value) where T : struct, System.IConvertible
        {
            return (T)System.Enum.ToObject(typeof(T), System.Convert.ToInt64(e) | System.Convert.ToInt64(value));
        }

        public static T RedactFlag<T>(this T e, T value) where T : struct, System.IConvertible
        {
            var x = System.Convert.ToInt64(e);
            var y = System.Convert.ToInt64(value);
            return (T)System.Enum.ToObject(typeof(T), x & ~(x & y));
        }

        public static T SetFlag<T>(this T e, T flag, bool status) where T : struct, System.IConvertible
        {
            var x = System.Convert.ToInt64(e);
            var y = System.Convert.ToInt64(flag);
            if (status)
                x |= y;
            else
                x &= ~(x & y);
            return (T)System.Enum.ToObject(typeof(T), x);
        }

        public static bool HasFlag(this System.Enum e, System.Enum value)
        {
            long v = System.Convert.ToInt64(value);
            return (System.Convert.ToInt64(e) & v) == v;
        }
        
        public static bool HasFlag(this System.Enum e, ulong value)
        {
            return (System.Convert.ToUInt64(e) & value) == value;
        }

        public static bool HasFlag(this System.Enum e, long value)
        {
            return (System.Convert.ToInt64(e) & value) == value;
        }

        public static bool HasAnyFlag(this System.Enum e, System.Enum value)
        {
            return (System.Convert.ToInt64(e) & System.Convert.ToInt64(value)) != 0;
        }

        public static bool HasAnyFlag(this System.Enum e, ulong value)
        {
            return (System.Convert.ToUInt64(e) & value) != 0;
        }

        public static bool HasAnyFlag(this System.Enum e, long value)
        {
            return (System.Convert.ToInt64(e) & value) != 0;
        }

        public static IEnumerable<System.Enum> GetFlags(System.Enum e)
        {
            if (e == null) throw new System.ArgumentNullException("e");

            var tp = e.GetType();
            ulong max = 0;
            foreach (var en in System.Enum.GetValues(tp))
            {
                ulong v = System.Convert.ToUInt64(en);
                if (v > max) max = v;
            }
            int loops = (int)System.Math.Log(max, 2) + 1;


            ulong ie = System.Convert.ToUInt64(e);
            for (int i = 0; i < loops; i++)
            {
                ulong j = (ulong)System.Math.Pow(2, i);
                if ((ie & j) != 0)
                {
                    var js = ToEnumsNumericType(j, e.GetTypeCode());
                    if (System.Enum.IsDefined(tp, js)) yield return (System.Enum)System.Enum.Parse(tp, js.ToString());
                }
            }
        }

        public static IEnumerable<T> GetFlags<T>(T e) where T : struct, System.IConvertible
        {
            var tp = e.GetType();
            if (!tp.IsEnum) throw new System.ArgumentException("Type must be an enum.", "T");

            ulong max = 0;
            foreach (var en in System.Enum.GetValues(tp))
            {
                ulong v = System.Convert.ToUInt64(en);
                if (v > max) max = v;
            }
            int loops = (int)System.Math.Log(max, 2) + 1;


            ulong ie = System.Convert.ToUInt64(e);
            for (int i = 0; i < loops; i++)
            {
                ulong j = (ulong)System.Math.Pow(2, i);
                if ((ie & j) != 0)
                {
                    var js = ToEnumsNumericType(j, e.GetTypeCode());
                    if (System.Enum.IsDefined(tp, js))
                    {
                        yield return (T)js;
                    }
                }
            }
        }

#if SP_LIB

        public static IEnumerable<System.Enum> GetUniqueEnumFlags(System.Type enumType)
        {
            if (enumType == null) throw new System.ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new System.ArgumentException("Type must be an enum.", "enumType");

            foreach (var e in System.Enum.GetValues(enumType))
            {
                var d = System.Convert.ToDecimal(e);
                if (d > 0 && MathUtil.IsPowerOfTwo(System.Convert.ToUInt64(d))) yield return e as System.Enum;
            }
        }

#endif
        
    }
}
