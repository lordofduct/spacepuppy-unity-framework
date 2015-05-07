using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Utils.Dynamic
{
    public class ObjectStateCache
    {

        #region Fields

        private Dictionary<string, object> _values = new Dictionary<string, object>();

        #endregion

        #region Properties

        public IEnumerable<string> Properties
        {
            get { return _values.Keys; }
        }

        #endregion

        #region Methods

        public void AddProperty(string name)
        {
            if (!_values.ContainsKey(name)) _values.Add(name, null);
        }

        public void RemoveProperty(string name)
        {
            _values.Remove(name);
        }

        public void CacheState(object obj)
        {
            foreach(var key in _values.Keys)
            {
                try
                {
                    _values[key] = ObjUtil.GetValue(obj, key);
                }
                catch
                {

                }
            }
        }

        public void SetState(object obj)
        {
            foreach(var pair in _values)
            {
                try
                {
                    ObjUtil.SetValue(obj, pair.Key, pair.Value);
                }
                catch
                {

                }
            }
        }

        #endregion

    }
}
