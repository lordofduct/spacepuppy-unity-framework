using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace com.spacepuppy.Pathfinding
{
    public class UnityPathSeeker : SPComponent, IPathSeeker
    {

        #region Fields

        [SerializeField]
        private int _areaMask = -1;

        #endregion

        #region CONSTRUCTOR
        
        #endregion

        #region IPathSeeker Interface

        public IPath CreatePath()
        {
            return UnityPath.CreatePath();
        }

        public bool ValidPath(IPath path)
        {
            return (path is UnityPath);
        }

        public void CalculatePath(Vector3 target, IPath path)
        {
            UnityPath.CalculatePath(this.entityRoot.transform.position, target, _areaMask, path);
        }

        public void CalculatePath(Vector3 start, Vector3 target, IPath path)
        {
            UnityPath.CalculatePath(start, target, _areaMask, path);
        }

        #endregion

    }
}
