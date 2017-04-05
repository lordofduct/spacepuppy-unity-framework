using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Waypoints
{

    /// <summary>
    /// Represents a catmull-rom cardinal spline.
    /// </summary>
    public class CardinalSplinePath : IConfigurableIndexedWaypointPath
    {

        #region Fields

        private const int SUBDIVISIONS_MULTIPLIER = 16;

        private bool _isClosed;
        private List<IWaypoint> _waypoints = new List<IWaypoint>();
        private bool _useConstantSpeed = true;

        private Vector3[] _points;
        private CurveConstantSpeedTable _speedTable = new CurveConstantSpeedTable();

        #endregion

        #region CONSTRUCTOR

        public CardinalSplinePath()
        {

        }

        public CardinalSplinePath(IEnumerable<IWaypoint> waypoints)
        {
            _waypoints.AddRange(waypoints);
            this.Clean_Imp();
        }

        #endregion

        #region Properties

        public bool UseConstantSpeed
        {
            get { return _useConstantSpeed; }
            set { _useConstantSpeed = value; }
        }

        #endregion

        #region Methods

        private void Clean_Imp()
        {
            if (_waypoints.Count == 0)
            {
                _points = new Vector3[] { };
                _speedTable.SetZero();
                return;
            }
            else if (_waypoints.Count == 1)
            {
                _points = new Vector3[] { _waypoints[0].Position };
                _speedTable.SetZero();
                return;
            }
            else
            {
                //get points
                _points = (_isClosed) ? new Vector3[_waypoints.Count + 3] : new Vector3[_waypoints.Count + 2];
                for (int i = 0; i < _waypoints.Count; i++) _points[i + 1] = _waypoints[i].Position;
                if (_isClosed)
                {
                    _points[0] = _waypoints[_waypoints.Count - 1].Position;
                    _points[_points.Length - 2] = _points[1];
                    _points[_points.Length - 1] = _points[2];
                }
                else
                {
                    _points[0] = _points[1];
                    var lastPnt = _waypoints[_waypoints.Count - 1].Position;
                    var diffV = lastPnt - _waypoints[_waypoints.Count - 2].Position;
                    _points[_points.Length - 1] = lastPnt + diffV;
                }

                _speedTable.Clean(SUBDIVISIONS_MULTIPLIER * _points.Length, this.GetRealPositionAt);
            }
        }

        /// <summary>
        /// Returns the position with out speed correction on the path of points. 
        /// This method does NOT validate itself, make sure the curve is clean before calling.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 GetRealPositionAt(float t)
        {
            int numSections = _points.Length - 3;
            int tSec = Mathf.FloorToInt(t * numSections);
            int currPt = numSections - 1;
            if (currPt > tSec) currPt = tSec;
            float u = t * numSections - currPt;

            Vector3 a = _points[currPt];
            Vector3 b = _points[currPt + 1];
            Vector3 c = _points[currPt + 2];
            Vector3 d = _points[currPt + 3];
            return 0.5f * (
                    (-a + 3f * b - 3f * c + d) * (u * u * u)
                    + (2f * a - 5f * b + 4f * c - d) * (u * u)
                    + (-a + c) * u
                    + 2f * b
                    );
        }

        #endregion

        #region IWaypointPath

        public bool IsClosed
        {
            get { return _isClosed; }
            set
            {
                if (value == _isClosed) return;
                _isClosed = value;
                _points = null;
            }
        }


        public Vector3 GetPositionAt(float t)
        {
            if (_speedTable.IsDirty) this.Clean_Imp();
            if (_points.Length < 2) return (_points.Length == 0) ? VectorUtil.NaNVector3 : _points[0];

            if (_useConstantSpeed) t = _speedTable.GetConstPathPercFromTimePerc(t);

            return GetRealPositionAt(t);
        }

        public Waypoint GetWaypointAt(float t)
        {
            if (_speedTable.IsDirty) this.Clean_Imp();
            if (_points.Length < 2) return (_points.Length == 0) ? Waypoint.Invalid : new Waypoint(_points[0], Vector3.zero);

            if (_useConstantSpeed) t = _speedTable.GetConstPathPercFromTimePerc(t);

            var p1 = this.GetRealPositionAt(t);
            var p2 = this.GetRealPositionAt(t + 0.01f); //TODO - figure out a more efficient way of calculating the tangent
            return new Waypoint(p1, (p2 - p1).normalized);
        }

        public float GetArcLength()
        {
            if (_speedTable.IsDirty) this.Clean_Imp();
            return _speedTable.TotalArcLength;
        }

        public Vector3[] GetDetailedPositions(float segmentLength)
        {
            int detail = Mathf.FloorToInt(this.GetArcLength() / segmentLength) + 1;
            Vector3[] arr = new Vector3[detail + 1];
            for (int i = 0; i <= detail; i++)
            {
                arr[i] = this.GetPositionAt((float)i / (float)detail);
            }
            return arr;
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
            if (_speedTable.IsDirty) this.Clean_Imp();
            if (_points.Length < 2) return (_points.Length == 0) ? VectorUtil.NaNVector3 : _points[0];

            index++; //index at 0 is an ignored control point, index should be 1-base
            int i = index * SUBDIVISIONS_MULTIPLIER;
            int j = (index + 1) * SUBDIVISIONS_MULTIPLIER;
            //float nt = _timesTable[i] + (_timesTable[j] - _timesTable[i]) * t;
            float nt = _speedTable.GetTimeAtSubdivision(i) + (_speedTable.GetTimeAtSubdivision(j) - _speedTable.GetTimeAtSubdivision(i)) * t;
            return this.GetRealPositionAt(nt);
        }

        public Waypoint GetWaypointAfter(int index, float t)
        {
            if (index < 0 || index >= _waypoints.Count) throw new System.IndexOutOfRangeException();
            if (_speedTable.IsDirty) this.Clean_Imp();
            if (_points.Length < 2) return (_points.Length == 0) ? Waypoint.Invalid : new Waypoint(_points[0], Vector3.zero);

            index++; //index at 0 is an ignored control point, index should be 1-base
            int i = index * SUBDIVISIONS_MULTIPLIER;
            int j = (index + 1) * SUBDIVISIONS_MULTIPLIER;
            //float nt = _timesTable[i] + (_timesTable[j] - _timesTable[i]) * t;
            float nt = _speedTable.GetTimeAtSubdivision(i) + (_speedTable.GetTimeAtSubdivision(j) - _speedTable.GetTimeAtSubdivision(i)) * t;

            var p1 = this.GetRealPositionAt(nt);
            var p2 = this.GetRealPositionAt(nt + 0.01f); //TODO - figure out a more efficient way of calculating the tangent
            return new Waypoint(p1, (p2 - p1).normalized);
        }

        public RelativePositionData GetRelativePositionData(float t)
        {
            int cnt = _waypoints.Count;
            switch (cnt)
            {
                case 0:
                    return new RelativePositionData(-1, 0f);
                case 1:
                    return new RelativePositionData(0, 0f);
                case 2:
                    return new RelativePositionData(0, t);
                default:
                    {
                        if (_speedTable.IsDirty) this.Clean_Imp();

                        if (_useConstantSpeed) t = _speedTable.GetConstPathPercFromTimePerc(t);

                        t = Mathf.Clamp01(t);
                        if (MathUtil.FuzzyEqual(t, 1f)) return new RelativePositionData(_waypoints.Count - 1, 0f);

                        int index;
                        float segmentTime;
                        if(_isClosed)
                        {
                            index = Mathf.FloorToInt(cnt * t);
                            segmentTime = 1f / cnt;
                        }
                        else
                        {
                            index = Mathf.FloorToInt((cnt - 1) * t);
                            segmentTime = 1f / (cnt - 1);
                        }
                        float lt = index * segmentTime;
                        float ht = (index + 1) * segmentTime;
                        float dt = MathUtil.PercentageMinMax(t, ht, lt);
                        
                        return new RelativePositionData(index, dt);
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

        public void DrawGizmos(float segmentLength)
        {
            if (_waypoints.Count <= 1) return;

            var length = this.GetArcLength();
            int divisions = Mathf.FloorToInt(length / segmentLength) + 1;

            for (int i = 0; i < divisions; i++)
            {
                float t1 = (float)i / (float)divisions;
                float t2 = (float)(i + 1) / (float)divisions;

                Gizmos.DrawLine(this.GetPositionAt(t1), this.GetPositionAt(t2));
            }
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
