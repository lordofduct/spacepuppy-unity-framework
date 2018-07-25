using UnityEngine;
using System.Text.RegularExpressions;

namespace com.spacepuppy.Utils
{
    public static class ConvertUtil
    {

        #region RegEx Constants

        private const string RX_ISHEX = @"(?<sign>[-+]?)(?<flag>0x|#|&H)(?<num>[\dA-F]+)(?<fractional>(\.[\dA-F]+)?)$";

        #endregion

        #region Color

        public static int ToInt(Color color)
        {
            return (Mathf.RoundToInt(color.a * 255) << 24) +
                   (Mathf.RoundToInt(color.r * 255) << 16) +
                   (Mathf.RoundToInt(color.g * 255) << 8) +
                   Mathf.RoundToInt(color.b * 255);
        }

        public static Color ToColor(int value)
        {
            var a = (float)(value >> 24 & 0xFF) / 255f;
            var r = (float)(value >> 16 & 0xFF) / 255f;
            var g = (float)(value >> 8 & 0xFF) / 255f;
            var b = (float)(value & 0xFF) / 255f;
            return new Color(r, g, b, a);
        }

        public static Color ToColor(string value)
        {
            return ToColor(ToInt(value));
        }

        public static Color ToColor(Color32 value)
        {
            return new Color((float)value.r / 255f,
                             (float)value.g / 255f,
                             (float)value.b / 255f,
                             (float)value.a / 255f);
        }

        public static Color ToColor(Vector3 value)
        {

            return new Color((float)value.x,
                             (float)value.y,
                             (float)value.z);
        }

        public static Color ToColor(Vector4 value)
        {
            return new Color((float)value.x,
                             (float)value.y,
                             (float)value.z,
                             (float)value.w);
        }

        public static Color ToColor(object value)
        {
            if (value is Color) return (Color)value;
            if (value is Color32) return ToColor((Color32)value);
            if (value is Vector3) return ToColor((Vector3)value);
            if (value is Vector4) return ToColor((Vector4)value);
            return ToColor(ToInt(value));
        }

        public static int ToInt(Color32 color)
        {
            return (color.a << 24) +
                   (color.r << 16) +
                   (color.g << 8) +
                   color.b;
        }

        public static Color32 ToColor32(int value)
        {
            byte a = (byte)(value >> 24 & 0xFF);
            byte r = (byte)(value >> 16 & 0xFF);
            byte g = (byte)(value >> 8 & 0xFF);
            byte b = (byte)(value & 0xFF);
            return new Color32(r, g, b, a);
        }
        
        public static Color32 ToColor32(string value)
        {
            return ToColor32(ToInt(value));
        }

        public static Color32 ToColor32(Color value)
        {
            return new Color32((byte)(value.r * 255f),
                               (byte)(value.g * 255f),
                               (byte)(value.b * 255f),
                               (byte)(value.a * 255f));
        }

        public static Color32 ToColor32(Vector3 value)
        {

            return new Color32((byte)(value.x * 255f),
                               (byte)(value.y * 255f),
                               (byte)(value.z * 255f), 255);
        }

        public static Color32 ToColor32(Vector4 value)
        {
            return new Color32((byte)(value.x * 255f),
                               (byte)(value.y * 255f),
                               (byte)(value.z * 255f),
                               (byte)(value.w * 255f));
        }

        public static Color32 ToColor32(object value)
        {
            if (value is Color32) return (Color32)value;
            if (value is Color) return ToColor32((Color)value);
            if (value is Vector3) return ToColor32((Vector3)value);
            if (value is Vector4) return ToColor32((Vector4)value);
            return ToColor32(ToInt(value));
        }

        #endregion

        #region ToEnum

