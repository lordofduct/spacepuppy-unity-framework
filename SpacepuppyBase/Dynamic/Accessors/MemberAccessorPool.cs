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
            switch (UnityEngine.Application.platform)
            {
                case UnityEngine.RuntimePlatform.OSXEditor:
                case UnityEngine.RuntimePlatform.OSXPlayer:
                case UnityEngine.RuntimePlatform.WindowsPlayer:
                case UnityEngine.RuntimePlatform.OSXDashboardPlayer:
                case UnityEngine.RuntimePlatform.WindowsEditor:
                case UnityEngine.RuntimePlatform.LinuxPlayer:
                case UnityEngine.RuntimePlatform.LinuxEditor:
                case UnityEngine.RuntimePlatform.WSAPlayerX86:
                case UnityEngine.RuntimePlatform.WSAPlayerX64:
                case UnityEngine.RuntimePlatform.WSAPlayerARM:
                    _ignoreEmit = false;
                    break;
                default:
                    _ignoreEmit = true;
                    break;
            }
        }

        public static bool UseEmitCompiledMemerAccessorIfSupported = true;
        private static bool _ignoreEmit;
        private static Dictionary<MemberInfo, IMemberAccessor> _pool;
        private static Queue<IMemberAccessor> _chainBuilder = new Queue<IMemberAccessor>();
        private static Dictionary<string, DynamicMemberAccessor> _dynPool;

        public static IMemberAccessor GetAccessor(MemberInfo memberInfo)
        {
            return GetAccessor(memberInfo, _ignoreEmit || !UseEmitCompiledMemerAccessorIfSupported);
        }

        public static IMemberAccessor GetAccessor(MemberInfo memberInfo, bool useBasicMemberAccessor)
        {
            if (memberInfo == null) throw new System.ArgumentNullException("memberInfo");

            if (_pool != null && _pool.ContainsKey(memberInfo))
            {
                return _pool[memberInfo];
            }

            if (_pool == null) _pool = new Dictionary<MemberInfo, IMemberAccessor>(new MemberInfoEqualityComparer());

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

                    objectType = DynamicUtil.GetReturnType(matches[0]);
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

                effectivelyAlteredValueType = DynamicUtil.GetReturnType(matches[0]);
                return GetAccessor(matches[0]);
            }

        }
        
        public static IMemberAccessor GetAccessor(Type objectType, string memberName)
        {
            Type memberType;
            return GetAccessor(objectType, memberName, out memberType);
        }
        
        public static IMemberAccessor GetDynamicAccessor(object target, string memberName, out System.Type memberType)
        {
            memberType = null;
            if (target == null) return null;

            if(!(target is IDynamic)) return GetAccessor(target.GetType(), memberName, out memberType);

            var dyn = target as IDynamic;
            var info = dyn.GetMember(memberName, false);
            if(info == null) return GetAccessor(target.GetType(), memberName, out memberType);

            memberType = DynamicUtil.GetReturnType(info);
            DynamicMemberAccessor accessor;
            if (_dynPool != null && _dynPool.TryGetValue(memberName, out accessor)) return accessor;

            accessor = new DynamicMemberAccessor(memberName);
            if (_dynPool == null) _dynPool = new Dictionary<string, DynamicMemberAccessor>();
            _dynPool[memberName] = accessor;
            return accessor;
        }






        #region Special Types

        private class MemberInfoEqualityComparer : IEqualityComparer<MemberInfo>
        {
            public bool Equals(MemberInfo x, MemberInfo y)
            {
                if (x == null) return y == null;
                if (y == null) return false;

                if (x is IDynamicMemberInfo && y is IDynamicMemberInfo)
                    return x.DeclaringType == y.DeclaringType && x.Name == y.Name && x.MemberType == y.MemberType;
                else
                    return x == y;
            }

            public int GetHashCode(MemberInfo obj)
            {
                if (obj == null) return 0;

                return obj.DeclaringType.GetHashCode() ^ obj.Name.GetHashCode();
            }
        }

        #endregion

    }
}
