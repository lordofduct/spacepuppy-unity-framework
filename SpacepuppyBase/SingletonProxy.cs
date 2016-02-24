#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;

namespace com.spacepuppy
{

    public class SingletonProxy : MonoBehaviour, IDynamic
    {

        #region Fields

        [TypeReference.Config(typeof(ISingleton), allowAbstractClasses = false, allowInterfaces = false, dropDownStyle = TypeDropDownListingStyle.Flat)]
        [SerializeField()]
        private TypeReference _singletonType;
        [SerializeField()]
        private bool _createIfNone;

        #endregion

        #region Properties

        public ISingleton Instance
        {
            get
            {
                if (_singletonType == null || _singletonType.Type == null) return null;

                if(_createIfNone)
                {
                    return Singleton.GetInstance(_singletonType.Type);
                }
                else
                {
                    ISingleton instance;
                    if (Singleton.HasInstance(_singletonType.Type, out instance))
                        return instance;
                    else
                        return null;
                }
            }
        }

        #endregion

        #region IDynamic Interface

        object IDynamic.this[string sMemberName]
        {
            get
            {
                return (this as IDynamic).GetValue(sMemberName);
            }
            set
            {
                (this as IDynamic).SetValue(sMemberName, value);
            }
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            return this.Instance.SetValue(sMemberName, value, index);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return this.Instance.GetValue(sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            return this.Instance.TryGetValue(sMemberName, out result, args);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return this.Instance.InvokeMethod(sMemberName, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return false;

            return DynamicUtil.TypeHasMember(_singletonType.Type, sMemberName, includeNonPublic);
        }

        IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return Enumerable.Empty<System.Reflection.MemberInfo>();
            return DynamicUtil.GetMembersFromType(_singletonType.Type, includeNonPublic);
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return null;
            return DynamicUtil.GetMemberFromType(_singletonType.Type, sMemberName, includeNonPublic);
        }

        #endregion
        
    }

    public abstract class PsuedoSingletonProxy<T> : MonoBehaviour, IDynamic
    {


        #region Properties

        public abstract T ConcreteInstance { get; }

        #endregion

        #region IDynamic Interface

        object IDynamic.this[string sMemberName]
        {
            get
            {
                return (this as IDynamic).GetValue(sMemberName);
            }
            set
            {
                (this as IDynamic).SetValue(sMemberName, value);
            }
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            return this.ConcreteInstance.SetValue(sMemberName, value, index);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return this.ConcreteInstance.GetValue(sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            return this.ConcreteInstance.TryGetValue(sMemberName, out result, args);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return this.ConcreteInstance.InvokeMethod(sMemberName, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            var obj = this.ConcreteInstance;
            if (obj != null)
                return DynamicUtil.HasMember(obj, sMemberName, includeNonPublic);
            else
                return DynamicUtil.TypeHasMember(typeof(T), sMemberName, includeNonPublic);
        }

        IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            var obj = this.ConcreteInstance;
            if (obj != null)
                return DynamicUtil.GetMembers(obj, includeNonPublic);
            else
                return DynamicUtil.GetMembersFromType(typeof(T), includeNonPublic);
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            var obj = this.ConcreteInstance;
            if (obj != null)
                return DynamicUtil.GetMember(obj, sMemberName, includeNonPublic);
            else
                return DynamicUtil.GetMemberFromType(typeof(T), sMemberName, includeNonPublic);
        }

        #endregion

    }

}
