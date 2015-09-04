using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;
using com.spacepuppy.Dynamic.Accessors;

namespace com.spacepuppy.Dynamic
{

    public interface IDynamic
    {
        object this[string sMemberName] { get; set; }

        bool SetValue(string sMemberName, object value, params object[] index);
        object GetValue(string sMemberName, params object[] args);
        object InvokeMethod(string sMemberName, params object[] args);

        bool HasMember(string sMemberName, bool includeNonPublic);
        IEnumerable<MemberInfo> GetMembers(bool includeNonPublic);
        MemberInfo GetMember(string sMemberName, bool includeNonPublic);

    }



    public static class DynamicUtil
    {


        #region IDynamic Methods

        public static bool SetValue(this object obj, string sprop, object value)
        {
            if (obj == null) return false;

            if (obj is IDynamic)
            {
                try
                {
                    return (obj as IDynamic).SetValue(sprop, value, (object[])null);
                }
                catch
                {

                }
            }
            else
            {
                return SetValueDirect(obj, sprop, value, (object[])null);
            }

            return false;
        }

        public static bool SetValue(this object obj, string sprop, object value, params object[] index)
        {
            if (obj == null) return false;

            if (obj is IDynamic)
            {
                try
                {
                    return (obj as IDynamic).SetValue(sprop, value, index);
                }
                catch
                {

                }
            }
            else
            {
                return SetValueDirect(obj, sprop, value, index);
            }

            return false;
        }

        public static object GetValue(this object obj, string sprop, params object[] args)
        {
            if (obj == null) return null;

            if (obj is IDynamic)
            {
                try
                {
                    return (obj as IDynamic).GetValue(sprop, args);
                }
                catch
                {

                }
            }
            else
            {
                return GetValueDirect(obj, sprop, args);
            }
            return null;
        }

        public static object InvokeMethod(this object obj, string name, params object[] args)
        {
            if (obj == null) return false;

            if(obj is IDynamic)
            {
                try
                {
                    return (obj as IDynamic).InvokeMethod(name, args);
                }
                catch
                {

                }
            }
            else
            {
                return InvokeMethodDirect(obj, name, args);
            }

            return null;
        }

        public static bool HasMember(object obj, string name, bool includeNonPublic)
        {
            if (obj == null) return false;

            if (obj is IDynamic)
            {
                return (obj as IDynamic).HasMember(name, includeNonPublic);
            }
            else
            {
                return HasMember(obj.GetType(), name, includeNonPublic);
            }
        }

        public static IEnumerable<MemberInfo> GetMembers(object obj, bool includeNonPublic)
        {
            if (obj == null) return Enumerable.Empty<MemberInfo>();

            if (obj is IDynamic)
            {
                return (obj as IDynamic).GetMembers(includeNonPublic);
            }
            else
            {
                return GetMembers(obj.GetType(), includeNonPublic);
            }
        }

        public static MemberInfo GetMember(object obj, string sMemberName, bool includeNonPublic)
        {
            if(obj == null) return null;

            if(obj is IDynamic)
            {
                return (obj as IDynamic).GetMember(sMemberName, includeNonPublic);
            }
            else
            {
                return GetMember(obj.GetType(), sMemberName, includeNonPublic);
            }
        }

        #endregion

        #region Direct Reflection

        public static bool SetValueDirect(object obj, string sprop, object value)
        {
            return SetValueDirect(obj, sprop, value, (object[])null);
        }

