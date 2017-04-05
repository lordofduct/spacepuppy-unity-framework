using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Waypoints
{
    public class LinearPath : IConfigurableIndexedWaypointPath
    {

        #region Fields

        private bool _isClosed;
        private List<IWaypoint> _waypoints = new List<IWaypoint>();

        private Vector3[] _points; //null if dirty
        private float[] _lengths;
        private float _totalArcLength = float.NaN;

        #endregion

        #region CONSTRUCTOR

        public LinearPath()
        {

        }

        public LinearPath(IEnumerable<IWaypoint> waypoints)
        {
            _waypoints.AddRange(waypoints);
            _points = null;
        }

        #endregion

        #region Methods

        private void Clean_Imp()
        {
            if (_waypoints.Count == 0)
            {
                _points = new Vector3[] { };
                _totalArcLength = float.NaN;
            }
            else if (_waypoints.Count == 1)
            {
                _points = new Vector3[] { _waypoints[0].Position };
                _totalArcLength = 0f;
            }
            else
            {
                _points = (_isClosed) ? new Vector3[_waypoints.Count + 1] : new Vector3[_waypoints.Count];
                for (int i = 0; i < _waypoints.Count; i++) _points[i] = _waypoints[i].Position;
                if (_isClosed) _points[_points.Length - 1] = _points[0];

                _lengths = new float[_points.Length];
                _totalArcLength = 0f;
                for (int i = 1; i < _points.Length; i++)
                {
                    float l = Vector3.Distance(_points[i], _points[i - 1]);
                    _lengths[i - 1] = l;
                    _totalArcLength += l;
                }
            }
        }

        #endregion

        #region IWaypointPath Interface

        public bool IsClosed
        {
            get { return _isClosed; }
            set
            {
                if (_isClosed == value) return;
                _isClosed = value;
                _points = null;
            }
        }

        public float GetArcLength()
        {
            if (_points == null) this.Clean_Imp();
            return _totalArcLength;
        }

        public Vector3 GetPositionAt(float t)
        {
            if (_points == null) this.Clean_Imp();
            if (_points.Length < 2) return (_points.Length == 0) ? VectorUtil.NaNVector3 : _points[0];

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
            return this.GetPositionAfter(i, dt);
        }

        public Waypoint GetWaypointAt(float t)
        {
            if (_points == null) this.Clean_Imp();
            if (_points.Length < 2) return (_points.Length == 0) ? Waypoint.Invalid : new Waypoint(_points[0], Vector3.zero);

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
            float dt = MathUtil.PercentageMinMax(t, ht, lt);
            return this.GetWaypointAfter(i, dt);
        }

        public Vector3[] GetDetailedPositions(float segmentLength)
        {
            if (_points == null) this.Clean_Imp();
            return _points.Clone() as Vector3[];
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

        public int IndexOf(IWaypoint waypoint)
        {
            return _waypoints.IndexOf(waypoint);
        }

        public Vector3 GetPositionAfter(int index, float t)
        {
            if (index < 0 || index >= _waypoints.Count) throw new System.IndexOutOfRangeException();
            if (_points == null) this.Clean_Imp();
            if (_points.Length < 2) return (_points.Length == 0) ? VectorUtil.NaNVector3 : _points[0];

            if (index == _points.Length - 1)
            {
                var pa = _points[index - 1];
                var pb = _points[index];
                var v = pb - pa;
                return pa + v * t;
            }
            else
            {
                return Vector3.Lerp(_points[index], _points[index + 1], t);
            }
        }

        public Waypoint GetWaypointAfter(int index, float t)
        {
            if (index < 0 || index >= _waypoints.Count) throw new System.IndexOutOfRangeException();
            if (_points == null) this.Clean_Imp();
            if (_points.Length < 2) return (_points.Length == 0) ? Waypoint.Invalid : new Waypoint(_points[0], Vector3.zero);

            if (index == _points.Length - 1)
            {
                var pa = _points[index - 1];
                var pb = _points[index];
                var v = pb - pa;
                return new Waypoint(pa + v * t, v.normalized);
            }
            else
            {
                var pa = _points[index];
                var pb = _points[index + 1];
                var v = pb - pa;
                return new Waypoint(pa + v * t, v.normalized);
            }
        }

        public RelativePositionData GetRelativePositionData(float t)
        {
            var cnt = _waypoints.Count;
            switch(cnt)
            {
                case 0:
                    return new RelativePositionData(-1, 0f);
                case 1:
                    return new RelativePositionData(0, 0f);
                case 2:
                    return new RelativePositionData(0, t);
                default:
                    {
                        if (_points == null) this.Clean_Imp();

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
                        float dt = MathUtil.PercentageMinMax(t, ht, lt);
                        
                        return new RelativePositionData(i % cnt, dt);
                    }
            }
        }

        public void Clean()
        {
            _points = null;
        }

        #endregion

        #region IConfigurableIndexedWaypointPath Interface

        public void AddControlPoint(IWaypoint waypoint)
        {
            _waypoints.Add(waypoint);
            _points = null;
        }

        public void InsertControlPoint(int index, IWaypoint waypoint)
        {
            _waypoints.Insert(index, waypoint);
            _points = null;
        }

        public void ReplaceControlPoint(int index, IWaypoint waypoint)
        {
            _waypoints[index] = waypoint;
            _points = null;
        }

        public void RemoveControlPointAt(int index)
        {
            _waypoints.RemoveAt(index);
            _points = null;
        }

        public void Clear()
        {
            _waypoints.Clear();
            _points = null;
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<IWaypoint> GetEnumerator()
        {
            return _waypoints.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _waypoints.GetEnumerator();
        }

        #endregion

    }
}
