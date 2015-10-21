using System.Reflection;

namespace com.spacepuppy.Dynamic.Accessors
{
    public class BasicMemberAccessor : IMemberAccessor
    {

        #region Fields

        private MemberInfo _memberInfo;

        #endregion

        #region CONSTRUCTOR

        public BasicMemberAccessor(MemberInfo info)
        {
            _memberInfo = info;
        }

        #endregion

        #region IMemberAccessor Interface

        public object Get(object target)
        {
            if (_memberInfo is PropertyInfo)
            {
                return (_memberInfo as PropertyInfo).GetValue(target, null);
            }
            else if (_memberInfo is FieldInfo)
            {
                return (_memberInfo as FieldInfo).GetValue(target);
            }

            return null;
        }

        public void Set(object target, object value)
        {
            if (_memberInfo is PropertyInfo)
            {
                try
                {
                    (_memberInfo as PropertyInfo).SetValue(target, value, null);
                }
                catch (System.InvalidCastException)
                {
                    // This happens only if a float is being assigned to an int.
                    (_memberInfo as PropertyInfo).SetValue(target, (int)System.Math.Floor((double)(float)value), null);
                }
                catch (System.ArgumentException)
                {
                    // This happens only on iOS if a float is being assigned to an int.
                    (_memberInfo as PropertyInfo).SetValue(target, (int)System.Math.Floor((double)(float)value), null);
                }
            }
            else if (_memberInfo is FieldInfo)
            {
                try
                {
                    (_memberInfo as FieldInfo).SetValue(target, value);
                }
                catch (System.InvalidCastException)
                {
                    // This happens only if a float is being assigned to an int.
                    (_memberInfo as FieldInfo).SetValue(target, (int)System.Math.Floor((double)(float)value));
                }
                catch (System.ArgumentException)
                {
                    // This happens only on iOS if a float is being assigned to an int.
                    (_memberInfo as FieldInfo).SetValue(target, (int)System.Math.Floor((double)(float)value));
                }
            }
        }

        #endregion

    }
}
