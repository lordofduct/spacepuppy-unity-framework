using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Geom;

namespace com.spacepuppy.Path
{

    public class SplinePath : IIndexedWaypointPath
    {

        #region Fields

        private I3dSpline _spline;

        #endregion

        #region IWaypointPath Interface

        public int Count
        {
            get { return _spline.Count; }
        }

        public IWaypoint ControlPoint(int index)
        {
            return new Waypoint(_spline.ControlPoint(index), Vector3.zero, 0f);
        }

        public float GetArcLength()
        {
            return _spline.GetArcLength();
        }

        public Waypoint GetWaypointAt(float t)
        {
            return new Waypoint(_spline.GetPosition(t), Vector3.zero, 0f);
        }
        public Waypoint GetWaypointAfter(int index, float t)
        {
            return new Waypoint(_spline.GetPositionAfter(index, t), Vector3.zero, 0f);
        }

        #endregion


        
    }

}
