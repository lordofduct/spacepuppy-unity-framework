using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Pathfinding
{

    [RequireComponentInEntity(typeof(NavMeshAgent))]
    public class UnityStandardPathAgent : SPComponent, IPathAgent
    {

        #region Fields

        [SerializeField]
        [DefaultFromSelf(UseEntity = true)]
        private NavMeshAgent _agent;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if (_agent == null)
            {
                if(!this.GetComponent<NavMeshAgent>(out _agent))
                {
                    Debug.LogWarning("No NavMeshAgent attached to this UnityPathAgent.");
                    this.enabled = false;
                }
            }

        }

        #endregion

        #region IPathAgent Interface

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
            if (object.ReferenceEquals(_agent, null)) throw new System.InvalidOperationException("UnityPathAgent was not configured correctly.");
            if (!(path is UnityPath)) throw new PathArgumentException();

            _agent.CalculatePath(target, (path as UnityPath)._path);
        }

        public void CalculatePath(Vector3 start, Vector3 target, IPath path)
        {
            if (object.ReferenceEquals(_agent, null)) throw new System.InvalidOperationException("UnityPathAgent was not configured correctly.");
            if (!(path is UnityPath)) throw new PathArgumentException();

            NavMesh.CalculatePath(start, target, _agent.areaMask, (path as UnityPath)._path);
        }

        public void SetPath(IPath path)
        {
            if (object.ReferenceEquals(_agent, null)) throw new System.InvalidOperationException("UnityPathAgent was not configured correctly.");
            if (!(path is UnityPath)) throw new PathArgumentException();

            _agent.SetPath((path as UnityPath)._path);
        }

        public void PathTo(Vector3 target)
        {
            if (object.ReferenceEquals(_agent, null)) throw new System.InvalidOperationException("UnityPathAgent was not configured correctly.");
            _agent.SetDestination(target);
        }

        public void ResetPath()
        {
            if (object.ReferenceEquals(_agent, null)) throw new System.InvalidOperationException("UnityPathAgent was not configured correctly.");
            _agent.ResetPath();
        }

        public void StopPath()
        {
            if (object.ReferenceEquals(_agent, null)) throw new System.InvalidOperationException("UnityPathAgent was not configured correctly.");
            _agent.Stop();
        }

        public void ResumePath()
        {
            if (object.ReferenceEquals(_agent, null)) throw new System.InvalidOperationException("UnityPathAgent was not configured correctly.");
            _agent.Resume();
        }

        #endregion

    }
}
