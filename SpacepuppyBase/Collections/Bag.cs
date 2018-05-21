using System;
using System.Collections;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// A simple collection of unique values that can be popped.
    /// Acts like an unordered Stack with uniqueness. (Think like a HashSet that can be popped)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Bag<T> : ICollection<T>
    {

        #region Fields
        
        private int[] _buckets;
        private Slot[] _slots;
        private int _count;
        private int _lastIndex;
        private int _freeList;
        private IEqualityComparer<T> _comparer;
        private int _generation;

        #endregion

        #region CONSTRUCTOR

        public Bag()
        {
            _comparer = EqualityComparer<T>.Default;
            _lastIndex = 0;
            _count = 0;
            _freeList = -1;
            _generation = 0;
        }

        public Bag(IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _lastIndex = 0;
            _count = 0;
            _freeList = -1;
            _generation = 0;
        }

        public Bag(int size)
        {
            _comparer = EqualityComparer<T>.Default;
            _lastIndex = 0;
            _count = 0;
            _freeList = -1;
            _generation = 0;
            this.Init(size);
        }

        public Bag(int size, IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _lastIndex = 0;
            _count = 0;
            _freeList = -1;
            _generation = 0;
            this.Init(size);
        }

        public Bag(IEnumerable<T> collection)
            : this(collection, (IEqualityComparer<T>)null)
        {
        }

        public Bag(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            int capacity = 5; //set some nominal size
            ICollection<T> c = collection as ICollection<T>;
            if (c != null)
                capacity = c.Count;

            _comparer = comparer ?? EqualityComparer<T>.Default;
            _lastIndex = 0;
            _count = 0;
            _freeList = -1;
            _generation = 0;
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
            if (item == null) return false;
            if (_buckets == null)
                this.Init(0);

            int hashCode = _comparer.GetHashCode(item) & int.MaxValue;
            int index1 = hashCode % _buckets.Length;
            for(int i = _buckets[hashCode % _buckets.Length] - 1; i >= 0; i = _slots[i].Next)
            {
                if (_slots[i].HashCode == hashCode && _comparer.Equals(_slots[i].Value, item))
                    return false;
            }

            int index2;
            if(_freeList >= 0)
            {
                index2 = _freeList;
                _freeList = _slots[index2].Next;
            }
            else
            {
                if(_lastIndex == _slots.Length)
                {
                    this.IncreaseCapacity();
                    index1 = hashCode % _buckets.Length;
                }
                index2 = _lastIndex;
                ++_lastIndex;
            }

            _slots[index2].HashCode = hashCode;
            _slots[index2].Value = item;
            _slots[index2].Next = _buckets[index1] - 1;
            _buckets[index1] = index2 + 1;
            ++_count;
            ++_generation;
            return true;
        }

        public T Pop()
        {
            if (_lastIndex <= 0) throw new System.InvalidOperationException("Attempted to Pop an empty Bag.");

            for(int i = 0; i < _lastIndex; ++i)
            {
                if(_slots[i].HashCode >= 0)
                {
                    T result = _slots[i].Value;

                    //remove entry
                    _buckets[_slots[i].HashCode % _buckets.Length] = _slots[i].Next + 1;
                    _slots[i].HashCode = -1;
                    _slots[i].Value = default(T);
                    _slots[i].Next = _freeList;
                    _freeList = i;
                    --_count;
                    ++_generation;

                    return result;
                }
            }

            return default(T);
        }

        public void TrimExcess()
        {
            if(_count == 0)
            {
                _buckets = null;
                _slots = null;
                ++_generation;
            }
            else
            {
                int prime = PrimeHelper.GetPrime(_count);
                Slot[] slotArray = new Slot[prime];
                int[] numArray = new int[prime];
                int index1 = 0;
                int index2;
                for(int i = 0; i < _lastIndex; ++i)
                {
                    if(_slots[i].HashCode >= 0)
                    {
                        slotArray[index1] = _slots[i];
                        index2 = slotArray[index1].HashCode % prime;
                        slotArray[index1].Next = numArray[index2] - 1;
                        numArray[index2] = index1 + 1;
                        ++index1;
                    }
                }
                _lastIndex = index1;
                _slots = slotArray;
                _buckets = numArray;
                _freeList = -1;
            }
        }
        


        private void Init(int size)
        {
            int prime = PrimeHelper.GetPrime(size);
            _buckets = new int[prime];
            _slots = new Slot[prime];
        }

        private void IncreaseCapacity()
        {
            int min = _count * 2;
            if (min < 0)
                min = _count;
            int prime = PrimeHelper.GetPrime(min);
            if (prime <= _count)
                throw new System.ArgumentException("Capacity Overflow");
            Slot[] slotArray = new Slot[prime];
            if (_slots != null)
                Array.Copy(_slots, 0, slotArray, 0, _lastIndex);
            int[] numArray = new int[prime];
            for(int i = 0; i < _lastIndex; ++i)
            {
                int j = slotArray[i].HashCode % prime;
                slotArray[i].Next = numArray[j] - 1;
                numArray[j] = i + 1;
            }
            _slots = slotArray;
            _buckets = numArray;
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
            if (item == null) return false;
            if(_buckets != null)
            {
                int hashCode = _comparer.GetHashCode(item) & int.MaxValue;
                for(int i = _buckets[hashCode % _buckets.Length] - 1; i >= 0; i = _slots[i].Next)
                {
                    if (_slots[i].HashCode == hashCode && _comparer.Equals(_slots[i].Value, item))
                        return true;
                }
            }
            return false;
        }

        public bool Remove(T item)
        {
            if (item == null) return false;
            if(_buckets != null)
            {
                int hashCode = _comparer.GetHashCode(item) & int.MaxValue;
                int index1 = hashCode % _buckets.Length;
                int index2 = -1;
                for(int i = _buckets[index1] - 1; i >= 0; i = _slots[i].Next)
                {
                    if(_slots[i].HashCode == hashCode && _comparer.Equals(_slots[i].Value, item))
                    {
                        if (index2 < 0)
                            _buckets[index1] = _slots[i].Next + 1;
                        else
                            _slots[index2].Next = _slots[i].Next;
                        _slots[i].HashCode = -1;
                        _slots[i].Value = default(T);
                        _slots[i].Next = _freeList;
                        _freeList = i;
                        --_count;
                        ++_generation;
                        return true;
                    }
                    index2 = i;
                }
            }
            return false;
        }

        public void Clear()
        {
            if(_lastIndex > 0)
            {
                Array.Clear(_slots, 0, _lastIndex);
                Array.Clear(_buckets, 0, _buckets.Length);
                _lastIndex = 0;
                _count = 0;
                _freeList = -1;
            }
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

            int num = 0;
            for(int i = 0; i < _lastIndex && num < _count; ++i)
            {
                if(_slots[i].HashCode >= 0)
                {
                    array[index + num] = _slots[i].Value;
                    ++num;
                }
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

        private struct Slot
        {
            public int HashCode;
            public T Value;
            public int Next;
        }

        public struct Enumerator : IEnumerator<T>
        {

            private Bag<T> _set;
            private int _index;
            private int _version;
            private T _current;

            internal Enumerator(Bag<T> coll)
            {
                if (coll == null) throw new System.ArgumentNullException("coll");
                _set = coll;
                _index = 0;
                _version = coll._generation;
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
            }

            public bool MoveNext()
            {
                if(_version != _set._generation)
                    throw new System.InvalidOperationException("Enumerator is out of sync.");
                for(;_index < _set._lastIndex; ++_index)
                {
                    if(_set._slots[_index].HashCode >= 0)
                    {
                        _current = _set._slots[_index].Value;
                        ++_index;
                        return true;
                    }
                }
                _index = _set._lastIndex + 1;
                _current = default(T);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (_version != _set._generation)
                    throw new System.InvalidOperationException("Enumerator is out of sync.");
                _index = 0;
                _current = default(T);
            }
            
        }

        #endregion

    }

}
