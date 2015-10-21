using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Cameras
{
    public interface ICamera : IGameObjectSource
    {

        /// <summary>
        /// The camera this ICamera currently represents. Can vary if this ICamera handles multiple cameras. 
        /// If this ICamera currnetly handles multiple active cameras, this is the one considered most important.
        /// </summary>
        Camera camera { get; }

        bool IsAlive { get; }

        /// <summary>
        /// Does this ICamera manage the supplied camera.
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        bool Contains(Camera cam);

    }
}
