using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Graphs
{
    public class LinkedNodeGraph<T> : IGraph<T>, ICollection<T> where T : INode
    {

        #region Fields

        private Dictionary<T, NeighbourInfo> _entries;

        #endregion

        #region CONSTRUCTOR

        public LinkedNodeGraph()
        {
            _entries = new Dictionary<T, NeighbourInfo>(EqualityComparer<T>.Default);
        }

        public LinkedNodeGraph(IEqualityComparer<T> comparer)
        {
            _entries = new Dictionary<T, NeighbourInfo>(comparer ?? EqualityComparer<T>.Default);
        }

        #endregion

        #region Properties

        public IEqualityComparer<T> Comparer
        {
            get { return _entries.Comparer; }
        }

        #endregion

        #region Methods

        public void Add(T node, T neighbour, bool oneway = false)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (neighbour == null) throw new ArgumentNullException("neighbour");

            NeighbourInfo info = null;
            if(_entries.TryGetValue(node, out info) && info != null)
            {
                while(info != null)
                {
                    if (_entries.Comparer.Equals(info.Value, neighbour))
                    {
                        break;
                    }
                    else if (info.Next == null)
                    {
                        info.Next = new NeighbourInfo()
                        {
                            Value = neighbour
                        };
                        break;
                    }
                    else
                    {
                        info = info.Next;
                    }
                }
            }
            else
            {
                _entries[node] = new NeighbourInfo()
                {
                    Value = neighbour
                };
            }

            if(!oneway)
            {
                this.Add(neighbour, node, true);
            }
            else if(!_entries.ContainsKey(neighbour))
            {
                _entries.Add(neighbour, null);
            }
        }
        
        public bool Remove(T node)
        {
            if (node == null) throw new ArgumentNullException("node");

            NeighbourInfo info;
            if (_entries.TryGetValue(node, out info) && _entries.Remove(node))
            {
                while(info != null)
                {
                    this.Disconnect(info.Value, node, true);
                }
                return true;
            }

            return false;
        }

        public bool Disconnect(T from, T to, bool oneway = false)
        {
            if (from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");

            if (!oneway) this.Disconnect(to, from, true);

            NeighbourInfo info;
            if (_entries.TryGetValue(from, out info) && info != null)
            {
                //if it was first, just replace first with next
                if(_entries.Comparer.Equals(info.Value, to))
                {
                    _entries[from] = info.Next;
                    return true;
                }
                var prev = info;
                info = info.Next;

                while (info != null)
                {
                    if(_entries.Comparer.Equals(info.Value, to))
                    {
                        prev.Next = info.Next;
                        return true;
                    }

                    prev = info;
                    info = info.Next;
                }
            }
            
            return false;
        }
        
        public int CountNeighbours(T node)
        {
            if (node == null) throw new ArgumentNullException("node");

            int cnt = 0;
            NeighbourInfo info;
            if (_entries.TryGetValue(node, out info))
            {
                while (info != null)
                {
                    cnt++;
                    info = info.Next;
                }
            }
            return cnt;
        }

        public bool HasNeighbour(T from, T to)
        {
            if (from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");

            NeighbourInfo info;
            if (_entries.TryGetValue(from, out info) && info != null)
            {
                while(info != null)
                {
                    if (_entries.Comparer.Equals(info.Value, to)) return true;
                }
            }

            return false;
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get
            {
                return _entries.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(T node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (_entries.ContainsKey(node)) return;

            _entries.Add(node, null);
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public bool Contains(T node)
        {
            return _entries.ContainsKey(node);
        }
        
        //bool Remove - implemented in methods

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _entries.Keys.CopyTo(array, arrayIndex);
        }

        #endregion

        #region IGraph Interface

        IEnumerable<INode> IGraph.GetNeighbours(INode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (!(node is T)) throw new NonMemberNodeException();

            NeighbourInfo info;
            if (_entries.TryGetValue((T)node, out info))
            {
                while (info != null)
                {
                    yield return info.Value;
                    info = info.Next;
                }
            }
        }

        public IEnumerable<T> GetNeighbours(T node)
        {
            if (node == null) throw new ArgumentNullException("node");

            NeighbourInfo info;
            if(_entries.TryGetValue(node, out info))
            {
                while(info != null)
                {
                    yield return info.Value;
                    info = info.Next;
                }
            }
        }

        int IGraph.GetNeighbours(INode node, ICollection<INode> buffer)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (!(node is T)) throw new NonMemberNodeException();
            if (buffer == null) throw new ArgumentNullException("buffer");

            int cnt = buffer.Count;
            NeighbourInfo info;
            if (_entries.TryGetValue((T)node, out info))
            {
                while (info != null)
                {
                    buffer.Add(info.Value);
                    info = info.Next;
                }
            }
            return buffer.Count - cnt;
        }

        public int GetNeighbours(T node, ICollection<T> buffer)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (buffer == null) throw new ArgumentNullException("buffer");

            int cnt = buffer.Count;
            NeighbourInfo info;
            if (_entries.TryGetValue(node, out info))
            {
                while (info != null)
                {
                    buffer.Add(info.Value);
                    info = info.Next;
                }
            }
            return buffer.Count - cnt;
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

            private Dictionary<T, NeighbourInfo>.KeyCollection.Enumerator _e;

            public Enumerator(LinkedNodeGraph<T> graph)
            {
                if (graph == null) throw new ArgumentNullException("graph");
                _e = graph._entries.Keys.GetEnumerator();
            }

            public T Current
            {
                get
                {
                    return _e.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _e.Current;
                }
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void IEnumerator.Reset()
            {
                //do nothing
            }
        }

        #endregion


        #region Special Types

        private class NeighbourInfo
        {
            public T Value;
            public NeighbourInfo Next;
        }
        
        #endregion

    }
}
