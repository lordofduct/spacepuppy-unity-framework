using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Waypoints
{
    public interface IWaypointPath
    {

        bool IsClosed { get; set; }

        float GetArcLength();
        Vector3 GetPositionAt(float t);
        Waypoint GetWaypointAt(float t);

        int GetDetailedPositions(ICollection<Vector3> coll, float segmentLength);
    }

    public interface IIndexedWaypointPath : IWaypointPath, IEnumerable<IWaypoint>
    {

        int Count { get; }
        IWaypoint ControlPoint(int index);
        int IndexOf(IWaypoint waypoint);
        Vector3 GetPositionAfter(int index, float t);
        Waypoint GetWaypointAfter(int index, float t);
        /// <summary>
        /// Returns data pertaining to the relative position between the 2 control points on either side of 't'.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        RelativePositionData GetRelativePositionData(float t);


        void Clean();

    }

    public struct RelativePositionData
    {
        public int Index;
        public float TPrime;

        public RelativePositionData(int index, float t)
        {
            this.Index = index;
            this.TPrime = t;
        }
    }

    public interface IConfigurableIndexedWaypointPath : IIndexedWaypointPath
    {
        void AddControlPoint(IWaypoint waypoint);
        void InsertControlPoint(int index, IWaypoint waypoint);
        void ReplaceControlPoint(int index, IWaypoint waypoint);
        void RemoveControlPointAt(int index);
        void Clear();

    }

}
