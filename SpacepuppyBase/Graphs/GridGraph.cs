using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Graphs
{

    /// <summary>
    /// Represents a 2d-grid that can be accessed as an IGraph.
    /// 
    /// Its nodes can be accessed by index from 0->Area, traversing over width by height.
    /// 
    /// Or its nodes can be accessed in an x,y grid where x correlates to a column along the width, 
    /// and y correlates to a row along the height. If x or y are out of range default(T) is returned. 
    /// This is as opposed to index access which throws exceptions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridGraph<T> : IGraph<T>, IList<T> where T : INode
    {
        
        #region Fields

        private int _rowCount;
        private int _colCount;
        private T[] _data;
        private IEqualityComparer<T> _comparer;

        #endregion

        #region CONSTRUCTOR

        public GridGraph(int width, int height)
        {
            _rowCount = height;
            _colCount = width;
            _data = new T[Math.Max(_rowCount * _colCount, 0)];
            _comparer = EqualityComparer<T>.Default;
        }

        public GridGraph(int width, int height, IEqualityComparer<T> comparer)
        {
            _rowCount = height;
            _colCount = width;
            _data = new T[Math.Max(_rowCount * _colCount, 0)];
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        #endregion

        #region Properties

        public int Width
        {
            get { return _colCount; }
        }

        public int Height
        {
            get { return _rowCount; }
        }

        public T this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= _colCount) return default(T);
                if (y < 0 || y >= _rowCount) return default(T);

                return _data[y * _colCount + x];
            }
            set
            {
                if (x < 0 || x >= _colCount) return;
                if (y < 0 || y >= _rowCount) return;

                _data[y * _colCount + x] = value;
            }
        }

        public IEqualityComparer<T> Comparer
        {
            get { return _comparer; }
        }

        #endregion

        #region Methods

        public IEnumerable<T> GetNeighbours(int index)
        {
            if (index < 0 || index >= _data.Length) throw new IndexOutOfRangeException();
            return this.GetNeighbours(index % _colCount, index / _colCount);
        }

        public IEnumerable<T> GetNeighbours(int x, int y)
        {
            if (x >= 0 && x < _colCount && y > -1 && y < _rowCount - 1)
                yield return this[x, y + 1];
            if (x >= -1 && x < _colCount - 1 && y >= 0 && y < _rowCount)
                yield return this[x + 1, y];
            if (x >= 0 && x < _colCount && y > 0 && y <= _rowCount)
                yield return this[x, y - 1];
            if (x > 0 && x <= _rowCount && y >= 0 && y < _rowCount)
                yield return this[x - 1, y];
        }

        public int GetNeighbours(int index, ICollection<T> buffer)
        {
            if (index < 0 || index >= _data.Length) throw new IndexOutOfRangeException();
            return this.GetNeighbours(index % _colCount, index / _colCount, buffer);
        }

        public int GetNeighbours(int x, int y, ICollection<T> buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");

            int cnt = buffer.Count;
            if (x >= 0 && x < _colCount && y > -1 && y < _rowCount - 1)
                buffer.Add(this[x, y + 1]);
            if (x >= -1 && x < _colCount - 1 && y >= 0 && y < _rowCount)
                buffer.Add(this[x + 1, y]);
            if (x >= 0 && x < _colCount && y > 0 && y <= _rowCount)
                buffer.Add(this[x, y - 1]);
            if (x > 0 && x <= _rowCount && y >= 0 && y < _rowCount)
                buffer.Add(this[x - 1, y]);
            return buffer.Count - cnt;
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
            for(int i = 0; i < _data.Length; i++)
            {
                if (_comparer.Equals(_data[i], item)) return i;
            }
            return -1;
        }

        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = default(T);
            }
        }

        public bool Contains(T item)
        {
            for(int i = 0; i < _data.Length; i++)
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

        int IGraph.Count
        {
            get { return _data.Length; }
        }

        public IEnumerable<T> GetNeighbours(T node)
        {
            if (node == null) throw new ArgumentNullException("node");

            int index = this.IndexOf(node);
            if (index < 0) return Enumerable.Empty<T>();

            return this.GetNeighbours(index % _colCount, index / _colCount);
        }

        IEnumerable<INode> IGraph.GetNeighbours(INode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (!(node is T)) throw new NonMemberNodeException();

            int index = this.IndexOf((T)node);
            if (index < 0) return Enumerable.Empty<INode>();

            return this.GetNeighbours(index % _colCount, index / _colCount).Cast<INode>();
        }
        
        public int GetNeighbours(T node, ICollection<T> buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");

            int index = this.IndexOf(node);
            if (index < 0) return 0;

            return this.GetNeighbours(index % _colCount, index / _colCount, buffer);
        }

        int IGraph.GetNeighbours(INode node, ICollection<INode> buffer)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (!(node is T)) throw new NonMemberNodeException();
            if (buffer == null) throw new ArgumentNullException("buffer");

            int index = this.IndexOf((T)node);
            if (index < 0) return 0;

            int x = index % _colCount;
            int y = index / _colCount;
            int cnt = buffer.Count;
            if (x >= 0 && x < _colCount && y > -1 && y < _rowCount - 1)
                buffer.Add(this[x, y + 1]);
            if (x >= -1 && x < _colCount - 1 && y >= 0 && y < _rowCount)
                buffer.Add(this[x + 1, y]);
            if (x >= 0 && x < _colCount && y > 0 && y <= _rowCount)
                buffer.Add(this[x, y - 1]);
            if (x > 0 && x <= _rowCount && y >= 0 && y < _rowCount)
                buffer.Add(this[x - 1, y]);
            return buffer.Count - cnt;
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

            public Enumerator(GridGraph<T> graph)
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
