using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Graphs
{

    public class DijkstraPathResolver<T> : IPathResolver<T> where T : class, INode
    {

        #region Fields

        private IGraph<T> _graph;
        private IHeuristic<T> _heuristic;
        private BinaryHeap<VertexInfo> _open;
        private List<T> _neighbours = new List<T>();

        private T _start;
        private T _goal;

        private bool _calculating;

        #endregion

        #region CONSTRUCTOR

        public DijkstraPathResolver(IGraph<T> graph, IHeuristic<T> heuristic)
        {
            _graph = graph;
            _heuristic = heuristic;
            _open = new BinaryHeap<VertexInfo>(graph.Count, VertexComparer.Default);
        }

        #endregion

        #region Properties

        public bool IsWorking
        {
            get { return _calculating; }
        }

        #endregion

        #region Methods

        public T Start
        {
            get { return _start; }
            set
            {
                if (_calculating) throw new InvalidOperationException("Cannot update start node when calculating.");
                _start = value;
            }
        }

        public T Goal
        {
            get { return _goal; }
            set
            {
                if (_calculating) throw new InvalidOperationException("Cannot update start node when calculating.");
                _goal = value;
            }
        }

        public IList<T> Reduce()
        {
            if (_calculating) throw new InvalidOperationException("PathResolver is already running.");
            if (_graph == null || _heuristic == null || _start == null || _goal == null) throw new InvalidOperationException("PathResolver is not initialized.");

            var lst = new List<T>();
            this.Reduce(lst);
            return lst;
        }

        public int Reduce(IList<T> path)
        {
            if (_calculating) throw new InvalidOperationException("PathResolver is already running.");
            if (_graph == null || _heuristic == null || _start == null || _goal == null) throw new InvalidOperationException("PathResolver is not initialized.");

            _calculating = true;

            try
            {
                _open.Clear();
                _neighbours.Clear();

                foreach (var n in _graph)
                {
                    float d = n == _goal ? 0f : float.PositiveInfinity;
                    _open.Add(new VertexInfo()
                    {
                        Node = n,
                        Next = null,
                        Distance = d
                    });
                }

                while (_open.Count > 0)
                {
                    var u = _open.Pop();
                    if (u.Node == _start)
                    {
                        int cnt = 0;
                        while (u.Next != null)
                        {
                            path.Add(u.Node);
                            u = u.Next;
                            cnt++;
                        }
                        path.Add(u.Node);
                        return cnt + 1;
                    }

                    _graph.GetNeighbours(u.Node, _neighbours);
                    var e = _neighbours.GetEnumerator();
                    while (e.MoveNext())
                    {
                        var n = e.Current;
                        var index = GetInfo(_open, n);
                        if (index < 0) continue;

                        var d = u.Distance + _heuristic.Distance(u.Node, n) + _heuristic.Weight(n);
                        if (d < _open[index].Distance)
                        {
                            _open[index].Distance = d;
                            _open[index].Next = u;
                            _open.Update(index);
                        }
                    }
                    _neighbours.Clear();
                }

                return 0;
            }
            finally
            {
                _open.Clear();
                _calculating = false;
            }
        }

        private static int GetInfo(BinaryHeap<VertexInfo> heap, T node)
        {
            for (int i = 0; i < heap.Count; i++)
            {
                if (heap[i].Node == node) return i;
            }
            return -1;
        }

        #endregion

        #region Special Types

        private class VertexInfo
        {
            public T Node;
            public VertexInfo Next;
            public float Distance;
        }

        private class VertexComparer : IComparer<VertexInfo>
        {
            public readonly static VertexComparer Default = new VertexComparer();

            public int Compare(VertexInfo x, VertexInfo y)
            {
                return y.Distance.CompareTo(x.Distance);
            }
        }

        #endregion

    }

}
