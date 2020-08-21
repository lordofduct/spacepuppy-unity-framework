using System;
using System.Text;
using System.Text.RegularExpressions;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{
    public static class StringUtil
    {

        public enum Alignment
        {
            Left = 0,
            Right = 1,
            Center = 2
        }

        #region Constants

        public const string RX_OPEN_TO_CLOSE_PARENS = @"\(" +
                                                      @"[^\(\)]*" +
                                                      @"(" +
                                                      @"(" +
                                                      @"(?<Open>\()" +
                                                      @"[^\(\)]*" +
                                                      @")+" +
                                                      @"(" +
                                                      @"(?<Close-Open>\))" +
                                                      @"[^\(\)]*" +
                                                      @")+" +
                                                      @")*" +
                                                      @"(?(Open)(?!))" +
                                                      @"\)";
        public const string RX_OPEN_TO_CLOSE_ANGLES = @"<" +
                                                      @"[^<>]*" +
                                                      @"(" +
                                                      @"(" +
                                                      @"(?<Open><)" +
                                                      @"[^<>]*" +
                                                      @")+" +
                                                      @"(" +
                                                      @"(?<Close-Open>>)" +
                                                      @"[^<>]*" +
                                                      @")+" +
                                                      @")*" +
                                                      @"(?(Open)(?!))" +
                                                      @">";
        public const string RX_OPEN_TO_CLOSE_BRACKETS = @"\[" +
                                                        @"[^\[\]]*" +
                                                        @"(" +
                                                        @"(" +
                                                        @"(?<Open>\[)" +
                                                        @"[^\[\]]*" +
                                                        @")+" +
                                                        @"(" +
                                                        @"(?<Close-Open>\])" +
                                                        @"[^\[\]]*" +
                                                        @")+" +
                                                        @")*" +
                                                        @"(?(Open)(?!))" +
                                                        @"\]";

        public const string RX_UNESCAPED_COMMA = @"(?<!\\),";
        public const string RX_UNESCAPED_COMMA_NOTINPARENS = @"(?<!\\),(?![^()]*\))";

        #endregion


        #region Matching

        public static bool IsNullOrEmpty(string value)
        {
            return System.String.IsNullOrEmpty(value);
            //return (value + "") == "";
        }

        public static bool IsNotNullOrEmpty(string value)
        {
            return !System.String.IsNullOrEmpty(value);
            //return (value + "") != "";
        }

        public static bool IsNullOrWhitespace(string value)
        {
            return (value + "").Trim() == "";
        }

        public static bool IsNotNullOrWhitespace(string value)
        {
            return (value + "").Trim() != "";
        }


        public static bool Equals(string valueA, string valueB, bool bIgnoreCase)
        {
            return (bIgnoreCase) ? String.Equals(valueA, valueB) : String.Equals(valueA, valueB, StringComparison.OrdinalIgnoreCase);
        }
        public static bool Equals(string valueA, string valueB)
        {
            return Equals(valueA, valueB, false);
        }
        public static bool Equals(string value, params string[] others)
        {
            if ((others == null || others.Length == 0))
            {
                return String.IsNullOrEmpty(value);
            }

            foreach (var sval in others)
            {
                if (value == sval) return true;
            }

            return false;
        }
        public static bool Equals(string value, string[] others, bool bIgnoreCase)
        {
            if ((others == null || others.Length == 0))
            {
                return String.IsNullOrEmpty(value);
            }

            foreach (var sval in others)
            {
                if (StringUtil.Equals(value, sval, bIgnoreCase)) return true;
            }

            return false;
        }

        public static bool StartsWith(string value, string start)
        {
            return StartsWith(value, start);
        }

        public static bool StartsWith(string value, string start, bool bIgnoreCase)
        {
            return value.StartsWith(start, (bIgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        public static bool EndsWith(string value, string end)
        {
            return EndsWith(value, end, false);
        }

        public static bool EndsWith(string value, string end, bool bIgnoreCase)
        {
            return value.EndsWith(end, (bIgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        public static bool Contains(string str, params string[] values)
        {
            if (str == null || values == null || values.Length == 0) return false;

            foreach (var sother in values)
            {
                if (str.Contains(sother)) return true;
            }

            return false;
        }

        public static bool Contains(string str, bool ignorCase, string sother)
        {
            if (string.IsNullOrEmpty(str)) return string.IsNullOrEmpty(sother);
            if (sother == null) return false;

            if (ignorCase)
            {
                str = str.ToLower();
                if (str.Contains(sother.ToLower())) return true;
            }
            else
            {
                if (str.Contains(sother)) return true;
            }

            return false;
        }

        public static bool Contains(string str, bool ignorCase, params string[] values)
        {
            if (str == null || values == null || values.Length == 0) return false;

            if (ignorCase)
            {
                str = str.ToLower();
                foreach (var sother in values)
                {
                    if (str.Contains(sother.ToLower())) return true;
                }
            }
            else
            {
                foreach (var sother in values)
                {
                    if (str.Contains(sother)) return true;
                }
            }

            return false;
        }

        #endregion

        #region Morphing

        public static string NicifyVariableName(string nm)
        {
            if (string.IsNullOrEmpty(nm)) return string.Empty;

            int index = 0;
            while(index < nm.Length && char.IsWhiteSpace(nm[index]))
            {
                index++;
            }
            if (index >= nm.Length) return string.Empty;

            if(nm.IndexOf("m_", index) == 0)
            {
                index += 2;
            }
            else if(nm[index] == 'k' && nm.Length > (index + 2) && char.IsUpper(nm[index + 1]))
            {
                index += 1;
            }

            while(index < nm.Length && nm[index] == '_')
            {
                index++;
            }
            if (index >= nm.Length) return string.Empty;


            var sb = GetTempStringBuilder();
            sb.Append(char.ToUpper(nm[index]));
            for (int i = index + 1; i < nm.Length; i++)
            {
                if(char.IsUpper(nm[i]))
                {
                    sb.Append(' ');
                }
                sb.Append(nm[i]);
            }
            return Release(sb);
        }

        public static string ToLower(string value)
        {
            //return (value != null) ? value.ToLower() : null;
            return (value + "").ToLower();
        }

        public static string ToUpper(string value)
        {
            //return (value != null) ? value.ToUpper() : null;
            return (value + "").ToUpper();
        }

        public static string Trim(string value)
        {
            if (value == null) return null;

            return value.Trim();
        }

        public static string[] Split(string value, string delim)
        {
            if (value == null) return null;
            return value.Split(new string[] { delim }, StringSplitOptions.None);
        }

        public static string[] Split(string value, params char[] delim)
        {
            if (value == null) return null;
            return value.Split(delim);
        }

        public static string[] SplitFixedLength(string value, string delim, int len)
        {
            if (value == null) return new string[len];

            string[] arr = value.Split(new string[] { delim }, StringSplitOptions.None);
            if (arr.Length != len) Array.Resize(ref arr, len);
            return arr;
        }

        public static string[] SplitFixedLength(string value, char delim, int len)
        {
            if (value == null) return new string[len];

            string[] arr = value.Split(delim);
            if (arr.Length != len) Array.Resize(ref arr, len);
            return arr;
        }

        public static string[] SplitFixedLength(string value, char[] delims, int len)
        {
            if (value == null) return new string[len];

            string[] arr = value.Split(delims);
            if (arr.Length != len) Array.Resize(ref arr, len);
            return arr;
        }

        public static string EnsureLength(string sval, int len, bool bPadWhiteSpace = false, Alignment eAlign = Alignment.Left)
        {
            if (sval.Length > len) sval = sval.Substring(0, len);

            if (bPadWhiteSpace) sval = PadWithChar(sval, len, eAlign, ' ');

            return sval;
        }

        public static string EnsureLength(string sval, int len, char cPadChar, Alignment eAlign = Alignment.Left)
        {
            if (sval.Length > len) sval = sval.Substring(0, len);

            sval = PadWithChar(sval, len, eAlign, cPadChar);

            return sval;
        }


        public static string PadWithChar(string sString,
                                              int iLength,
                                              Alignment eAlign = 0,
                                              char sChar = ' ')
        {
            if (sChar == '\0') return null;

            switch (eAlign)
            {
                case Alignment.Right:
                    return new String(sChar, (int)Math.Max(0, iLength - sString.Length)) + sString;
                case Alignment.Center:
                    iLength = Math.Max(0, iLength - sString.Length);
                    var sr = new String(sChar, (int)(Math.Ceiling(iLength / 2.0f))); // if odd, pad more on the right
                    var sl = new String(sChar, (int)(Math.Floor(iLength / 2.0f)));
                    return sl + sString + sr;
                case Alignment.Left:
                    return sString + new String(sChar, (int)Math.Max(0, iLength - sString.Length));
            }

            //default trap
            return sString;
        }

        public static string PadWithChar(string sString,
                                          int iLength,
                                          char sAlign,
                                          char sChar = ' ')
        {
            switch (Char.ToUpper(sAlign))
            {
                case 'L':
                    return PadWithChar(sString, iLength, Alignment.Left, sChar);
                case 'C':
                    return PadWithChar(sString, iLength, Alignment.Center, sChar);
                case 'R':
                    return PadWithChar(sString, iLength, Alignment.Right, sChar);

            }

            return null;
        }

        #endregion


        #region Special Formatting

        private class LoDExtendedFormatter : ICustomFormatter, IFormatProvider
        {

            #region ICustomFormatter Interface

            public string Format(string formatString, object arg, IFormatProvider formatProvider)
            {
                const string sREGX = @"^((p-?\d+)|(str)|(string)|(num\?)|(float)|(single)|(dbl)|(double)|(num)|(dec)|(cash)|(int)|(bool)|(ascii)|(char)|(date)|(time)):";

                try
                {
                    if (System.String.IsNullOrEmpty(formatString)) return Convert.ToString(arg);

                    var m = Regex.Match(formatString, sREGX, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        string sType = m.Value.Substring(0, m.Value.Length - 1);
                        formatString = formatString.Substring(m.Length);
                        if (formatString.EndsWith("??"))
                        {
                            if (arg == null || (arg is string && StringUtil.IsNullOrWhitespace(arg as string)))
                                return "";
                            else
                                formatString = formatString.Substring(0, formatString.Length - 2);
                        }

                        arg = ConvertUtil.LoDConvertTo(arg, sType);
                    }
                    else
                    {
                        if (formatString.EndsWith("??"))
                        {
                            if (arg == null || (arg is string && StringUtil.IsNullOrWhitespace(arg as string)))
                                return "";
                            else
                                formatString = formatString.Substring(0, formatString.Length - 2);
                        }
                    }

                    if (arg is bool && formatString.Contains(";"))
                    {
                        //bool
                        string[] arr = StringUtil.Split(formatString, ";");
                        return (ConvertUtil.ToBool(arg)) ? arr[0] : arr[1];
                    }
                    else if (arg is string)
                    {
                        //string
                        string sval = arg as String;
                        if (Regex.Match(formatString, @"^\<\d+$").Success)
                        {
                            int len = int.Parse(formatString.Substring(1));
                            return StringUtil.EnsureLength(sval, len, true, Alignment.Left);
                        }
                        else if (Regex.Match(formatString, @"^\>\d+$").Success)
                        {
                            int len = int.Parse(formatString.Substring(1));
                            return StringUtil.EnsureLength(sval, len, true, Alignment.Right);
                        }
                        else if (Regex.Match(formatString, @"^\^\d+$").Success)
                        {
                            int len = int.Parse(formatString.Substring(1));
                            return StringUtil.EnsureLength(sval, len, true, Alignment.Center);
                        }
                        else
                        {
                            return System.String.Format("{0:" + formatString + "}", sval);
                        }
                    }
                    else if (arg is TimeSpan && formatString.StartsWith("-?"))
                    {
                        //timespan with neg
                        formatString = formatString.Substring(2);
                        if ((TimeSpan)arg < TimeSpan.Zero)
                            return "-" + System.String.Format("{0:" + formatString + "}", arg);
                        else
                            return System.String.Format("{0:" + formatString + "}", arg);
                    }
                    else if (arg != null)
                    {
                        //all else
                        return System.String.Format("{0:" + formatString + "}", arg);
                    }
                    else
                    {
                        return "";
                    }
                }
                catch
                {

                }

                return "";
            }

            #endregion

            #region FormatPrivider Interface

            public object GetFormat(Type formatType)
            {
                return this;
            }

            #endregion

        }

        private static LoDExtendedFormatter _cachedFormatter = new LoDExtendedFormatter();

        public static string Format(string sFormatString, params object[] oParams)
        {
            try
            {
                return String.Format(_cachedFormatter, sFormatString, oParams);
            }
            catch
            {
                throw new ArgumentException("format string syntax error");
            }
        }

        public static string FormatValue(object obj, string sFormat)
        {
            return String.Format(_cachedFormatter, "{0:" + sFormat + "}", obj);
        }

        public static string[] FormatValues(object[] objs, string sFormat)
        {
            if (objs == null) return null;
            if (objs.Length == 0) return new String[] { };

            string[] arr = new string[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                arr[i] = String.Format(_cachedFormatter, "{0:" + sFormat + "}", objs[i]);
            }
            return arr;
        }

        #endregion


        #region Replace Chars

        //####################
        //EnsureNotStartWith

        public static string EnsureNotStartWith(this string value, string start)
        {
            if (value.StartsWith(start)) return value.Substring(start.Length);
            else return value;
        }

        public static string EnsureNotStartWith(this string value, string start, bool ignoreCase)
        {
            if (value.StartsWith(start, StringComparison.OrdinalIgnoreCase)) return value.Substring(start.Length);
            else return value;
        }

        public static string EnsureNotStartWith(this string value, string start, bool ignoreCase, System.Globalization.CultureInfo culture)
        {
            if (value.StartsWith(start, ignoreCase, culture)) return value.Substring(start.Length);
            else return value;
        }

        public static string EnsureNotStartWith(this string value, string start, StringComparison comparison)
        {
            if (value.StartsWith(start, comparison)) return value.Substring(start.Length);
            else return value;
        }


        //####################
        //EnsureNotStartWith

        public static string EnsureNotEndsWith(this string value, string end)
        {
            if (value.EndsWith(end)) return value.Substring(0, value.Length - end.Length);
            else return value;
        }

        public static string EnsureNotEndsWith(this string value, string end, bool ignoreCase)
        {
            if (value.EndsWith(end, StringComparison.OrdinalIgnoreCase)) return value.Substring(0, value.Length - end.Length);
            else return value;
        }

        public static string EnsureNotEndsWith(this string value, string end, bool ignoreCase, System.Globalization.CultureInfo culture)
        {
            if (value.EndsWith(end, ignoreCase, culture)) return value.Substring(0, value.Length - end.Length);
            else return value;
        }

        public static string EnsureNotEndsWith(this string value, string end, StringComparison comparison)
        {
            if (value.EndsWith(end, comparison)) return value.Substring(0, value.Length - end.Length);
            else return value;
        }

        #endregion





        #region StringBuilders

        private static ObjectCachePool<StringBuilder> _pool = new ObjectCachePool<StringBuilder>(10, () => new StringBuilder());
        
        public static StringBuilder GetTempStringBuilder()
        {
            return _pool.GetInstance();
        }

        public static string Release(StringBuilder b)
        {
            if (b == null) return null;

            var result = b.ToString();
            b.Length = 0;
            _pool.Release(b);
            return result;
        }
        
        public static string ToStringHax(this StringBuilder sb)
        {
            var info = typeof(StringBuilder).GetField("_str",
                                                        System.Reflection.BindingFlags.NonPublic |
                                                        System.Reflection.BindingFlags.Instance);
            if (info == null)
                return sb.ToString();
            else
                return info.GetValue(sb) as string;

        }

        #endregion

    }
}

