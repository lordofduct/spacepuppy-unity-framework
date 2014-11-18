using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    [System.Serializable]
    public struct AABBox : IGeom, System.Runtime.Serialization.ISerializable
    {

        #region Fields

        private Vector3 _center;
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

        public bool Intersects(IGeom geom)
        {
            //TODO
            throw new System.NotImplementedException();
        }

        public bool Intersects(Bounds bounds)
        {
            return this.GetBounds().Intersects(bounds);
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


    }
}
