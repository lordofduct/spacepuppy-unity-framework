using System;
using System.Collections.Generic;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic.Accessors
{
    public static class MemberAccessorPool
    {

        static MemberAccessorPool()
        {
            _isAOT = (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer);
        }

        private static bool _isAOT;
        private static Dictionary<MemberInfo, IMemberAccessor> _pool;
        private static Queue<IMemberAccessor> _chainBuilder = new Queue<IMemberAccessor>();

        public static IMemberAccessor GetAccessor(MemberInfo memberInfo)
        {
            return GetAccessor(memberInfo, _isAOT);
        }

        public static IMemberAccessor GetAccessor(MemberInfo memberInfo, bool useBasicMemberAccessor)
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
                result = FastTransformMemberAccessor.Create(memberInfo, useBasicMemberAccessor);
            }
            else if (useBasicMemberAccessor)
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

        public static IMemberAccessor GetAccessor(Type objectType, string memberName, out System.Type effectivelyAlteredValueType)
        {
            if (objectType == null) throw new System.ArgumentNullException("objectType");
            const MemberTypes MASK_MEMBERTYPES = MemberTypes.Field | MemberTypes.Property;
            const BindingFlags MASK_BINDINGS = BindingFlags.Public | BindingFlags.Instance;

            if(memberName.Contains('.'))
            {
                var arr = memberName.Split('.');
                _chainBuilder.Clear();
                for(int i = 0; i < arr.Length; i++)
                {
                    var matches = objectType.GetMember(arr[i],
                                                       MASK_MEMBERTYPES,
                                                       MASK_BINDINGS);
                    if (matches == null || matches.Length == 0)
                        throw new MemberAccessorException(string.Format("Member \"{0}\" does not exist for type {1}.", memberName, objectType));

                    objectType = GetMemberType(matches[0]);
                    _chainBuilder.Enqueue(GetAccessor(matches[0], true));
                }

                //the currentObjectType value will be the type effectively being manipulated
                effectivelyAlteredValueType = objectType;
                IMemberAccessor accessor = _chainBuilder.Dequeue();
                while(_chainBuilder.Count > 0)
                {
                    accessor = new ChainingAccessor(_chainBuilder.Dequeue(), accessor);
                }
                return accessor;
            }
            else
            {
                var matches = objectType.GetMember(memberName,
                                                   MASK_MEMBERTYPES,
                                                   MASK_BINDINGS);
                if (matches == null || matches.Length == 0)
                    throw new MemberAccessorException(string.Format("Member \"{0}\" does not exist for type {1}.", memberName, objectType));

                effectivelyAlteredValueType = GetMemberType(matches[0]);
                return GetAccessor(matches[0]);
            }

        }
        
        public static IMemberAccessor GetAccessor(Type objectType, string memberName)
        {
            Type memberType;
            return GetAccessor(objectType, memberName, out memberType);
        }

        private static Type GetMemberType(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo)
                return (memberInfo as PropertyInfo).PropertyType;
            else if (memberInfo is FieldInfo)
                return (memberInfo as FieldInfo).FieldType;
            else
                return null;
        }


    }
}