        public static T ToEnum<T>(string val, T defaultValue) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                T result = (T)System.Enum.Parse(typeof(T), val, true);
                return result;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(int val, T defaultValue) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            //object obj = val;
            //if(System.Enum.IsDefined(typeof(T), obj))
            //{
            //    return (T)obj;
            //}
            //else
            //{
            //    return defaultValue;
            //}
            try
            {
                return (T)System.Enum.ToObject(typeof(T), val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(long val, T defaultValue) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                return (T)System.Enum.ToObject(typeof(T), val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(object val, T defaultValue) where T : struct, System.IConvertible
        {
            return ToEnum<T>(System.Convert.ToString(val), defaultValue);
        }

        public static T ToEnum<T>(string val) where T : struct, System.IConvertible
        {
            return ToEnum<T>(val, default(T));
        }

        public static T ToEnum<T>(int val) where T : struct, System.IConvertible
        {
            return ToEnum<T>(val, default(T));
        }

        public static T ToEnum<T>(object val) where T : struct, System.IConvertible
        {
            return ToEnum<T>(System.Convert.ToString(val), default(T));
        }

        public static System.Enum ToEnumOfType(System.Type enumType, object value)
        {
            return System.Enum.Parse(enumType, System.Convert.ToString(value), true) as System.Enum;
        }

        public static bool TryToEnum<T>(object val, out T result) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                result = (T)System.Enum.Parse(typeof(T), System.Convert.ToString(val), true);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        #endregion

        #region ConvertToUInt
        /// <summary>
        /// This will convert an integer to a uinteger. The negative integer value is treated as what the memory representation of that negative 
        /// value would be as a uinteger.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static uint ToUInt(sbyte value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(byte value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(short value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(ushort value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(int value)
        {
            return System.Convert.ToUInt32(value & 0xffffffffu);
        }

        public static uint ToUInt(uint value)
        {
            return value;
        }

        public static uint ToUInt(long value)
        {
            return System.Convert.ToUInt32(value & 0xffffffffu);
        }

        public static uint ToUInt(ulong value)
        {
            return System.Convert.ToUInt32(value & 0xffffffffu);
        }

        ////public static uint ToUInt(float value)
        ////{
        ////    return System.Convert.ToUInt32(value & 0xffffffffu);
        ////}

        ////public static uint ToUInt(double value)
        ////{
        ////    return System.Convert.ToUInt32(value & 0xffffffffu);
        ////}

        ////public static uint ToUInt(decimal value)
        ////{
        ////    return System.Convert.ToUInt32(value & 0xffffffffu);
        ////}

        public static uint ToUInt(bool value)
        {
            return (value) ? 1u : 0u;
        }

        public static uint ToUInt(char value)
        {
            return System.Convert.ToUInt32(value);
        }

        public static uint ToUInt(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToUInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToUInt(value.ToString());
            }
        }

        public static uint ToUInt(string value, System.Globalization.NumberStyles style)
        {
            return ToUInt(ToDouble(value, style));
        }

        public static uint ToUInt(string value)
        {
            return ToUInt(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region ConvertToInt

        public static int ToInt(sbyte value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(byte value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(short value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(ushort value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(int value)
        {
            return value;
        }

        public static int ToInt(uint value)
        {
            if (value > int.MaxValue)
            {
                return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            }
            else
            {
                return System.Convert.ToInt32(value & 0xffffffff);
            }
        }

        public static int ToInt(long value)
        {
            if (value > int.MaxValue)
            {
                return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            }
            else
            {
                return System.Convert.ToInt32(value & 0xffffffff);
            }
        }

        public static int ToInt(ulong value)
        {
            if (value > int.MaxValue)
            {
                return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            }
            else
            {
                return System.Convert.ToInt32(value & 0xffffffff);
            }
        }

        public static int ToInt(float value)
        {
            return System.Convert.ToInt32(value);
            //if (value > int.MaxValue)
            //{
            //    return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            //}
            //else
            //{
            //    return System.Convert.ToInt32(value & 0xffffffff);
            //}
        }

        public static int ToInt(double value)
        {
            return System.Convert.ToInt32(value);
            //if (value > int.MaxValue)
            //{
            //    return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            //}
            //else
            //{
            //    return System.Convert.ToInt32(value & 0xffffffff);
            //}
        }

        public static int ToInt(decimal value)
        {
            return System.Convert.ToInt32(value);
            //if (value > int.MaxValue)
            //{
            //    return int.MinValue + System.Convert.ToInt32(value & 0x7fffffff);
            //}
            //else
            //{
            //    return System.Convert.ToInt32(value & 0xffffffff);
            //}
        }

        public static int ToInt(bool value)
        {
            return value ? 1 : 0;
        }

        public static int ToInt(char value)
        {
            return System.Convert.ToInt32(value);
        }

        public static int ToInt(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToInt(value.ToString());
            }
        }

        public static int ToInt(string value, System.Globalization.NumberStyles style)
        {
            return ToInt(ToDouble(value, style));
        }
        public static int ToInt(string value)
        {
            return ToInt(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ConvertToULong"
        /// <summary>
        /// This will System.Convert an integer to a uinteger. The negative integer value is treated as what the memory representation of that negative 
        /// value would be as a uinteger.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ulong ToULong(sbyte value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(byte value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(short value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(ushort value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(int value)
        {
            return System.Convert.ToUInt64(value & long.MaxValue);
        }

        public static ulong ToULong(uint value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(long value)
        {
            return System.Convert.ToUInt64(value & long.MaxValue);
        }

        public static ulong ToULong(ulong value)
        {
            return value;
        }

        ////public static ulong ToULong(float value)
        ////{
        ////    return System.Convert.ToUInt64(value & long.MaxValue);
        ////}

        ////public static ulong ToULong(double value)
        ////{
        ////    return System.Convert.ToUInt64(value & long.MaxValue);
        ////}

        ////public static ulong ToULong(decimal value)
        ////{
        ////    return System.Convert.ToUInt64(value & long.MaxValue);
        ////}

        public static ulong ToULong(bool value)
        {
            return (value) ? 1ul : 0ul;
        }

        public static ulong ToULong(char value)
        {
            return System.Convert.ToUInt64(value);
        }

        public static ulong ToULong(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToUInt64(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToULong(value.ToString());
            }
        }

        public static ulong ToULong(string value, System.Globalization.NumberStyles style)
        {
            return ToULong(ToDouble(value, style));
        }
        public static ulong ToULong(string value)
        {
            return ToULong(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ConvertToLong"
        public static long ToLong(sbyte value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(byte value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(short value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(ushort value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(int value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(uint value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(long value)
        {
            return value;
        }

        public static long ToLong(ulong value)
        {
            if (value > long.MaxValue)
            {
                return int.MinValue + System.Convert.ToInt32(value & long.MaxValue);
            }
            else
            {
                return System.Convert.ToInt64(value & long.MaxValue);
            }
        }

        ////public static long ToLong(float value)
        ////{
        ////    return System.Convert.ToInt64(value & long.MaxValue);
        ////}

        ////public static long ToLong(double value)
        ////{
        ////    return System.Convert.ToInt64(value & long.MaxValue);
        ////}

        ////public static long ToLong(decimal value)
        ////{
        ////    return System.Convert.ToInt64(value & long.MaxValue);
        ////}

        public static long ToLong(bool value)
        {
            return value ? 1 : 0;
        }

        public static long ToLong(char value)
        {
            return System.Convert.ToInt64(value);
        }

        public static long ToLong(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToInt64(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return ToLong(value.ToString());
            }
        }

        public static long ToLong(string value, System.Globalization.NumberStyles style)
        {
            return ToLong(ToDouble(value, style));
        }

        public static long ToLong(string value)
        {
            return ToLong(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ToSingle"
        public static float ToSingle(sbyte value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(byte value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(short value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(ushort value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(int value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(uint value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(long value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(ulong value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(float value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(double value)
        {
            return (float)value;
        }

        public static float ToSingle(decimal value)
        {
            return System.Convert.ToSingle(value);
        }

        public static float ToSingle(bool value)
        {
            return value ? 1 : 0;
        }

        public static float ToSingle(char value)
        {
            return ToSingle(System.Convert.ToInt32(value));
        }

        public static float ToSingle(Vector2 value)
        {
            return value.x;
        }

        public static float ToSingle(Vector3 value)
        {
            return value.x;
        }

        public static float ToSingle(Vector4 value)
        {
            return value.x;
        }
        
        public static float ToSingle(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToSingle(value);
                }
                catch
                {
                    return 0;
                }
            }
            else if(value is Vector2)
            {
                return ToSingle((Vector2)value);
            }
            else if(value is Vector3)
            {
                return ToSingle((Vector3)value);
            }
            else if (value is Vector4)
            {
                return ToSingle((Vector3)value);
            }
            else
            {
                return ToSingle(value.ToString());
            }
        }

        public static float ToSingle(string value, System.Globalization.NumberStyles style)
        {
            return System.Convert.ToSingle(ToDouble(value, style));
        }
        public static float ToSingle(string value)
        {
            return System.Convert.ToSingle(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ToDouble"
        public static double ToDouble(sbyte value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(byte value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(short value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(ushort value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(int value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(uint value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(long value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(ulong value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(float value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(double value)
        {
            return value;
        }

        public static double ToDouble(decimal value)
        {
            return System.Convert.ToDouble(value);
        }

        public static double ToDouble(bool value)
        {
            return value ? 1 : 0;
        }

        public static double ToDouble(char value)
        {
            return ToDouble(System.Convert.ToInt32(value));
        }

        public static double ToDouble(Vector2 value)
        {
            return value.x;
        }

        public static double ToDouble(Vector3 value)
        {
            return value.x;
        }

        public static double ToDouble(Vector4 value)
        {
            return value.x;
        }
        
        public static double ToDouble(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToDouble(value);
                }
                catch
                {
                    return 0;
                }
            }
            else if (value is Vector2)
            {
                return ToDouble((Vector2)value);
            }
            else if (value is Vector3)
            {
                return ToDouble((Vector3)value);
            }
            else if (value is Vector4)
            {
                return ToDouble((Vector3)value);
            }
            else
            {
                return ToDouble(value.ToString(), System.Globalization.NumberStyles.Any, null);
            }
        }

        /// <summary>
        /// System.Converts any string to a number with no errors.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="style"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        /// <remarks>
        /// TODO: I would also like to possibly include support for other number system bases. At least binary and octal.
        /// </remarks>
        public static double ToDouble(string value, System.Globalization.NumberStyles style, System.IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(value)) return 0d;

            style = style & System.Globalization.NumberStyles.Any;
            double dbl = 0;
            if(double.TryParse(value, style, provider, out dbl))
            {
                return dbl;
            }
            else
            {
                //test hex
                int i;
                bool isNeg = false;
                for (i = 0; i < value.Length; i++)
                {
                    if (value[i] == ' ' || value[i] == '+') continue;
                    if (value[i] == '-')
                    {
                        isNeg = !isNeg;
                        continue;
                    }
                    break;
                }

                if (i < value.Length - 1 &&
                        (
                        (value[i] == '#') ||
                        (value[i] == '0' && (value[i + 1] == 'x' || value[i + 1] == 'X')) ||
                        (value[i] == '&' && (value[i + 1] == 'h' || value[i + 1] == 'H'))
                        ))
                {
                    //is hex
                    style = (style & System.Globalization.NumberStyles.HexNumber) | System.Globalization.NumberStyles.AllowHexSpecifier;

                    if (value[i] == '#') i++;
                    else i += 2;
                    int j = value.IndexOf('.', i);

                    if (j >= 0)
                    {
                        long lng = 0;
                        long.TryParse(value.Substring(i, j - i), style, provider, out lng);

                        if (isNeg)
                            lng = -lng;

                        long flng = 0;
                        string sfract = value.Substring(j + 1).Trim();
                        long.TryParse(sfract, style, provider, out flng);
                        return System.Convert.ToDouble(lng) + System.Convert.ToDouble(flng) / System.Math.Pow(16d, sfract.Length);
                    }
                    else
                    {
                        string num = value.Substring(i);
                        long l;
                        if (long.TryParse(num, style, provider, out l))
                            return System.Convert.ToDouble(l);
                        else
                            return 0d;
                    }
                }
                else
                {
                    return 0d;
                }
            }
            

            ////################
            ////OLD garbage heavy version

            //if (value == null) return 0d;
            //value = value.Trim();
            //if (string.IsNullOrEmpty(value)) return 0d;

            //#if UNITY_WEBPLAYER
            //			Match m = Regex.Match(value, RX_ISHEX, RegexOptions.IgnoreCase);
            //#else
            //            Match m = Regex.Match(value, RX_ISHEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //#endif

            //if (m.Success)
            //{
            //    long lng = 0;
            //    style = (style & System.Globalization.NumberStyles.HexNumber) | System.Globalization.NumberStyles.AllowHexSpecifier;
            //    long.TryParse(m.Groups["num"].Value, style, provider, out lng);

            //    if (m.Groups["sign"].Value == "-")
            //        lng = -lng;

            //    if (m.Groups["fractional"].Success)
            //    {
            //        long flng = 0;
            //        string sfract = m.Groups["fractional"].Value.Substring(1);
            //        long.TryParse(sfract, style, provider, out flng);
            //        return System.Convert.ToDouble(lng) + System.Convert.ToDouble(flng) / System.Math.Pow(16d, sfract.Length);
            //    }
            //    else
            //    {
            //        return System.Convert.ToDouble(lng);
            //    }

            //}
            //else
            //{
            //    style = style & System.Globalization.NumberStyles.Any;
            //    double dbl = 0;
            //    double.TryParse(value, style, provider, out dbl);
            //    return dbl;

            //}
        }

        public static double ToDouble(string value, System.Globalization.NumberStyles style)
        {
            return ToDouble(value, style, null);
        }

        public static double ToDouble(string value)
        {
            return ToDouble(value, System.Globalization.NumberStyles.Any, null);
        }
        #endregion

        #region "ToDecimal"
        public static decimal ToDecimal(sbyte value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(byte value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(short value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(ushort value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(int value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(uint value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(long value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(ulong value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(float value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(double value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static decimal ToDecimal(decimal value)
        {
            return value;
        }

        public static decimal ToDecimal(bool value)
        {
            return value ? 1 : 0;
        }

        public static decimal ToDecimal(char value)
        {
            return ToDecimal(System.Convert.ToInt32(value));
        }

        public static decimal ToDecimal(Vector2 value)
        {
            return (decimal)value.x;
        }

        public static decimal ToDecimal(Vector3 value)
        {
            return (decimal)value.x;
        }

        public static decimal ToDecimal(Vector4 value)
        {
            return (decimal)value.x;
        }

        public static decimal ToDecimal(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToDecimal(value);
                }
                catch
                {
                    return 0;
                }
            }
            else if (value is Vector2)
            {
                return ToDecimal((Vector2)value);
            }
            else if (value is Vector3)
            {
                return ToDecimal((Vector3)value);
            }
            else if (value is Vector4)
            {
                return ToDecimal((Vector3)value);
            }
            else
            {
                return ToDecimal(value.ToString());
            }
        }

        public static decimal ToDecimal(string value, System.Globalization.NumberStyles style)
        {
            return System.Convert.ToDecimal(ToDouble(value, style));
        }
        public static decimal ToDecimal(string value)
        {
            return System.Convert.ToDecimal(ToDouble(value, System.Globalization.NumberStyles.Any));
        }
        #endregion

        #region "ToBool"
        public static bool ToBool(sbyte value)
        {
            return value != 0;
        }

        public static bool ToBool(byte value)
        {
            return value != 0;
        }

        public static bool ToBool(short value)
        {
            return value != 0;
        }

        public static bool ToBool(ushort value)
        {
            return value != 0;
        }

        public static bool ToBool(int value)
        {
            return value != 0;
        }

        public static bool ToBool(uint value)
        {
            return value != 0;
        }

        public static bool ToBool(long value)
        {
            return value != 0;
        }

        public static bool ToBool(ulong value)
        {
            return value != 0;
        }

        public static bool ToBool(float value)
        {
            return value != 0;
        }

        public static bool ToBool(double value)
        {
            return value != 0;
        }

        public static bool ToBool(decimal value)
        {
            return value != 0;
        }

        public static bool ToBool(bool value)
        {
            return value;
        }

        public static bool ToBool(char value)
        {
            return System.Convert.ToInt32(value) != 0;
        }

        public static bool ToBool(object value)
        {
            if (value == null)
            {
                return false;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToBoolean(value);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return ToBool(value.ToString());
            }
        }

        /// <summary>
        /// Converts a string to boolean. Is FALSE greedy.
        /// A string is considered TRUE if it DOES meet one of the following criteria:
        /// 
        /// doesn't read blank: ""
        /// doesn't read false (not case-sensitive)
        /// doesn't read 0
        /// doesn't read off (not case-sensitive)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool ToBool(string str)
        {
            //str = (str + "").Trim().ToLower();
            //return !System.Convert.ToBoolean(string.IsNullOrEmpty(str) || str == "false" || str == "0" || str == "off");

            return !(string.IsNullOrEmpty(str) || str.Equals("false", System.StringComparison.OrdinalIgnoreCase) || str.Equals("0", System.StringComparison.OrdinalIgnoreCase) || str.Equals("off", System.StringComparison.OrdinalIgnoreCase));
        }


        public static bool ToBoolInverse(sbyte value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(byte value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(short value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(ushort value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(int value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(uint value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(long value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(ulong value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(float value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(double value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(decimal value)
        {
            return value != 0;
        }

        public static bool ToBoolInverse(bool value)
        {
            return value;
        }

        public static bool ToBoolInverse(char value)
        {
            return System.Convert.ToInt32(value) != 0;
        }

        public static bool ToBoolInverse(object value)
        {
            if (value == null)
            {
                return false;
            }
            else if (value is System.IConvertible)
            {
                try
                {
                    return System.Convert.ToBoolean(value);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return ToBoolInverse(value.ToString());
            }
        }

        /// <summary>
        /// Converts a string to boolean. Is TRUE greedy (inverse of ToBool)
        /// A string is considered TRUE if it DOESN'T meet any of the following criteria:
        /// 
        /// reads blank: ""
        /// reads false (not case-sensitive)
        /// reads 0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool ToBoolInverse(string str)
        {
            //str = (str + "").Trim().ToLower();
            //return (!string.IsNullOrEmpty(str) && str != "false" && str != "0");

            return !string.IsNullOrEmpty(str) && 
                   !str.Equals("false", System.StringComparison.OrdinalIgnoreCase) && 
                   !str.Equals("0", System.StringComparison.OrdinalIgnoreCase) && 
                   !str.Equals("off", System.StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region "Time/Date"

        /// <summary>
        /// Converts an object value to a date value first by straight conversion then by changing it to string if the 
        /// straight conversion doesn't work
        /// </summary>
        /// <param name="value">Object vale</param>
        /// <returns>Date</returns>
        /// <remarks></remarks>
        public static System.DateTime ToDate(object value)
        {
            try
            {
                //'try straight convert
                return System.Convert.ToDateTime(value);

            }
            catch
            {
            }

            try
            {
                //'if straight convert failed, try by string
                return System.Convert.ToDateTime(System.Convert.ToString(value));

            }
            catch
            {
            }

            //'if all fail, return Date(0)
            return new System.DateTime(0);
        }

        public static System.DateTime ToDate(object day, object secondsIntoDay)
        {
            return ConvertUtil.ToDate(day).Date + ConvertUtil.ToTime(secondsIntoDay);
        }

        /// <summary>
        /// Converts the input to the time of day. If value is numeric it is considered as seconds into the day.
        /// </summary>
        /// <param name="value">Object Value treated as number of seconds in the day</param>
        /// <returns>TimeSpan Value</returns>
        /// <remarks>
        /// If input is a TimeSpan, that TimeSpan is just returned.
        /// 
        /// If input is numeric, it is considered as seconds into the day.
        /// 
        /// If value is formatted as LoD supported time format (i.e. '5 seconds', '3.2 days', '100 ticks', etc), it is converted accordingly
        /// 
        /// Lastly it'll attempt to parse to a TimeSpan using normal .Net TimeSpan formatting with system region.
        /// 
        /// If all else, TimeSpan.Zero is returned.
        /// 
        /// 
        /// Supported LoD time formats:
        /// Ticks (any decimal will be rounded off):
        /// #.# t
        /// #.# tick
        /// #.# ticks
        /// 
        /// Milliseconds:
        /// #.# ms
        /// #.# millisecond
        /// #.# milliseconds
        /// 
        /// Seconds:
        /// #.# s
        /// #.# sec
        /// #.# secs
        /// #.# seconds
        /// #.# seconds
        /// 
        /// Minutes:
        /// #.# m
        /// #.# min
        /// #.# mins
        /// #.# minute
        /// #.# minutes
        /// 
        /// Hours:
        /// #.# h
        /// #.# hour
        /// #.# hours
        /// 
        /// Days:
        /// #.# d
        /// #.# day
        /// #.# days
        /// </remarks>
        public static System.TimeSpan ToTime(object value)
        {
            const string RX_TIME = "^\\d+(\\.\\d+)?\\s+((t)|(tick)|(ticks)|(ms)|(millisecond)|(milliseconds)|(s)|(sec)|(secs)|(second)|(seconds)|(m)|(min)|(mins)|(minute)|(minutes)|(h)|(hour)|(hours)|(d)|(day)|(days))$";

            if (value is System.TimeSpan)
            {
                return (System.TimeSpan)value;
            }
            else
            {
                if (IsNumeric(value))
                {
                    return System.TimeSpan.FromSeconds(ToDouble(value));
                }
                else
                {
                    var sval = System.Convert.ToString(value);
#if UNITY_WEBPLAYER
					if (sval != null && Regex.IsMatch(sval.Trim(), RX_TIME, RegexOptions.IgnoreCase))
#else
                    if (sval != null && Regex.IsMatch(sval.Trim(), RX_TIME, RegexOptions.IgnoreCase | RegexOptions.Compiled))
#endif
                    {
                        sval = Regex.Replace(sval.Trim(), "\\s+", " ");
                        var arr = sval.Split(' ');
                        switch (arr[1].ToLower())
                        {
                            case "t":
                            case "tick":
                            case "ticks":
                                return System.TimeSpan.FromTicks(ConvertUtil.ToLong(arr[0]));
                            case "ms":
                            case "millisecond":
                            case "milliseconds":
                                return System.TimeSpan.FromMilliseconds(ConvertUtil.ToDouble(arr[0]));
                            case "s":
                            case "sec":
                            case "secs":
                            case "second":
                            case "seconds":
                                return System.TimeSpan.FromSeconds(ConvertUtil.ToDouble(arr[0]));
                            case "m":
                            case "min":
                            case "mins":
                            case "minute":
                            case "minutes":
                                return System.TimeSpan.FromMinutes(ConvertUtil.ToDouble(arr[0]));
                            case "h":
                            case "hour":
                            case "hours":
                                return System.TimeSpan.FromHours(ConvertUtil.ToDouble(arr[0]));
                            case "d":
                            case "day":
                            case "days":
                                return System.TimeSpan.FromDays(ConvertUtil.ToDouble(arr[0]));
                            default:
                                return System.TimeSpan.Zero;
                        }
                    }
                    else
                    {
                        System.TimeSpan result = default(System.TimeSpan);
                        if (System.TimeSpan.TryParse(sval, out result))
                        {
                            return result;
                        }
                        else
                        {
                            return System.TimeSpan.Zero;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Returns number of seconds into the day a timeofday is. Acts similar to 'TimeToJulian' from old Dockmaster.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int TimeOfDayToSeconds(object value)
        {
            if (value is System.TimeSpan)
            {
                return (int)((System.TimeSpan)value).TotalSeconds;
            }
            else if (value is System.DateTime)
            {
                return (int)((System.DateTime)value).TimeOfDay.TotalSeconds;

            }
            else
            {
                try
                {
                    return (int)System.DateTime.Parse(ConvertUtil.ToString(value)).TimeOfDay.TotalSeconds;
                }
                catch
                {
                    return 0;
                }

            }

        }

        public static double TimeOfDayToMinutes(object value)
        {
            if (value is System.TimeSpan)
            {
                return ((System.TimeSpan)value).TotalMinutes;
            }
            else if (value is System.DateTime)
            {
                return ((System.DateTime)value).TimeOfDay.TotalMinutes;

            }
            else
            {
                try
                {
                    return System.DateTime.Parse(ConvertUtil.ToString(value)).TimeOfDay.TotalMinutes;
                }
                catch
                {
                    return 0;
                }

            }

        }

        public static double TimeOfDayToHours(object value)
        {
            if (value is System.TimeSpan)
            {
                return ((System.TimeSpan)value).TotalHours;
            }
            else if (value is System.DateTime)
            {
                return ((System.DateTime)value).TimeOfDay.TotalHours;

            }
            else
            {
                try
                {
                    return System.DateTime.Parse(ConvertUtil.ToString(value)).TimeOfDay.TotalHours;
                }
                catch
                {
                    return 0;
                }

            }

        }

        #endregion

        #region "Object Only odd prims, TODO"

        public static sbyte ToSByte(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToSByte(ToInt(value.ToString()) & 0x7f);
            }
        }

        public static byte ToByte(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToByte(ToInt(value.ToString()) & 0xff);
            }
        }

        public static short ToShort(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToInt16(ToInt(value.ToString()) & 0x7fff);
            }
        }

        public static System.UInt16 ToUShort(object value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToUInt16(ToInt(value.ToString()) & 0xffff);
            }
        }

        public static char ToChar(object value)
        {
            try
            {
                return System.Convert.ToChar(value);

            }
            catch (System.Exception)
            {
            }

            return System.Char.Parse("");
        }

        #endregion

        #region "ToString"
        public static string ToString(sbyte value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(byte value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(short value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(ushort value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(int value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(uint value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(long value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(ulong value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(float value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(double value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(decimal value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(bool value, string sFormat)
        {
            switch (sFormat)
            {
                case "num":
                    return (value) ? "1" : "0";
                case "normal":
                case "":
                case null:
                    return System.Convert.ToString(value);
                default:
                    return System.Convert.ToString(value);
            }
        }

        public static string ToString(bool value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(char value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(object value)
        {
            return System.Convert.ToString(value);
        }

        public static string ToString(string str)
        {
            return str;
        }
        #endregion

        #region ToVector2

        public static Vector2 ToVector2(string sval)
        {
            if (System.String.IsNullOrEmpty(sval)) return Vector2.zero;

            var arr = StringUtil.SplitFixedLength(sval, ',', 2);

            return new Vector2(ConvertUtil.ToSingle(arr[0]), ConvertUtil.ToSingle(arr[1]));
        }

        public static Vector2 ToVector2(float value)
        {
            return new Vector2(value, value);
        }

        /// <summary>
        /// Creates Vector2 from X and Y values of a Vector3
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public static Vector2 ToVector2(Vector4 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public static Vector2 ToVector2(Quaternion vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public static Vector2 ToVector2(object value)
        {
            if (value == null) return Vector2.zero;
            if (value is Vector2) return (Vector2)value;
            if (value is Vector3)
            {
                var v = (Vector3)value;
                return new Vector2(v.x, v.y);
            }
            if (value is Vector4)
            {
                var v = (Vector4)value;
                return new Vector2(v.x, v.y);
            }
            if (value is Quaternion)
            {
                var q = (Quaternion)value;
                return new Vector2(q.x, q.y);
            }
            if (value is Color)
            {
                var c = (Color)value;
                return new Vector2(c.r, c.g);
            }
            if (ValueIsNumericType(value))
            {
                return Vector2.one * ToSingle(value);
            }
            return ToVector2(System.Convert.ToString(value));
        }

        #endregion

        #region ToVector3

        public static Vector3 ToVector3(float value)
        {
            return new Vector3(value, value, value);
        }

        public static Vector3 ToVector3(Vector2 vec)
        {
            return new Vector3(vec.x, vec.y, 0);
        }

        public static Vector3 ToVector3(Vector4 vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }

        public static Vector3 ToVector3(Quaternion vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }

        public static Vector3 ToVector3(string sval)
        {
            if (System.String.IsNullOrEmpty(sval)) return Vector3.zero;

            var arr = StringUtil.SplitFixedLength(sval, ',', 3);

            return new Vector3(ConvertUtil.ToSingle(arr[0]), ConvertUtil.ToSingle(arr[1]), ConvertUtil.ToSingle(arr[2]));
        }

        public static Vector3 ToVector3(object value)
        {
            if (value == null) return Vector3.zero;
            if (value is Vector2)
            {
                var v = (Vector2)value;
                return new Vector3(v.x, v.y, 0f);
            }
            if (value is Vector3)
            {
                return (Vector3)value;
            }
            if (value is Vector4)
            {
                var v = (Vector4)value;
                return new Vector3(v.x, v.y, v.z);
            }
            if (value is Quaternion)
            {
                var q = (Quaternion)value;
                return new Vector3(q.x, q.y, q.z);
            }
            if (value is Color)
            {
                var c = (Color)value;
                return new Vector3(c.r, c.g, c.b);
            }
            if (ValueIsNumericType(value))
            {
                return Vector3.one * ToSingle(value);
            }
            return ToVector3(System.Convert.ToString(value));
        }

        #endregion

        #region ToVector4

        public static Vector4 ToVector4(float value)
        {
            return new Vector4(value, value, value, value);
        }

        public static Vector4 ToVector4(Vector2 vec)
        {
            return new Vector4(vec.x, vec.y, 0f, 0f);
        }

        public static Vector4 ToVector4(Vector3 vec)
        {
            return new Vector4(vec.x, vec.y, vec.z, 0f);
        }

        public static Vector4 ToVector4(Quaternion vec)
        {
            return new Vector4(vec.x, vec.y, vec.z, vec.w);
        }

        public static Vector4 ToVector4(string sval)
        {
            if (System.String.IsNullOrEmpty(sval)) return Vector3.zero;

            var arr = StringUtil.SplitFixedLength(sval, ',', 4);

            return new Vector4(ConvertUtil.ToSingle(arr[0]), ConvertUtil.ToSingle(arr[1]), ConvertUtil.ToSingle(arr[2]), ConvertUtil.ToSingle(arr[3]));
        }

        public static Vector4 ToVector4(object value)
        {
            if (value == null) return Vector4.zero;
            if (value is Vector2)
            {
                var v = (Vector2)value;
                return new Vector4(v.x, v.y, 0f, 0f);
            }
            if (value is Vector3)
            {
                var v = (Vector3)value;
                return new Vector4(v.x, v.y, v.z, 0f);
            }
            if (value is Vector4)
            {
                return (Vector4)value;
            }
            if (value is Quaternion)
            {
                var q = (Quaternion)value;
                return new Vector4(q.x, q.y, q.z, q.w);
            }
            if (value is Color)
            {
                var c = (Color)value;
                return new Vector4(c.r, c.g, c.b, c.a);
            }
            if (value is Rect)
            {
                var r = (Rect)value;
                return new Vector4(r.x, r.y, r.width, r.height);
            }
            if (ValueIsNumericType(value))
            {
                return new Vector4(ToSingle(value), 0f);
            }
            return Vector4.one * ToSingle(value);
        }

        #endregion

        #region ToQuaternion

        public static Quaternion ToQuaternion(Vector2 vec)
        {
            return new Quaternion(vec.x, vec.y, 0f, 0f);
        }

        public static Quaternion ToQuaternion(Vector3 vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, 0f);
        }

        public static Quaternion ToQuaternion(Vector4 vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }

        /// <summary>
        /// Parses a Quaterion
        /// </summary>
        /// <param name="v"></param>
        /// <param name="a"></param>
        /// <param name="axis"></param>
        /// <param name="bUseRadians"></param>
        /// <returns></returns>
        public static Quaternion ToQuaternion(string sval)
        {
            if (string.IsNullOrEmpty(sval)) return Quaternion.identity;

            var arr = StringUtil.SplitFixedLength(sval.Replace(" ", ""), ',', 4);
            return new Quaternion(ConvertUtil.ToSingle(arr[0]), ConvertUtil.ToSingle(arr[1]), ConvertUtil.ToSingle(arr[2]), ConvertUtil.ToSingle(arr[3]));
        }

        public static Quaternion ToQuaternion(object value)
        {
            if (value == null) return Quaternion.identity;
            if (value is Vector2)
            {
                var v = (Vector2)value;
                return new Quaternion(v.x, v.y, 0f, 0f);
            }
            if (value is Vector3)
            {
                var v = (Vector3)value;
                return new Quaternion(v.x, v.y, v.z, 0f);
            }
            if (value is Vector4)
            {
                var v = (Vector4)value;
                return new Quaternion(v.x, v.y, v.z, v.w);
            }
            if (value is Quaternion)
            {
                return (Quaternion)value;
            }
            if (ValueIsNumericType(value))
            {
                return new Quaternion(ToSingle(value), 0f, 0f, 0f);
            }
            return ToQuaternion(System.Convert.ToString(value));
        }
        
        #endregion


        #region "Is Supported"

        /// <summary>
        /// If type passed is a support TypeCode
        /// </summary>
        /// <param name="tp">Type</param>
        /// <returns>Returns true is type given is a supported type</returns>
        /// <remarks></remarks>
        public static bool IsSupportedType(System.Type tp)
        {
            System.TypeCode code = System.Type.GetTypeCode(tp);
            return IsSupportedType(tp, code);
        }

        private static bool IsSupportedType(System.Type tp, System.TypeCode code)
        {
            if (tp !=null && tp.IsEnum) return true;

            if (code == System.TypeCode.Object)
            {
                return object.ReferenceEquals(tp, typeof(object)) || object.ReferenceEquals(tp, typeof(System.TimeSpan));
            }
            else
            {
                return !(code < 0 || (int)code > 18 || (int)code == 17);
            }
        }

        /// <summary>
        /// Returns true if the object could be converted to a number by ConvertUtil
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsNumeric(object value, System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Any, System.IFormatProvider provider = null, bool bBlankIsZero = false)
        {
            if (value == null) return bBlankIsZero;
            if (ValueIsNumericType(value)) return true;

            string sval = System.Convert.ToString(value);
            if (string.IsNullOrEmpty(sval))
                return bBlankIsZero;

            sval = sval.Trim();

            if(IsHex(sval))
            {
                return true;
            }
            else
            {
                style = style & System.Globalization.NumberStyles.Any;
                double dbl = 0;
                return double.TryParse(sval, style, provider, out dbl);
            }


            ////################
            ////OLD garbage heavy version

            //#if UNITY_WEBPLAYER
            //			Match m = Regex.Match(sval, RX_ISHEX, RegexOptions.IgnoreCase);
            //#else
            //            Match m = Regex.Match(sval, RX_ISHEX, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //#endif

            //            if (m.Success)
            //            {
            //                return true;
            //            }
            //            else
            //            {
            //                style = style & System.Globalization.NumberStyles.Any;
            //                double dbl = 0;
            //                return double.TryParse(sval, style, provider, out dbl);
            //            }


        }

        public static bool IsHex(string value)
        {
            int i;
            for (i = 0; i < value.Length; i++)
            {
                if (value[i] == ' ' || value[i] == '+' || value[i] == '-') continue;

                break;
            }

            return (i < value.Length - 1 &&
                    (
                    (value[i] == '#') ||
                    (value[i] == '0' && (value[i + 1] == 'x' || value[i + 1] == 'X')) ||
                    (value[i] == '&' && (value[i + 1] == 'h' || value[i + 1] == 'H'))
                    ));
        }
        
        public static bool IsInteger(object value, bool bBlankIsZero = false)
        {
            if (value == null) return bBlankIsZero;
            if (ValueIsNumericType(value)) return true;

            //string sval = (System.Convert.ToString(value) + "").Trim();
            string sval = System.Convert.ToString(value);

            if (string.IsNullOrEmpty(sval))
                return bBlankIsZero;

            //'Return Regex.Match(sval, "\d+", RegexOptions.Compiled).Success
            int i = 0;
            return int.TryParse(sval, out i);
        }

        /// <summary>
        /// Tests if the typeof the value passed in is a numeric type. Handles IWrapper types as well.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool ValueIsNumericType(object obj)
        {

            if (obj == null)
                return false;

            return IsNumericType(obj.GetType());

        }

        /// <summary>
        /// Tests if the type is a numeric type.
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsNumericType(System.Type tp)
        {
            return IsNumericType(System.Type.GetTypeCode(tp));
        }

        public static bool IsNumericType(System.TypeCode code)
        {
            switch (code)
            {
                case System.TypeCode.SByte:
                    //5
                    return true;
                case System.TypeCode.Byte:
                    //6
                    return true;
                case System.TypeCode.Int16:
                    //7
                    return true;
                case System.TypeCode.UInt16:
                    //8
                    return true;
                case System.TypeCode.Int32:
                    //9
                    return true;
                case System.TypeCode.UInt32:
                    //10
                    return true;
                case System.TypeCode.Int64:
                    //11
                    return true;
                case System.TypeCode.UInt64:
                    //12
                    return true;
                case System.TypeCode.Single:
                    //13
                    return true;
                case System.TypeCode.Double:
                    //14
                    return true;
                case System.TypeCode.Decimal:
                    //15

                    return true;
                default:
                    return false;
            }
        }
        
        #endregion

        #region "wildcard"

        //'ToString
        public const string LOD_STR = "str";
        //'ToString
        public const string LOD_STRING = "string";
        //'To Nullable Decimal
        public const string LOD_NULLABLE_NUM = "num?";
        //'ToSingle
        public const string LOD_FLOAT = "float";
        //'ToSingle
        public const string LOD_SINGLE = "single";
        //TODOuble
        public const string LOD_DBL = "dbl";
        //TODOuble
        public const string LOD_DOUBLE = "double";
        //'ToDecimal
        public const string LOD_NUM = "num";
        //'ToDecimal
        public const string LOD_DEC = "dec";
        //'ToDecimal
        public const string LOD_CASH = "cash";
        //'ToInt
        public const string LOD_INT = "int";
        //'ToBool
        public const string LOD_BOOL = "bool";
        //'get ascii code for a char
        public const string LOD_ASCII = "ascii";
        //'get char from an ascii code
        public const string LOD_CHAR = "char";
        //'ToDate
        public const string LOD_DATE = "date";
        //'ToTime
        public const string LOD_TIME = "time";

        public const string LOD_TIMEOFDAYTOSECONDS = "timeofdaytoseconds";

        public const string LOD_PSHIFT_0 = "p0";
        public const string LOD_PSHIFT_L1 = "p1";
        public const string LOD_PSHIFT_L2 = "p2";
        public const string LOD_PSHIFT_L3 = "p3";
        public const string LOD_PSHIFT_L4 = "p4";
        public const string LOD_PSHIFT_L5 = "p5";
        public const string LOD_PSHIFT_L6 = "p6";
        public const string LOD_PSHIFT_R1 = "p-1";
        public const string LOD_PSHIFT_R2 = "p-2";
        public const string LOD_PSHIFT_R3 = "p-3";
        public const string LOD_PSHIFT_R4 = "p-4";
        public const string LOD_PSHIFT_R5 = "p-5";

        public const string LOD_PSHIFT_R6 = "p-6";
        public const string LOD_STRARR_COMMA = "str[,]";
        public const string LOD_STRARR_SEMICOLON = "str[;]";

        public const string LOD_STRARR_PIPE = "str[|]";
        public static bool IsLoDConvertType(string sType)
        {
            //sType = (sType + "").Trim();
            if (string.IsNullOrEmpty(sType)) return false;

            const string sREGX = "^((p-?\\d+)|(str)|(string)|(num\\?)|(float)|(single)|(dbl)|(double)|(num)|(dec)|(cash)|(int)|(bool)|(ascii)|(char)|(date)|(time)|(timeofday))(\\[.\\])?$";

            return Regex.IsMatch(sType, sREGX, RegexOptions.IgnoreCase);

        }

        public static bool IsLoDNumericConvertType(string sType)
        {
            //sType = (sType + "").Trim();
            if (string.IsNullOrEmpty(sType)) return false;

            const string sREGX = "^((p-?\\d+)|(num\\?)|(float)|(single)|(dbl)|(double)|(num)|(dec)|(cash)|(int))$";

            return Regex.IsMatch(sType, sREGX, RegexOptions.IgnoreCase);

        }

        /// <summary>
        /// Returns true if the format string is preceeded by an lod conversion.
        /// </summary>
        /// <param name="sFormatString"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsValidLoDLedFormatString(string sFormatString)
        {
            const string sREGX = "^((p-?\\d+)|(str)|(string)|(num\\?)|(float)|(single)|(dbl)|(double)|(num)|(dec)|(cash)|(int)|(bool)|(ascii)|(char)|(date)|(time)|(timeofday))(\\[.\\])?:";

            if (sFormatString == null)
                return false;

            return Regex.IsMatch(sFormatString, sREGX, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Returns true if the format string is preceeded by an lod conversion.
        /// </summary>
        /// <param name="sFormatString">The format string to test.</param>
        /// <param name="outLoDConvertType">A return argument for the lod convert type.</param>
        /// <param name="outRemainingFormatString">A return argument for the formatstring with the lod convert spliced off.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsValidLoDLedFormatString(string sFormatString, out string outLoDConvertType)
        {
            string blargh = null;
            return IsValidLoDLedFormatString(sFormatString, out outLoDConvertType, out blargh);
        }

        /// <summary>
        /// Returns true if the format string is preceeded by an lod conversion.
        /// </summary>
        /// <param name="sFormatString">The format string to test.</param>
        /// <param name="outLoDConvertType">A return argument for the lod convert type.</param>
        /// <param name="outRemainingFormatString">A return argument for the formatstring with the lod convert spliced off.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsValidLoDLedFormatString(string sFormatString, out string outLoDConvertType, out string outRemainingFormatString)
        {
            const string sREGX = "^((p-?\\d+)|(str)|(string)|(num\\?)|(float)|(single)|(dbl)|(double)|(num)|(dec)|(cash)|(int)|(bool)|(ascii)|(char)|(date)|(timeofdaytoseconds)|(time)|(timeofday))(\\[.\\])?:";

            outLoDConvertType = null;
            outRemainingFormatString = null;

            if (sFormatString == null)
                return false;

            Match match = Regex.Match(sFormatString, sREGX, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                outLoDConvertType = match.Value.Substring(0, match.Value.Length - 1);
                outRemainingFormatString = sFormatString.Substring(match.Length, sFormatString.Length - match.Length);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the Type associated with an LoD convert type. 
        /// </summary>
        /// <param name="sType"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static System.Type GetLoDConvertType(string sType)
        {

            const string sREGX = "^(?<type>(p-?\\d+)|(str)|(string)|(num\\?)|(float)|(single)|(dbl)|(double)|(num)|(dec)|(cash)|(int)|(bool)|(ascii)|(char)|(date)|(time)|(timeofday))(?<array>(\\[.\\]))?$";

            //sType = (sType + "").Trim();
            if (string.IsNullOrEmpty(sType)) return null;

            var match = Regex.Match(sType, sREGX, RegexOptions.IgnoreCase);
            if (!match.Success)
                return null;

            sType = match.Groups["type"].Value.ToLower();
            bool bIsArray = match.Groups["array"].Success;

            if (Regex.IsMatch(sType, "^p-?\\d+$"))
            {
                return bIsArray ? typeof(decimal[]) : typeof(decimal);
            }

            switch (sType)
            {
                case LOD_STR:
                case LOD_STRING:
                    return bIsArray ? typeof(string[]) : typeof(string);
                case LOD_FLOAT:
                case LOD_SINGLE:
                    return bIsArray ? typeof(float[]) : typeof(float);
                case LOD_DBL:
                case LOD_DOUBLE:
                    return bIsArray ? typeof(double[]) : typeof(double);
                case LOD_NUM:
                case LOD_DEC:
                case LOD_CASH:
                    return bIsArray ? typeof(decimal[]) : typeof(decimal);
                case LOD_INT:
                    return bIsArray ? typeof(int[]) : typeof(int);
                case LOD_NULLABLE_NUM:
                    return bIsArray ? typeof(decimal[]) : typeof(decimal);
                case LOD_BOOL:
                    return bIsArray ? typeof(bool[]) : typeof(bool);
                case LOD_ASCII:
                    return bIsArray ? typeof(byte[]) : typeof(byte);
                case LOD_CHAR:
                    return bIsArray ? typeof(char[]) : typeof(char);
                case LOD_DATE:
                    return bIsArray ? typeof(System.DateTime[]) : typeof(System.DateTime);
                case LOD_TIME:
                    return bIsArray ? typeof(System.TimeSpan[]) : typeof(System.TimeSpan);
                case LOD_TIMEOFDAYTOSECONDS:
                    return bIsArray ? typeof(int[]) : typeof(int);
                default:

                    return null;
            }

        }

        /// <summary>
        /// Converts an object to a type described as a string from a list of approved string names.
        /// </summary>
        /// <param name="value">Object to be converted</param>
        /// <param name="sType">String type you want sval converted to</param>
        /// <returns></returns>
        /// <remarks>
        /// Approved types:<br/>
        /// String - str, string<br/>
        /// Single - float, single<br/>
        /// Double - dbl, double<br/>
        /// Decimal - num, dec, cash<br/>
        /// Nullable Decimal - num?<br/>
        /// Integer - int<br/>
        /// Boolean - bool<br/>
        /// Asc() - ascii<br/>
        /// Char - char<br/>
        /// DateTime - date, pickdate, pickdatetodate<br/>
        ///     *date is treated as a string formatted as a date, ex: 8/17/2011. Region will be decided by runtime machines region settings.<br/>
        ///     *pickdate is treated as an integer count of days from Dec 31st 1967<br/>
        /// TimeSpan - time, timeofday, timeofdaytoseconds<br/>
        ///     *time value is treated as number of seconds from midnight<br/>
        ///     *timeofday value is treated as number of seconds and returns as a string formatted as hh:mm tt
        ///     *timeofdaytoseconds value is first converted to time, and then extracts the seconds into that day as an int
        /// </remarks>
        public static object LoDConvertTo(object value, string sType)
        {
            const string sREGX = "^(?<type>(p-?\\d+)|(str)|(string)|(num\\?)|(float)|(single)|(dbl)|(double)|(num)|(dec)|(cash)|(int)|(bool)|(ascii)|(char)|(date)|(timeofdaytoseconds)|(time)|(timeofday))(?<array>(\\[.\\]))?$";

            //if (value == null)
            //    value = "";
            //if (string.IsNullOrEmpty(sType))
            //    sType = "str";
            //else
            //    sType = sType.Trim();
            if (string.IsNullOrEmpty(sType)) return null;
            
            var match = Regex.Match(sType, sREGX, RegexOptions.IgnoreCase);
            if (!match.Success)
                return null;

            sType = match.Groups["type"].Value.ToLower();

            if (match.Groups["array"].Success)
            {
                char cDelim = match.Groups["array"].Value[1];
                var sval = System.Convert.ToString(value);
                var arr = string.IsNullOrEmpty(sval) ? new string[] { } : sval.Split(cDelim);
                if (arr == null)
                    return null;

                if (Regex.IsMatch(sType, "^p-?\\d+$"))
                {
                    decimal[] result = new decimal[arr.Length];
                    int move = 0;
                    System.Int32.TryParse(sType.Substring(1), out move);

                    for (int i = 0; i <= arr.Length - 1; i++)
                    {
                        decimal argNum = ConvertUtil.ToDecimal(arr[i]);
                        result[i] = argNum / (decimal)Mathf.Pow(10, move);
                    }

                    return result;
                }

                switch (sType)
                {
                    case LOD_STR:
                    case LOD_STRING:
                        string[] strResult = new string[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            strResult[i] = System.Convert.ToString(arr[i]);
                        }


                        return strResult;
                    case LOD_FLOAT:
                    case LOD_SINGLE:
                        float[] singleResult = new float[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            singleResult[i] = ConvertUtil.ToSingle(arr[i]);
                        }

                        return singleResult;
                    case LOD_DBL:
                    case LOD_DOUBLE:
                        double[] dblResult = new double[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            dblResult[i] = ConvertUtil.ToDouble(arr[i]);
                        }

                        return dblResult;
                    case LOD_NUM:
                    case LOD_DEC:
                    case LOD_CASH:
                        decimal[] decResult = new decimal[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            decResult[i] = ConvertUtil.ToDecimal(arr[i]);
                        }

                        return decResult;
                    case LOD_INT:
                        int[] intResult = new int[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            intResult[i] = ConvertUtil.ToInt(arr[i]);
                        }

                        return intResult;
                    case LOD_NULLABLE_NUM:
                        decimal?[] nullNumResult = new decimal?[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            if (StringUtil.IsNullOrWhitespace(arr[i]))
                            {
                                nullNumResult[i] = null;
                            }
                            else
                            {
                                nullNumResult[i] = ConvertUtil.ToDecimal(arr[i]);
                            }
                        }

                        return nullNumResult;
                    case LOD_BOOL:
                        bool[] boolResult = new bool[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            boolResult[i] = ConvertUtil.ToBool(arr[i]);
                        }

                        return boolResult;
                    case LOD_ASCII:
                        int[] ascResult = new int[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            if (string.IsNullOrEmpty(arr[i]))
                            {
                                ascResult[i] = 0;
                            }
                            else
                            {
                                ascResult[i] = (int)arr[i][0];
                            }
                        }

                        return ascResult;
                    case LOD_CHAR:
                        char[] charResult = new char[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            if (string.IsNullOrEmpty(arr[i]))
                            {
                                charResult[i] = ' ';
                            }
                            else
                            {
                                charResult[i] = System.Convert.ToChar(arr[i][0]);
                            }
                        }

                        return charResult;
                    case LOD_DATE:
                        System.DateTime[] dateResult = new System.DateTime[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            dateResult[i] = ConvertUtil.ToDate(arr[i]);
                        }

                        return dateResult;
                    case LOD_TIME:
                        System.TimeSpan[] timeResult = new System.TimeSpan[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            timeResult[i] = ConvertUtil.ToTime(arr[i]);
                        }


                        return timeResult;
                    case LOD_TIMEOFDAYTOSECONDS:
                        int[] secondsResult = new int[arr.Length];
                        for (int i = 0; i <= arr.Length - 1; i++)
                        {
                            secondsResult[i] = ConvertUtil.TimeOfDayToSeconds(arr[i]);
                        }

                        return secondsResult;
                }

            }
            else
            {
                if (Regex.IsMatch(sType, "^p-?\\d+$"))
                {
                    if (value == null)
                        return 0m;
                    if (value is string && StringUtil.IsNullOrWhitespace(value as string))
                        return 0m;

                    decimal argNum = ConvertUtil.ToDecimal(value);
                    int move = 0;
                    int.TryParse(sType.Substring(1), out move);
                    return argNum / (decimal)Mathf.Pow(10, move);
                }

                switch (sType)
                {
                    case LOD_STR:
                    case LOD_STRING:

                        return System.Convert.ToString(value);
                    case LOD_FLOAT:
                    case LOD_SINGLE:

                        return ConvertUtil.ToSingle(value);
                    case LOD_DBL:
                    case LOD_DOUBLE:

                        return ConvertUtil.ToDouble(value);
                    case LOD_NUM:
                    case LOD_DEC:
                    case LOD_CASH:

                        return ConvertUtil.ToDecimal(value);
                    case LOD_INT:

                        return ConvertUtil.ToInt(value);
                    case LOD_NULLABLE_NUM:
                        if (value == null || StringUtil.IsNullOrWhitespace(value.ToString()))
                        {
                            return null;
                        }
                        else
                        {
                            return ConvertUtil.ToDecimal(value);
                        }

                    case LOD_BOOL:

                        return ConvertUtil.ToBool(value);
                    case LOD_ASCII:
                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                        {
                            return 0;
                        }
                        else
                        {
                            //return Strings.Asc(value);
                            if (value is char)
                            {
                                return (int)((char)value);
                            }
                            else
                            {
                                return (int)(value.ToString())[0];
                            }
                        }

                    case LOD_CHAR:
                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                        {
                            return "";
                        }
                        else
                        {
                            return System.Convert.ToChar(value.ToString()[0]);
                        }

                    case LOD_DATE:
                        return ConvertUtil.ToDate(value);

                    case LOD_TIME:
                        return ConvertUtil.ToTime(value);

                    case LOD_TIMEOFDAYTOSECONDS:
                        return ConvertUtil.TimeOfDayToSeconds(value);
                }
            }

            //should never reach here
            return null;

        }

        /// <summary>
        /// Format an abstract value as a string to a specific manner. Value is first converted to supplied type before formatting. 
        /// Available types include:
        /// str - string
        /// float - a double floating-point value
        /// dec - a decimal
        /// int - an integer
        /// num - a numeric value (includes hex)
        /// bool - a boolean
        /// ascii - converts the string to its ascii code
        /// char - converts an integer value to its char
        /// 
        /// default is string
        /// </summary>
        /// <param name="value">Object Value to be converted</param>
        /// <param name="sType">Type that value is to be converted to</param>
        /// <param name="sFormatStr">Formatting characters</param>
        /// <returns>Value converted to specified type and formated based on formatting string</returns>
        /// <remarks>
        /// TODO!: change around dict.config.xml so that we no longer have {0:...} in it, and rewrite this to enable a better method of formatting by the dict config
        /// </remarks>
        public static string LoDConvertTo(object value, string sType, string sFormatStr)
        {
            if (value == null)
                return "";
            if (string.IsNullOrEmpty(sType))
                sType = "str";
            if (string.IsNullOrEmpty(sFormatStr))
                sFormatStr = "{0}";
            else
                sFormatStr = "{0:" + sFormatStr + "}";

            value = LoDConvertTo(value, sType);

            return StringUtil.Format(sFormatStr, value);

        }

        /// <summary>
        /// Converts value to a Prim type of "T"
        /// </summary>
        /// <typeparam name="T">Prim type to be converted to</typeparam>
        /// <param name="value">Object value to be convertyed</param>
        /// <returns>Value as new converted type</returns>
        /// <remarks></remarks>
        public static T ToPrim<T>(object value)
        {
            System.Type tp = typeof(T);
            System.TypeCode code = System.Type.GetTypeCode(tp);
            if (!ConvertUtil.IsSupportedType(tp, code))
            {
                throw new System.Exception(typeof(T).Name + " is not accepted as a generic type for ConvertUtil.ToPrim.");
            }

            return (T)ConvertUtil.ToPrim(value, tp, code);
        }

        public static object ToPrim(object value, System.Type tp)
        {
            if (tp == null) throw new System.ArgumentException("Type must be non-null", "tp");
            System.TypeCode code = System.Type.GetTypeCode(tp);
            if (!ConvertUtil.IsSupportedType(tp, code))
            {
                throw new System.Exception(tp.Name + " is not accepted as a generic type for ConvertUtil.ToPrim.");
            }

            return ConvertUtil.ToPrim(value, tp, code);
        }

        public static bool TryToPrim<T>(object value, out T output)
        {
            try
            {
                output = ToPrim<T>(value);
                return true;

            }
            catch
            {
                output = default(T);
            }

            return false;
        }

        public static bool TryToPrim(object value, System.Type tp, out object output)
        {
            try
            {
                output = ToPrim(value, tp);
                return true;
            }
            catch
            {
                output = null;
            }

            return false;
        }

        public static object ToPrim(object value, System.TypeCode code)
        {
            return ToPrim(value, null, code);
        }

        private static object ToPrim(object value, System.Type tp, System.TypeCode code)
        {
            //first make sure it's not an enum
            if(tp != null && tp.IsEnum)
            {
                if (value is string)
                    return System.Enum.Parse(tp, value as string, true);

                var vtp = value.GetType();
                switch (System.Type.GetTypeCode(vtp))
                {
                    case System.TypeCode.SByte:
                    case System.TypeCode.Byte:
                    case System.TypeCode.Int16:
                    case System.TypeCode.UInt16:
                    case System.TypeCode.Int32:
                    case System.TypeCode.UInt32:
                    case System.TypeCode.Int64:
                    case System.TypeCode.UInt64:
                        return System.Enum.ToObject(tp, value);
                    default:
                        return System.Enum.Parse(tp, System.Convert.ToString(value), true);
                }
            }

            //now base off of the TypeCode
            switch (code)
            {
                case System.TypeCode.Empty:
                    //0
                    return null;
                case System.TypeCode.Object:
                    //1
                    if (object.ReferenceEquals(tp, typeof(System.TimeSpan)))
                    {
                        return ConvertUtil.ToTime(value);
                    }
                    else if (tp == null || object.ReferenceEquals(tp, typeof(object)))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }

                case System.TypeCode.DBNull:
                    //2
                    return System.DBNull.Value;
                case System.TypeCode.Boolean:
                    //3
                    return ConvertUtil.ToBool(value);
                case System.TypeCode.Char:
                    //4
                    return ConvertUtil.ToChar(value);
                case System.TypeCode.SByte:
                    //5
                    return ConvertUtil.ToSByte(value);
                case System.TypeCode.Byte:
                    //6
                    return ConvertUtil.ToByte(value);
                case System.TypeCode.Int16:
                    //7
                    return ConvertUtil.ToShort(value);
                case System.TypeCode.UInt16:
                    //8
                    return ConvertUtil.ToUShort(value);
                case System.TypeCode.Int32:
                    //9
                    return ConvertUtil.ToInt(value);
                case System.TypeCode.UInt32:
                    //10
                    return ConvertUtil.ToUInt(value);
                case System.TypeCode.Int64:
                    //11
                    return ConvertUtil.ToLong(value);
                case System.TypeCode.UInt64:
                    //12
                    return ConvertUtil.ToULong(value);
                case System.TypeCode.Single:
                    //13
                    return ConvertUtil.ToSingle(value);
                case System.TypeCode.Double:
                    //14
                    return ConvertUtil.ToDouble(value);
                case System.TypeCode.Decimal:
                    //15
                    return ConvertUtil.ToDecimal(value);
                case System.TypeCode.DateTime:
                    //16
                    return ConvertUtil.ToDate(value);
                case System.TypeCode.String:
                    //18
                    return System.Convert.ToString(value);
                default:
                    return null;
            }
        }
        
        #endregion

        #region "BitArray"

        public static byte[] ToByteArray(this System.Collections.BitArray bits)
        {
            int numBytes = (int)System.Math.Ceiling(bits.Count / 8.0f);
            byte[] bytes = new byte[numBytes];

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                {
                    int j = i / 8;
                    int m = i % 8;
                    bytes[j] |= (byte)(1 << m);
                }
            }

            return bytes;
        }

        #endregion

        #region CastTo Methods

        /*

        /// <summary>
        /// Unique casting routine that will allow for casting between components of a gameobject 
        /// this allows you to treat a gameobject as its component...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T CastTo<T>(object obj)
        {
            if (obj is T) return (T)obj;

            var tp = typeof(T);
            int itype = 0;
            if (TypeUtil.IsType(tp, typeof(GameObject)))
                itype = 1;
            if (TypeUtil.IsType(tp, typeof(Component)))
                itype = 2;
            else if (TypeUtil.IsType(tp, typeof(IComponent)))
                itype = 3;

            if (itype == 1 && GameObjectUtil.IsGameObjectSource(obj))
                return (T)(object)GameObjectUtil.GetGameObjectFromSource(obj);

            if (itype > 1 && GameObjectUtil.IsGameObjectSource(obj))
            {
                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                T comp = go.GetFirstLikeComponent<T>();
                return comp;
            }

            if (ConvertUtil.IsSupportedType(tp))
            {
                return ConvertUtil.ToPrim<T>(obj);
            }

            return default(T);
        }

        public static T[] CastManyTo<T>(System.Collections.IEnumerable objects)
        {
            var lst = new List<T>();

            var tp = typeof(T);
            int itype = 0;
            if (TypeUtil.IsType(tp, typeof(GameObject)))
                itype = 1;
            if (TypeUtil.IsType(tp, typeof(Component)))
                itype = 2;
            else if (TypeUtil.IsType(tp, typeof(IComponent)))
                itype = 3;

            foreach (object obj in objects)
            {
                if (obj is T)
                {
                    lst.Add((T)obj);
                }
                else if (itype == 1 && GameObjectUtil.IsGameObjectSource(obj))
                {
                    lst.Add((T)(object)GameObjectUtil.GetGameObjectFromSource(obj));
                }
                else if (itype > 1 && GameObjectUtil.IsGameObjectSource(obj))
                {
                    var go = GameObjectUtil.GetGameObjectFromSource(obj);
                    T comp = go.GetFirstLikeComponent<T>();
                    lst.Add(comp);
                }
                else if (ConvertUtil.IsSupportedType(tp))
                {
                    lst.Add(ConvertUtil.ToPrim<T>(obj));
                }
                else
                {
                    lst.Add(default(T));
                }
            }

            return lst.ToArray();
        }
 
         */

        #endregion

    }
}

