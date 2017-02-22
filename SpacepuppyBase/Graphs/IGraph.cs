using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Graphs
{

    public interface IGraph : System.Collections.IEnumerable
    {

        int Count { get; }

        IEnumerable<INode> GetNeighbours(INode node);
        int GetNeighbours(INode node, ICollection<INode> buffer);

    }

    public interface IGraph<T> : IGraph, IEnumerable<T> where T : INode
    {

        IEnumerable<T> GetNeighbours(T node);
        int GetNeighbours(T node, ICollection<T> buffer);

    }
}
