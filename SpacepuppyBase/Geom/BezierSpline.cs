using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{

    /// <summary>
    /// Represents a N-Degree Bezier Spline.
    /// </summary>
    public class BezierSpline : I3dSpline
    {

        #region Fields

        private List<Vector3> _points = new List<Vector3>();

        #endregion

        #region CONSTRUCTOR

        public BezierSpline()
        {

        }

        #endregion

        #region Properties

        public Vector3 this[int index]
        {
            get { return this.ControlPoint(index); }
            set { this.MoveControlPoint(index, value); }
        }

        #endregion

        #region Methods

        private Vector3 ComputeNaive(float t)
        {
            if (_points.Count == 0) return Vector3.zero;
            if (_points.Count == 1) return _points[0];

            t = Mathf.Clamp01(t);
            var arr = _points.ToArray();
            var c = arr.Length;
            while (c > 1)
            {
                for (int i = 1; i < c; i++)
                {
                    arr[i - 1] = Vector3.Lerp(arr[i - 1], arr[i], t);
                }

                c--;
            }

            return arr[0];
        }

        private Vector3 ComputeNaive(int index, float t)
        {
            if (index < 0 || index >= _points.Count) throw new System.IndexOutOfRangeException();

            t = Mathf.Clamp01(t);
            float pt = 1.0f / (float)(_points.Count - 1);
            return ComputeNaive(pt * ((float)index + t));
        }

        private float ComputeNaiveArcLength()
        {
            //todo
            return float.NaN;
        }

        #endregion

        #region I3dSpline Interface

        public int Count { get { return _points.Count; } }

        public Vector3 ControlPoint(int index)
        {
            if (index < 0 || index >= _points.Count) throw new System.IndexOutOfRangeException();
            return _points[index];
        }

        public void AddControlPoint(Vector3 w)
        {
            _points.Add(w);
        }

        public void MoveControlPoint(int index, Vector3 w)
        {
            if (index < 0 || index >= _points.Count) throw new System.IndexOutOfRangeException();
            _points[index] = w;
        }

        public void RemoveControlPoint(int index)
        {
            _points.RemoveAt(index);
        }

        public float GetArcLength()
        {
            return ComputeNaiveArcLength();
        }

        public Vector3 GetPosition(float t)
        {
            return ComputeNaive(t);
        }

        public Vector3 GetPositionAfter(int index, float t)
        {
            return ComputeNaive(index, t);
        }

        #endregion

        #region IEnumerable Interface

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
