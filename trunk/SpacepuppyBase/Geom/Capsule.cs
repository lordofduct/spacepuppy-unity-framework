using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    [System.Serializable]
    public struct Capsule : IGeom, IPhysicsGeom, System.Runtime.Serialization.ISerializable
    {

        #region Fields

        private Vector3 _start;
        private Vector3 _end;
        private float _rad;

        #endregion

        #region CONSTRUCTOR

        public Capsule(Vector3 start, Vector3 end, float radius)
        {
            _start = start;
            _end = end;
            _rad = radius;
        }

        public Capsule(Vector3 center, Vector3 up, float height, float radius)
        {
            var h = Mathf.Max(0f,(height - (radius * 2.0f)) / 2.0f);
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
                    return _rad * 2.0f;
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

        public bool IsSpherical
        {
            get { return _end == _start; }
        }

        #endregion


        #region IGeom Interface

        public AxisInterval Project(Vector3 axis)
        {
            axis.Normalize();
            var c1 = Vector3.Dot(_start, axis);
            var c2 = Vector3.Dot(_end, axis);
            var p1 = c1 - _rad;
            var p2 = c1 + _rad;
            var p3 = c2 - _rad;
            var p4 = c2 + _rad;

            return new AxisInterval(axis, Mathf.Min(p1, p2, p3, p4), Mathf.Max(p1, p2, p3, p4));
        }

        public Bounds GetBounds()
        {
            Vector3 c = this.Center;
            Vector3 sz = new Vector3();
            sz.x = Mathf.Abs(_start.x - c.x) + _rad;
            sz.y = Mathf.Abs(_start.y - c.y) + _rad;
            sz.z = Mathf.Abs(_start.z - c.y) + _rad;
            return new Bounds(c, sz);
        }

        public Sphere GetBoundingSphere()
        {
            return new Sphere(this.Center, (_end - _start).magnitude + _rad);
        }

        public bool Contains(Vector3 pos)
        {
            var sqrRad = _rad * _rad;

            if (this.IsSpherical)
            {
                return Vector3.SqrMagnitude(pos - _start) <= sqrRad;
            }
            else
            {
                if (Vector3.SqrMagnitude(pos - _start) <= sqrRad) return true;
                if (Vector3.SqrMagnitude(pos - _end) <= sqrRad) return true;
            }

            var rail = _end - _start;
            var rod = pos - _start;
            var sqrLen = rod.sqrMagnitude;
            var dot = Vector3.Dot(rod, rail);

            if (dot < 0f || dot > sqrLen)
            {
                return false;
            }
            else
            {
                var disSqr = rod.sqrMagnitude - dot * dot / sqrLen;
                if (disSqr > sqrRad)
                    return false;
                else
                    return true;
            }
        }

        public bool Intersects(IGeom geom)
        {
            //TODO
            throw new System.NotImplementedException();
        }

        public bool Intersects(Bounds bounds)
        {
            //TODO
            throw new System.NotImplementedException();
        }

        #endregion

        #region IPhysicsGeom Interface

        public bool TestOverlap(int layerMask)
        {
            if (_start == _end)
            {
                return Physics.CheckSphere(_start, _rad, layerMask);
            }
            else
            {
                return Physics.CheckCapsule(_start, _end, _rad, layerMask);
            }
        }

        public IEnumerable<Collider> Overlap(int layerMask)
        {
            var hits = new List<Collider>();

            //first overlap start sphere
            hits.AddRange(Physics.OverlapSphere(_start, _rad, layerMask));
            //now overlap the end sphere, don't add duplicates
            foreach (var c in Physics.OverlapSphere(_end, _rad, layerMask))
            {
                if (!hits.Contains(c)) hits.Add(c);
            }
            //lastly cast from start to end, don't add duplicates
            var dir = _end - _start;
            var dist = dir.magnitude;
            foreach (var h in Physics.SphereCastAll(_start, _rad, dir, dist, layerMask))
            {
                if (!hits.Contains(h.collider)) hits.Add(h.collider);
            }

            return hits;
        }

        public bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask)
        {
            if (_start == _end)
            {
                return Physics.SphereCast(_start, _rad, direction, out hitinfo, distance, layerMask);
            }
            else
            {
                return Physics.CapsuleCast(_start, _end, _rad, direction, out hitinfo, distance, layerMask);
            }
        }

        public IEnumerable<RaycastHit> CastAll(Vector3 direction, float distance, int layerMask)
        {
            if (_start == _end)
            {
                return Physics.SphereCastAll(_start, _rad, direction, distance, layerMask);
            }
            else
            {
                return Physics.CapsuleCastAll(_start, _end, _rad, direction, distance, layerMask);
            }
        }

        #endregion


        #region ISerializable Interface

        private Capsule(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _start = new Vector3(info.GetSingle("start.x"), info.GetSingle("start.y"), info.GetSingle("start.z"));
            _end = new Vector3(info.GetSingle("end.x"), info.GetSingle("end.y"), info.GetSingle("end.z"));
            _rad = info.GetSingle("radius");
        }

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("start.x", _start.x);
            info.AddValue("start.y", _start.y);
            info.AddValue("start.z", _start.z);
            info.AddValue("end.x", _end.x);
            info.AddValue("end.y", _end.y);
            info.AddValue("end.z", _end.z);
            info.AddValue("radius", _rad);
        }

        #endregion


        #region Static Interface

        public static Capsule FromCollider(CharacterController cap)
        {
            var cent = cap.transform.position + cap.center;
            var hsc = Mathf.Max(cap.transform.lossyScale.x, cap.transform.lossyScale.y);
            var vsc = cap.transform.lossyScale.y;

            return new Capsule(cent, Vector3.up, cap.height * vsc, cap.radius * hsc);
        }

        public static Capsule FromCollider(CapsuleCollider cap)
        {
            Vector3 axis;
            float hsc;
            float vsc;
            switch (cap.direction)
            {
                case 0:
                    axis = cap.transform.right;
                    hsc = Mathf.Max(cap.transform.lossyScale.x, cap.transform.lossyScale.y);
                    vsc = cap.transform.lossyScale.x;
                    break;
                case 1:
                    axis = cap.transform.up;
                    hsc = Mathf.Max(cap.transform.lossyScale.x, cap.transform.lossyScale.y);
                    vsc = cap.transform.lossyScale.y;
                    break;
                case 2:
                    axis = cap.transform.forward;
                    hsc = Mathf.Max(cap.transform.lossyScale.z, cap.transform.lossyScale.y);
                    vsc = cap.transform.lossyScale.z;
                    break;
                default:
                    return new Capsule();
            }

            var cent = cap.center;

            cent = cap.transform.TransformPoint(cent);
            return new Capsule(cent, axis, cap.height * vsc, cap.radius * hsc);
        }

        public static Capsule FromCollider(SphereCollider sph)
        {
            var cent = sph.transform.TransformPoint(sph.center);
            return new Capsule(cent, cent, sph.radius);
        }

        #endregion

    }
}
