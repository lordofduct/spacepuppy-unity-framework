using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Path
{

    public class WaypointPathComponent : SPComponent
    {

        #region Fields

        [SerializeField()]
        private TransformWaypoint[] _waypoints;
        [Tooltip("If the waypoints move at runtime, and you'd like the WaypointPath to automatically update itself, flag this true.")]
        [SerializeField()]
        private bool _waypointsAnimate;

        [System.NonSerialized()]
        private BezierChainPath _path;
        [System.NonSerialized()]
        private RadicalCoroutine _autoCleanRoutine;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _path = new BezierChainPath();
            _path.SuspendAutoClean();
            for(int i = 0; i < _waypoints.Length; i++)
            {
                _path.AddControlPoint(_waypoints[i]);
            }
            _path.ResumeAutoClean();

            if (_waypointsAnimate) _autoCleanRoutine = this.StartRadicalCoroutine(this.AutoCleanRoutine());
        }

        #endregion

        #region Properties

        public bool WaypointsAnimate
        {
            get { return _waypointsAnimate; }
            set
            {
                if (_waypointsAnimate == value) return;
                _waypointsAnimate = value;
                if(_waypointsAnimate && _autoCleanRoutine != null)
                {
                    _autoCleanRoutine = this.StartRadicalCoroutine(this.AutoCleanRoutine());
                }
            }
        }

        #endregion

        #region Methods

        public void Clean()
        {
            if (_path != null) _path.Clean();
        }

        private System.Collections.IEnumerator AutoCleanRoutine()
        {
            var lst = new List<Waypoint>();
            int i;
            for (i = 0; i < _waypoints.Length; i++)
            {
                lst.Add(new Waypoint(_waypoints[i]));
            }

            yield return null;

            Waypoint w;
            while (_waypointsAnimate)
            {
                if(_waypoints.Length != lst.Count)
                {
                    //refill path
                    _path.SuspendAutoClean();
                    _path.Clear();
                    for(i = 0; i < _waypoints.Length; i++)
                    {
                        _path.AddControlPoint(_waypoints[i]);
                    }
                    _path.ResumeAutoClean();
                }
                else
                {
                    bool cleaned = false;
                    for (i = 0; i < _waypoints.Length; i++)
                    {
                        if(!object.ReferenceEquals(_waypoints[i], _path.ControlPoint(i)))
                        {
                            _path.ReplaceControlPoint(i, _waypoints[i]);
                        }
                        else if (!cleaned && !Waypoint.Compare(lst[i], _waypoints[i]))
                        {
                            cleaned = true;
                            _path.Clean();
                        }
                        lst[i] = new Waypoint(_waypoints[i]);
                    }
                }
            }

            _autoCleanRoutine = null;
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        private class TransformWaypoint : IWaypoint
        {

            #region Fields

            [SerializeField()]
            private Transform _transform;
            [SerializeField()]
            private float _strength = 1f;

            #endregion

            #region Properties

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
                    return (_transform != null) ? _transform.position : Vector3.zero;
                }
                set
                {
                    if (_transform == null) return;
                    _transform.position = value;
                }
            }

            public Vector3 Heading
            {
                get
                {
                    return (_transform != null) ? _transform.forward : Vector3.forward;
                }
                set
                {
                    if (_transform == null) return;
                    _transform.rotation = Quaternion.LookRotation(value);
                }
            }

            public float Strength
            {
                get
                {
                    return _strength;
                }
                set
                {
                    if (_strength == value) return;
                    _strength = value;
                }
            }

            #endregion

        }

        #endregion

    }

}
