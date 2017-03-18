using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{

    public class TempList<T> : List<T>, ITempCollection<T>
    {

        private const int MAX_SIZE_INBYTES = 1024;

        #region Fields

        private static ObjectCachePool<TempList<T>> _pool = new ObjectCachePool<TempList<T>>(-1, () => new TempList<T>());

        private int _maxCapacityOnRelease;
        private int _version;

        #endregion

        #region CONSTRUCTOR

        public TempList()
            : base()
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType && !tp.IsEnum) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
        }

        public TempList(IEnumerable<T> e)
            : base(e)
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType && !tp.IsEnum) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
        }

        public TempList(int count)
            : base(count)
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType && !tp.IsEnum) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            this.Clear();
            if(_pool.Release(this))
            {
                if(this.Capacity > _maxCapacityOnRelease / Math.Min(_version, 4))
                {
                    this.Capacity = _maxCapacityOnRelease / Math.Min(_version, 4);
                    _version = 1;
                }
                else
                {
                    _version++;
                }
            }
        }

        #endregion

        #region Static Methods

        public static TempList<T> GetList()
        {
            return _pool.GetInstance();
        }

        public static TempList<T> GetList(IEnumerable<T> e)
        {
            TempList<T> result;
            if(_pool.TryGetInstance(out result))
            {
                //result.AddRange(e);
                var e2 = new LightEnumerator<T>(e);
                while(e2.MoveNext())
                {
                    result.Add(e2.Current);
                }
            }
            else
            {
                result = new TempList<T>(e);
            }
            return result;
        }

        public static TempList<T> GetList(int count)
        {
            TempList<T> result;
            if (_pool.TryGetInstance(out result))
            {
                if (result.Capacity < count) result.Capacity = count;
                return result;
            }
            else
            {
                result = new TempList<T>(count);
            }
            return result;
        }

        #endregion

    }

}
