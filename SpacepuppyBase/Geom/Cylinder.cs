using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    public struct Cylinder : IGeom
    {

        #region Fields

        private Vector3 _start;
        private Vector3 _end;
        private float _rad;

        #endregion

        #region CONSTRUCTOR

        public Cylinder(Vector3 start, Vector3 end, float radius)
        {
            _start = start;
            _end = end;
            _rad = radius;
        }

        public Cylinder(Vector3 center, Vector3 up, float height, float radius)
        {
            var h = (height - (radius * 2.0f)) / 2.0f;
            var change = up.normalized * h;

            _start = center - change;
            _end = center + change;
            _rad = radius;
        }

        #endregion

        #region Properties

        public Vector3 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector3 End
        {
            get { return _end; }
            set { _end = value; }
        }

        public float Radius
        {
            get { return _rad; }
            set { _rad = value; }
        }

        public float Height
        {
            get
            {
                if (_end == _start)
                    return _rad;
                else
                    return (_end - _start).magnitude + _rad * 2.0f;
            }
            set
            {
                var c = this.Center;
                var up = (_end - _start).normalized;
                var change = up * (value - (_rad * 2.0f));
                _start = c - change;
                _end = c + change;
            }
        }

        public Vector3 Center
        {
            get
            {
                if (_end == _start)
                    return _start;
                else
                    return _start + (_end - _start) * 0.5f;
            }
            set
            {
                var change = (value - this.Center);
                _start += change;
                _end += change;
            }
        }

        public Vector3 Up
        {
            get
            {
                if (_end == _start)
                    return Vector3.up;
                else
                    return (_end - _start).normalized;
            }
        }

        #endregion



        #region IGeom Interface

        public void Move(Vector3 mv)
        {
            _start += mv;
            _end += mv;
        }

        public void RotateAroundPoint(Vector3 point, Quaternion rot)
        {
            _start = point + rot * (_start - point);
            _end = point + rot * (_end - point);
        }

        public AxisInterval Project(Vector3 axis)
        {
            //TODO
            throw new System.NotImplementedException();
        }

        public Bounds GetBounds()
        {
            //TODO
            throw new System.NotImplementedException();
        }

        public Sphere GetBoundingSphere()
        {
            //TODO
            throw new System.NotImplementedException();
        }

        public IEnumerable<Vector3> GetAxes()
        {
            //TODO
            return System.Linq.Enumerable.Empty<Vector3>();
        }

        public bool Contains(Vector3 pos)
        {
            var rail = _end - _start;
            var rod = pos - _start;
            var dot = Vector3.Dot(rod, rail);
            var sqrRailLength = rail.sqrMagnitude;

            if (dot < 0f || dot > sqrRailLength)
            {
                return false;
            }
            else
            {
                if (rod.sqrMagnitude - dot * dot / sqrRailLength > _rad * _rad)
                    return false;
                else
                    return true;
            }
        }

        #endregion

        #region Static Utils

        public static bool ContainsPoint(Vector3 start, Vector3 end, float radius, Vector3 pnt)
        {
            var rail = end - start;
            var rod = pnt - start;
            var dot = Vector3.Dot(rod, rail);
            var sqrRailLength = rail.sqrMagnitude;

            if (dot < 0f || dot > sqrRailLength)
            {
                return false;
            }
            else
            {
                if (rod.sqrMagnitude - dot * dot / sqrRailLength > radius * radius)
                    return false;
                else
                    return true;
            }
        }

        public static bool ContainsPoint(Vector3 start, Vector3 end, float radius, float innerRadius, Vector3 pnt)
        {
            var rail = end - start;
            var rod = pnt - start;
            var dot = Vector3.Dot(rod, rail);
            var sqrRailLength = rail.sqrMagnitude;

            if (dot < 0f || dot > sqrRailLength)
            {
                return false;
            }
            else
            {
                float radialDistSqr = rod.sqrMagnitude - dot * dot / sqrRailLength;
                if (radialDistSqr > radius * radius || radialDistSqr < innerRadius * innerRadius)
                    return false;
                else
                    return true;
            }
        }

        #endregion
    }
}
