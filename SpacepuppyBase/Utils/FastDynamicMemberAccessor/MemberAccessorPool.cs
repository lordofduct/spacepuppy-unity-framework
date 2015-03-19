using System;
using System.Collections.Generic;
using System.Reflection;

namespace com.spacepuppy.Utils.FastDynamicMemberAccessor
{
    public static class MemberAccessorPool
    {

        static MemberAccessorPool()
        {
            _isAOT = (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer);
        }

        private static bool _isAOT;
        private static Dictionary<MemberInfo, IMemberAccessor> _pool;

        public static IMemberAccessor Get(MemberInfo memberInfo)
        {
            if (memberInfo == null) throw new System.ArgumentNullException("memberInfo");

            if (_pool != null && _pool.ContainsKey(memberInfo))
            {
                return _pool[memberInfo];
            }

            if (_pool == null) _pool = new Dictionary<MemberInfo, IMemberAccessor>();

            IMemberAccessor result;
            if(typeof(UnityEngine.Transform).IsAssignableFrom(memberInfo.DeclaringType))
            {
                result = FastTransformMemberAccessor.Create(memberInfo, _isAOT);
            }
            else if (_isAOT)
            {
                if (memberInfo is PropertyInfo || memberInfo is FieldInfo)
                {
                    result = new BasicMemberAccessor(memberInfo);
                }
                else
                {
                    throw new System.ArgumentException("MemberInfo must be either a PropertyInfo or a FieldInfo.");
                }
            }
            else
            {
                if (memberInfo is PropertyInfo)
                {
                    result = new PropertyAccessor(memberInfo as PropertyInfo);
                }
                else if (memberInfo is FieldInfo)
                {
                    result = new FieldAccessor(memberInfo as FieldInfo);
                }
                else
                {
                    throw new System.ArgumentException("MemberInfo must be either a PropertyInfo or a FieldInfo.");
                }
            }

            _pool.Add(memberInfo, result);
            return result;
        }

        public static IMemberAccessor Get(object target, string memberName)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            var memberInfo = GetMember(target.GetType(), memberName);
            return Get(memberInfo);
        }

        public static IMemberAccessor Get(Type objectType, string memberName)
        {
            MemberInfo memberInfo;
            System.Type memberType;
            GetMember(objectType, memberName, out memberInfo, out memberType);
            return Get(memberInfo);
        }

        public static MemberInfo GetMember(Type objectType, string memberName)
        {
            MemberInfo[] matches = objectType.GetMember(memberName,
                                                        MemberTypes.Field | MemberTypes.Property,
                                                        BindingFlags.Public | BindingFlags.Instance);

            if ((matches == null) || (matches.Length == 0))
                throw new MemberAccessorException(string.Format("Member \"{0}\" does not exist for type {1}.", memberName, objectType));

            return matches[0];
        }

        public static void GetMember(Type objectType, string memberName, out MemberInfo memberInfo, out System.Type memberType)
        {
            memberInfo = GetMember(objectType, memberName);
            memberType = null;
            if (memberInfo is PropertyInfo)
                memberType = (memberInfo as PropertyInfo).PropertyType;
            else if (memberInfo is FieldInfo)
                memberType = (memberInfo as FieldInfo).FieldType;
        }

    }
}
