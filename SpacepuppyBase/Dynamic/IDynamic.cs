using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic
{

    public interface IDynamic
    {
        object this[string sMemberName] { get; set; }

        bool SetValue(string sMemberName, object value, params object[] index);
        object GetValue(string sMemberName, params object[] args);
        bool TryGetValue(string sMemberName, out object result, params object[] args);
        object InvokeMethod(string sMemberName, params object[] args);

        bool HasMember(string sMemberName, bool includeNonPublic);
        IEnumerable<MemberInfo> GetMembers(bool includeNonPublic);
        MemberInfo GetMember(string sMemberName, bool includeNonPublic);
        
    }
    
    [System.Flags()]
    public enum DynamicMemberAccess
    {
        Read = 1,
        Write = 2,
        ReadWrite = 3
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

        public static bool TryGetValue(this object obj, string sMemberName, out object result, params object[] args)
        {
            if(obj == null)
            {
                result = null;
                return false;
            }

            if(obj is IDynamic)
            {
                try
                {
                    return (obj as IDynamic).TryGetValue(sMemberName, out result, args);
                }
                catch
                {
                    result = null;
                    return false;
                }
            }
            else
            {
                return TryGetValueDirect(obj, sMemberName, out result, args);
            }
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
                return TypeHasMember(obj.GetType(), name, includeNonPublic);
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
                return GetMembersFromType(obj.GetType(), includeNonPublic);
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
                return GetMemberFromType(obj.GetType(), sMemberName, includeNonPublic);
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
            if (obj == null) return false;
            var vtp = (value != null) ? value.GetType() : null;
            var member = GetValueSetterMemberFromType(obj.GetType(), sprop, vtp, false);
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

        public static bool TryGetValueDirect(object obj, string sprop, out object result, params object[] args)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            result = null;
            if (obj == null) return false;

            try
            {
                var tp = obj.GetType();

                while (tp != null)
                {
                    var members = tp.GetMember(sprop, BINDING);
                    if (members == null || members.Length == 0) return false;

                    foreach (var member in members)
                    {
                        switch (member.MemberType)
                        {
                            case System.Reflection.MemberTypes.Field:
                                var field = member as System.Reflection.FieldInfo;
                                result = field.GetValue(obj);
                                return true;

                            case System.Reflection.MemberTypes.Property:
                                {
                                    var prop = member as System.Reflection.PropertyInfo;
                                    var paramInfos = prop.GetIndexParameters();
                                    if (prop.CanRead && DynamicUtil.ParameterSignatureMatches(args, paramInfos, false))
                                    {
                                        result = prop.GetValue(obj, args);
                                        return true;
                                    }
                                    break;
                                }
                            case System.Reflection.MemberTypes.Method:
                                {
                                    var meth = member as System.Reflection.MethodInfo;
                                    var paramInfos = meth.GetParameters();
                                    if (DynamicUtil.ParameterSignatureMatches(args, paramInfos, false))
                                    {
                                        result = meth.Invoke(obj, args);
                                        return true;
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
            return false;
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

            return TypeHasMember(obj.GetType(), name, includeNonPublic);
        }

        public static IEnumerable<MemberInfo> GetMembersDirect(object obj, bool includeNonPublic)
        {
            if (obj == null) return Enumerable.Empty<MemberInfo>();

            return GetMembersFromType(obj.GetType(), includeNonPublic);
        }





        public static bool TypeHasMember(System.Type tp, string name, bool includeNonPublic)
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

        public static IEnumerable<MemberInfo> GetMembersFromType(System.Type tp, bool includeNonPublic)
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

        public static MemberInfo GetMemberFromType(Type tp, string sMemberName, bool includeNonPublic)
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

        public static MemberInfo GetValueSetterMemberFromType(Type tp, string sprop, Type valueType, bool includeNonPublic)
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

        public static MemberInfo GetValueGetterMemberFromType(Type tp, string sprop, bool includeNonPublic)
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
                    {
                        var paramInfos = (info as MethodBase).GetParameters();
                        Type[] arr = new Type[paramInfos.Length];
                        for(int i = 0; i < arr.Length; i++)
                        {
                            arr[i] = paramInfos[i].ParameterType;
                        }
                        return arr;
                    }
                default:
                    return new System.Type[] {};
            }
        }

        public static Type GetReturnType(MemberInfo info)
        {
            if (info == null) return null;

            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    return (info as FieldInfo).FieldType;
                case MemberTypes.Property:
                    return (info as PropertyInfo).PropertyType;
                case MemberTypes.Method:
                    return (info as MethodInfo).ReturnType;
            }
            return null;

            //if (info is IDynamicMemberInfo)
            //    return (info as IDynamicMemberInfo).ReturnType;
            //else if (info is PropertyInfo)
            //    return (info as PropertyInfo).PropertyType;
            //else if (info is FieldInfo)
            //    return (info as FieldInfo).FieldType;
            //else if (info is MethodInfo)
            //    return (info as MethodInfo).ReturnType;
            //else
            //    return null;
        }

        public static object GetValueWithMember(MemberInfo info, object targObj)
        {
            if (info == null) return null;

            try
            {
                switch (info.MemberType)
                {
                    case MemberTypes.Field:
                        return (info as FieldInfo).GetValue(targObj);
                    case MemberTypes.Property:
                        return (info as PropertyInfo).GetValue(targObj, null);
                    case MemberTypes.Method:
                        return (info as MethodInfo).Invoke(targObj, null);
                }
            }
            catch
            {

            }
            
            return null;
        }

        public static IEnumerable<System.Reflection.MemberInfo> GetEasilySerializedMembers(object obj, MemberTypes mask = MemberTypes.All, DynamicMemberAccess access = DynamicMemberAccess.ReadWrite)
        {
            if (obj == null) yield break;

            bool bRead = access.HasFlag(DynamicMemberAccess.Read);
            bool bWrite = access.HasFlag(DynamicMemberAccess.Write);
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
                            if (!p.CanRead && bRead) continue;
                            if (!p.CanWrite && bWrite) continue;
                            if (p.GetIndexParameters().Length > 0) continue; //indexed properties are not allowed

                            if (VariantReference.AcceptableType(p.PropertyType)) yield return p;
                        }
                        break;
                }

            }
        }

        public static IEnumerable<System.Reflection.MemberInfo> GetEasilySerializedMembersFromType(System.Type tp, MemberTypes mask = MemberTypes.All, DynamicMemberAccess access = DynamicMemberAccess.ReadWrite)
        {
            if (tp == null) yield break;

            bool bRead = access.HasFlag(DynamicMemberAccess.Read);
            bool bWrite = access.HasFlag(DynamicMemberAccess.Write);
            var members = com.spacepuppy.Dynamic.DynamicUtil.GetMembersFromType(tp, false);
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
                            if (!p.CanRead && bRead) continue;
                            if (!p.CanWrite && bWrite) continue;
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
                    //if (ConvertUtil.IsNumericType(paramInfos[i].ParameterType) && ConvertUtil.IsNumeric(args[i]))
                    //{
                    //    args[i] = ConvertUtil.ToPrim(args[i], paramInfos[i].ParameterType);
                    //    continue;
                    //}
                    if (ConvertUtil.IsNumericType(paramInfos[i].ParameterType) && args[i] is IConvertible)
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
