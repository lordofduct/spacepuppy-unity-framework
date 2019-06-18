using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Graphs
{
    public class CovariantGraph<TInner, TOuter> : IGraph<TOuter> where TInner : class, TOuter
    {

        #region Fields

        private IGraph<TInner> _graph;

        #endregion

        #region CONSTRUCTOR

        public CovariantGraph(IGraph<TInner> graph)
        {
            if (graph == null) throw new System.ArgumentNullException("graph");
            _graph = graph;
        }

        #endregion

        #region Properties

        public IGraph<TInner> InnerGraph { get { return _graph; } }

        #endregion

        #region IGraph Interface

        public int Count { get { return _graph.Count; } }

        public IEnumerable<TOuter> GetNeighbours(TOuter node)
        {
            if (node != null && !(node is TInner)) return Enumerable.Empty<TOuter>();
            return _graph.GetNeighbours(node as TInner).Cast<TOuter>();
        }

        public int GetNeighbours(TOuter node, ICollection<TOuter> buffer)
        {
            if (node != null && !(node is TInner)) return 0;

            using (var coll = com.spacepuppy.Collections.TempCollection.GetList<TInner>())
            {
                int cnt = _graph.GetNeighbours(node as TInner, coll);
                for(int i = 0; i < cnt; i++)
                {
                    buffer.Add(coll[i]);
                }
                return cnt;
            }
        }

        public bool Contains(TOuter node)
        {
            if (node != null && !(node is TInner)) return false;

            return _graph.Contains(node as TInner);
        }

        public IEnumerator<TOuter> GetEnumerator()
        {
            return _graph.Cast<TOuter>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _graph.GetEnumerator();
        }

        #endregion

        public static CovariantGraph<TInner, TOuter> Sync(ref CovariantGraph<TInner, TOuter> covariant, IGraph<TInner> inner)
        {
            if (covariant == null || covariant.InnerGraph != inner)
            {
                covariant = new CovariantGraph<TInner, TOuter>(inner);
            }

            return covariant;
        }

    }
}
