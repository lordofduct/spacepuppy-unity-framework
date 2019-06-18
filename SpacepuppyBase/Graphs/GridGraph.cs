using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Graphs
{

    /// <summary>
    /// Represents a 2d-grid that can be accessed as an IGraph.
    /// 
    /// Its nodes can be accessed by index from 0->Area, traversing over width by height.
    /// 
    /// Or its nodes can be accessed in an x,y grid where x correlates to a column along the width, 
    /// and y correlates to a row along the length. If x or y are out of range default(T) is returned. 
    /// This is as opposed to index access which throws exceptions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridGraph<T> : IGraph<T>, IList<T>
    {
        
        #region Fields

        private int _rowCount;
        private int _colCount;
        private T[] _data;
        private IEqualityComparer<T> _comparer;
        private bool _includeDiagonals;

        #endregion

        #region CONSTRUCTOR
        
        public GridGraph(int width, int length, bool includeDiagonals)
        {
            _rowCount = length;
            _colCount = width;
            _data = new T[Math.Max(_rowCount * _colCount, 0)];
            _includeDiagonals = includeDiagonals;
            _comparer = EqualityComparer<T>.Default;
        }

        public GridGraph(int width, int length, bool includeDiagonals, IEqualityComparer<T> comparer)
        {
            _rowCount = length;
            _colCount = width;
            _data = new T[Math.Max(_rowCount * _colCount, 0)];
            _includeDiagonals = includeDiagonals;
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The width of the grid
        /// </summary>
        public int Width
        {
            get { return _colCount; }
        }

        /// <summary>
        /// The height of the grid
        /// </summary>
        public int Length
        {
            get { return _rowCount; }
        }

        /// <summary>
        /// If diagonals are considered neighbours when calling 'GetNeighbours'. 
        /// Calling GetNeighbour/s with a GridNeighbour enum ignores this property.
        /// </summary>
        public bool IncludeDiagonals
        {
            get { return _includeDiagonals; }
            set { _includeDiagonals = value; }
        }

        /// <summary>
        /// Access a node by its x,y position in the grid.
        /// Returns null if out of range.
        /// </summary>
        /// <param name="x">The position along the width</param>
        /// <param name="y">The position along the length</param>
        /// <returns></returns>
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

        /// <summary>
        /// Comparer used when comparing nodes.
        /// </summary>
        public IEqualityComparer<T> Comparer
        {
            get { return _comparer; }
        }

        #endregion

        #region Methods
        
        public bool GetXYOfIndex(int index, out int x, out int y)
        {
            if(index < 0)
            {
                x = -1;
                y = -1;
                return false;
            }
            else
            {
                x = index % _colCount;
                y = index / _colCount;
                return true;
            }
        }

        public bool GetXY(T node, out int x, out int y)
        {
            int index = this.IndexOf(node);
            if(index < 0)
            {
                x = -1;
                y = -1;
                return false;
            }
            else
            {
                x = index % _colCount;
                y = index / _colCount;
                return true;
            }
        }

        public T GetNeighbour(int x, int y, GridNeighbour side)
        {
            switch (side)
            {
                case GridNeighbour.North:
                    return this[x, y + 1];
                case GridNeighbour.NE:
                    return this[x + 1, y + 1];
                case GridNeighbour.East:
                    return this[x + 1, y];
                case GridNeighbour.SE:
                    return this[x + 1, y - 1];
                case GridNeighbour.South:
                    return this[x, y - 1];
                case GridNeighbour.SW:
                    return this[x - 1, y - 1];
                case GridNeighbour.West:
                    return this[x - 1, y];
                case GridNeighbour.NW:
                    return this[x - 1, y + 1];
                default:
                    return default(T);
            }
        }

        public T GetNeighbourOfIndex(int index, GridNeighbour side)
        {
            return GetNeighbour(index % _colCount, index / _colCount, side);
        }

        public T GetNeighbour(T node, GridNeighbour side)
        {
            if (node == null) throw new ArgumentNullException("node");

            int index = this.IndexOf(node);
            if (index < 0) return default(T);

            return GetNeighbour(index % _colCount, index / _colCount, side);
        }
        


        public IEnumerable<T> GetNeighboursOfIndex(int index)
        {
            if (index < 0 || index >= _data.Length) throw new IndexOutOfRangeException();
            return this.GetNeighbours(index % _colCount, index / _colCount);
        }

        public IEnumerable<T> GetNeighbours(int x, int y)
        {
            if (x >= 0 && x < _colCount && y > -1 && y < _rowCount - 1)
                yield return this[x, y + 1]; //n
            if (_includeDiagonals && x >= -1 && x < _colCount - 1 && y > -1 && y < _rowCount - 1)
                yield return this[x + 1, y + 1]; //ne
            if (x >= -1 && x < _colCount - 1 && y >= 0 && y < _rowCount)
                yield return this[x + 1, y]; //e
            if (_includeDiagonals && x >= -1 && x < _colCount - 1 && y > 0 && y < _rowCount)
                yield return this[x + 1, y - 1]; //se
            if (x >= 0 && x < _colCount && y > 0 && y <= _rowCount)
                yield return this[x, y - 1]; //s
            if (_includeDiagonals && x > 0 && x <= _colCount && y > 0 && y <= _rowCount)
                yield return this[x - 1, y - 1]; //sw
            if (x > 0 && x <= _colCount && y >= 0 && y < _rowCount)
                yield return this[x - 1, y]; //w
            if (_includeDiagonals && x > 0 && x <= _colCount && y > -1 && y < _rowCount - 1)
                yield return this[x - 1, y + 1]; //nw
        }

        public int GetNeighboursOfIndex(int index, ICollection<T> buffer)
        {
            if (index < 0 || index >= _data.Length) throw new IndexOutOfRangeException();
            return this.GetNeighbours(index % _colCount, index / _colCount, buffer);
        }

        public int GetNeighbours(int x, int y, ICollection<T> buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");

            int cnt = buffer.Count;
            if (x >= 0 && x < _colCount && y > -1 && y < _rowCount - 1)
                buffer.Add(this[x, y + 1]); //n
            if (_includeDiagonals && x >= -1 && x < _colCount - 1 && y > -1 && y < _rowCount - 1)
                buffer.Add(this[x + 1, y + 1]); //ne
            if (x >= -1 && x < _colCount - 1 && y >= 0 && y < _rowCount)
                buffer.Add(this[x + 1, y]); //e
            if (_includeDiagonals && x >= -1 && x < _colCount - 1 && y > 0 && y < _rowCount)
                buffer.Add(this[x + 1, y - 1]); //se
            if (x >= 0 && x < _colCount && y > 0 && y <= _rowCount)
                buffer.Add(this[x, y - 1]); //s
            if (_includeDiagonals && x > 0 && x <= _colCount && y > 0 && y <= _rowCount)
                buffer.Add(this[x - 1, y - 1]); //sw
            if (x > 0 && x <= _colCount && y >= 0 && y < _rowCount)
                buffer.Add(this[x - 1, y]); //w
            if (_includeDiagonals && x > 0 && x <= _colCount && y > -1 && y < _rowCount - 1)
                buffer.Add(this[x - 1, y + 1]); //nw

            return buffer.Count - cnt;
        }



        public IEnumerable<T> GetNeighbours(int x, int y, GridNeighbour sides)
        {
            int e = (int)sides;
            for (int i = 0; i < 8; i++)
            {
                int f = 1 << i;
                if ((e & (1 << i)) != 0) yield return this.GetNeighbour(x, y, (GridNeighbour)f);
            }
        }

        public int GetNeighbours(int x, int y, GridNeighbour sides, ICollection<T> buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");

            int cnt = 0;
            int e = (int)sides;
            for (int i = 0; i < 8; i++)
            {
                int f = 1 << i;
                if ((e & (1 << i)) != 0)
                {
                    buffer.Add(this.GetNeighbour(x, y, (GridNeighbour)f));
                    cnt++;
                }
            }
            return cnt;
        }

        public IEnumerable<T> GetNeighboursOfIndex(int index, GridNeighbour sides)
        {
            return GetNeighbours(index % _colCount, index / _colCount, sides);
        }

        public int GetNeighboursOfIndex(int index, GridNeighbour sides, ICollection<T> buffer)
        {
            return GetNeighbours(index % _colCount, index / _colCount, sides, buffer);
        }

        public IEnumerable<T> GetNeighbours(T node, GridNeighbour sides) 
        {
            if (node == null) return Enumerable.Empty<T>();

            int index = this.IndexOf(node);
            if (index < 0) return Enumerable.Empty<T>();

            return GetNeighbours(index % _colCount, index / _colCount, sides);
        }

        public int GetNeighbours(T node, GridNeighbour sides, ICollection<T> buffer)
        {
            if (node == null) return 0;

            int index = this.IndexOf(node);
            if (index < 0) return 0;

            return GetNeighbours(index % _colCount, index / _colCount, sides, buffer);
        }



        public IEnumerable<int> GetNeighbourIndicesOfIndex(int index)
        {
            int tx, ty;
            this.GetXYOfIndex(index, out tx, out ty);

            return this.GetNeighbourIndices(tx, ty);
        }

        public IEnumerable<int> GetNeighbourIndices(int x, int y)
        {
            if (x >= 0 && x < _colCount && y > -1 && y < _rowCount - 1)
                yield return (y + 1) * _colCount + x; // this[x, y + 1]; //n
            if (_includeDiagonals && x >= -1 && x < _colCount - 1 && y > -1 && y < _rowCount - 1)
                yield return (y + 1) * _colCount + (x + 1); // this[x + 1, y + 1]; //ne
            if (x >= -1 && x < _colCount - 1 && y >= 0 && y < _rowCount)
                yield return y * _colCount + (x + 1); //this[x + 1, y]; //e
            if (_includeDiagonals && x >= -1 && x < _colCount - 1 && y > 0 && y < _rowCount)
                yield return (y - 1) * _colCount + (x + 1); // this[x + 1, y - 1]; //se
            if (x >= 0 && x < _colCount && y > 0 && y <= _rowCount)
                yield return (y - 1) * _colCount + x; // this[x, y - 1]; //s
            if (_includeDiagonals && x > 0 && x <= _colCount && y > 0 && y <= _rowCount)
                yield return (y - 1) * _colCount + (x - 1); // this[x - 1, y - 1]; //sw
            if (x > 0 && x <= _colCount && y >= 0 && y < _rowCount)
                yield return y * _colCount + (x - 1); // this[x - 1, y]; //w
            if (_includeDiagonals && x > 0 && x <= _colCount && y > -1 && y < _rowCount - 1)
                yield return (y + 1) * _colCount + (x - 1); // this[x - 1, y + 1]; //nw
        }


        /// <summary>
        /// Calculates which side of 'from' that 'to' is on.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public GridNeighbour GetSide(T from, T to)
        {
            int index = this.IndexOf(from);
            if (index < 0) return GridNeighbour.None;
            if (!this.Contains(to)) return GridNeighbour.None;

            int x = index % _colCount;
            int y = index / _colCount;

            for(int i = 0; i < 8; i++)
            {
                GridNeighbour f = (GridNeighbour)(1 << i);
                if (_comparer.Equals(this.GetNeighbour(x, y, f), to)) return f;
            }

            return GridNeighbour.None;
        }

        public GridNeighbour GetSideOfIndex(int from, int to)
        {
            if (from < 0 || from >= this.Count) return GridNeighbour.None;
            if (to < 0 || to >= this.Count) return GridNeighbour.None;

            int fx = from % _colCount;
            int fy = from / _colCount;

            int tx = to % _colCount;
            int ty = to / _colCount;

            
            if(tx == fx)
            {
                if (ty == fy - 1)
                    return GridNeighbour.South;
                else if (ty == fy + 1)
                    return GridNeighbour.North;
            }
            else if (tx == fx - 1)
            {
                if (ty == fy)
                    return GridNeighbour.West;
                else if (ty == fy - 1)
                    return GridNeighbour.SW;
                else if (ty == fy + 1)
                    return GridNeighbour.NW;
            }
            else if (tx == fx + 1)
            {
                if (ty == fy)
                    return GridNeighbour.East;
                else if (ty == fy - 1)
                    return GridNeighbour.SE;
                else if (ty == fy + 1)
                    return GridNeighbour.NE;
            }

            return GridNeighbour.None;
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
            Array.Clear(_data, 0, _data.Length);
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

        int IGraph<T>.Count
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
        
        public int GetNeighbours(T node, ICollection<T> buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");

            int index = this.IndexOf(node);
            if (index < 0) return 0;

            return this.GetNeighbours(index % _colCount, index / _colCount, buffer);
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
