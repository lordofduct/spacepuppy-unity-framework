using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Pathfinding
{
    public static class PathUtil
    {

        public static bool IsDone(this IPath path)
        {
            return path.Status != PathCalculateStatus.Uncalculated;
        }

        /// <summary>
        /// Gets the next target waypoint after currentIndex. Updating currentIndex if you've passed it.
        /// </summary>
        /// <param name="path">The path to find the next target on.</param>
        /// <param name="currentPosition">The current position of the agent</param>
        /// <param name="currentIndex">The index of the waypoint that was last targeted, 0 if this is first call</param>
        /// <returns>Returns the target position or a NaN vector if no target found.</returns>
        public static Vector3 GetNextTarget(this IPath path, Vector3 currentPosition, ref int currentIndex)
        {
            if (path == null) throw new System.ArgumentNullException("path");

            var waypoints = path.Waypoints;
            if (waypoints == null || waypoints.Count == 0) return VectorUtil.NaNVector3;
            if (currentIndex >= waypoints.Count - 1) return path.Waypoints[path.Waypoints.Count - 1];

            if (currentIndex < 0)
                currentIndex = 0;

            var targ = path.Waypoints[currentIndex];
            if (currentIndex == path.Waypoints.Count - 1)
            {
                return targ;
            }

            var dir1 = targ - currentPosition;
            var dir2 = waypoints[currentIndex + 1] - targ;

            if (Vector3.Dot(dir1, dir2) <= 0f)
            {
                currentIndex++;
                targ = path.Waypoints[currentIndex];
            }

            return targ;
        }

        /// <summary>
        /// Finds the best next target by finding the nearest waypoint to the 
        /// currentPosition after currentIndex, then calculates the next target 
        /// from there.
        /// 
        /// This is beneficial if you're starting from an arbitrary point along the path.
        /// 
        /// This is slower than GetNextTarget, use that if you know your starting position.
        /// </summary>
        /// <param name="path">The path to find the next target on.</param>
        /// <param name="currentPosition">The current position of the agent</param>
        /// <returns>Returns the target position or a NaN vector if no target found.</returns>
        public static Vector3 GetBestTarget(this IPath path, Vector3 currentPosition)
        {
            int index = 0;
            return GetBestTarget(path, currentPosition, ref index);
        }

        /// <summary>
        /// Finds the best next target by finding the nearest waypoint to the 
        /// currentPosition after currentIndex, then calculates the next target 
        /// from there.
        /// 
        /// This is beneficial if you're starting from an arbitrary point along the path.
        /// 
        /// This is slower than GetNextTarget, use that if you know your starting position.
        /// </summary>
        /// <param name="path">The path to find the next target on.</param>
        /// <param name="currentPosition">The current position of the agent</param>
        /// <param name="currentIndex">The index to start searching from, pass in 0 to search entire path, 
        /// method will return this value with the selected best target</param>
        /// <returns>Returns the target position or a NaN vector if no target found.</returns>
        public static Vector3 GetBestTarget(this IPath path, Vector3 currentPosition, ref int currentIndex)
        {
            if (path == null) throw new System.ArgumentNullException("path");

            var waypoints = path.Waypoints;
            if (waypoints == null || waypoints.Count == 0) return VectorUtil.NaNVector3;
            if (currentIndex >= waypoints.Count - 1) return path.Waypoints[path.Waypoints.Count - 1];

            if (currentIndex < 0)
                currentIndex = 0;

            float dist = float.PositiveInfinity;
            for (int i = currentIndex; i < waypoints.Count; i++)
            {
                var d = (waypoints[i] - currentPosition).SetY(0f).sqrMagnitude;
                if (d < dist)
                {
                    currentIndex = i;
                    dist = d;
                }
            }

            var targ = path.Waypoints[currentIndex];
            if (currentIndex == path.Waypoints.Count - 1)
            {
                return targ;
            }

            var dir1 = targ - currentPosition;
            var dir2 = waypoints[currentIndex + 1] - targ;

            if (Vector3.Dot(dir1, dir2) <= 0f)
            {
                currentIndex++;
                targ = path.Waypoints[currentIndex];
            }

            return targ;
        }

    }
}
