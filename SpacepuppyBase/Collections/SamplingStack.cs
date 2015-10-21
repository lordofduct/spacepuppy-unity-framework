using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// Represents a stack of static size. If you push a value onto the stack when it's full, the value at the bottom (oldest) of the stack is removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SamplingStack<T> : IEnumerable<T>, ICollection<T>
    {

        #region Fields

        private T[] _values;
        private int _count;
        private int _head;

        #endregion
        
        #region CONSTRUCTOR

        public SamplingStack(int size)
        {
            if (size < 0) throw new System.ArgumentException("Size must be non-negative.", "size");
            _values = new T[size];
            _count = 0;
            _head = -1;
        }

        #endregion

        #region Properties

        public int Size { get { return _values.Length; } }

        public int Count { get { return _count; } }

        #endregion

        #region Methods

        public void Push(T value)
        {
            if (_values.Length == 0) return;

            _head = (_head + 1) % _values.Length;
            _values[_head] = value;
            if(_count < _values.Length)
                _count++;
        }

        public T Pop()
        {
            if (_count == 0) throw new InvalidOperationException("SamplingStack is empty.");

            var result = _values[_head];
            _values[_head] = default(T);
            _head = (_head > 0) ? _head - 1 : _values.Length - 1;
            _count--;
            return result;
        }

        public T Shift()
        {
            if (_count == 0) throw new InvalidOperationException("SamplingStack is empty.");

            int index = (_head - _count + 1);
            if (index < 0) index += _values.Length;
            var result = _values[index];
            _values[index] = default(T);
            _count--;
            return result;
        }

        public T Peek()
        {
            if (_count == 0) throw new InvalidOperationException("SamplingStack is empty.");

            return _values[_head];
        }

        public IEnumerable<T> Peek(int cnt)
        {
            int index = _head;
            if (cnt > _count) cnt = _count;
            for(int i = 0; i < cnt; i++)
            {
                yield return _values[index];
                _head = (_head > 0) ? _head - 1 : _values.Length - 1;
            }
        }

        public T PeekShift()
        {
            if (_count == 0) throw new InvalidOperationException("SamplingStack is empty.");

            int index = (_head - _count + 1);
            if (index < 0) index += _values.Length;
            return _values[index];
        }

        public void Resize(int size)
        {
            if (size < 0) throw new System.ArgumentException("Size must be non-negative.", "size");

            if(_head >= _count - 1)
            {
                System.Array.Resize(ref _values, size);
            }
            else
            {
                T[] arr = new T[size];
                int index = _head;
                _count = Math.Min(size, _count);
                for (int i = 0; i < _count; i++)
                {
                    arr[i] = _values[index];
                    index = (index > 0) ? index - 1 : _values.Length - 1;
                }
                _values = arr;
                _head = _count - 1;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                _values[i] = default(T);
            }
            _count = 0;
            _head = -1;
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<T> GetEnumerator()
        {
            int index = _head;
            for(int i = 0; i < _count; i++)
            {
                yield return _values[index];
                index = (index > 0) ? index - 1 : _values.Length - 1;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region ICollection Interface

        void ICollection<T>.Add(T item)
        {
            this.Push(item);
        }

        public bool Contains(T item)
        {
            return Array.IndexOf(_values, item) >= 0;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            int index = _head;
            for (int i = 0; i < _count; i++)
            {
                array[arrayIndex + 1] = _values[index];
                index = (index > 0) ? index - 1 : _values.Length - 1;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new System.NotSupportedException();
        }

        #endregion

    }
}
