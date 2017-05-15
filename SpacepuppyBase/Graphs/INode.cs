using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Graphs
{

    /// <summary>
    /// Contract for a graph node that can determine its own neighbours.
    /// 
    /// When implementing this contract T should be typed as itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INode<T> where T : INode<T>
    {

        IEnumerable<T> GetNeighbours();
        int GetNeighbours(ICollection<T> buffer);
    }

}
