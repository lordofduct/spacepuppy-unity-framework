using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    [System.Serializable]
    public struct Sphere : IGeom, IPhysicsGeom, System.Runtime.Serialization.ISerializable
    {

        #region Fields

        [SerializeField()]
        private Vector3 _cent;
        [SerializeField()]
        private float _rad;

        #endregion

        #region CONSTRUCTOR

        public Sphere(Vector3 cent, float r)
        {
            _cent = cent;
            _rad = r;
        }

        #endregion

        #region Properties

        public Vector3 Center
        {
            get { return _cent; }
            set { _cent = value; }
        }

        public float Radius
        {
            get { return _rad; }
            set { _rad = value; }
        }

        #endregion

        #region Methods

        public void Encapsulate(IGeom geom)
        {
            /*
            var s = geom.GetBoundingSphere();
            var v = (s.Center - _cent).normalized;
            var p1 = this.Project(v);
			var p2 = s.Project(v);
			var p = p1;
            p.Concat(p2);
			
			_rad = p.Length / 2.0f;
            float dot = Vector3.Dot(_cent, p.Axis);
			float offset = Mathf.Abs(p.Min - dot);
			_cent = (_cent - v * offset) + v * _rad;
             */

            var s = geom.GetBoundingSphere();
            var v = s.Center - _cent;
            var l = v.magnitude;

            if (l + s.Radius > _rad)
            {
                v.Normalize();
                _rad = (l + s.Radius + _rad) / 2.0f;
                _cent += v * (l + s.Radius - _rad);
            }

        }

        #endregion


        #region IGeom Interface

        public AxisInterval Project(Vector3 axis)
        {
            axis.Normalize();
            var c = Vector3.Dot(_cent, axis);
            return new AxisInterval(axis, c - _rad, c + _rad);
        }

        public Bounds GetBounds()
        {
            return new Bounds(_cent, Vector3.one * _rad);
        }

        public Sphere GetBoundingSphere()
        {
            return this;
        }

        public bool Contains(Vector3 pos)
        {
            return (pos - _cent).sqrMagnitude < (_rad * _rad);
        }

        public IEnumerable<Vector3> GetAxes()
        {
            return System.Linq.Enumerable.Empty<Vector3>();
        }

        #endregion

        #region IPhysicsGeom Interface

        public bool TestOverlap(int layerMask)
        {
            return Physics.CheckSphere(_cent, _rad, layerMask);
        }

        public IEnumerable<Collider> Overlap(int layerMask)
        {
            return Physics.OverlapSphere(_cent, _rad, layerMask);
        }

        public bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask)
        {
            return Physics.SphereCast(_cent, _rad, direction, out hitinfo, distance, layerMask);
        }

        public IEnumerable<RaycastHit> CastAll(Vector3 direction, float distance, int layerMask)
        {
            return Physics.SphereCastAll(_cent, _rad, direction, distance, layerMask);
        }

        #endregion

        
        #region ISerializable Interface

        private Sphere(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _cent = new Vector3(info.GetSingle("center.x"), info.GetSingle("center.y"), info.GetSingle("center.z"));
            _rad = info.GetSingle("radius");
        }

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("center.x", _cent.x);
            info.AddValue("center.y", _cent.y);
            info.AddValue("center.z", _cent.z);
            info.AddValue("radius", _rad);
        }

        #endregion


        #region Static Interface

        public static Sphere FromCollider(SphereCollider s)
        {
            var sc = s.transform.lossyScale;
            var cent = s.transform.TransformPoint(s.center);
            var msc = Mathf.Max(sc.x, sc.y, sc.z);
            return new Sphere(cent, s.radius * msc);
        }

        public static Sphere FromCollider(CapsuleCollider c)
        {
            var sc = c.transform.lossyScale;
            var cent = c.transform.TransformPoint(c.center);
            var msc = Mathf.Max(sc.x, sc.y, sc.z);
            var r = Mathf.Max(c.radius, c.height / 2.0f);
            return new Sphere(cent, r * msc);
        }

        public static Sphere FromCollider(Collider c)
        {
            if (c is SphereCollider)
            {
                return FromCollider(c as SphereCollider);
            }
            else if (c is CapsuleCollider)
            {
                return FromCollider(c as CapsuleCollider);
            }
            else
            {
                var bounds = c.bounds;
                return new Sphere(bounds.center, bounds.extents.magnitude);
            }
        }

        public static Sphere FromMesh(Mesh mesh)
        {
            if (mesh == null) return new Sphere();

            //TODO - calculate this more accurately using some 'SmallestSphere' algorithm
            var bounds = mesh.bounds;
            return new Sphere(bounds.center, Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z));
        }

        /// <summary>
        /// Measures the distance between 2 spheres. If the value is negative, the spheres are overlapping.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static float DistanceBetween(Sphere s1, Sphere s2)
        {
            return (s1.Center - s2.Center).magnitude - (s1.Radius + s2.Radius);
        }

        #endregion

    }
}
