using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace com.spacepuppy.Pathfinding.Unity
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
        
        public IPathFactory PathFactory
        {
            get { return UnityPathFactory.Default; }
        }

        public bool ValidPath(IPath path)
        {
            return (path is UnityPath);
        }

        public void CalculatePath(IPath path)
        {
            if (!(path is UnityPath)) throw new PathArgumentException();

            (path as UnityPath).CalculatePath(_areaMask);
        }
        
        #endregion

    }
}
