using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Graphs
{
    public interface IPathResolver<T>
    {

        T Start { get; set; }
        T Goal { get; set; }

        IList<T> Reduce();
        int Reduce(IList<T> path);

    }

    public interface ISteppingPathResolver<T> : IPathResolver<T>
    {
        /// <summary>
        /// Start the stepping path resolver for reducing.
        /// </summary>
        void BeginSteppedReduce();
        /// <summary>
        /// Take a step at reducing the path resolver.
        /// </summary>
        /// <returns>Returns true if reached goal.</returns>
        bool Step();
        /// <summary>
        /// Get the result of reducing the path.
        /// </summary>
        /// <param name="path"></param>
        int EndSteppedReduce(IList<T> path);
        /// <summary>
        /// Reset the resolver so a new Step sequence could be started.
        /// </summary>
        void Reset();
    }
}
