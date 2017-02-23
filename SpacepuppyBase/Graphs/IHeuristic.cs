using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Graphs
{
    public interface IHeuristic<T> where T : INode
    {
        float Weight(T n);
        float Distance(T x, T y);
    }

    public class ComponentHeuristic<T> : IHeuristic<T> where T : INode, IComponent
    {

        private static ComponentHeuristic<T> _default;
        public static ComponentHeuristic<T> Default
        {
            get
            {
                if (_default == null) _default = new ComponentHeuristic<T>();
                return _default;
            }
        }


        public virtual float Weight(T n)
        {
            return 0f;
        }

        public virtual float Distance(T x, T y)
        {
            return (x.transform.position - y.transform.position).sqrMagnitude;
        }

    }

}
