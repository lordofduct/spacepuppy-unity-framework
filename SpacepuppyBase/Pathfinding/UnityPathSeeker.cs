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

        public IPath CreatePath(Vector3 target)
        {
            return UnityPath.CreatePath(target);
        }

        public bool ValidPath(IPath path)
        {
            return (path is UnityPath);
        }

        public void CalculatePath(IPath path)
        {
            if (!(path is UnityPath)) throw new PathArgumentException();
            
            UnityPath.CalculatePath(this.entityRoot.transform.position, path, _areaMask);
        }
        
        #endregion

    }
}
