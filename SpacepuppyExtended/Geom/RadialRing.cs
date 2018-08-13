using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Geom
{

    /// <summary>
    /// 
    /// </summary>
    /// <programmernotes>
    /// It should be noted that this ring, or any circle for that matter, in 3 space. Is a circle on a surface of a plane. 
    /// The plane is defined by the same center and axis of the circle.
    /// 
    /// The standard formula for a plane is:
    /// Ax + By + Cz = D
    /// 
    /// The 'reduced' version of the standard formula will make the normal (A,B,C) and the distance from the origin on that vector is D.
    /// 
    /// From the center and normal, the reduced standard formula can be calculated as:
    /// 
    /// n.x * x + n.y * y + n.z * z = Dot(c, n)
    /// </programmernotes>
    public struct RadialRing : IPhysicsGeom
    {

        #region Fields

        public const int DEFAULT_DETAIL = 36;

        private Vector3 _center;
        private Vector3 _axis;
        private float _radius;
        private float _thickness;
        private byte _detail;

        #endregion

        #region CONSTRUCTOR

        public RadialRing(Vector3 cent, Vector3 axis, float rad, float thick, byte detail)
        {
            _center = cent;
            _axis = axis.normalized;
            _radius = rad;
            _thickness = thick;
            _detail = detail;
        }

        #endregion

        #region Properties

        public Vector3 Center
        {
            get { return _center; }
            set { _center = value; }
        }

        public Vector3 Axis
        {
            get { return _axis; }
            set { _axis = value.normalized; }
        }

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public float Thickness
        {
            get { return _thickness; }
            set { _thickness = value; }
        }

        public byte Detail
        {
            get { return _detail; }
            set { _detail = value; }
        }

        #endregion

        #region Methods

        public bool TestOverlap(Vector3 primeRadial, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (Vector3.Dot(primeRadial, _axis) != 0)
            {
                primeRadial -= Vector3.Cross(_axis, Vector3.Cross(primeRadial, _axis));
            }
            primeRadial.Normalize();

            var da = 360f / _detail;
            Vector3 va;
            Vector3 vb = _center + primeRadial * _radius;
            for (int i = 1; i <= _detail; i++)
            {
                va = vb;
                vb = _center + Quaternion.AngleAxis(da * i, _axis) * primeRadial * _radius;

                var dir = vb - va;
                var dis = dir.magnitude;
                if (_thickness == 0f)
                {
                    if (Physics.Raycast(new Ray(va, dir), dis, layerMask, query)) return true;
                }
                else
                {
                    if (Physics.SphereCast(new Ray(va, dir), _thickness, dis, layerMask, query)) return true;
                }
            }

            return false;
        }

        public int Overlap(Vector3 primeRadial, ICollection<Collider> results, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            if (Vector3.Dot(primeRadial, _axis) != 0)
            {
                primeRadial -= Vector3.Cross(_axis, Vector3.Cross(primeRadial, _axis));
            }
            primeRadial.Normalize();

            int cnt = results.Count;
            using (var tmp = com.spacepuppy.Collections.TempCollection.GetSet<Collider>())
            {
                var da = 360f / _detail;
                Vector3 va;
                Vector3 vb = _center + primeRadial * _radius;
                for (int i = 1; i <= _detail; i++)
                {
                    va = vb;
                    vb = _center + Quaternion.AngleAxis(da * i, _axis) * primeRadial * _radius;

                    var dir = vb - va;
                    var dis = dir.magnitude;

                    if (_thickness == 0f)
                    {
                        foreach (var h in Physics.RaycastAll(new Ray(va, dir), dis, layerMask, query))
                        {
                            if (!tmp.Contains(h.collider))
                            {
                                results.Add(h.collider);
                                tmp.Add(h.collider);
                            }
                        }
                    }
                    else
                    {
                        foreach (var h in Physics.SphereCastAll(new Ray(va, dir), _thickness, dis, layerMask, query))
                        {
                            if (!tmp.Contains(h.collider))
                            {
                                results.Add(h.collider);
                                tmp.Add(h.collider);
                            }
                        }
                    }
                }
            }
            return results.Count - cnt;
        }

        public bool RadialCast(Vector3 primeRadial, out RaycastHit hitinfo, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (Vector3.Dot(primeRadial, _axis) != 0)
            {
                primeRadial -= Vector3.Cross(_axis, Vector3.Cross(primeRadial, _axis));
            }
            primeRadial.Normalize();

            var da = 360f / _detail;
            Vector3 va;
            Vector3 vb = primeRadial * _radius;
            for (int i = 1; i <= _detail; i++)
            {
                va = vb;
                vb = _center + Quaternion.AngleAxis(da * i, _axis) * primeRadial * _radius;

                var dir = vb - va;
                var dis = dir.magnitude;
                if (_thickness == 0f)
                {
                    if (Physics.Raycast(new Ray(va, dir), out hitinfo, dis, layerMask, query)) return true;
                }
                else
                {
                    if (Physics.SphereCast(new Ray(va, dir), _thickness, out hitinfo, dis, layerMask, query)) return true;
                }
            }

            hitinfo = new RaycastHit();
            return false;
        }

        public bool Cast(Vector3 primeRadial, Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (Vector3.Dot(primeRadial, _axis) != 0)
            {
                primeRadial -= Vector3.Cross(_axis, Vector3.Cross(primeRadial, _axis));
            }
            primeRadial.Normalize();

            var da = 360f / _detail;
            for (int i = 0; i < _detail; i++)
            {
                var v = _center + Quaternion.AngleAxis(da * i, _axis) * primeRadial * _radius;

                if (_thickness == 0f)
                {
                    if (Physics.Raycast(new Ray(v, direction), out hitinfo, distance, layerMask, query)) return true;
                }
                else
                {
                    if (Physics.SphereCast(new Ray(v, direction), _thickness, out hitinfo, distance, layerMask, query)) return true;
                }
            }

            hitinfo = new RaycastHit();
            return false;
        }

        public int CastAll(Vector3 primeRadial, Vector3 direction, ICollection<RaycastHit> results, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            if (Vector3.Dot(primeRadial, _axis) != 0)
            {
                primeRadial -= Vector3.Cross(_axis, Vector3.Cross(primeRadial, _axis));
            }
            primeRadial.Normalize();

            int cnt = results.Count;
            var da = 360f / _detail;
            for (int i = 0; i < _detail; i++)
            {
                var v = _center + Quaternion.AngleAxis(da * i, _axis) * primeRadial * _radius;

                if (_thickness == 0f)
                {
                    foreach (var h in Physics.RaycastAll(new Ray(v, direction), distance, layerMask, query))
                    {
                        results.Add(h);
                    }
                }
                else
                {
                    foreach (var h in Physics.SphereCastAll(new Ray(v, direction), _thickness, distance, layerMask, query))
                    {
                        results.Add(h);
                    }
                }
            }

            return results.Count - cnt;
        }

        #endregion

        #region IGeom Interface

        public void Move(Vector3 mv)
        {
            _center += mv;
        }

        public void RotateAroundPoint(Vector3 point, Quaternion rot)
        {
            _center = point + rot * (_center - point);
            _axis = rot * _axis;
        }

        public AxisInterval Project(Vector3 axis)
        {
            throw new System.NotImplementedException();
        }

        public Bounds GetBounds()
        {
            throw new System.NotImplementedException();
        }

        public Sphere GetBoundingSphere()
        {
            return new Sphere(_center, _radius + _thickness);
        }

        public bool Contains(Vector3 pos)
        {
            var d = Vector3.Distance(_center, pos);
            return d >= _radius - _thickness && d <= _radius + _thickness;
        }

        public IEnumerable<Vector3> GetAxes()
        {
            //TODO
            return System.Linq.Enumerable.Empty<Vector3>();
        }

        #endregion

        #region IPhysicsGeom Interface

        public bool TestOverlap(int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return TestOverlap((_axis == Vector3.up) ? Vector3.right : Vector3.up, layerMask, query);
        }

        public int Overlap(ICollection<Collider> results, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return Overlap((_axis == Vector3.up) ? Vector3.right : Vector3.up, results, layerMask, query);
        }

        public bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return Cast((_axis == Vector3.up) ? Vector3.right : Vector3.up, direction, out hitinfo, distance, layerMask, query);
        }

        public int CastAll(Vector3 direction, ICollection<RaycastHit> results, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return CastAll((_axis == Vector3.up) ? Vector3.right : Vector3.up, direction, results, distance, layerMask, query);
        }

        #endregion



        public IEnumerable<RaycastInfo> GetSegmentRays(Vector3 primeRadial)
        {
            if (Vector3.Dot(primeRadial, _axis) != 0)
            {
                primeRadial -= Vector3.Cross(_axis, Vector3.Cross(primeRadial, _axis));
            }
            primeRadial.Normalize();

            var da = 360f / _detail;
            Vector3 va;
            Vector3 vb = _center + primeRadial * _radius;
            for (int i = 1; i <= _detail; i++)
            {
                va = vb;
                vb = _center + Quaternion.AngleAxis(da * i, _axis) * primeRadial * _radius;

                var dir = vb - va;
                var dis = dir.magnitude;
                yield return new RaycastInfo(va, dir, dis);
            }
        }

    }
}
