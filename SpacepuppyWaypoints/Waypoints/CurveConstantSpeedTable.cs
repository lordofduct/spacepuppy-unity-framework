using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Waypoints
{

    /// <summary>
    /// Normalizes t over spline so that its position appears to progress over the spline at a constant rate.
    /// </summary>
    internal class CurveConstantSpeedTable
    {

        #region Fields
        
        private float _totalArcLength = float.NaN;
        private float[] _timesTable;
        private float[] _lengthsTable;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public float TotalArcLength
        {
            get { return _totalArcLength; }
        }

        public bool IsDirty
        {
            get { return float.IsNaN(_totalArcLength); }
        }

        public int SubdivisionCount
        {
            get { return (_timesTable != null) ? _timesTable.Length : 0; }
        }

        #endregion

        #region Methods

        public void Clean(int subdivisions, System.Func<float, Vector3> getRealPositionAt)
        {
            _totalArcLength = 0f;
            float incr = 1f / subdivisions;
            _timesTable = new float[subdivisions];
            _lengthsTable = new float[subdivisions];

            var prevP = getRealPositionAt(0);
            float perc;
            Vector3 currP;
            for (int i = 1; i <= subdivisions; ++i)
            {
                perc = incr * i;
                currP = getRealPositionAt(perc);
                _totalArcLength += Vector3.Distance(currP, prevP);
                prevP = currP;
                _timesTable[i - 1] = perc;
                _lengthsTable[i - 1] = _totalArcLength;
            }
        }

        public void SetDirty()
        {
            _totalArcLength = float.NaN;
            _timesTable = null;
            _lengthsTable = null;
        }

        public void SetZero()
        {
            _totalArcLength = 0f;
            _timesTable = null;
            _lengthsTable = null;
        }

        public float GetConstPathPercFromTimePerc(float t)
        {
            if (float.IsNaN(_totalArcLength) || _totalArcLength == 0f) return t;

            //Apply constant speed
            if (t > 0f && t < 1f)
            {
                float tLen = _totalArcLength * t;
                float t0 = 0f, l0 = 0f, t1 = 0f, l1 = 0f;
                int alen = _lengthsTable.Length;
                for (int i = 0; i < alen; ++i)
                {
                    if (_lengthsTable[i] > tLen)
                    {
                        t1 = _timesTable[i];
                        l1 = _lengthsTable[i];
                        if (i > 0) l0 = _lengthsTable[i - 1];
                        break;
                    }
                    t0 = _timesTable[i];
                }
                t = t0 + ((tLen - l0) / (l1 - l0)) * (t1 - t0);
            }

            if (t > 1f) t = 1f;
            else if (t < 0f) t = 0f;
            return t;
        }

        public float GetTimeAtSubdivision(int index)
        {
            if (_timesTable == null) return 0f;
            return _timesTable[index];
        }

        #endregion

    }
}
