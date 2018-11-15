using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{

    public struct Cone : IGeom
    {

        #region Fields

        private Vector3 _start;
        private Vector3 _end;
        private float _startRad;
        private float _endRad;

        #endregion

        #region CONSTRUCTOR

        public Cone(Vector3 start, Vector3 end, float startRadius, float endRadius)
        {
            _start = start;
            _end = end;
            _startRad = startRadius;
            _endRad = endRadius;
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

        public float StartRadius
        {
            get { return _startRad; }
            set { _startRad = value; }
        }

        public float EndRadius
        {
            get { return _endRad; }
            set { _endRad = value; }
        }

        public float Height
        {
            get
            {
                return (_end - _start).magnitude;
            }
            set
            {
                var c = this.Center;
                var up = (_end - _start).normalized;
                var change = up * value / 2.0f;
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
            var rod = _end - _start;
            double r1 = rod.magnitude;
            rod /= (float)r1;
            r1 /= 2d;
            double r2 = System.Math.Max(_startRad, _endRad);

            return new Sphere(_start + rod * (float)r1, (float)System.Math.Sqrt(r1 * r1 + r2 * r2));
        }

        public IEnumerable<Vector3> GetAxes()
        {
            //TODO
            return System.Linq.Enumerable.Empty<Vector3>();
        }

        public bool Contains(Vector3 pos)
        {
            return ContainsPoint(_start, _end, _startRad, _endRad, pos);
        }

        #endregion



        #region Static Utils

        public static bool ContainsPoint(Vector3 start, Vector3 end, float startRadius, float endRadius, Vector3 pnt)
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
                float radius;
                if (sqrRailLength < 0.0001f)
                    radius = Mathf.Max(startRadius, endRadius);
                else
                    radius = startRadius + (endRadius - startRadius) * dot / sqrRailLength;

                if (rod.sqrMagnitude - dot * dot / sqrRailLength > radius * radius)
                    return false;
                else
                    return true;
            }
        }

        public static bool ContainsSphere(Vector3 start, Vector3 end, float startRadius, float endRadius, Vector3 pnt, float sphereRadius)
        {
            var rail = end - start;
            var rod = pnt - start;
            var dot = Vector3.Dot(rod, rail);
            var sqrRailLength = rail.sqrMagnitude;
            float sqrSphereRad = sphereRadius * sphereRadius;

            if (dot < -sqrSphereRad || dot > sqrRailLength + sqrSphereRad)
            {
                return false;
            }
            else
            {
                float radius;
                if (sqrRailLength < 0.0001f)
                    radius = Mathf.Max(startRadius, endRadius);
                else
                    radius = startRadius + (endRadius - startRadius) * dot / sqrRailLength;

                if (rod.sqrMagnitude - dot * dot / sqrRailLength > radius * radius + sqrSphereRad)
                    return false;
                else
                    return true;
            }
        }

        #endregion

    }

}
