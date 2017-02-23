using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{
    public class BinaryHeap<T> : ICollection<T>
    {

        #region Fields

        private static T[] _zeroArray = new T[0];

        private T[] _heap;
        private int _tail;
        private IComparer<T> _comparer;

        #endregion

        #region CONSTRUCTOR

        public BinaryHeap()
        {
            _heap = _zeroArray;
            _tail = 0;
            _comparer = Comparer<T>.Default;
        }

        public BinaryHeap(int capacity)
            : this(capacity, null)
        {

        }

        public BinaryHeap(IEnumerable<T> e)
            : this(e, null)
        {

        }

        public BinaryHeap(IComparer<T> comparer)
        {
            _heap = _zeroArray;
            _tail = 0;
            _comparer = comparer ?? Comparer<T>.Default;
        }

        public BinaryHeap(int capacity, IComparer<T> comparer)
        {
            _heap = new T[capacity];
            _tail = 0;
            _comparer = comparer ?? Comparer<T>.Default;
        }

        public BinaryHeap(IEnumerable<T> e, IComparer<T> comparer)
        {
            _heap = new T[e.Count()];
            _tail = 0;
            _comparer = comparer ?? Comparer<T>.Default;

            foreach (var v in e)
            {
                _heap[_tail++] = v;
            }

            for (int i = GetParent(_tail - 1); i >= 0; i--)
            {
                this.BubbleDown(i);
            }
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _tail; }
        }

        public IComparer<T> Comparer
        {
            get { return _comparer; }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _tail) throw new IndexOutOfRangeException();
                return _heap[index];
            }
            set
            {
                if (index < 0 || index >= _tail) throw new IndexOutOfRangeException();
                _heap[index] = value;
                if (_tail > 1)
                {
                    this.Update(index);
                }
            }
        }

        #endregion

        #region Methods

        public void Add(T item)
        {
            if (_tail == _heap.Length)
            {
                var arr = new T[_heap.Length == 0 ? 4 : 2 * _heap.Length];
                Array.Copy(_heap, 0, arr, 0, _tail);
                _heap = arr;
            }

            _heap[_tail++] = item;
            this.BubbleUp(_tail - 1);
        }

        public T Peek()
        {
            if (_tail == 0) throw new InvalidOperationException("Heap is empty.");
            return _heap[0];
        }

        public T Pop()
        {
            if (_tail == 0) throw new InvalidOperationException("Heap is empty.");
            T result = _heap[0];
            _heap[0] = default(T);
            _tail--;
            this.Swap(_tail, 0);
            this.BubbleDown(0);
            return result;
        }

        public int IndexOf(T item)
        {
            if (_tail == 0) return -1;
            int i = Array.IndexOf(_heap, item);
            if (i < _tail) return i;
            else return -1;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _tail) throw new IndexOutOfRangeException();
            _heap[index] = default(T);
            _tail--;
            if (_tail > 0)
            {
                this.Swap(index, _tail);
                this.BubbleUp(_tail);
            }
        }

        /// <summary>
        /// Force the collection to reassert the status of an element.
        /// </summary>
        /// <param name="item"></param>
        public void Update(T item)
        {
            if (_tail <= 1) return;

            int i = Array.IndexOf(_heap, item);
            if (i < 0) return;

            this.Update(i);
        }

        public void Update(int index)
        {
            if (_tail <= 1) return;
            if (index < 0 || index >= _tail) return;

            int p = GetParent(index);
            if (p == 0 || _comparer.Compare(_heap[p], _heap[index]) >= 0)
            {
                this.BubbleDown(index);
            }
            else
            {
                this.BubbleUp(index);
            }
        }





        private void BubbleUp(int i)
        {
            while (true)
            {
                int p = GetParent(i);
                if (i == 0 || _comparer.Compare(_heap[p], _heap[i]) >= 0)
                    return;

                this.Swap(i, p);
                i = p;
            }
        }

        private void BubbleDown(int i)
        {
            while (true)
            {
                int dominant = GetDominantRelative(i);
                if (dominant == i) return;

                this.Swap(i, dominant);
                i = dominant;
            }
        }

        private void Swap(int i, int j)
        {
            T tmp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = tmp;
        }

        private int GetDominantRelative(int i)
        {
            int dominating = i;
            int c = GetLeftChild(i);
            if (c < _tail && _comparer.Compare(_heap[dominating], _heap[c]) < 0)
                dominating = c;
            c = GetRightChild(i);
            if (c < _tail && _comparer.Compare(_heap[dominating], _heap[c]) < 0)
                dominating = c;
            return dominating;
        }

        private static int GetParent(int i)
        {
            return (i - 1) / 2;
        }

        private static int GetLeftChild(int i)
        {
            return 2 * i + 1;
        }

        private static int GetRightChild(int i)
        {
            return 2 * i + 2;
        }

        #endregion

        #region ICollection Interface

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        public void Clear()
        {
            Array.Clear(_heap, 0, _tail);
            _tail = 0;
        }

        public bool Contains(T item)
        {
            if (_tail == 0) return false;
            int i = Array.IndexOf(_heap, item);
            return i >= 0 && i < _tail;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_heap, 0, array, arrayIndex, _tail);
        }

        public bool Remove(T item)
        {
            int i = this.IndexOf(item);
            if (i < 0) return false;

            this.RemoveAt(i);
            return true;
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable Interface

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

        public struct Enumerator : IEnumerator<T>
        {

            private BinaryHeap<T> _heap;
            private int _index;

            public Enumerator(BinaryHeap<T> heap)
            {
                if (heap == null) throw new ArgumentNullException("heap");

                _heap = heap;
                _index = -1;
            }

            public T Current
            {
                get
                {
                    if (_heap == null) return default(T);
                    return _heap._heap[_index];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (_heap == null) return default(T);
                    return _heap._heap[_index];
                }
            }

            public bool MoveNext()
            {
                if (_heap == null) return false;
                if (_index >= _heap._tail - 1) return false;

                _index++;
                return true;
            }

            public void Dispose()
            {
                _heap = null;
                _index = 0;
            }

            void IEnumerator.Reset()
            {
                _index = -1;
            }
        }

        #endregion

    }

}
