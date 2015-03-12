using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Path
{

    public struct Waypoint : IWaypoint
    {
        public Vector3 Position;
        private Vector3 _heading;
        public float Strength;

        public Waypoint(Vector3 p, Vector3 h, float s)
        {
            Position = p;
            _heading = h.normalized;
            Strength = s;
        }

        public Waypoint(IWaypoint waypoint)
        {
            this.Position = waypoint.Position;
            _heading = waypoint.Heading.normalized;
            this.Strength = waypoint.Strength;
        }

        #region IWaypoint Interface

        public Vector3 Heading
        {
            get { return _heading; }
            set { _heading = value.normalized; }
        }

        Vector3 IWaypoint.Position
        {
            get
            {
                return this.Position;
            }
            set
            {
                this.Position = value;
            }
        }

        float IWaypoint.Strength
        {
            get
            {
                return this.Strength;
            }
            set
            {
                this.Strength = value;
            }
        }

        #endregion


        #region Operator Interface

        public static bool Compare(IWaypoint a, IWaypoint b)
        {
            return (a.Position == b.Position) && (a.Heading == b.Heading) && (a.Strength == b.Strength);
        }

        public static bool Compare(Waypoint a, IWaypoint b)
        {
            return (a.Position == b.Position) && (a._heading == b.Heading) && (a.Strength == b.Strength);
        }

        public static bool Compare(IWaypoint a, Waypoint b)
        {
            return (a.Position == b.Position) && (a.Heading == b._heading) && (a.Strength == b.Strength);
        }

        #endregion

    }

}
