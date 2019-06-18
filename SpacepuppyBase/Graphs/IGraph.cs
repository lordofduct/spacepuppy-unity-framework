using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Graphs
{

    public interface IGraph<T> : IEnumerable<T>
    {

        int Count { get; }

        IEnumerable<T> GetNeighbours(T node);
        int GetNeighbours(T node, ICollection<T> buffer);
        bool Contains(T node);

    }
}
