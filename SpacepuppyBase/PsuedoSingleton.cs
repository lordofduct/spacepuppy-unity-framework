using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

using com.spacepuppy.Dynamic;

namespace com.spacepuppy
{

    /// <summary>
    /// Allows creating a singleton access point of any class that isn't inherently a Singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PsuedoSingleton<T> : Singleton, IDynamic where T : Component
    {

        #region Static Interface

        private static PsuedoSingleton<T> _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null) return null;
                else return _instance.ConcreteInstance;
            }
        }

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            _instance = this;
        }

        #endregion

        #region Properties

        public abstract T ConcreteInstance { get; }

        #endregion


        #region IDynamic Interface

        public object this[string key]
        {
            get
            {
                return DynamicUtil.GetValue(PsuedoSingleton<T>.Instance, key);
            }
            set
            {
                DynamicUtil.SetValue(PsuedoSingleton<T>.Instance, key, value);
            }
        }

        MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            return DynamicUtil.GetMember(PsuedoSingleton<T>.Instance, sMemberName, includeNonPublic);
        }

        IEnumerable<MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            return DynamicUtil.GetMembers(PsuedoSingleton<T>.Instance, includeNonPublic);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return DynamicUtil.GetValue(PsuedoSingleton<T>.Instance, sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            return DynamicUtil.TryGetValue(PsuedoSingleton<T>.Instance, sMemberName, out result, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            return DynamicUtil.HasMember(PsuedoSingleton<T>.Instance, sMemberName, includeNonPublic);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return DynamicUtil.InvokeMethod(PsuedoSingleton<T>.Instance, sMemberName, args);
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            return DynamicUtil.SetValue(PsuedoSingleton<T>.Instance, sMemberName, value, index);
        }

        #endregion


    }

}
