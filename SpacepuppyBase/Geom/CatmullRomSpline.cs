using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Geom
{
    public class CatmullRomSpline : I3dSpline
    {

        #region Fields

        private const int SUBDIVISIONS_MULTIPLIER = 16;

        private List<Vector3> _points = new List<Vector3>();
        private bool _useConstantSpeed = true;

        private bool _dirty;
        private float _totalArcLength = float.NaN;
        private float[] _timesTable;
        private float[] _lengthsTable;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public bool IsValid { get { return _points.Count >= 4; } }

        public bool UseConstantSpeed
        {
            get { return _useConstantSpeed; }
            set { _useConstantSpeed = value; }
        }

        #endregion

        #region Methods

        private void ForceClean()
        {
            if (!this.IsValid)
            {
                _totalArcLength = float.NaN;
                _timesTable = null;
                _lengthsTable = null;
                return;
            }

            _totalArcLength = 0f;
            int subdivisions = SUBDIVISIONS_MULTIPLIER * _points.Count;
            float incr = 1f / subdivisions;
            _timesTable = new float[subdivisions];
            _lengthsTable = new float[subdivisions];

            var prevP = this.GetRealPosition(0);
            float perc;
            Vector3 currP;
            for (int i = 1; i <= subdivisions; ++i)
            {
                perc = incr * i;
                currP = this.GetRealPosition(perc);
                _totalArcLength += Vector3.Distance(currP, prevP);
                prevP = currP;
                _timesTable[i - 1] = perc;
                _lengthsTable[i - 1] = _totalArcLength;
            }
        }

        private Vector3 GetRealPosition(float t)
        {
            int numSections = _points.Count - 3;
            int tSec = Mathf.FloorToInt(t * numSections);
            int currPt = numSections - 1;
            if (currPt > tSec) currPt = tSec;
            float u = t * numSections - currPt;

            Vector3 a = _points[currPt];
            Vector3 b = _points[currPt + 1];
            Vector3 c = _points[currPt + 2];
            Vector3 d = _points[currPt + 3];
            return 0.5f * (
                    (-a + 3f * b - 3f * c + d) * (u * u * u)
                    + (2f * a - 5f * b + 4f * c - d) * (u * u)
                    + (-a + c) * u
                    + 2f * b
                    );
        }

        private float GetConstPathPercFromTimePerc(float t)
        {
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

        #endregion

        #region I3DSpline Interface

        public int Count
        {
            get { return _points.Count; }
        }

        public Vector3 ControlPoint(int index)
        {
            return _points[index];
        }

        public void AddControlPoint(Vector3 w)
        {
            _points.Add(w);
            _dirty = true;
        }

        public void MoveControlPoint(int index, Vector3 w)
        {
            _points[index] = w;
            _dirty = true;
        }

        public void RemoveControlPoint(int index)
        {
            _points.RemoveAt(index);
        }

        public float GetArcLength()
        {
            if (_dirty) this.ForceClean();
            return _totalArcLength;
        }

        public Vector3 GetPosition(float t)
        {
            if (_points.Count < 4) throw new System.InvalidOperationException("Cardinal Spline must contain at least 4 waypoints.");
            if (_dirty) this.ForceClean();

            if (_useConstantSpeed) t = this.GetConstPathPercFromTimePerc(t);

            return GetRealPosition(t);
        }

        public Vector3 GetPositionAfter(int index, float t)
        {
            if (_points.Count < 4) throw new System.InvalidOperationException("Cardinal Spline must contain at least 4 waypoints.");
            if (index < 0 || index >= _points.Count) throw new System.IndexOutOfRangeException();
            if (index == _points.Count - 1) return _points[index];

            int i = index * SUBDIVISIONS_MULTIPLIER;
            int j = (index + 1) * SUBDIVISIONS_MULTIPLIER;
            float nt = _timesTable[i] + (_timesTable[j] - _timesTable[i]) * t;
            return this.GetRealPosition(nt);
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        #endregion

    }
}
