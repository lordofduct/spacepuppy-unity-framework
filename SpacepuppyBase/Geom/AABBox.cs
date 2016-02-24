using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    [System.Serializable]
    public struct AABBox : IGeom, System.Runtime.Serialization.ISerializable, IPhysicsGeom
    {

        #region Fields

        [SerializeField()]
        private Vector3 _center;
        [SerializeField()]
        private Vector3 _size;

        #endregion

        #region CONSTRUCTOR

        public AABBox(Vector3 cent, Vector3 sz)
        {
            _center = cent;
            _size = sz;
        }

        public AABBox(Bounds bounds)
        {
            _center = bounds.center;
            _size = bounds.size;
        }

        #endregion

        #region Properties

        public Vector3 Center { get { return _center; } set { _center = value; } }
        public Vector3 Size { get { return _size; } set { _size = value; } }

        public Vector3 Extents
        {
            get { return _size / 2.0f; }
        }

        public Vector3 Min
        {
            get { return this.Center - (this.Size / 2.0f); }
        }

        public Vector3 Max
        {
            get { return this.Center + (this.Size / 2.0f); }
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
        }

        public AxisInterval Project(Vector3 axis)
        {
            axis.Normalize();
            var a = Vector3.Dot(this.Min, axis);
            var b = Vector3.Dot(this.Max, axis);
            return new AxisInterval(axis, a, b);
        }

        public Bounds GetBounds()
        {
            return new Bounds(Center, Size);
        }

        public Sphere GetBoundingSphere()
        {
            //return new Sphere(Center, com.spacepuppy.Utils.VectorUtil.GetMaxScalar(Size) / 2.0f);
            return new Sphere(Center, this.Extents.magnitude);
        }

        public bool Contains(Vector3 pos)
        {
            return this.GetBounds().Contains(pos);
        }

        public IEnumerable<Vector3> GetAxes()
        {
            yield return Vector3.up;
            yield return Vector3.right;
            yield return Vector3.forward;
        }

        /*
        public System.Collections.Generic.IEnumerable<RaycastInfo> GetRays(float detail)
        {
            //infinitely thin objects don't cast anywhere
            if (Size.y == 0.0f) yield break;

            var min = Center - (Size / 2.0f);
            var dir = new Vector3(0, MathUtil.Sign(Size.y), 0);
            var dist = Mathf.Abs(Size.y);
            var dx = new Vector3(MathUtil.Sign(Size.x), 0);
            var dz = new Vector3(MathUtil.Sign(Size.y), 0);

            int xp = Mathf.CeilToInt(this.Size.x / detail); //number of moves in the x-dir
            int zp = Mathf.CeilToInt(this.Size.z / detail); //number of moves in the z-dir

            float xdt = this.Size.x / (float)xp; //adjusted detail in the x-dir
            float zdt = this.Size.z / (float)zp; //adjusted detail in the z-dir

            int cnt = xp * zp;
            for (int i = 0; i < cnt; i++)
            {
                var ix = i % xp;
                var iz = i / xp;
                yield return new RaycastInfo(min + ix * xdt * dx + iz * zdt * dz, dir, dist);
            }
        }

        public System.Collections.Generic.IEnumerable<RaycastInfo> GetRays(Vector3 dir, float detail)
        {
            //infinitely thin objects don't cast anywhere
            if (Vector3.Dot(Size, dir) == 0) yield break;


        }

        public System.Collections.Generic.IEnumerable<RaycastInfo> GetRays(Vector3 dir, float dist, float detail)
        {
            //TODO
            yield break;
        }
         */

        #endregion

        #region IPhysicsGeom Interface

        public bool TestOverlap(int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.CheckBox(_center, this.Extents, Quaternion.identity, layerMask, query);
        }

        public int Overlap(ICollection<Collider> results, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return PhysicsUtil.OverlapBox(_center, this.Extents, results, Quaternion.identity, layerMask, query);
        }

        public bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.BoxCast(_center, this.Extents, direction, out hitinfo, Quaternion.identity, distance, layerMask, query);
        }

        public int CastAll(Vector3 direction, ICollection<RaycastHit> results, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return PhysicsUtil.BoxCastAll(_center, this.Extents, direction, results, Quaternion.identity, distance, layerMask, query);
        }

        #endregion


        #region ISerializable Interface

        private AABBox(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _center = new Vector3(info.GetSingle("center.x"), info.GetSingle("center.y"), info.GetSingle("center.z"));
            _size = new Vector3(info.GetSingle("size.x"), info.GetSingle("size.y"), info.GetSingle("size.z"));
        }

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("center.x", this.Center.x);
            info.AddValue("center.y", this.Center.y);
            info.AddValue("center.z", this.Center.z);
            info.AddValue("size.x", this.Size.x);
            info.AddValue("size.y", this.Size.y);
            info.AddValue("size.z", this.Size.z);
        }

        #endregion


        #region Static Interface

        public static AABBox FromCollider(BoxCollider c, bool local = false)
        {
            if(local)
            {
                return new AABBox(c.center, c.size);
            }
            else
            {
                return new AABBox(c.bounds);
            }
        }

        public static AABBox FromCollider(Collider c, bool local = false)
        {
            if(c == null) return new AABBox();

            if(local)
            {
                if (c is CharacterController)
                {
                    var cap = c as CharacterController;
                    float r = cap.radius;
                    float d = r * 2f;
                    float h = Mathf.Max(r, cap.height);
                    return new AABBox(cap.center, new Vector3(d, h, d));
                }
                if (c is CapsuleCollider)
                {
                    var cap = c as CapsuleCollider;
                    float r = cap.radius;
                    float d = r * 2f;
                    float h = Mathf.Max(r, cap.height);
                    Vector3 sz;
                    switch (cap.direction)
                    {
                        case 0:
                            sz = new Vector3(h, d, d);
                            break;
                        case 1:
                            sz = new Vector3(d, h, d);
                            break;
                        case 2:
                            sz = new Vector3(d, d, h);
                            break;
                        default:
                            sz = Vector3.zero;
                            break;
                    }
                    return new AABBox(cap.center, sz);
                }
                else if (c is BoxCollider)
                {
                    var box = c as BoxCollider;
                    return new AABBox(box.center, box.size);
                }
                else if (c is SphereCollider)
                {
                    var s = c as SphereCollider;
                    return new AABBox(s.center, Vector3.one * s.radius * 2f);
                }
                else if (c is MeshCollider)
                {
                    return new AABBox((c as MeshCollider).sharedMesh.bounds);
                }
                else
                {
                    //otherwise just return bounds as AABBox
                    var bounds = c.bounds;
                    var cent = c.transform.InverseTransformPoint(bounds.center);
                    var size = c.transform.InverseTransformDirection(bounds.size);
                    return new AABBox(cent, size);
                }
            }
            else
            {
                return new AABBox(c.bounds);
            }
        }

        public static AABBox FromPoints(Vector3[] points)
        {
            float minx, miny, minz;
            minx = miny = minz = float.PositiveInfinity;
            float maxx, maxy, maxz;
            maxx = maxy = maxz = float.NegativeInfinity;

            foreach(var v in points)
            {
                if (v.x < minx) minx = v.x;
                if (v.y < miny) miny = v.y;
                if (v.z < minz) minz = v.z;

                if (v.x > maxx) maxx = v.x;
                if (v.y > maxy) maxy = v.y;
                if (v.z > maxz) maxz = v.z;
            }

            var size = new Vector3((maxx - minx),
                                   (maxy - miny),
                                   (maxz - minz));
            var cent = new Vector3(size.x / 2f + minx,
                                   size.y / 2f + miny,
                                   size.z / 2f + minz);
            return new AABBox(cent, size);
        }

        #endregion

    }
}
