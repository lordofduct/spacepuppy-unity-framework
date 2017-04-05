using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Movement;


namespace com.spacepuppy.Pathfinding
{

    [RequireComponent(typeof(MovementMotor))]
    public class PathingMovementStyle : DumbMovementStyle, IPathFollower
    {

        public enum PathingStatus
        {
            Invalid = 0,
            Pathing = 1,
            Complete = 2
        }

        #region Fields

        [SerializeField()]
        private float _speed = 1.0f;
        [SerializeField]
        [MinRange(0f)]
        private float _waypointTolerance = 0.1f;
        [SerializeField]
        [Tooltip("If motion is truly 3d set this true, otherwise motion is calculated on a plane with y-up.")]
        private bool _motion3D = false;
        [SerializeField]
        private SPTime _timeSupplier;
        
        [System.NonSerialized]
        private IPath _currentPath;
        [System.NonSerialized]
        private int _currentNode;
        [System.NonSerialized]
        private bool _paused;


        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public float WaypointTolerance
        {
            get { return _waypointTolerance; }
            set { _waypointTolerance = System.Math.Max(0f, value); }
        }

        public bool Motion3D
        {
            get { return _motion3D; }
            set { _motion3D = value; }
        }

        public SPTime TimeSupplier
        {
            get { return _timeSupplier; }
            set { _timeSupplier = value; }
        }

        public PathingStatus Status
        {
            get
            {
                if (_currentPath == null)
                    return PathingStatus.Invalid;
                else if (_currentPath.Status == PathCalculateStatus.Partial || _currentNode < _currentPath.Waypoints.Count)
                    return PathingStatus.Pathing;
                else
                    return PathingStatus.Complete;
            }
        }

        public IPath Path
        {
            get { return _currentPath; }
        }

        public int CurrentNodeIndex
        {
            get { return _currentNode; }
            set
            {
                _currentNode = Mathf.Clamp(value, 0, _currentPath != null ? _currentPath.Waypoints.Count : 0);
            }
        }

        public bool PathIsPaused
        {
            get { return _paused; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns zero vector if no waypoint.
        /// </summary>
        public Vector3 GetCurrentWaypoint()
        {
            if (_currentPath == null) return Vector3.zero;
            int cnt = _currentPath.Waypoints.Count;
            if (cnt == 0) return Vector3.zero;
            if (_currentNode >= cnt) return _currentPath.Waypoints[cnt - 1];
            return _currentPath.Waypoints[_currentNode];
        }

        #endregion

        #region IPathFollower Interface

        public virtual void SetPath(IPath path)
        {
            if (path == null) throw new System.ArgumentNullException("path");

            _currentPath = path;
            _currentNode = 0;
        }

        public virtual void ResetPath()
        {
            _currentPath = null;
            _paused = false;
        }

        public virtual void ResumePath()
        {
            _paused = false;
        }

        public virtual void StopPath()
        {
            _paused = true;
        }

        #endregion

        #region IMovementStyle Interface
        
        protected override void UpdateMovement()
        {
            if (_paused) return;
        Start:
            switch(this.Status)
            {
                case PathingStatus.Invalid:
                case PathingStatus.Complete:
                    break;
                case PathingStatus.Pathing:
                    {
                        Vector3 target = _currentPath.Waypoints[_currentNode];
                        Vector3 pos = this.Position;
                        Vector3 dir = target - pos;
                        if (!_motion3D) dir.y = 0f;
                        float dist = dir.sqrMagnitude;
                        if (dist <= _waypointTolerance * _waypointTolerance)
                        {
                            _currentNode++;
                            goto Start;
                        }
                        dir.Normalize();

                        Vector3 mv = dir * _speed * _timeSupplier.Delta;
                        if (mv.sqrMagnitude > dist)
                        {
                            _currentNode++;
                        }

                        this.Move(mv);
                    }
                    break;
            }
        }

        

        #endregion

    }
}
