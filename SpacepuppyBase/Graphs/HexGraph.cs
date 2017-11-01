using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Graphs
{
    public class HexGraph<T> : IGraph<T>, IList<T>
    {

        #region Fields

        private int _rowCount;
        private int _nominalWidth;
        private int _doubleWidth;
        private T[] _data;
        private IEqualityComparer<T> _comparer;

        #endregion

        #region CONSTRUCTOR

        public HexGraph(int rowCount, int nominalWidth)
        {
            _rowCount = rowCount;
            _nominalWidth = nominalWidth;
            _doubleWidth = _nominalWidth + _nominalWidth - 1;
            int cnt = (int)Math.Ceiling((double)_doubleWidth * (double)rowCount / 2d);
            _data = new T[Math.Max(cnt, 0)];
            _comparer = EqualityComparer<T>.Default;
        }

        public HexGraph(int rowCount, int nominalWidth, IEqualityComparer<T> comparer)
        {
            _rowCount = rowCount;
            _nominalWidth = nominalWidth;
            _doubleWidth = _nominalWidth + _nominalWidth - 1;
            int cnt = (int)Math.Ceiling((double)_doubleWidth * (double)rowCount / 2d);
            _data = new T[Math.Max(cnt, 0)];
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        #endregion

        #region Properties

        public int RowCount
        {
            get { return _rowCount; }
        }

        public int NominalWidth
        {
            get { return _nominalWidth; }
        }

        public T this[int row, int index]
        {
            get
            {
                if (row % 2 == 0)
                {
                    //if even row
                    return _data[(_doubleWidth * row / 2) + index];
                }
                else
                {
                    //if odd row
                    return _data[(_doubleWidth * (row - 1) / 2) + _nominalWidth + index];
                }
            }
            set
            {
                if (row % 2 == 0)
                {
                    //if even row
                    _data[(_doubleWidth * row / 2) + index] = value;
                }
                else
                {
                    //if odd row
                    _data[(_doubleWidth * (row - 1) / 2) + _nominalWidth + index] = value;
                }
            }
        }

        #endregion

        #region Methods

        public IEnumerable<T> GetNeighbours(int row, int index)
        {
            if (row % 2 == 0)
            {
                //if even row
                return this.GetNeighbours((_doubleWidth * row / 2) + index);
            }
            else
            {
                //if odd row
                return this.GetNeighbours((_doubleWidth * (row - 1) / 2) + _nominalWidth + index);
            }
        }

        public int GetNeighbours(int row, int index, ICollection<T> buffer)
        {
            if (row % 2 == 0)
            {
                //if even row
                return this.GetNeighbours((_doubleWidth * row / 2) + index, buffer);
            }
            else
            {
                //if odd row
                return this.GetNeighbours((_doubleWidth * (row - 1) / 2) + _nominalWidth + index, buffer);
            }
        }

        public IEnumerable<T> GetNeighbours(int index)
        {
            if (index < 0 || index >= _data.Length) yield break;

            int row = index / _doubleWidth;
            int rowIndex = index % _doubleWidth;
            if (rowIndex >= _nominalWidth)
            {
                row++;
                rowIndex -= _nominalWidth;
            }
            int rowWidth = (row % 2 == 0) ? _nominalWidth : _nominalWidth - 1;

            int low = _nominalWidth - 1;
            int high = _nominalWidth;
            int i;

            i = index - high;
            if (i >= 0) yield return _data[i];
            i = index - low;
            if (i >= 0) yield return _data[i];
            if (rowIndex > 0) yield return _data[index - 1];
            if (rowIndex < rowWidth - 1) yield return _data[index + 1];
            i = index + low;
            if (i < _data.Length) yield return _data[i];
            i = index + high;
            if (i < _data.Length) yield return _data[i];
        }

        public int GetNeighbours(int index, ICollection<T> buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (index < 0 || index >= _data.Length) return 0;

            int row = index / _doubleWidth;
            int rowIndex = index % _doubleWidth;
            if (rowIndex >= _nominalWidth)
            {
                row++;
                rowIndex -= _nominalWidth;
            }
            int rowWidth = (row % 2 == 0) ? _nominalWidth : _nominalWidth - 1;

            int low = _nominalWidth - 1;
            int high = _nominalWidth;
            int i;
            int cnt = 0;

            i = index - high;
            if (i >= 0)
            {
                cnt++;
                buffer.Add(_data[i]);
            }
            i = index - low;
            if (i >= 0)
            {
                cnt++;
                buffer.Add(_data[i]);
            }
            if (rowIndex > 0)
            {
                cnt++;
                buffer.Add(_data[index - 1]);
            }
            if (rowIndex < rowWidth - 1)
            {
                cnt++;
                buffer.Add(_data[index + 1]);
            }
            i = index + low;
            if (i < _data.Length)
            {
                cnt++;
                buffer.Add(_data[i]);
            }
            i = index + high;
            if (i < _data.Length)
            {
                cnt++;
                buffer.Add(_data[i]);
            }

            return cnt;
        }

        private void GetCoords(int index, out int row, out int rowIndex)
        {
            row = index / _doubleWidth;
            rowIndex = index % _doubleWidth;
            if (rowIndex >= _nominalWidth)
            {
                row++;
                rowIndex -= _nominalWidth;
            }
        }

        #endregion

        #region IList Interface

        public T this[int index]
        {
            get
            {
                return _data[index];
            }

            set
            {
                _data[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return _data.Length;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }


        public int IndexOf(T item)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                if (_comparer.Equals(_data[i], item)) return i;
            }
            return -1;
        }

        public void Clear()
        {
            Array.Clear(_data, 0, _data.Length);
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                if (_comparer.Equals(_data[i], item)) return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            System.Array.Copy(_data, 0, array, arrayIndex, _data.Length);
        }

        public bool Remove(T item)
        {
            if (item == null) return false;

            bool result = false;
            for (int i = 0; i < _data.Length; i++)
            {
                if (_comparer.Equals(_data[i], item))
                {
                    result = true;
                    _data[i] = default(T);
                }
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            _data[index] = default(T);
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new System.NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new System.NotSupportedException();
        }

        #endregion

        #region IGraph Interface

        int IGraph<T>.Count
        {
            get { return _data.Length; }
        }

        public IEnumerable<T> GetNeighbours(T node)
        {
            if (node == null) throw new ArgumentNullException("node");

            int index = this.IndexOf(node);
            if (index < 0) return Enumerable.Empty<T>();
            
            return this.GetNeighbours(index);
        }

        public int GetNeighbours(T node, ICollection<T> buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");

            int index = this.IndexOf(node);
            if (index < 0) return 0;
            
            return this.GetNeighbours(index, buffer);
        }

        #endregion

        #region IEnumerable Interface

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<T>
        {

            private T[] _graph;
            private int _index;
            private T _current;

            public Enumerator(HexGraph<T> graph)
            {
                if (graph == null) throw new ArgumentNullException("graph");
                _graph = graph._data;
                _index = 0;
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
                _graph = null;
                _index = 0;
                _current = default(T);
            }

            public bool MoveNext()
            {
                if (_graph == null) return false;
                if (_index >= _graph.Length) return false;

                _current = _graph[_index];
                _index++;
                return true;
            }

            public void Reset()
            {
                _index = 0;
                _current = default(T);
            }
        }

        #endregion

    }
}
