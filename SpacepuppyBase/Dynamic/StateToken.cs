using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic
{
    public class StateToken : IDynamic, System.Collections.IEnumerable
    {

        #region Fields

        private Dictionary<string, object> _table = new Dictionary<string, object>();

        #endregion

        #region Constructor

        public StateToken()
        {

        }

        #endregion

        #region Properties

        public object this[string sMemberName]
        {
            get
            {
                return this.GetValue(sMemberName);
            }
            set
            {
                this.SetValue(sMemberName, value);
            }
        }

        #endregion

        #region Methods

        public void SetValue(string skey, object value)
        {
            _table[skey] = value;
        }

        public object GetValue(string skey)
        {
            if (_table.ContainsKey(skey))
                return _table[skey];
            else
                return null;
        }

        public T GetValue<T>(string skey)
        {
            if (_table.ContainsKey(skey))
            {
                var obj = _table[skey];
                if (obj is T) return (T)obj;
                if (ConvertUtil.IsSupportedType(typeof(T))) return ConvertUtil.ToPrim<T>(obj);
                return default(T);
            }
            else
                return default(T);
        }

        public bool HasKey(string skey)
        {
            return _table.ContainsKey(skey);
        }

        #endregion

        #region IDynamic Interface

        object IDynamic.this[string sMemberName]
        {
            get
            {
                return this.GetValue(sMemberName);
            }
            set
            {
                this.SetValue(sMemberName, value);
            }
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            this.SetValue(sMemberName, value);
            return true;
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return this.GetValue(sMemberName);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return this.GetValue(sMemberName);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            return this.HasKey(sMemberName);
        }

        IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            var tp = this.GetType();
            foreach(var k in _table.Keys)
            {
                yield return new DynamicPropertyInfo(k, tp);
            }
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            if (_table.ContainsKey(sMemberName))
                return new DynamicPropertyInfo(sMemberName, this.GetType());
            else
                return null;
        }

        #endregion

        #region IEnumerable Interface

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _table.Values.GetEnumerator();
        }

        #endregion

    }
}
