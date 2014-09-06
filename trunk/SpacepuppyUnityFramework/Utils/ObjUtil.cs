using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0168 // variable declared but not used.

namespace com.spacepuppy.Utils
{
    public static class ObjUtil
    {

        public static bool IsNullOrDestroyed(this System.Object obj)
        {
            return ReferenceEquals(obj, null) || obj.Equals(null);
        }

        public static bool EqualsAny(System.Object obj, params System.Object[] others)
        {
            return System.Array.IndexOf(others, obj) >= 0;
        }

        #region Enum Methods

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

        #endregion

        public static System.Type[] GetTypesAssignableFrom(System.Reflection.Assembly assemb, System.Type rootType)
        {
            var lst = new List<System.Type>();

            foreach (var tp in assemb.GetTypes())
            {
                if (rootType.IsAssignableFrom(tp) && rootType != tp) lst.Add(tp);
            }

            return lst.ToArray();
        }

        public static bool IsType(System.Type tp, System.Type assignableType)
        {
            return assignableType.IsAssignableFrom(tp);
        }

        public static bool IsType(System.Type tp, params System.Type[] assignableTypes)
        {
            foreach (var otp in assignableTypes)
            {
                if (otp.IsAssignableFrom(tp)) return true;
            }

            return false;
        }

        public static object GetValue(this object obj, string sprop, params object[] args)
        {
            const System.Reflection.BindingFlags BINDING = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            if (obj == null) return null;

            try
            {
                var tp = obj.GetType();
                var members = tp.GetMember(sprop, BINDING);
                if (members == null || members.Length == 0) return null;

                foreach (var member in members)
                {
                    switch (member.MemberType)
                    {
                        case System.Reflection.MemberTypes.Field:
                            var field = member as System.Reflection.FieldInfo;
                            return field.GetValue(obj);

                        case System.Reflection.MemberTypes.Property:
                            var prop = member as System.Reflection.PropertyInfo;
                            if (prop.CanRead && prop.GetIndexParameters().Length == args.Length)
                            {
                                if (args.Length > 0)
                                {
                                    //TODO - check types... but need to allow downcasting... how?
                                    return prop.GetValue(obj, args);
                                }
                                else
                                {
                                    return prop.GetValue(obj, null);
                                }
                            }
                            break;

                        case System.Reflection.MemberTypes.Method:
                            var meth = member as System.Reflection.MethodInfo;
                            if (meth.GetParameters().Length == args.Length)
                            {
                                if (args.Length > 0)
                                {
                                    //TODO - check types... but need to allow downcasting... how?
                                    return meth.Invoke(obj, args);
                                }
                                else
                                {
                                    return meth.Invoke(obj, null);
                                }
                            }
                            break;
                    }
                }
            }
            catch
            {

            }

            return null;
        }

        public static bool SetValue(this object obj, string sprop, object value)
        {
            const System.Reflection.BindingFlags BINDING = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            if (obj == null) return false;

            try
            {
                var tp = obj.GetType();

                var members = tp.GetMember(sprop, BINDING);
                if (members == null || members.Length == 0) return false;

                System.Type vtp = (value != null) ? value.GetType() : null;

                //first strict test
                foreach (var member in members)
                {
                    switch (member.MemberType)
                    {
                        case System.Reflection.MemberTypes.Field:
                            var field = member as System.Reflection.FieldInfo;
                            if (vtp == null || field.FieldType == vtp)
                            {
                                field.SetValue(obj, value);
                                return true;
                            }
                            break;
                        case System.Reflection.MemberTypes.Property:
                            var prop = member as System.Reflection.PropertyInfo;
                            if (prop.CanWrite && (vtp == null || prop.PropertyType == vtp) && prop.GetIndexParameters().Length == 0)
                            {
                                prop.SetValue(obj, value, null);
                                return true;
                            }
                            break;
                        case System.Reflection.MemberTypes.Method:
                            var meth = member as System.Reflection.MethodInfo;
                            var args = meth.GetParameters();
                            if (args.Length == 1 && (vtp == null || args[0].ParameterType == vtp))
                            {
                                meth.Invoke(obj, new object[] { value });
                                return true;
                            }
                            break;
                    }
                }

                //now weak test
                foreach (var member in members)
                {
                    switch (member.MemberType)
                    {
                        case System.Reflection.MemberTypes.Field:
                            var field = member as System.Reflection.FieldInfo;
                            field.SetValue(obj, value);
                            return true;
                        case System.Reflection.MemberTypes.Property:
                            var prop = member as System.Reflection.PropertyInfo;
                            if (prop.CanWrite)
                            {
                                prop.SetValue(obj, value, null);
                                return true;
                            }
                            break;
                        case System.Reflection.MemberTypes.Method:
                            var meth = member as System.Reflection.MethodInfo;
                            var args = meth.GetParameters();
                            if (args.Length == 1)
                            {
                                meth.Invoke(obj, new object[] { value });
                                return true;
                            }
                            break;
                    }
                }
            }
            catch
            {

            }

            return false;
        }

        public static T ExtractDelegate<T>(object obj, string name, bool ignoreCase = false, bool throwOnBindFailure = false) where T : class
        {
            if (obj == null) throw new System.ArgumentNullException("obj");
            var delegateType = typeof(T);
            if (!delegateType.IsSubclassOf(typeof(System.Delegate))) throw new System.ArgumentException("type must be a delegate type");

            return System.Delegate.CreateDelegate(delegateType, obj, name, ignoreCase, throwOnBindFailure) as T;
        }

        public static System.Delegate ExtractDelegate(System.Type delegateType, object obj, string name, bool ignoreCase = false, bool throwOnBindFailure = false)
        {
            if (delegateType == null) throw new System.ArgumentNullException("delegateType");
            if (obj == null) throw new System.ArgumentNullException("obj");
            if (!delegateType.IsSubclassOf(typeof(System.Delegate))) throw new System.ArgumentException("type must be a delegate type");

            return System.Delegate.CreateDelegate(delegateType, obj, name, ignoreCase, throwOnBindFailure);
        }


        public static bool IsListType(this System.Type tp)
        {
            if (tp == null) return false;

            if (tp.IsArray) return true;

            var interfaces = tp.GetInterfaces();
            if (interfaces.Contains(typeof(System.Collections.IList)) || interfaces.Contains(typeof(IList<>)))
            {
                return true;
            }

            return false;
        }

        public static System.Type GetElementTypeOfListType(this System.Type tp)
        {
            if (tp == null) return null;

            if (tp.IsArray) return tp.GetElementType();

            var interfaces = tp.GetInterfaces();
            if (interfaces.Contains(typeof(System.Collections.IList)) || interfaces.Contains(typeof(IList<>)))
            {
                if (tp.IsGenericType) return tp.GetGenericArguments()[0];
                else return typeof(object);
            }

            return null;
        }

    }
}
