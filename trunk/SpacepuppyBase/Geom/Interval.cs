﻿using UnityEngine;

namespace com.spacepuppy.Geom
{

    [System.Serializable()]
    public struct Interval
    {
        
        #region Fields

        [SerializeField()]
        private float _min;
        [SerializeField()]
        private float _max;

        #endregion

        #region CONSTRUCTOR

        public Interval(float a, float b)
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

        #endregion

        #region Properties

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

        public float Mid
        {
            get { return _min + (_max - _min) * 0.5f; }
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

        /// <summary>
        /// Tests if Min and Max intersects.
        /// </summary>
        /// <param name="inter"></param>
        /// <returns></returns>
        public bool Intersects(Interval inter)
        {
            if (_max < inter.Min) return false;
            if (_min > inter.Max) return false;
            return true;
        }

        /// <summary>
        /// Appends the interval passed in.
        /// </summary>
        /// <param name="inter"></param>
        public void Concat(Interval inter)
        {
            var a = Mathf.Min(this.Min, inter.Min);
            var b = Mathf.Max(this.Max, inter.Max);
            this.SetExtents(a, b);
        }

        /// <summary>
        /// Get the interval intersection of this and another interval.
        /// </summary>
        /// <param name="inter"></param>
        /// <returns></returns>
        public Interval Intersection(Interval inter)
        {
            if (_max < inter.Min) return new Interval(0f, 0f);
            if (_min > inter.Max) return new Interval(0f, 0f);

            return new Interval(Mathf.Max(_min, inter.Min), Mathf.Min(_max, inter.Max));
        }

        /// <summary>
        /// Finds the mid value of the Min and Max of both intervals.
        /// </summary>
        /// <param name="inter"></param>
        /// <returns></returns>
        public float MidValue(Interval inter)
        {
            float[] arr = new float[] { _min, _max, inter.Min, inter.Max };
            System.Array.Sort(arr);
            return (arr[2] + arr[1]) / 2.0f;
        }

        /// <summary>
        /// Total distance the two intervals pass.
        /// </summary>
        /// <param name="inter"></param>
        /// <returns></returns>
        public float Distance(Interval inter)
        {
            if (_max > inter.Min) return 0;
            if (_min < inter.Max) return 0;

            return Mathf.Abs(_max - inter.Min);
        }

        #endregion

        #region Static Members

        public static Interval Empty
        {
            get { return new Interval(float.NaN, float.NaN); }
        }

        /// <summary>
        /// Returns an interval centered over some mid value that extends in either direction distance stutter.
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="stutter"></param>
        /// <returns></returns>
        public static Interval Stutter(float mid, float stutter)
        {
            return new Interval(mid - stutter, mid + stutter);
        }

        /// <summary>
        /// Returns an interval starting at some value and extending for some length.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Interval OfLength(float start, float length)
        {
            return new Interval(start, start + length);
        }

        public static Interval MinMax(float min, float max)
        {
            return new Interval(min, max);
        }

        #endregion

    }

}