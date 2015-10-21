using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{

    public class TempList<T> : List<T>, ITempCollection<T>
    {

        private const int MAX_SIZE_INBYTES = 1024;

        #region Fields

        internal static TempList<T> _instance;

        private int _maxCapacityOnRelease;
        private int _version;
        internal bool _global;

        #endregion

        #region CONSTRUCTOR

        public TempList()
            : base()
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
            _global = true;
        }

        public TempList(IEnumerable<T> e)
            : base(e)
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
            _global = true;
        }

        public TempList(int count)
            : base(count)
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
            _global = true;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            this.Clear();
            if (_instance != null) return;

            if (_global && _instance == null) _instance = this;
            if (this.Capacity > _maxCapacityOnRelease / Math.Min(_version, 4))
            {
                this.Capacity = _maxCapacityOnRelease / Math.Min(_version, 4);
                _version = 0;
            }

            _version++;
        }

        #endregion

    }

}
