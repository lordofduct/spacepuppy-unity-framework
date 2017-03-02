using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Pathfinding
{

    public interface IPath
    {

        IList<Vector3> Waypoints { get; }

        PathCalculateStatus Status { get; }

    }

}
