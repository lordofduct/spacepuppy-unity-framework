using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Pathfinding
{

    public interface IPathFactory
    {

        IPath Create(IPathSeeker seeker, Vector3 target);
        IPath Create(Vector3 start, Vector3 end);

    }

}
