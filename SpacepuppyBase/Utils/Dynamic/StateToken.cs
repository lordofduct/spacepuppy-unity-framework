using System;
using System.Collections.Generic;

namespace com.spacepuppy.Utils.Dynamic
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

        void IDynamic.SetValue(string sMemberName, object value)
        {
            this.SetValue(sMemberName, value);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return this.GetValue(sMemberName);
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
