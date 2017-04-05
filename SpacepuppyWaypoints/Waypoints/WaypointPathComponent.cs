using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Waypoints
{

    public class WaypointPathComponent : SPComponent
    {

        public enum PathType
        {
            Cardinal = 0,
            Linear = 1,
            BezierChain = 2,
            BezierSpline = 3
        }

        #region Fields

        [Tooltip("The algorithm used for determining the path through the waypoints. NOTE - BezierSpline doesn't pass through the points and shouldn't be used with large numbers of waypoints, especially if the points animated.")]
        [SerializeField()]
        private PathType _pathType;
        [SerializeField()]
        [Tooltip("The curve makes a complete trip around to waypoint 0.")]
        private bool _closed;
        [SerializeField()]
        [Tooltip("When pathing on this path, use values relative to this transform instead of the global values.")]
        private Transform _transformRelativeTo;
        [SerializeField()]
        private TransformWaypoint[] _waypoints;

        [Tooltip("If the waypoints move at runtime, and you'd like the WaypointPath to automatically update itself, flag this true.")]
        [SerializeField()]
        private bool _waypointsAnimate;

        [System.NonSerialized()]
        private IConfigurableIndexedWaypointPath _path;
        [System.NonSerialized()]
        private RadicalCoroutine _autoCleanRoutine;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            for(int i = 0; i < _waypoints.Length; i++)
            {
                if(_waypoints[i] != null) _waypoints[i].Owner = this;
            }
            _path = GetPath(this, false);

            if (_waypointsAnimate) _autoCleanRoutine = this.StartRadicalCoroutine(this.AutoCleanRoutine(), RadicalCoroutineDisableMode.Pauses);
        }

        #endregion

        #region Properties

        public PathType Type
        {
            get { return _pathType; }
            set
            {
                if (_pathType == value) return;
                _pathType = value;
                _path = null;
            }
        }

        public bool Closed
        {
            get { return _closed; }
            set
            {
                _closed = value;
                if (_path != null) _path.IsClosed = _closed;
            }
        }

        public bool WaypointsAnimate
        {
            get { return _waypointsAnimate; }
            set
            {
                if (_waypointsAnimate == value) return;
                _waypointsAnimate = value;
                if(_waypointsAnimate && _autoCleanRoutine != null)
                {
                    _autoCleanRoutine = this.StartRadicalCoroutine(this.AutoCleanRoutine(), RadicalCoroutineDisableMode.Pauses);
                }
            }
        }

        public Transform TransformRelativeTo
        {
            get { return _transformRelativeTo; }
            set { _transformRelativeTo = value; }
        }

        public IConfigurableIndexedWaypointPath Path
        {
            get { return _path; }
        }

        #endregion

        #region Methods
       
        public IConfigurableIndexedWaypointPath GetPathClone()
        {
            return GetPath(this, true);
        }

        public void SetWaypoints(IEnumerable<Transform> waypoints)
        {
            _waypoints = (from t in waypoints select new TransformWaypoint(t)).ToArray();
            this.Clean();
        }

        public void Clean()
        {
            if (_path != null)
            {
                _path.IsClosed = _closed;
                _path.Clear();
                foreach (var wp in _waypoints) _path.AddControlPoint(wp);
            }
        }

        private System.Collections.IEnumerator AutoCleanRoutine()
        {
            yield return null;

            while (_waypointsAnimate)
            {
                _path.IsClosed = _closed;
                if (_waypoints.Length != _path.Count)
                {
                    //refill path
                    _path.Clear();
                    for(int i = 0; i < _waypoints.Length; i++)
                    {
                        _path.AddControlPoint(_waypoints[i]);
                    }
                }
                else
                {
                    bool needsCleaning = false;
                    for (int i = 0; i < _waypoints.Length; i++)
                    {
                        if(!object.ReferenceEquals(_waypoints[i], _path.ControlPoint(i)))
                        {
                            _path.ReplaceControlPoint(i, _waypoints[i]);
                        }
                        else if (!needsCleaning && !Waypoint.Compare(_path.ControlPoint(i), _waypoints[i]))
                        {
                            needsCleaning = true;
                        }
                    }

                    if(needsCleaning)
                    {
                        _path.Clean();
                    }
                }

                yield return null;
            }

            _autoCleanRoutine = null;
        }

        #endregion

        #region Static Interface

        public static IConfigurableIndexedWaypointPath GetPath(WaypointPathComponent c, bool cloneWaypoints)
        {
            IConfigurableIndexedWaypointPath path = null;
            switch(c._pathType)
            {
                case PathType.Cardinal:
                    path = new CardinalSplinePath();
                    break;
                case PathType.Linear:
                    path = new LinearPath();
                    break;
                case PathType.BezierChain:
                    path = new BezierChainPath();
                    break;
                case PathType.BezierSpline:
                    path = new BezierSplinePath();
                    break;
            }
            if(path != null)
            {
                path.IsClosed = c._closed;
                if(c._waypoints != null)
                {
                    for (int i = 0; i < c._waypoints.Length; i++)
                    {
                        if(cloneWaypoints)
                            path.AddControlPoint(new Waypoint(c._waypoints[i]));
                        else
                            path.AddControlPoint(c._waypoints[i]);
                    }
                }
            }
            return path;
        }

        public static IConfigurableIndexedWaypointPath GetPath(PathType type, IEnumerable<IWaypoint> waypoints, bool isClosed, bool cloneWaypoints)
        {
            IConfigurableIndexedWaypointPath path = null;
            switch (type)
            {
                case PathType.Cardinal:
                    path = new CardinalSplinePath();
                    break;
                case PathType.Linear:
                    path = new LinearPath();
                    break;
                case PathType.BezierChain:
                    path = new BezierChainPath();
                    break;
                case PathType.BezierSpline:
                    path = new BezierSplinePath();
                    break;
            }
            if (path != null)
            {
                path.IsClosed = isClosed;
                foreach(var wp in waypoints)
                {
                    if(cloneWaypoints)
                        path.AddControlPoint(new Waypoint(wp));
                    else
                        path.AddControlPoint(wp);
                }
            }
            return path;
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        private class TransformWaypoint : IWeightedWaypoint, IGameObjectSource
        {

            #region Fields

            [SerializeField()]
            private Transform _transform;

            [System.NonSerialized()]
            private WaypointPathComponent _owner;

            #endregion

            #region CONSTRUCTOR
            
            public TransformWaypoint(Transform t)
            {
                _transform = t;
            }

            #endregion

            #region Properties

            public WaypointPathComponent Owner
            {
                get { return _owner; }
                internal set { _owner = value; }
            }

            public Transform Transform
            {
                get { return _transform; }
                set { _transform = value; }
            }

            #endregion

            #region IWaypoint Interface

            public Vector3 Position
            {
                get
                {
                    if (_transform == null) return Vector3.zero;
                    //return _transform.position;
                    
                    return (!object.ReferenceEquals(_owner, null) && _owner._transformRelativeTo != null) ? _transform.GetRelativePosition(_owner._transformRelativeTo) : _transform.position;
                }
                set
                {
                    if (_transform == null) return;
                    //_transform.position = value;
                    if (!object.ReferenceEquals(_owner, null) && _owner._transformRelativeTo != null)
                        _transform.localPosition = value;
                    else
                        _transform.position = value;
                }
            }

            public Vector3 Heading
            {
                get
                {
                    if (_transform == null) return Vector3.forward;
                    //return _transform.forward;
                    return (!object.ReferenceEquals(_owner, null) && _owner._transformRelativeTo != null) ? _transform.GetRelativeRotation(_owner._transformRelativeTo) * Vector3.forward : _transform.forward;
                }
                set
                {
                    if (_transform == null) return;
                    //_transform.rotation = Quaternion.LookRotation(value);
                    if (!object.ReferenceEquals(_owner, null) && _owner._transformRelativeTo != null)
                        _transform.localRotation = Quaternion.LookRotation(value);
                    else
                        _transform.rotation = Quaternion.LookRotation(value);
                }
            }

            public float Strength
            {
                get
                {
                    if (_transform == null) return 0f;
                    return _transform.localScale.z;
                }
                set
                {
                    if (_transform == null) return;
                    _transform.localScale = Vector3.one * value;
                }
            }

            #endregion

            #region IComponent Interface

            GameObject IGameObjectSource.gameObject
            {
                get
                {
                    return (_transform != null) ? _transform.gameObject : null;
                }
            }

            Transform IGameObjectSource.transform
            {
                get
                {
                    return _transform;
                }
            }


            #endregion

        }

        #endregion

    }

}
