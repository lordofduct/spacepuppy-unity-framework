using UnityEngine;

namespace com.spacepuppy.Geom
{
    /// <summary>
    /// Represents an interval on some axis. This is useful for projection of geometry on an axis. The axis is stored for token 
    /// purposes only and is not used for any actions with other intervals.
    /// </summary>
    [System.Serializable()]
    public struct AxisInterval
    {

        #region Fields

        [SerializeField()]
        private Vector3 _axis;
        [SerializeField()]
        private float _min;
        [SerializeField()]
        private float _max;

        #endregion

        #region CONSTRUCTOR

        public AxisInterval(Vector3 axis, float a, float b)
        {
            _axis = axis;
            _min = a;
            _max = b;

            if (_min > _max)
            {
                var c = _min;
                _min = _max;
                _max = c;
            }

            var l = _axis.sqrMagnitude;
            if (l != 0.0f && l != 1.0f) _axis.Normalize();
        }

        #endregion

        #region Properties

        public Vector3 Axis
        {
            get { return _axis; }
            set
            {
                _axis = value.normalized;
            }
        }

        public float Min
        {
            get { return _min; }
        }

        public float Max
        {
            get { return _max; }
        }

        public float Length
        {
            get { return Mathf.Abs(Max - Min); }
        }

        #endregion

        #region Methods

        public void SetExtents(float a, float b)
        {
            _min = a;
            _max = b;

            if (_min > _max)
            {
                var c = _min;
                _min = _max;
                _max = c;
            }
        }

        public bool IsEmpty()
        {
            return _axis == Vector3.zero;
        }

        /// <summary>
        /// Tests if Min and Max intersects, ignores axis
        /// </summary>
        /// <param name="inter"></param>
        /// <returns></returns>
        public bool Intersects(AxisInterval inter)
        {
            if (_max < inter.Min) return false;
            if (_min > inter.Max) return false;
            return true;
        }

        /// <summary>
        /// Appends the interval passed in, conserves this axis.
        /// </summary>
        /// <param name="inter"></param>
        public void Concat(AxisInterval inter)
        {
            var a = Mathf.Min(this.Min, inter.Min);
            var b = Mathf.Max(this.Max, inter.Max);
            this.SetExtents(a, b);
        }

        /// <summary>
        /// Get the interval intersection of this and another interval, conserves this axis.
        /// </summary>
        /// <param name="inter"></param>
        /// <returns></returns>
        public AxisInterval Intersection(AxisInterval inter)
        {
            if (_max < inter.Min) return new AxisInterval(_axis, 0, 0);
            if (_min > inter.Max) return new AxisInterval(_axis, 0, 0);

            return new AxisInterval(_axis, Mathf.Max(_min, inter.Min), Mathf.Min(_max, inter.Max));
        }

        /// <summary>
        /// Finds the mid value of the Min and Max of both intervals, ignores axis
        /// </summary>
        /// <param name="inter"></param>
        /// <returns></returns>
        public float MidValue(AxisInterval inter)
        {
            float[] arr = new float[] { _min, _max, inter.Min, inter.Max };
            System.Array.Sort(arr);
            return (arr[2] + arr[1]) / 2.0f;
        }

        /// <summary>
        /// Total distance the two intervals pass, ignores axis
        /// </summary>
        /// <param name="inter"></param>
        /// <returns></returns>
        public float Distance(AxisInterval inter)
        {
            if (_max > inter.Min) return 0;
            if (_min < inter.Max) return 0;

            return Mathf.Abs(_max - inter.Min);
        }

        #endregion

        #region Static Members

        public static AxisInterval Empty
        {
            get { return new AxisInterval(Vector3.zero, float.NaN, float.NaN); }
        }

        public static AxisInterval NoHit(Vector3 axis)
        {
            return new AxisInterval(axis, float.NaN, float.NaN);
        }

        #endregion
    }
}