        public static bool SetValueDirect(object obj, string sprop, object value, params object[] index)
        {
            /*
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
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
                                    prop.SetValue(obj, value, index);
                                    return true;
                                }
                                break;
                            case System.Reflection.MemberTypes.Method:
                                {
                                    var meth = member as System.Reflection.MethodInfo;
                                    var paramInfos = meth.GetParameters();
                                    var arr = new object[] { value };
                                    if (DynamicUtil.ParameterSignatureMatches(arr, paramInfos, false))
                                    {
                                        meth.Invoke(obj, arr);
                                        return true;
                                    }
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
                                {
                                    var meth = member as System.Reflection.MethodInfo;
                                    var paramInfos = meth.GetParameters();
                                    var arr = new object[] { value };
                                    if (DynamicUtil.ParameterSignatureMatches(arr, paramInfos, true))
                                    {
                                        meth.Invoke(obj, arr);
                                        return true;
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

            return false;
             */

            if (obj == null) return false;
            var vtp = (value != null) ? value.GetType() : null;
            var member = GetValueSetterMember(obj.GetType(), sprop, vtp, false);
            if (member == null) return false;

            try
            {
                switch(member.MemberType)
                {
                    case MemberTypes.Field:
                        (member as FieldInfo).SetValue(obj, value);
                        return true;
                    case MemberTypes.Property:
                        (member as PropertyInfo).SetValue(obj, value, index);
                        return true;
                    case MemberTypes.Method:
                        var arr = ArrayUtil.Temp(value);
                        (member as MethodInfo).Invoke(obj, arr);
                        ArrayUtil.ReleaseTemp(arr);
                        return true;
                }
            }
            catch
            {

            }

            return false;
        }

        public static object GetValueDirect(object obj, string sprop, params object[] args)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
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
                                {
                                    var prop = member as System.Reflection.PropertyInfo;
                                    var paramInfos = prop.GetIndexParameters();
                                    if (prop.CanRead && DynamicUtil.ParameterSignatureMatches(args, paramInfos, false))
                                    {
                                        return prop.GetValue(obj, args);
                                    }
                                    break;
                                }
                            case System.Reflection.MemberTypes.Method:
                                {
                                    var meth = member as System.Reflection.MethodInfo;
                                    var paramInfos = meth.GetParameters();
                                    if (DynamicUtil.ParameterSignatureMatches(args, paramInfos, false))
                                    {
                                        return meth.Invoke(obj, args);
                                    }
                                    break;
                                }
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

        public static object InvokeMethodDirect(object obj, string name, params object[] args)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                         BindingFlags.InvokeMethod | BindingFlags.OptionalParamBinding;
            if (obj == null) return false;

            var tp = obj.GetType();
            try
            {
                return tp.InvokeMember(name, BINDING, null, obj, args);
            }
            catch
            {
                return null;
            }
        }

        public static bool HasMemberDirect(object obj, string name, bool includeNonPublic)
        {
            if (obj == null) return false;

            return HasMember(obj.GetType(), name, includeNonPublic);
        }

        public static IEnumerable<MemberInfo> GetMembersDirect(object obj, bool includeNonPublic)
        {
            if (obj == null) return Enumerable.Empty<MemberInfo>();

            return GetMembers(obj.GetType(), includeNonPublic);
        }





        public static bool HasMember(System.Type tp, string name, bool includeNonPublic)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags PRIV_BINDING = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            if (tp == null) return false;

            if (tp.GetMember(name, BINDING) != null) return true;

