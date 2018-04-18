using UnityEngine;

using com.spacepuppy.Dynamic;
using System.Collections.Generic;
using System.Reflection;

namespace com.spacepuppy
{

    /// <summary>
    /// A component you can stick variables into willy-nilly.
    /// </summary>
    public class VariableStore : SPComponent, IDynamic, IToken
    {

        #region Fields

        [SerializeField]
        private TypeReference _reflectNamesFromType;

        [SerializeField()]
        private VariantCollection _variables = new VariantCollection();

        #endregion

        #region Properties

        public System.Type ReflectNamesFromType
        {
            get { return _reflectNamesFromType.Type; }
            set { _reflectNamesFromType.Type = value; }
        }

        public VariantCollection Variables
        {
            get { return _variables; }
        }

        public object this[string key]
        {
            get
            {
                return _variables[key];
            }
            set
            {
                _variables[key] = value;
            }
        }

        #endregion

        #region IDynamic Interface
        
        MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            return (_variables as IDynamic).GetMember(sMemberName, includeNonPublic);
        }

        IEnumerable<MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            return (_variables as IDynamic).GetMembers(includeNonPublic);
        }

        IEnumerable<string> IDynamic.GetMemberNames(bool includeNonPublic)
        {
            return (_variables as IDynamic).GetMemberNames(includeNonPublic);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return (_variables as IDynamic).GetValue(sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            return (_variables as IDynamic).TryGetValue(sMemberName, out result, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            return (_variables as IDynamic).HasMember(sMemberName, includeNonPublic);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return (_variables as IDynamic).InvokeMethod(sMemberName, args);
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            return (_variables as IDynamic).SetValue(sMemberName, value, index);
        }

        #endregion

        #region IToken Interface

        public void CopyTo(object obj)
        {
            _variables.CopyTo(obj);
        }

        public void SyncFrom(object obj)
        {
            _variables.SyncFrom(obj);
        }

        #endregion

    }

    /// <summary>
    /// A ScriptableObject you can stick variables into willy-nilly.
    /// </summary>
    [CreateAssetMenu(fileName = "VariableStore", menuName = "Spacepuppy/VariableStore")]
    public class VariableStoreAsset : ScriptableObject, IDynamic, IToken
    {

        #region Fields

        [SerializeField]
        private TypeReference _reflectNamesFromType;

        [SerializeField()]
        private VariantCollection _variables = new VariantCollection();

        #endregion

        #region Properties

        public System.Type ReflectNamesFromType
        {
            get { return _reflectNamesFromType.Type; }
            set { _reflectNamesFromType.Type = value; }
        }

        public VariantCollection Variables
        {
            get { return _variables; }
        }

        public object this[string key]
        {
            get
            {
                return _variables[key];
            }
            set
            {
                _variables[key] = value;
            }
        }

        #endregion

        #region IDynamic Interface

        MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            return (_variables as IDynamic).GetMember(sMemberName, includeNonPublic);
        }

        IEnumerable<MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            return (_variables as IDynamic).GetMembers(includeNonPublic);
        }

        IEnumerable<string> IDynamic.GetMemberNames(bool includeNonPublic)
        {
            return (_variables as IDynamic).GetMemberNames(includeNonPublic);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return (_variables as IDynamic).GetValue(sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            return (_variables as IDynamic).TryGetValue(sMemberName, out result, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            return (_variables as IDynamic).HasMember(sMemberName, includeNonPublic);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return (_variables as IDynamic).InvokeMethod(sMemberName, args);
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            return (_variables as IDynamic).SetValue(sMemberName, value, index);
        }

        #endregion

        #region IToken Interface

        public void CopyTo(object obj)
        {
            _variables.CopyTo(obj);
        }

        public void SyncFrom(object obj)
        {
            _variables.SyncFrom(obj);
        }

        #endregion

    }

}
