using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Graphs
{

    public class NodeGraph<T> : IGraph<T> where T : class, INode<T>
    {

        #region Fields

        private HashSet<T> _graph = new HashSet<T>();

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Methods

        public void Add(T node)
        {
            if (node == null) throw new System.ArgumentNullException("node");
            _graph.Add(node);
        }

        public bool Remove(T node)
        {
            if (node == null) throw new System.ArgumentNullException("node");
            return _graph.Remove(node);
        }

        #endregion

        #region IGraph Interface

        public int Count
        {
            get
            {
                return _graph.Count;
            }
        }

        public IEnumerable<T> GetNeighbours(T node)
        {
            if (!_graph.Contains(node)) throw new System.ArgumentException("Graph does not contain node.");
            return node.GetNeighbours();
        }

        public int GetNeighbours(T node, ICollection<T> buffer)
        {
            if (!_graph.Contains(node)) throw new System.ArgumentException("Graph does not contain node.");
            return node.GetNeighbours(buffer);
        }

        public bool Contains(T node)
        {
            return _graph.Contains(node);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _graph.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _graph.GetEnumerator();
        }

        #endregion

    }

}
