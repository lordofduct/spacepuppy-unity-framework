using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Path
{
    public interface IWaypointPath
    {

        float GetArcLength();
        Waypoint GetWaypointAt(float t);

    }

    public interface IIndexedWaypointPath : IWaypointPath
    {

        int Count { get; }
        IWaypoint ControlPoint(int index);

        Waypoint GetWaypointAfter(int index, float t);

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
