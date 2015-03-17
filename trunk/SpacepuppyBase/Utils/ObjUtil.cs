using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0168 // variable declared but not used.

namespace com.spacepuppy.Utils
{
    public static class ObjUtil
    {

        public static void SmartDestroy(Object obj)
        {
            if(Application.isPlaying)
            {
                Object.Destroy(obj);
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }

        /// <summary>
        /// Returns true if the object is either a null reference or has been destroyed by unity.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrDestroyed(this System.Object obj)
        {
            return ReferenceEquals(obj, null) || obj.Equals(null);
        }

        /// <summary>
        /// Unlike IsNullOrDestroyed, this only returns true if the managed object half of the object still exists, 
        /// but the unmanaged half has been destroyed by unity.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDestroyed(this System.Object obj)
        {
            return !ReferenceEquals(obj, null) && obj.Equals(null);
        }

        public static bool EqualsAny(System.Object obj, params System.Object[] others)
        {
            return System.Array.IndexOf(others, obj) >= 0;
        }




        public static object GetValue(this object obj, string sprop, params object[] args)
        {
            const System.Reflection.BindingFlags BINDING = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            if (obj == null) return null;

            try
            {
                var tp = obj.GetType();

                while (tp != null)
                {
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

                    tp = tp.BaseType;
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

                while (tp != null)
                {
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

                    tp = tp.BaseType;
                }
            }
            catch
            {

            }

            return false;
        }

        /*
        public static bool CallMethod(this object obj, string name, object optionalArg = null)
        {
            const System.Reflection.BindingFlags BINDING = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
            if (obj == null) return false;

            var tp = obj.GetType();
            System.Reflection.MethodInfo meth;
            bool hasParam = false;

            if(optionalArg != null)
            {
                hasParam = true;
                var argTp = optionalArg.GetType();
                try
                {
                    meth = tp.GetMethod(name, BINDING, null, new System.Type[] { argTp }, null);
                    if (meth != null) goto DoCallMethod;
                }
                catch
                {
                    meth = null;
                }

                foreach(var m in tp.GetMethods(BINDING))
                {
                    if(m.Name == name)
                    {
                        var parr = m.GetParameters();
                        if (parr.Length != 1) continue;

                        if(TypeUtil.IsType(argTp, parr[0].ParameterType))
                        {
                            meth = m;
                            goto DoCallMethod;
                        }
                    }
                }

                return false;
            }
            else
            {
                try
                {
                    meth = tp.GetMethod(name, BINDING, null, System.Type.EmptyTypes, null);
                    if (meth != null) goto DoCallMethod;
                }
                catch
                {
                    meth = null;
                }

                foreach (var m in tp.GetMethods(BINDING))
                {
                    if (m.Name == name)
                    {
                        var parr = m.GetParameters();
                        if (parr.Length != 1) continue;

                        meth = m;
                        goto DoCallMethod;
                    }
                }

                return false;
            }
            

        DoCallMethod:
            if(hasParam)
            {
                meth.Invoke(obj, new object[] { optionalArg });
            }
            else
            {
                meth.Invoke(obj, null);
            }
            return true;
        }
         */

        public static bool CallMethod(this object obj, string name, params object[] args)
        {
            const System.Reflection.BindingFlags BINDING = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                                                           System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.OptionalParamBinding;
            if (obj == null) return false;

            var tp = obj.GetType();
            try
            {
                tp.InvokeMember(name, BINDING, null, obj, args);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public static T ExtractDelegate<T>(object obj, string name, bool ignoreCase = false, bool throwOnBindFailure = false) where T : class
        {
            if (obj == null) throw new System.ArgumentNullException("obj");
            var delegateType = typeof(T);
            if (!delegateType.IsSubclassOf(typeof(System.Delegate))) throw new TypeArgumentMismatchException(delegateType, typeof(System.Delegate), "T");

            return System.Delegate.CreateDelegate(delegateType, obj, name, ignoreCase, throwOnBindFailure) as T;
        }

        public static System.Delegate ExtractDelegate(System.Type delegateType, object obj, string name, bool ignoreCase = false, bool throwOnBindFailure = false)
        {
            if (delegateType == null) throw new System.ArgumentNullException("delegateType");
            if (obj == null) throw new System.ArgumentNullException("obj");
            if (!delegateType.IsSubclassOf(typeof(System.Delegate))) throw new TypeArgumentMismatchException(delegateType, typeof(System.Delegate), "delegateType");

            return System.Delegate.CreateDelegate(delegateType, obj, name, ignoreCase, throwOnBindFailure);
        }


    }
}
