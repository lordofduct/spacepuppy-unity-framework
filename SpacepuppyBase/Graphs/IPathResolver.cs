using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Graphs
{
    public interface IPathResolver<T> where T : INode
    {

        T Start { get; set; }
        T Goal { get; set; }

        IList<T> Reduce();
        int Reduce(IList<T> path);

    }
}
