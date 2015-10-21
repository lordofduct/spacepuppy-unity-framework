using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        public static Sphere FromCollider(SphereCollider s, bool local = false)
        {
            if (s == null) return new Sphere();

            if(local)
            {
                return new Sphere(s.center, s.radius);
            }
            else
            {
                var sc = s.transform.lossyScale;
                var cent = s.transform.TransformPoint(s.center);
                var msc = Mathf.Max(sc.x, sc.y, sc.z);
                return new Sphere(cent, s.radius * msc);
            }
        }

        public static Sphere FromCollider(CapsuleCollider c, bool local = false)
        {
            if(local)
            {
                var r = Mathf.Max(c.radius, c.height / 2.0f);
                return new Sphere(c.center, r);
            }
            else
            {
                var sc = c.transform.lossyScale;
                var cent = c.transform.TransformPoint(c.center);
                var msc = Mathf.Max(sc.x, sc.y, sc.z);
                var r = Mathf.Max(c.radius, c.height / 2.0f);
                return new Sphere(cent, r * msc);
            }
        }

        public static Sphere FromCollider(Collider c, bool local = false)
        {
            if (c is SphereCollider)
            {
                return FromCollider(c as SphereCollider, local);
            }
            else if (c is CapsuleCollider)
            {
                return FromCollider(c as CapsuleCollider, local);
            }
            else if (GeomUtil.DefaultBoundingSphereAlgorithm != BoundingSphereAlgorithm.FromBounds && c is MeshCollider)
            {
                return FromMesh((c as MeshCollider).sharedMesh, GeomUtil.DefaultBoundingSphereAlgorithm, Trans.GetGlobal(c.transform));
            }
            else
            {
                var bounds = AABBox.FromCollider(c, local);
                return new Sphere(bounds.Center, bounds.Extents.magnitude);
            }
        }

        public static Sphere FromCollider(Collider c, BoundingSphereAlgorithm algorithm, bool local = false)
        {
            if (c is SphereCollider)
            {
                return FromCollider(c as SphereCollider, local);
            }
            else if (c is CapsuleCollider)
            {
                return FromCollider(c as CapsuleCollider, local);
            }
            else if (algorithm != BoundingSphereAlgorithm.FromBounds && c is MeshCollider)
            {
                return FromMesh((c as MeshCollider).sharedMesh, algorithm, Trans.GetGlobal(c.transform));
            }
            else
            {
                var bounds = AABBox.FromCollider(c, local);
                return new Sphere(bounds.Center, bounds.Extents.magnitude);
            }
        }

        public static Sphere FromMesh(Mesh mesh, BoundingSphereAlgorithm algorithm)
        {
            if (mesh == null) return new Sphere();

            if (algorithm != BoundingSphereAlgorithm.FromBounds)
            {
                var arr = mesh.vertices;
                if (arr.Length > 0)
                {
                    return FromPoints(arr, algorithm);
                }
                else
                {
                    return new Sphere();
                }
            }
            else
            {
                var bounds = mesh.bounds;
                return new Sphere(bounds.center, bounds.extents.magnitude);
            }
        }

        public static Sphere FromMesh(Mesh mesh, BoundingSphereAlgorithm algorithm, Trans transform)
        {
            if (mesh == null) return new Sphere();

            var arr = mesh.vertices;
            if (arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = transform.TransformPoint(arr[i]);
                }
                return FromPoints(arr, algorithm);
            }
            else
            {
                return new Sphere();
            }
        }

        public static Sphere FromBounds(Bounds bounds)
        {
            return new Sphere(bounds.center, Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z));
        }

        public static Sphere FromPoints(IEnumerable<Vector3> points, BoundingSphereAlgorithm algorithm)
        {
            return FromPoints(points.ToArray(), algorithm);
        }

        public static Sphere FromPoints(Vector3[] points, BoundingSphereAlgorithm algorithm)
        {

            switch (algorithm)
            {
                case BoundingSphereAlgorithm.FromBounds:
                    {
                        var bounds = AABBox.FromPoints(points);
                        return new Sphere(bounds.Center, bounds.Extents.magnitude);
                    }
                case BoundingSphereAlgorithm.Average:
                    {
                        Vector3 sum = Vector3.zero;
                        foreach(var v in points)
                        {
                            sum += v;
                        }
                        sum /= points.Length;
                        float dist = 0f;
                        float d;
                        foreach(var v in points)
                        {
                            d = (v - sum).sqrMagnitude;
                            if (d > dist) dist = d;
                        }
                        dist = Mathf.Sqrt(dist);
                        return new Sphere(sum, dist);
                    }
                case BoundingSphereAlgorithm.Ritter:
                    {
                        Vector3 xmin, xmax, ymin, ymax, zmin, zmax;
                        xmin = ymin = zmin = Vector3.one * float.PositiveInfinity;
                        xmax = ymax = zmax = Vector3.one * float.NegativeInfinity;
                        foreach (var p in points)
                        {
                            if (p.x < xmin.x) xmin = p;
                            if (p.x > xmax.x) xmax = p;
                            if (p.y < ymin.y) ymin = p;
                            if (p.y > ymax.y) ymax = p;
                            if (p.z < zmin.z) zmin = p;
                            if (p.z > zmax.z) zmax = p;
                        }
                        var xSpan = (xmax - xmin).sqrMagnitude;
                        var ySpan = (ymax - ymin).sqrMagnitude;
                        var zSpan = (zmax - zmin).sqrMagnitude;
                        var dia1 = xmin;
                        var dia2 = xmax;
                        var maxSpan = xSpan;
                        if (ySpan > maxSpan)
                        {
                            maxSpan = ySpan;
                            dia1 = ymin; dia2 = ymax;
                        }
                        if (zSpan > maxSpan)
                        {
                            dia1 = zmin; dia2 = zmax;
                        }
                        var center = (dia1 + dia2) * 0.5f;
                        var sqRad = (dia2 - center).sqrMagnitude;
                        var radius = Mathf.Sqrt(sqRad);
                        foreach (var p in points)
                        {
                            float d = (p - center).sqrMagnitude;
                            if (d > sqRad)
                            {
                                var r = Mathf.Sqrt(d);
                                radius = (radius + r) * 0.5f;
                                sqRad = radius * radius;
                                var offset = r - radius;
                                center = (radius * center + offset * p) / r;
                            }
                        }
                        return new Sphere(center, radius);
                    }
            }

            return new Sphere();
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
