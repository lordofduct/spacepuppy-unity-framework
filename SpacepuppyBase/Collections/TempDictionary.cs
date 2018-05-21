using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{

    public class TempDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ITempCollection<KeyValuePair<TKey, TValue>>
    {

        private const int MAX_SIZE_INBYTES = 1024;

        #region Fields

        private static ObjectCachePool<TempDictionary<TKey, TValue>> _pool = new ObjectCachePool<TempDictionary<TKey, TValue>>(10, () => new TempDictionary<TKey, TValue>());

        private int _maxCapacityOnRelease;
        //private int _version;

        #endregion

        #region CONSTRUCTOR

        public TempDictionary()
            : base()
        {
            var tp = typeof(TKey);
            int sz = (tp.IsValueType && !tp.IsEnum) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            //_version = 1;
        }

        public TempDictionary(IDictionary<TKey, TValue> dict)
            : base()
        {
            var tp = typeof(TKey);
            int sz = (tp.IsValueType && !tp.IsEnum) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            //_version = 1;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            this.Clear();
            _pool.Release(this);
        }

        #endregion

        #region Static Methods

        public static TempDictionary<TKey, TValue> GetDict()
        {
            return _pool.GetInstance();
        }

        public static TempDictionary<TKey, TValue> GetDict(IDictionary<TKey, TValue> dict)
        {
            TempDictionary<TKey, TValue> result;
            if (_pool.TryGetInstance(out result))
            {
                var le = LightEnumerator.Create(dict);
                while (le.MoveNext())
                {
                    result.Add(le.Current.Key, le.Current.Value);
                }
            }
            else
            {
                result = new TempDictionary<TKey, TValue>(dict);
            }
            return result;
        }

        #endregion

    }

}
