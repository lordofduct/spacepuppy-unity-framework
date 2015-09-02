using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public class SingletonProxy : MonoBehaviour, IDynamic
    {

        #region Fields

        [TypeReference.Config(typeof(ISingleton), allowAbstractClasses = false, allowInterfaces = false, dropDownStyle = TypeDropDownListingStyle.Flat)]
        [SerializeField()]
        private TypeReference _singletonType;

        #endregion

        #region Properties

        public ISingleton Instance
        {
            get
            {
                if (_singletonType == null || _singletonType.Type == null) return null;
                ISingleton instance;
                if (Singleton.HasInstance(_singletonType.Type, out instance))
                    return instance;
                else
                    return null;
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

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return this.Instance.InvokeMethod(sMemberName, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return false;

            return DynamicUtil.HasMember(_singletonType.Type, sMemberName, includeNonPublic);
        }

        IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return Enumerable.Empty<System.Reflection.MemberInfo>();
            return DynamicUtil.GetMembers(_singletonType.Type, includeNonPublic);
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            if (_singletonType == null || _singletonType.Type == null) return null;
            return DynamicUtil.GetMember(_singletonType.Type, sMemberName, includeNonPublic);
        }

        #endregion
        
    }

}
