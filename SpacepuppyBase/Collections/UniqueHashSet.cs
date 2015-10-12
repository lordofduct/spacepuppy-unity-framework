using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// Represents a HashSet whose members always generate unique hash values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UniqueHashSet<T> : ICollection<T>
    {

        #region Fields

        private const int INITIAL_SIZE = 10;
        private const float DEFAULT_LOAD_FACTOR = 0.9f;
        private const int NO_SLOT = -1;
        private const int HASH_FLAG = -2147483648;

        private int[] _table;
        private Link[] _links;
        private T[] _slots;
        private int _touched;
        private int _empty_slot;
        private int _count;
        private int _threshold;
        private IEqualityComparer<T> _comparer;
        private int _generation;

        #endregion

        #region CONSTRUCTOR

        public UniqueHashSet()
        {
            _comparer = EqualityComparer<T>.Default;
            this.Init(0);
        }

        public UniqueHashSet(IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
            this.Init(0);
        }

        public UniqueHashSet(IEnumerable<T> collection)
            : this(collection, (IEqualityComparer<T>) null)
        {
        }

        public UniqueHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            int capacity = 0;
            ICollection<T> collection1 = collection as ICollection<T>;
            if (collection1 != null)
                capacity = collection1.Count;

            _comparer = comparer ?? EqualityComparer<T>.Default;
            this.Init(capacity);

            foreach (T obj in collection)
                this.Add(obj);
        }
        
        #endregion

        #region Properties

        public IEqualityComparer<T> Comparer
        {
            get { return _comparer; }
        }
        
        #endregion

        #region Methods

        public bool Add(T item)
        {
            int itemHashCode = _comparer.GetHashCode(item);
            int index1 = (itemHashCode & int.MaxValue) % _table.Length;
            if (this.SlotsContainsAt(index1, itemHashCode, item))
                return false;
            if (++_count > _threshold)
            {
                this.Resize();
                index1 = (itemHashCode & int.MaxValue) % _table.Length;
            }
            int index2 = _empty_slot;
            if (index2 == -1)
                index2 = _touched++;
            else
                _empty_slot = _links[index2].Next;
            _links[index2].HashCode = itemHashCode;
            _links[index2].Next = _table[index1] - 1;
            _table[index1] = index2 + 1;
            _slots[index2] = item;
            ++_generation;
            return true;
        }

        public void TrimExcess()
        {
            this.Resize();
        }

        public T GetValue(int hash)
        {
            T result;
            this.TryGetValue(hash, out result);
            return result;
        }

        public bool TryGetValue(int hash, out T result)
        {
            int ti = (hash & int.MaxValue) % _table.Length;
            var i = _table[ti] - 1;
            if (i < 0)
            {
                result = default(T);
                return false;
            }

            var link = _links[i];
            if (link.HashCode != hash)
            {
                result = default(T);
                return false;
            }
            else
            {
                result = _slots[i];
                return true;
            }
        }

        public bool ContainsHashCode(int hash)
        {
            int ti = (hash & int.MaxValue) % _table.Length;
            var i = _table[ti] - 1;
            if (i < 0) return false;

            var link = _links[i];
            return link.HashCode == hash;
        }



        private void Init(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("capacity");
            else if (size == 0)
                size = 5; //set nominal start size
            size = PrimeHelper.GetPrime(size);

            _table = new int[size];
            _links = new Link[size];
            _empty_slot = -1;
            _slots = new T[size];
            _touched = 0;
            _threshold = (int)((double)_table.Length * 0.899999976158142);
            if (_threshold != 0 || _table.Length <= 0)
                return;
            _threshold = 1;
            _generation = 0;
        }

        private void InitArrays(int size)
        {
            
        }

        private bool SlotsContainsAt(int index, int hash, T item)
        {
            Link link;
            for (int i = _table[index] - 1; i != -1; i = link.Next)
            {
                link = _links[i];
                if (link.HashCode == hash && _comparer.Equals(_slots[i], item))
                    return true;
            }
            return false;
        }

        private int GetLinkHashCode(int index)
        {
            return _links[index].HashCode & int.MinValue;
        }

        private void Resize()
        {
            int length = PrimeHelper.ExpandPrime(_table.Length);
            int[] numArray = new int[length];
            Link[] linkArray = new Link[length];
            for (int index1 = 0; index1 < _table.Length; ++index1)
            {
                for (int index2 = _table[index1] - 1; index2 != -1; index2 = _links[index2].Next)
                {
                    int index3 = ((linkArray[index2].HashCode = _comparer.GetHashCode(_slots[index2])) & int.MaxValue) % length;
                    linkArray[index2].Next = numArray[index3] - 1;
                    numArray[index3] = index2 + 1;
                }
            }
            _table = numArray;
            _links = linkArray;
            T[] objArray = new T[length];
            Array.Copy((Array)_slots, 0, (Array)objArray, 0, _touched);
            _slots = objArray;
            _threshold = (int)((double)length * 0.899999976158142);
        }

        #endregion


        #region ICollection Interface

        public int Count
        {
            get
            {
                return _count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        public bool Contains(T item)
        {
            int itemHashCode = _comparer.GetHashCode(item);
            return this.SlotsContainsAt((itemHashCode & int.MaxValue) % _table.Length, itemHashCode, item);
        }

        public bool Remove(T item)
        {
            int itemHashCode = _comparer.GetHashCode(item);
            int index1 = (itemHashCode & int.MaxValue) % _table.Length;
            int index2 = _table[index1] - 1;
            if (index2 == -1)
                return false;
            int index3 = -1;
            do
            {
                Link link = _links[index2];
                if (link.HashCode != itemHashCode || !_comparer.Equals(_slots[index2], item))
                {
                    index3 = index2;
                    index2 = link.Next;
                }
                else
                    break;
            }
            while (index2 != -1);
            if (index2 == -1)
                return false;
            --_count;
            if (index3 == -1)
                _table[index1] = _links[index2].Next + 1;
            else
                _links[index3].Next = _links[index2].Next;
            _links[index2].Next = _empty_slot;
            _empty_slot = index2;
            _links[index2].HashCode = 0;
            _slots[index2] = default(T);
            ++_generation;
            return true;
        }

        public void Clear()
        {
            _count = 0;
            Array.Clear((Array)_table, 0, _table.Length);
            Array.Clear((Array)_slots, 0, _slots.Length);
            Array.Clear((Array)_links, 0, _links.Length);
            _empty_slot = -1;
            _touched = 0;
            ++_generation;
        }

        public void CopyTo(T[] array, int index)
        {
            this.CopyTo(array, index, _count);
        }

        public void CopyTo(T[] array, int index, int count)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (index > array.Length)
                throw new ArgumentException("index larger than largest valid index of array");
            if (array.Length - index < count)
                throw new ArgumentException("Destination array cannot hold the requested elements!");
            int index1 = 0;
            for (int index2 = 0; index1 < _touched && index2 < count; ++index1)
            {
                if (this.GetLinkHashCode(index1) != 0)
                    array[index++] = _slots[index1];
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Special Types

        private struct Link
        {
            public int HashCode;
            public int Next;
        }

        public struct Enumerator : IEnumerator<T>
        {

            private UniqueHashSet<T> _coll;
            private int _next;
            private int _stamp;
            private T _current;

            internal Enumerator(UniqueHashSet<T> coll)
            {
                if (coll == null) throw new System.ArgumentNullException("coll");
                _coll = coll;
                _stamp = coll._generation;
                _next = 0;
                _current = default(T);
            }

            public T Current
            {
                get
                {
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            public void Dispose()
            {
                _coll = null;
            }

            public bool MoveNext()
            {
                this.CheckState();
                if (_next < 0)
                    return false;
                while (_next < _coll._touched)
                {
                    int index = _next++;
                    if (_coll.GetLinkHashCode(index) != 0)
                    {
                        _current = _coll._slots[index];
                        return true;
                    }
                }
                _next = -1;
                return false;
            }

            void IEnumerator.Reset()
            {
                this.CheckState();
                _next = 0;
            }


            private void CheckState()
            {
                if (_coll == null)
                    throw new ObjectDisposedException(null);
                if (_coll._generation != _stamp)
                    throw new InvalidOperationException("HashSet have been modified while it was iterated over");
            }
        }

        #endregion

    }
}
