using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

namespace com.spacepuppy.Pathfinding.Unity
{

    public abstract class UnityPath : IPath
    {

        #region Fields

        private NavMeshPath _path = new NavMeshPath();

        #endregion

        #region Properties

        public NavMeshPath NavMeshPath
        {
            get { return _path; }
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

        #region Methods

        public abstract bool CalculatePath(int areaMask);

        #endregion

    }

    public class UnityFromToPath : UnityPath
    {

        #region CONSTRUCTOR

        public UnityFromToPath(Vector3 start, Vector3 target)
        {
            this.Start = start;
            this.Target = target;
        }

        #endregion

        #region Properties

        public Vector3 Start
        {
            get;
            set;
        }

        public Vector3 Target
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public override bool CalculatePath(int areaMask)
        {
            return NavMesh.CalculatePath(this.Start, this.Target, areaMask, this.NavMeshPath);
        }

        #endregion
        
    }
}
