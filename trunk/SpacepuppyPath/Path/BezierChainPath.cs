using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Path
{

    public class BezierChainPath : IConfigurableIndexedWaypointPath
    {

        #region Fields

        private List<IWaypoint> _waypoints = new List<IWaypoint>();
        private bool _suspended;

        private List<Vector3> _points = new List<Vector3>();
        private float[] _lengths;
        private float _totalArcLength = float.NaN;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void SuspendAutoClean()
        {
            _suspended = true;
        }

        public void ResumeAutoClean()
        {
            _suspended = false;
            this.Clean();
        }

        public void Clean()
        {
            _points.Clear();
            _totalArcLength = 0f;
            if (_waypoints.Count == 0) return;

            _points.Add(_waypoints[0].Position);

            for(int i = 1; i < _waypoints.Count; i++)
            {
                var w1 = _waypoints[i];
                var w2 = _waypoints[i - 1];
                var v = (w1.Position - w2.Position);
                var s = v.magnitude / 2.0f; //distance of the control point

                _points.Add(w2.Position + (w2.Heading * w2.Strength * s));
                _points.Add(w1.Position - (w1.Heading * w1.Strength * s));
                _points.Add(_waypoints[i].Position);
            }

            _lengths = new float[_waypoints.Count];
            for (int i = 0; i < _lengths.Length - 1; i++)
            {
                int j = i * 3;
                var p0 = _points[i];
                var p1 = _points[i + 1];
                var p2 = _points[i + 2];
                var p3 = _points[i + 3];

                //approximation is sum of all 3 legs, and the coord from p0 to p3, all divided by 2.
                float t = (p1 - p0).magnitude + (p2 - p1).magnitude + (p3 - p2).magnitude * (p3 - p0).magnitude;
                _lengths[i] = t / 2.0f;
            }

            _totalArcLength = _lengths.Sum();
        }

        #endregion

        #region IIndexedWaypointPath Interface

        public int Count
        {
            get { return _waypoints.Count; }
        }

        public IWaypoint ControlPoint(int index)
        {
            return _waypoints[index];
        }

        public float GetArcLength()
        {
            return _totalArcLength;
        }

        public Waypoint GetWaypointAt(float t)
        {
            if (_waypoints.Count == 0) throw new System.InvalidOperationException("Cannot calculate waypoint of empty path.");
            if (float.IsNaN(_totalArcLength)) throw new System.InvalidOperationException("Cannot calculate waypoint of uninitialized path.");
            if (_waypoints.Count == 1) return new Waypoint(_waypoints[0]);
            if (_waypoints.Count == 2) return this.GetWaypointAfter(0, t);


            float len = _lengths[0];
            float tot = len;
            int i = 0;
            while (tot / _totalArcLength < t && i < _lengths.Length)
            {
                i++;
                len = _lengths[i];
                tot += len;
            }

            float lt = (tot - len) / _totalArcLength;
            float ht = tot / _totalArcLength;
            float dt = com.spacepuppy.Utils.MathUtil.PercentageMinMax(t, ht, lt);
            return this.GetWaypointAfter(i, dt);
        }

        public Waypoint GetWaypointAfter(int index, float t)
        {
            if (index < 0 || index >= _waypoints.Count) throw new System.IndexOutOfRangeException();
            if (float.IsNaN(_totalArcLength)) throw new System.InvalidOperationException("Cannot calculate waypoint of uninitialized path.");
            if (_waypoints.Count == 1) return new Waypoint(_waypoints[0]);
            if (index == _waypoints.Count - 1) return new Waypoint(_waypoints[index]);

            t = Mathf.Clamp01(t);
            var ft = 1 - t;

            var i = index * 3;
            var p0 = _points[i];
            var p1 = _points[i + 1];
            var p2 = _points[i + 2];
            var p3 = _points[i + 3];

            /*
             C(t) = P0*(1-t)^3 + P1*3*t(1-t)^2 + 3*P2*t^2*(1-t) + P3*t^3

             dC(t)/dt = T(t) =
             -3*P0*(1 - t)^2 + 
             P1*(3*(1 - t)^2 - 6*(1 - t)*t) + 
             P2*(6*(1 - t)*t - 3*t^2) +
             3*P3*t^2
             */
            var p = (ft * ft * ft) * p0 +
                    3 * (ft * ft) * t * p1 +
                    3 * (1 - t) * (t * t) * p2 +
                    (t * t * t) * p3;
            var tan = -3 * p0 * (ft * ft) +
                      p1 * (3 * (ft * ft) - 6 * ft * t) +
                      p2 * (6 * ft * t - 3 * (t * t)) +
                      3 * p3 * (t * t);

            return new Waypoint(p, tan, 1.0f);
        }

        #endregion

        #region IIndexedWaypointPath Interface

        public void AddControlPoint(IWaypoint waypoint)
        {
            _waypoints.Add(waypoint);
            if (!_suspended) this.Clean();
        }

        public void InsertControlPoint(int index, IWaypoint waypoint)
        {
            _waypoints.Insert(index, waypoint);
            if (!_suspended) this.Clean();
        }

        public void ReplaceControlPoint(int index, IWaypoint waypoint)
        {
            _waypoints[index] = waypoint;
            if (!_suspended) this.Clean();
        }

        public void RemoveControlPointAt(int index)
        {
            _waypoints.RemoveAt(index);
            if (!_suspended) this.Clean();
        }

        public void Clear()
        {
            _waypoints.Clear();
            if (!_suspended) this.Clean();
        }

        #endregion

    }

}
