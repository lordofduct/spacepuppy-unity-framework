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

        #region Properties

        public Vector3 Target
        {
            get;
            set;
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

        public static UnityPath CreatePath(Vector3 target)
        {
            return new UnityPath()
            {
                Target = target
            };
        }
        
        public static bool CalculatePath(Vector3 start, IPath path, int areaMask)
        {
            if (!(path is UnityPath)) throw new PathArgumentException();

            var p = path as UnityPath;
            return NavMesh.CalculatePath(start, p.Target, areaMask, (path as UnityPath)._path);
        }

        #endregion

    }
}
