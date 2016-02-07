using UnityEngine;

namespace com.spacepuppy.Geom
{

    [System.Serializable()]
    public struct Line
    {

        [SerializeField()]
        private Vector3 _pnt;
        [SerializeField()]
        private Vector3 _dir;

        public Line(Vector3 p, Vector3 d)
        {
            _pnt = p;
            _dir = d.normalized;
        }

        public Vector3 Point
        {
            get { return _pnt; }
            set { _pnt = value; }
        }

        public Vector3 Direction
        {
            get { return _dir; }
            set { _dir = value.normalized; }
        }

        /// <summary>
        /// Update line so that Point is the nearest point to the origin as possible
        /// </summary>
        public void Normalize()
        {
            _pnt = Line.ClosestPoint(this, Vector3.zero);
        }

        #region Static Methods

        public static bool Intersection(Line line1, Line line2, out Vector3 point)
        {
            const float EPSILON = 0.0001f;

            point = Vector3.zero;

            var dir3 = line2.Point - line1.Point;
            var pnorm1 = Vector3.Cross(line1.Direction, line2.Direction);
            var pnorm2 = Vector3.Cross(dir3, line2.Direction);

            if (Mathf.Abs(Vector3.Dot(dir3, pnorm1)) > EPSILON) return false; //lines aren't coplanar

            float s = Vector3.Dot(pnorm2, pnorm1) / pnorm1.sqrMagnitude;
            if (s >= 0 && s <= 1.0f)
            {
                point = line1.Point + (line1.Direction * s);
                return true;
            }

            return false;
        }

        public static bool ClosestPoints(Line line1, Line line2, out Vector3 line1ClosestPoint, out Vector3 line2ClosestPoint)
        {
            //a and e are going to be 1, drop them out...
            float b = Vector3.Dot(line1.Direction, line2.Direction);

            float d = 1 - b * b;
            if (d != 0.0f)
            {
                var r = line1.Point - line2.Point;
                float c = Vector3.Dot(line1.Direction, r);
                float f = Vector3.Dot(line2.Direction, r);

                float s = (b * f - c) / d;
                float t = (f - c * b) / d;

                line1ClosestPoint = line1.Point + line1.Direction * s;
                line2ClosestPoint = line2.Point + line2.Direction * t;
                return true;
            }
            else
            {
                line1ClosestPoint = Vector3.zero;
                line2ClosestPoint = Vector3.zero;
                return false;
            }
        }

        public static Vector3 ClosestPoint(Line line, Vector3 point)
        {
            //if (line.Point == point) return point;

            //var v = (line.Point - point).normalized;
            //v = Vector3.Cross(line.Direction, v);

            //float b = Vector3.Dot(line.Direction, v);

            //float d = 1 - b * b;
            //var r = line.Point - point;
            //float c = Vector3.Dot(line.Direction, r);
            //float f = Vector3.Dot(v, r);

            //float s = (b * f - c) / d;
            //return line.Point + line.Direction * s;


            var v = point - line.Point;
            var t = Vector3.Dot(v, line.Direction);
            return line.Point + line.Direction * t;
        }

        #endregion

    }

}
