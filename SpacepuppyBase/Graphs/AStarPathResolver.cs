using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Graphs
{

    public class AStarPathResolver<T> : IPathResolver<T> where T : class, INode
    {

        #region Fields

        private IGraph<T> _graph;
        private IHeuristic<T> _heuristic;

        private BinaryHeap<VertexInfo> _open;
        private HashSet<T> _closed = new HashSet<T>();
        private HashSet<VertexInfo> _tracked = new HashSet<VertexInfo>();
        private List<T> _neighbours = new List<T>();

        private T _start;
        private T _goal;

        private bool _calculating;

        #endregion

        #region CONSTRUCTOR

        public AStarPathResolver(IGraph<T> graph, IHeuristic<T> heuristic)
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

        #region IPathResolver Interface

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

            //calculate backwards
            var start = _goal;
            var goal = _start;

            try
            {
                _open.Clear();
                _closed.Clear();
                _tracked.Clear();
                _neighbours.Clear();

                _open.Add(this.CreateInfo(start, _heuristic.Weight(start), goal));

                while (_open.Count > 0)
                {
                    var u = _open.Pop();

                    if (u.Node == goal)
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

                    _closed.Add(u.Node);

                    _graph.GetNeighbours(u.Node, _neighbours);
                    var e = _neighbours.GetEnumerator();
                    while(e.MoveNext())
                    {
                        var n = e.Current;
                        if (_closed.Contains(n)) continue;

                        float g = u.g + _heuristic.Distance(u.Node, n) + _heuristic.Weight(n);

                        int i = GetInfo(_open, n);
                        if (i < 0)
                        {
                            var v = this.CreateInfo(n, g, goal);
                            v.Next = u;
                            _open.Add(v);
                        }
                        else if (g < _open[i].g)
                        {
                            var v = _open[i];
                            v.Next = u;
                            v.g = g;
                            v.f = g + v.h;
                            _open.Update(i);
                        }
                    }
                    _neighbours.Clear();
                }

            }
            finally
            {
                var e = _tracked.GetEnumerator();
                while(e.MoveNext())
                {
                    _pool.Release(e.Current);
                }
                _open.Clear();
                _closed.Clear();
                _tracked.Clear();
                _calculating = false;
            }

            return 0;
        }


        private VertexInfo CreateInfo(T node, float g, T goal)
        {
            var v = _pool.GetInstance();
            v.Node = node;
            v.Next = null;
            v.g = g;
            v.h = _heuristic.Distance(node, goal);
            v.f = g + v.f;
            _tracked.Add(v);
            return v;
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

        private static ObjectCachePool<VertexInfo> _pool = new ObjectCachePool<VertexInfo>(-1, () => new VertexInfo(), (v) =>
        {
            v.Node = null;
            v.Next = null;
            v.g = 0f;
            v.h = 0f;
            v.f = 0f;
        });

        private class VertexInfo
        {
            public T Node;
            public VertexInfo Next;
            public float g;
            public float h;
            public float f;
        }

        private class VertexComparer : IComparer<VertexInfo>
        {
            public readonly static VertexComparer Default = new VertexComparer();

            public int Compare(VertexInfo x, VertexInfo y)
            {
                return y.f.CompareTo(x.f);
            }
        }

        #endregion

    }
}
