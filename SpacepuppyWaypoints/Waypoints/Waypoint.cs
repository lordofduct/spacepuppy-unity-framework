using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Waypoints
{

    public struct Waypoint : IWaypoint
    {
        public Vector3 Position;
        private Vector3 _heading;

        public Waypoint(Vector3 p, Vector3 h)
        {
            Position = p;
            _heading = h.normalized;
        }

        public Waypoint(IWaypoint waypoint)
        {
            this.Position = waypoint.Position;
            _heading = waypoint.Heading.normalized;
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

        #endregion


        #region Operator Interface

        public static bool Compare(IWaypoint a, IWaypoint b)
        {
            //return (a.Position == b.Position) && (a.Heading == b.Heading);
            return VectorUtil.FuzzyEquals(a.Position, b.Position) && VectorUtil.FuzzyEquals(a.Heading, b.Heading);
        }

        public static bool Compare(Waypoint a, IWaypoint b)
        {
            //return (a.Position == b.Position) && (a._heading == b.Heading);
            return VectorUtil.FuzzyEquals(a.Position, b.Position) && VectorUtil.FuzzyEquals(a.Heading, b.Heading);
        }

        public static bool Compare(IWaypoint a, Waypoint b)
        {
            //return (a.Position == b.Position) && (a.Heading == b._heading);
            return VectorUtil.FuzzyEquals(a.Position, b.Position) && VectorUtil.FuzzyEquals(a.Heading, b.Heading);
        }

        #endregion

        public static Waypoint Invalid { get { return new Waypoint(new Vector3(float.NaN, float.NaN, float.NaN), new Vector3(float.NaN, float.NaN, float.NaN)); } }

    }

    public struct WeightedWaypoint : IWeightedWaypoint
    {
        public Vector3 Position;
        private Vector3 _heading;
        public float Strength;

        public WeightedWaypoint(Vector3 p, Vector3 h, float s)
        {
            Position = p;
            _heading = h.normalized;
            Strength = s;
        }

        public WeightedWaypoint(IWeightedWaypoint waypoint)
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

        float IWeightedWaypoint.Strength
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

        public static bool Compare(IWeightedWaypoint a, IWeightedWaypoint b)
        {
            return (a.Position == b.Position) && (a.Heading == b.Heading) && (a.Strength == b.Strength);
        }

        public static bool Compare(WeightedWaypoint a, IWeightedWaypoint b)
        {
            return (a.Position == b.Position) && (a._heading == b.Heading) && (a.Strength == b.Strength);
        }

        public static bool Compare(IWeightedWaypoint a, WeightedWaypoint b)
        {
            return (a.Position == b.Position) && (a.Heading == b._heading) && (a.Strength == b.Strength);
        }

        #endregion

    }

}
