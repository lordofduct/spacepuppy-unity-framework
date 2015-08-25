using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// This is intended for a short lived collection that needs to be memory efficient and fast. 
    /// Call the static 'GetCollection' method to get a cached collection for use. 
    /// When you're done with the collection you call Release to make it available for reuse again later. 
    /// Do NOT use it again after calling Release.
    /// 
    /// Due to the design of this, it should only ever be used in a single threaded manner. Primarily intended 
    /// for the main Unity thread. 
    /// 
    /// If you're in a separate thread, it's best to cache your own list local to there, and don't even bother with 
    /// this.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TempCollection<T> : IList<T>, IDisposable
    {

        private const int MAX_SIZE_INBYTES = 1024;

        #region Fields

        private List<T> _coll;
        private int _maxCapacityOnRelease;
        private int _version;

        #endregion

        #region CONSTRUCTOR

        private TempCollection()
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
            _coll = new List<T>();
        }

        private TempCollection(IEnumerable<T> e)
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
            _coll = new List<T>(e);
        }

        #endregion

        #region Methods

        public void Sort(Comparison<T> comparison)
        {
            _coll.Sort(comparison);
        }

        public void Sort(IComparer<T> comparer)
        {
            _coll.Sort(comparer);
        }

        public void Release()
        {
            this.Clear();
            if (_instance != null) return;

            _instance = this;
            if (_coll.Capacity > _maxCapacityOnRelease / Math.Min(_version, 4))
            {
                _coll.Capacity = _maxCapacityOnRelease / Math.Min(_version, 4);
                _version = 0;
            }

            _version++;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            this.Release();
        }

        #endregion

        #region ICollection Interface

        public void Add(T item)
        {
            _coll.Add(item);
        }

        public void Clear()
        {
            _coll.Clear();
        }

        public bool Contains(T item)
        {
            return _coll.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _coll.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _coll.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return _coll.Remove(item);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_coll.GetEnumerator());
        }
        

        IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList Interface

        public int IndexOf(T item)
        {
            return _coll.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _coll.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _coll.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _coll[index];
            }
            set
            {
                _coll[index] = value;
            }
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<T>
        {

            private List<T>.Enumerator _e;

            public Enumerator(List<T>.Enumerator e)
            {
                _e = e;
            }


            public T Current
            {
                get { return _e.Current; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _e.Current; }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            public void Reset()
            {
                (_e as IEnumerator<T>).Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }
        }

        #endregion


        #region Static Interface

        private static TempCollection<T> _instance;

        public static TempCollection<T> GetCollection()
        {
            if(_instance == null)
            {
                return new TempCollection<T>();
            }
            else
            {
                var coll = _instance;
                _instance = null;
                return coll;
            }
        }

        public static TempCollection<T> GetCollection(IEnumerable<T> e)
        {
            if(_instance == null)
            {
                return new TempCollection<T>(e);
            }
            else
            {
                var coll = _instance;
                _instance = null;
                coll._coll.AddRange(e);
                return coll;
            }
        }

        #endregion

    }
}
