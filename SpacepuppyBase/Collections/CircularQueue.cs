using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{
    public class CircularQueue<T> : IEnumerable<T>
    {

        #region Fields

        private T[] _values;
        private int _count;
        private int _head;
        private int _rear;

        #endregion

        #region CONSTRUCTOR

        public CircularQueue(int size)
        {
            if (size < 0) throw new System.ArgumentException("Size must be non-negative.", "size");
            _values = new T[size];
            _count = 0;
            _head = 0;
            _rear = 0;
        }

        #endregion

        #region Properties

        public int Size { get { return _values.Length; } }

        public int Count { get { return _count; } }

        #endregion

        #region Methods

        public void Enqueue(T value)
        {
            if (_values.Length == 0) return;

            _values[_rear] = value;

            if (_count == _values.Length)
                _head = (_head + 1) % _values.Length;
            _rear = (_rear + 1) % _values.Length;
            _count = Math.Min(_count + 1, _values.Length);
        }

        public T Dequeue()
        {
            if (_count == 0) throw new InvalidOperationException("CircularQueue is empty.");

            T result = _values[_head];
            _values[_head] = default(T);
            _head = (_head + 1) % _values.Length;
            _count--;

            return result;
        }

        public T Peek()
        {
            if (_count == 0) throw new InvalidOperationException("CircularQueue is empty.");

            return _values[_head];
        }

        public T PopLast()
        {
            if (_count == 0) throw new InvalidOperationException("CircularQueue is empty.");

            _rear = (_rear > 0) ? _rear - 1 : _values.Length - 1;
            T result = _values[_rear];
            _values[_rear] = default(T);
            _count--;

            return result;
        }

        public T PeekLast()
        {
            if (_count == 0) throw new InvalidOperationException("CircularQueue is empty.");

            int index = (_rear > 0) ? _rear - 1 : _values.Length - 1;
            return _values[index];
        }

        public void Resize(int size)
        {
            if (size < 0) throw new System.ArgumentException("Size must be non-negative.", "size");

            if(size > _values.Length && _head < _rear)
            {
                //if growing, and the queue doesn't wrap, we can just resize
                System.Array.Resize(ref _values, size);
            }
            else if(size < _values.Length && _head < _rear && size > _rear)
            {
                //if shrinking, and the queue doesn't wrap, and the size we're resizing to still fits the queue, we can just resize
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
                    index = (index + 1) % _values.Length;
                }
                _values = arr;
                _head = 0;
                _rear = _count;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                _values[i] = default(T);
            }
            _count = 0;
            _head = 0;
            _rear = 0;
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<T> GetEnumerator()
        {
            int cnt = _count;
            int index = _head;
            for (int i = 0; i < cnt; i++)
            {
                yield return _values[index];
                index = (index + 1) % _values.Length;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }
}
