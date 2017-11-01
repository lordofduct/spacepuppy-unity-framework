using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Movement;

namespace com.spacepuppy.Pathfinding.Unity
{
    public class UnityPathAgentMovementStyle : PathingMovementStyle, IPathAgent
    {

        #region Fields

        [SerializeField]
        private int _areaMask = -1;

        [System.NonSerialized]
        private UnityFromToPath _path;

        #endregion

        #region IPathAgent Interface

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
        
        public virtual void PathTo(Vector3 target)
        {
            if (_path == null) _path = new UnityFromToPath(Vector3.zero, Vector3.zero);

            _path.Start = this.entityRoot.transform.position;
            _path.Target = target;
            this.CalculatePath(_path);
            this.SetPath(_path);
        }

        public virtual void PathTo(IPath path)
        {
            if (!(path is UnityPath)) throw new PathArgumentException();
            
            this.CalculatePath(path);
            this.SetPath(path);
        }

        #endregion

    }
}
