using UnityEngine;

namespace com.spacepuppy.Waypoints
{

    public interface IWaypoint
    {

        Vector3 Position { get; set; }
        Vector3 Heading { get; set; }

    }

    public interface IWeightedWaypoint : IWaypoint
    {
        float Strength { get; set; }
    }

}
