using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Movement;

namespace com.spacepuppy.Pathfinding
{
    public class UnityPathAgentMovementStyle : PathingMovementStyle, IPathAgent
    {

        #region Fields

        [SerializeField]
        private int _areaMask = -1;

        [System.NonSerialized]
        private UnityPath _path;

        #endregion

        #region IPathAgent Interface
        
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

            var p = (path as UnityPath);
            UnityPath.CalculatePath(this.entityRoot.transform.position, path, _areaMask);
        }
        
        public virtual void PathTo(Vector3 target)
        {
            if (_path == null) _path = UnityPath.CreatePath(target);

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