            if(includeNonPublic)
            {
                while (tp != null)
                {
                    if (tp.GetMember(name, PRIV_BINDING) != null) return true;
                    tp = tp.BaseType;
                }
            }
            return false;
        }

        public static IEnumerable<MemberInfo> GetMembers(System.Type tp, bool includeNonPublic)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags PRIV_BINDING = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            const MemberTypes MASK = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;
            if (tp == null) yield break;

            foreach (var m in tp.GetMembers(BINDING))
            {
                if ((m.MemberType & MASK) != 0)
                {
                    yield return m;
                }
            }

            if (includeNonPublic)
            {
                while (tp != null)
                {
                    foreach (var m in tp.GetMembers(PRIV_BINDING))
                    {
                        if ((m.MemberType & MASK) != 0)
                        {
                            yield return m;
                        }
                    }
                    tp = tp.BaseType;
                }
            }
        }

        public static MemberInfo GetMember(Type tp, string sMemberName, bool includeNonPublic)
        {
            const BindingFlags BINDING_PUBLIC = BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags BINDING_NONPUBLIC = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            try
            {
                while (tp != null)
                {
                    var members = tp.GetMember(sMemberName, (includeNonPublic) ? BINDING_NONPUBLIC : BINDING_PUBLIC);
                    if (members == null || members.Length == 0) return null;

                    foreach (var member in members)
                    {
                        switch (member.MemberType)
                        {
                            case System.Reflection.MemberTypes.Field:
                                return member;

                            case System.Reflection.MemberTypes.Property:
                                {
                                    var prop = member as System.Reflection.PropertyInfo;
                                    if (prop.CanRead && prop.GetIndexParameters().Length == 0) return prop;
                                    break;
                                }
                            case System.Reflection.MemberTypes.Method:
                                {
                                    var meth = member as System.Reflection.MethodInfo;
                                    if (meth.GetParameters().Length == 0) return meth;
                                    break;
                                }
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

        public static MemberInfo GetValueSetterMember(Type tp, string sprop, Type valueType, bool includeNonPublic)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            if (tp == null) throw new ArgumentNullException("tp");

            try
            {
                while (tp != null)
                {
                    var members = tp.GetMember(sprop, BINDING);
                    if (members == null || members.Length == 0) return null;

                    if (valueType != null)
                    {
                        //first strict test
                        foreach (var member in members)
                        {
                            switch (member.MemberType)
                            {
                                case System.Reflection.MemberTypes.Field:
                                    var field = member as System.Reflection.FieldInfo;
                                    if (valueType == null || field.FieldType == valueType)
                                    {
                                        return field;
                                    }

                                    break;
                                case System.Reflection.MemberTypes.Property:
                                    var prop = member as System.Reflection.PropertyInfo;
                                    if (prop.CanWrite && (valueType == null || prop.PropertyType == valueType) && prop.GetIndexParameters().Length == 0)
                                    {
                                        return prop;
                                    }
                                    break;
                                case System.Reflection.MemberTypes.Method:
                                    {
                                        var meth = member as System.Reflection.MethodInfo;
                                        var paramInfos = meth.GetParameters();
                                        if (paramInfos.Length == 1 && paramInfos[0].ParameterType.IsAssignableFrom(valueType)) return meth;
                                    }
                                    break;
                            }
                        }
                    }

                    //now weak test
                    foreach (var member in members)
                    {
                        switch (member.MemberType)
                        {
                            case System.Reflection.MemberTypes.Field:
                                return member;
                            case System.Reflection.MemberTypes.Property:
                                var prop = member as System.Reflection.PropertyInfo;
                                if (prop.CanWrite)
                                {
                                    return prop;
                                }
                                break;
                            case System.Reflection.MemberTypes.Method:
                                {
                                    var meth = member as System.Reflection.MethodInfo;
                                    var paramInfos = meth.GetParameters();
                                    if (paramInfos.Length == 1) return meth;
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

        public static MemberInfo GetValueGetterMember(Type tp, string sprop, bool includeNonPublic)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            try
            {
                while (tp != null)
                {
                    var members = tp.GetMember(sprop, BINDING);
                    if (members == null || members.Length == 0) return null;

                    foreach (var member in members)
                    {
                        switch (member.MemberType)
                        {
                            case System.Reflection.MemberTypes.Field:
                                return member;

                            case System.Reflection.MemberTypes.Property:
                                {
                                    var prop = member as System.Reflection.PropertyInfo;
                                    if (prop.CanRead && prop.GetIndexParameters().Length == 0) return prop;
                                    break;
                                }
                            case System.Reflection.MemberTypes.Method:
                                {
                                    var meth = member as System.Reflection.MethodInfo;
                                    if (meth.GetParameters().Length == 0) return meth;
                                    break;
                                }
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

        public static System.Type[] GetParameters(MemberInfo info)
        {
            switch(info.MemberType)
            {
                case MemberTypes.Field:
                    return new System.Type[] { (info as FieldInfo).FieldType };
                case MemberTypes.Property:
                    return new System.Type[] { (info as PropertyInfo).PropertyType };
                case MemberTypes.Method:
                    return (from p in (info as MethodBase).GetParameters() select p.ParameterType).ToArray();
                default:
                    return new System.Type[] {};
            }
        }

        public static Type GetReturnType(MemberInfo info)
        {
            if (info == null) return null;

            switch(info.MemberType)
            {
                case MemberTypes.Field:
                    return (info as FieldInfo).FieldType;
                case MemberTypes.Property:
                    return (info as PropertyInfo).PropertyType;
                case MemberTypes.Method:
                    return (info as MethodInfo).ReturnType;
            }
            return null;
        }

        public static IEnumerable<System.Reflection.MemberInfo> GetEasilySerializedMembers(object obj, MemberTypes mask = MemberTypes.All)
        {
            if (obj == null) yield break;

            var members = com.spacepuppy.Dynamic.DynamicUtil.GetMembers(obj, false);
            foreach (var mi in members)
            {
                if ((mi.MemberType & mask) == 0) continue;

                if (mi.DeclaringType.IsAssignableFrom(typeof(UnityEngine.MonoBehaviour)) ||
                    mi.DeclaringType.IsAssignableFrom(typeof(SPComponent)) ||
                    mi.DeclaringType.IsAssignableFrom(typeof(SPNotifyingComponent))) continue;

                switch (mi.MemberType)
                {
                    case System.Reflection.MemberTypes.Method:
                        {
                            var m = mi as System.Reflection.MethodInfo;
                            if (m.IsSpecialName) continue;
                            if (m.IsGenericMethod) continue;

                            var parr = m.GetParameters();
                            if (parr.Length == 0)
                            {
                                yield return m;
                            }
                            else
                            {
                                bool pass = true;
                                foreach (var p in parr)
                                {
                                    if (!(VariantReference.AcceptableType(p.ParameterType) || p.ParameterType == typeof(object)))
                                    {
                                        pass = false;
                                        break;
                                    }
                                }
                                if (pass) yield return m;
                            }
                        }
                        break;
                    case System.Reflection.MemberTypes.Field:
                        {
                            var f = mi as System.Reflection.FieldInfo;
                            if (f.IsSpecialName) continue;

                            if (VariantReference.AcceptableType(f.FieldType)) yield return f;
                        }
                        break;
                    case System.Reflection.MemberTypes.Property:
                        {
                            var p = mi as System.Reflection.PropertyInfo;
                            if (p.IsSpecialName) continue;
                            if (!p.CanRead || !p.CanWrite) continue;
                            if (p.GetIndexParameters().Length > 0) continue; //indexed properties are not allowed

                            if (VariantReference.AcceptableType(p.PropertyType)) yield return p;
                        }
                        break;
                }

            }
        }

        #endregion




        #region Some Minor Helpers

        private static bool ParameterSignatureMatches(object[] args, ParameterInfo[] paramInfos, bool convertToParamTypeIfCan)
        {
            if (args.Length != paramInfos.Length) return false;

            for (int i = 0; i < paramInfos.Length; i++)
            {
                if (args[i] == null)
                {
                    if (convertToParamTypeIfCan) args[i] = paramInfos[i].ParameterType.GetDefaultValue();
                    continue;
                }
                if (args[i].GetType().IsAssignableFrom(paramInfos[i].ParameterType))
                {
                    continue;
                }
                if (convertToParamTypeIfCan)
                {
                    if(ConvertUtil.IsNumericType(paramInfos[i].ParameterType) && ConvertUtil.IsNumeric(args[i]))
                    {
                        args[i] = ConvertUtil.ToPrim(args[i], paramInfos[i].ParameterType);
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        #endregion

    }

}
