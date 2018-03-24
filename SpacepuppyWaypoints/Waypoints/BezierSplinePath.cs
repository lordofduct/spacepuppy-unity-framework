using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Waypoints
{

    /// <summary>
    /// Acts as a long bezier spline.
    /// </summary>
    public class BezierSplinePath : IConfigurableIndexedWaypointPath
    {

        #region Fields

        private bool _isClosed;
        private List<IWaypoint> _waypoints = new List<IWaypoint>();

        private Vector3[] _points;
        private CurveConstantSpeedTable _speedTable = new CurveConstantSpeedTable();

        #endregion
        
        #region CONSTRUCTOR

        public BezierSplinePath()
        {

        }

        public BezierSplinePath(IEnumerable<IWaypoint> waypoints)
        {
            _waypoints.AddRange(waypoints);
            this.Clean_Imp();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        private void Clean_Imp()
        {
            if (_waypoints.Count == 0)
            {
                _speedTable.SetZero();
            }
            else if (_waypoints.Count == 1)
            {
                _speedTable.SetZero();
            }
            else
            {
                var arr = this.GetPointArray();
                float estimatedLength = 0f;
                for(int i = 1; i < arr.Length; i++)
                {
                    estimatedLength += (arr[i] - arr[i - 1]).magnitude;
                }
                int detail = Mathf.RoundToInt(estimatedLength / 0.1f);
                
                _speedTable.Clean(detail, this.GetRealPositionAt);
            }
        }

        private Vector3[] GetPointArray()
        {
            int l = _waypoints.Count;
            int cnt = l;
            if (_isClosed) cnt++;
            if (_points == null)
                _points = new Vector3[cnt];
            else if(_points.Length != cnt)
                System.Array.Resize(ref _points, cnt);

            for (int i = 0; i < l; i++ )
            {
                _points[i] = _waypoints[i].Position;
            }
            if(_isClosed)
            {
                _points[cnt - 1] = _waypoints[0].Position;
            }

            return _points;
        }

        private Vector3 GetRealPositionAt(float t)
        {
            var arr = this.GetPointArray();
            var c = arr.Length;
            while (c > 1)
            {
                for (int i = 1; i < c; i++)
                {
                    arr[i - 1] = Vector3.Lerp(arr[i - 1], arr[i], t);
                }

                c--;
            }
            return arr[0];
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
                _speedTable.SetDirty();
            }
        }

        public float GetArcLength()
        {
            if (_speedTable.IsDirty) this.Clean_Imp();
            return _speedTable.TotalArcLength;
        }

        public Vector3 GetPositionAt(float t)
        {
            if (_waypoints.Count == 0) return Vector3.zero;
            if (_waypoints.Count == 1) return _waypoints[0].Position;

            if (_speedTable.IsDirty) this.Clean_Imp();
            return this.GetRealPositionAt(_speedTable.GetConstPathPercFromTimePerc(t));
        }

        public Waypoint GetWaypointAt(float t)
        {
            if (_waypoints.Count == 0) return Waypoint.Invalid;
            if (_waypoints.Count == 1) return new Waypoint(_waypoints[0]);

            t = _speedTable.GetConstPathPercFromTimePerc(t);
            var p1 = this.GetRealPositionAt(t);
            var p2 = this.GetRealPositionAt(t + 0.01f);
            return new Waypoint(p1, (p2 - p1));
        }

        public int GetDetailedPositions(ICollection<Vector3> coll, float segmentLength)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            int detail = Mathf.FloorToInt(this.GetArcLength() / segmentLength) + 1;
            for (int i = 0; i <= detail; i++)
            {
                coll.Add(this.GetPositionAt((float)i / (float)detail));
            }
            return detail + 1;
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

            float range = 1f / (_waypoints.Count - 1);
            return this.GetPositionAt(index * range + t * range);
        }

        public Waypoint GetWaypointAfter(int index, float t)
        {
            if (index < 0 || index >= _waypoints.Count) throw new System.IndexOutOfRangeException();

            float range = 1f / (_waypoints.Count - 1);
            return this.GetWaypointAt(index * range + t * range);
        }

        public RelativePositionData GetRelativePositionData(float t)
        {
            int cnt = _waypoints.Count;
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
                        float range = 1f / (_waypoints.Count - 1);
                        int i = Mathf.Clamp(Mathf.FloorToInt(t / range), 0, _waypoints.Count - 1);
                        float dt = (t - i * range) / range;
                        return new RelativePositionData(i, dt);
                    }
            }
        }

        public void Clean()
        {
            _speedTable.SetDirty();
        }

        #endregion

        #region IConfigurableIndexedWaypointPath Interface

        public void AddControlPoint(IWaypoint waypoint)
        {
            _waypoints.Add(waypoint);
            _speedTable.SetDirty();
        }

        public void InsertControlPoint(int index, IWaypoint waypoint)
        {
            _waypoints.Insert(index, waypoint);
            _speedTable.SetDirty();
        }

        public void ReplaceControlPoint(int index, IWaypoint waypoint)
        {
            _waypoints[index] = waypoint;
            _speedTable.SetDirty();
        }

        public void RemoveControlPointAt(int index)
        {
            _waypoints.RemoveAt(index);
            _speedTable.SetDirty();
        }

        public void Clear()
        {
            _waypoints.Clear();
            _speedTable.SetDirty();
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
