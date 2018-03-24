using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Geom;

namespace com.spacepuppy.Waypoints
{

    public class SplinePath : IIndexedWaypointPath
    {

        #region Fields

        private I3dSpline _spline;

        #endregion

        #region CONSTRUCTOR

        public SplinePath(I3dSpline spline)
        {
            _spline = spline;
        }

        #endregion

        #region IWaypointPath Interface

        public int Count
        {
            get { return _spline.Count; }
        }

        public bool IsClosed
        {
            get { return false; }
            set
            {
                throw new System.NotSupportedException();
            }
        }

        public IWaypoint ControlPoint(int index)
        {
            return new Waypoint(_spline.ControlPoint(index), Vector3.zero);
        }

        public int IndexOf(IWaypoint waypoint)
        {
            return -1;
        }

        public float GetArcLength()
        {
            return _spline.GetArcLength();
        }

        public Vector3 GetPositionAt(float t)
        {
            return _spline.GetPosition(t);
        }
        public Waypoint GetWaypointAt(float t)
        {
            return new Waypoint(_spline.GetPosition(t), Vector3.zero);
        }

        public Vector3 GetPositionAfter(int index, float t)
        {
            return _spline.GetPositionAfter(index, t);
        }
        public Waypoint GetWaypointAfter(int index, float t)
        {
            return new Waypoint(_spline.GetPositionAfter(index, t), Vector3.zero);
        }

        public int GetDetailedPositions(ICollection<Vector3> coll, float segmentLength)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            int detail = Mathf.FloorToInt(_spline.GetArcLength() / segmentLength) + 1;
            for(int i = 0; i <= detail; i++)
            {
                coll.Add(_spline.GetPosition((float)i / (float)detail));
            }
            return detail + 1;
        }

        public RelativePositionData GetRelativePositionData(float t)
        {
            throw new System.NotImplementedException();
        }

        void IIndexedWaypointPath.Clean()
        {
            //do nothing
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<IWaypoint> GetEnumerator()
        {
            for(int i = 0; i < _spline.Count; i++)
            {
                yield return new Waypoint(_spline.ControlPoint(i), Vector3.zero);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }

}
