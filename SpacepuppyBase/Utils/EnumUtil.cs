using System.Collections.Generic;

namespace com.spacepuppy.Utils
{
    public static class EnumUtil
    {

        public static bool EnumValueIsDefined(object value, System.Type enumType)
        {
            try
            {
                return System.Enum.IsDefined(enumType, value);
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public static bool EnumValueIsDefined<T>(object value)
        {
            return EnumValueIsDefined(value, typeof(T));
        }

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

        public static bool HasFlag(this System.Enum e, System.Enum value)
        {
            return (System.Convert.ToInt64(e) & System.Convert.ToInt64(value)) != 0;
        }

        public static bool HasFlag(this System.Enum e, ulong value)
        {
            return (System.Convert.ToUInt64(e) & value) != 0;
        }

        public static bool HasFlag(this System.Enum e, long value)
        {
            return (System.Convert.ToInt64(e) & value) != 0;
        }

    }
}
