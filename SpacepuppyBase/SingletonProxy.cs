#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;

namespace com.spacepuppy
{

    [CreateAssetMenu(fileName = "SingletonProxy", menuName = "Spacepuppy/SingletonProxy")]
    public class SingletonProxy : ScriptableObject, IDynamic, IProxy
    {

        #region Fields

        [TypeReference.Config(typeof(IManagedSingleton), allowAbstractClasses = false, allowInterfaces = false, dropDownStyle = TypeDropDownListingStyle.Flat)]
        [SerializeField()]
        private TypeReference _singletonType;
        [SerializeField()]
        private bool _createIfNone;

        #endregion

        #region IProxy Interface

        public ISingleton GetTarget()
        {
            if (_singletonType == null || _singletonType.Type == null) return null;

            //return Singleton.GetInstance(_singletonType.Type, !_createIfNone);
            return Singletons.Get(_singletonType.Type, _createIfNone && Application.isPlaying);
        }

        bool IProxy.QueriesTarget
        {
            get { return false; }
        }

        object IProxy.GetTarget()
        {
            return this.GetTarget();
        }

        object IProxy.GetTarget(object arg)
        {
            return this.GetTarget();
        }

        System.Type IProxy.GetTargetType()
        {
            return _singletonType.Type ?? typeof(ISingleton);
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
            return this.GetTarget().SetValue(sMemberName, value, index);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return this.GetTarget().GetValue(sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            return this.GetTarget().TryGetValue(sMemberName, out result, args);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return this.GetTarget().InvokeMethod(sMemberName, args);
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

        IEnumerable<string> IDynamic.GetMemberNames(bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return Enumerable.Empty<string>();
            return DynamicUtil.GetMemberNamesFromType(_singletonType.Type, includeNonPublic);
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return null;
            return DynamicUtil.GetMemberFromType(_singletonType.Type, sMemberName, includeNonPublic);
        }

        #endregion

    }

    /*

    public class SingletonProxy : MonoBehaviour, IDynamic, IProxy
    {

        #region Fields

        [TypeReference.Config(typeof(IManagedSingleton), allowAbstractClasses = false, allowInterfaces = false, dropDownStyle = TypeDropDownListingStyle.Flat)]
        [SerializeField()]
        private TypeReference _singletonType;
        [SerializeField()]
        private bool _createIfNone;

        #endregion
        
        #region IProxy Interface

        public ISingleton GetTarget()
        {
            if (_singletonType == null || _singletonType.Type == null) return null;

            //return Singleton.GetInstance(_singletonType.Type, !_createIfNone);
            return Singletons.Get(_singletonType.Type, _createIfNone);
        }

        object IProxy.GetTarget()
        {
            return this.GetTarget();
        }

        object IProxy.GetTarget(object arg)
        {
            return this.GetTarget();
        }

        System.Type IProxy.GetTargetType()
        {
            return _singletonType.Type ?? typeof(ISingleton);
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
            return this.GetTarget().SetValue(sMemberName, value, index);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return this.GetTarget().GetValue(sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            return this.GetTarget().TryGetValue(sMemberName, out result, args);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return this.GetTarget().InvokeMethod(sMemberName, args);
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

        IEnumerable<string> IDynamic.GetMemberNames(bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return Enumerable.Empty<string>();
            return DynamicUtil.GetMemberNamesFromType(_singletonType.Type, includeNonPublic);
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return null;
            return DynamicUtil.GetMemberFromType(_singletonType.Type, sMemberName, includeNonPublic);
        }

        #endregion
        
    }

    [System.Obsolete("No idea why this even exists.")]
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

        IEnumerable<string> IDynamic.GetMemberNames(bool includeNonPublic)
        {
            var obj = this.ConcreteInstance;
            if (obj != null)
                return DynamicUtil.GetMemberNames(obj, includeNonPublic);
            else
                return DynamicUtil.GetMemberNamesFromType(typeof(T), includeNonPublic);
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

    */

}
