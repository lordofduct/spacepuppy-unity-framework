using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace com.spacepuppy.Pathfinding
{
    public class UnityPath : IPath
    {

        #region Fields

        internal NavMeshPath _path = new NavMeshPath();

        #endregion

        #region CONSTRUCTOR

        private UnityPath()
        {

        }

        #endregion

        #region IPath Interface

        public IList<Vector3> Waypoints
        {
            get
            {
                return _path.corners;
            }
        }

        public PathCalculateStatus Status
        {
            get
            {
                switch (_path.status)
                {
                    case NavMeshPathStatus.PathInvalid:
                        return PathCalculateStatus.Invalid;
                    case NavMeshPathStatus.PathPartial:
                        return PathCalculateStatus.Partial;
                    case NavMeshPathStatus.PathComplete:
                        return PathCalculateStatus.Success;
                    default:
                        return PathCalculateStatus.Invalid;
                }
            }
        }

        #endregion

        #region Static Interface

        public static UnityPath CreatePath()
        {
            return new UnityPath();
        }
        
        public static void CalculatePath(Vector3 start, Vector3 target, int areaMask, IPath path)
        {
            if (!(path is UnityPath)) throw new PathArgumentException();

            NavMesh.CalculatePath(start, target, areaMask, (path as UnityPath)._path);
        }

        #endregion

    }
}
