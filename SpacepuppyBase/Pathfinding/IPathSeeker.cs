using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Pathfinding
{
    public interface IPathSeeker
    {

        IPath CreatePath();
        /// <summary>
        /// Returns true if the path can be used when calculating a path with this IPathSeeker.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool ValidPath(IPath path);

        void CalculatePath(Vector3 target, IPath path);
        void CalculatePath(Vector3 start, Vector3 target, IPath path);

    }

    public interface IPathFollower
    {

        void SetPath(IPath path);

        /// <summary>
        /// Resets the automatic path, clearing it, and stopping all motion.
        /// </summary>
        void ResetPath();

        /// <summary>
        /// Stop pathing to target.
        /// </summary>
        void StopPath();

        /// <summary>
        /// Resume pathing to target
        /// </summary>
        void ResumePath();

    }

    /// <summary>
    /// Contract that combines both seeker and follower
    /// </summary>
    public interface IPathAgent : IPathSeeker, IPathFollower
    {

        /// <summary>
        /// Start automatically pathing to target.
        /// </summary>
        /// <param name="target"></param>
        void PathTo(Vector3 target);

    }

}
