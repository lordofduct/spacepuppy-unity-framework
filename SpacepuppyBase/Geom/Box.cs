using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    public struct Box : IGeom, System.Runtime.Serialization.ISerializable, IPhysicsGeom
    {

        #region Fields

        [SerializeField()]
        private Vector3 _center;
        [SerializeField()]
        private Vector3 _size;
        [SerializeField()]
        private Quaternion _orientation;

        #endregion

        #region CONSTRUCTOR

        public Box(Vector3 cent, Vector3 sz, Quaternion orientation)
        {
            _center = cent;
            _size = sz;
            _orientation = orientation;
        }

        public Box(Bounds bounds)
        {
            _center = bounds.center;
            _size = bounds.size;
            _orientation = Quaternion.identity;
        }

        #endregion

        #region Properties

        public Vector3 Center { get { return _center; } set { _center = value; } }
        public Vector3 Size { get { return _size; } set { _size = value; } }

        public Quaternion Orientation { get { return _orientation; } set { _orientation = value; } }

        public Vector3 Extents
        {
            get { return _size / 2.0f; }
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
            _orientation = _orientation * rot;
        }

        public AxisInterval Project(Vector3 axis)
        {
            axis.Normalize();
            Vector3 extents = _size / 2.0f;
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;
            for (int i = 0; i < 8; i++)
            {
                //gets one of the 8 corners
                var v = extents;
                if ((i / 2) % 2 == 0) v.x = -v.x;
                if ((i / 4) % 2 == 0) v.y = -v.y;
                if (((i + 1) / 2) % 2 == 0) v.z = -v.z;

                v = _center + _orientation * v;
                var d = Vector3.Dot(v, axis);
                if (d < min) min = d;
                if (d > max) max = d;
            }

            return new AxisInterval(axis, min, max);
        }

        public Bounds GetBounds()
        {
            var ix = this.Project(Vector3.right);
            var iy = this.Project(Vector3.up);
            var iz = this.Project(Vector3.forward);

            return new Bounds(new Vector3(ix.Min + (ix.Max - ix.Min) * 0.5f,
                                iy.Min + (iy.Max - ix.Min) * 0.5f,
                                iz.Min + (iz.Max - iz.Min) * 0.5f),
                              new Vector3(ix.Length, iy.Length, iz.Length));
        }

        public Sphere GetBoundingSphere()
        {
            return new Sphere(Center, this.Extents.magnitude);
        }

        public bool Contains(Vector3 pos)
        {
            pos -= _center;
            pos = Quaternion.Inverse(_orientation) * pos;

            var extents = this.Extents;
            return pos.x >= -extents.x && pos.x <= extents.x &&
                   pos.y >= -extents.y && pos.y <= extents.y &&
                   pos.z >= -extents.z && pos.z <= extents.z;
        }

        public IEnumerable<Vector3> GetAxes()
        {
            yield return _orientation * Vector3.up;
            yield return _orientation * Vector3.right;
            yield return _orientation * Vector3.forward;
        }
        
        #endregion

        #region IPhysicsGeom Interface

        public bool TestOverlap(int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.CheckBox(_center, this.Extents, _orientation, layerMask, query);
        }

        public int Overlap(ICollection<Collider> results, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return PhysicsUtil.OverlapBox(_center, this.Extents, results, _orientation, layerMask, query);
        }

        public bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.BoxCast(_center, this.Extents, direction, out hitinfo, _orientation, distance, layerMask, query);
        }

        public int CastAll(Vector3 direction, ICollection<RaycastHit> results, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return PhysicsUtil.BoxCastAll(_center, this.Extents, direction, results, _orientation, distance, layerMask, query);
        }

        #endregion




        #region ISerializable Interface

        private Box(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _center = new Vector3(info.GetSingle("center.x"), info.GetSingle("center.y"), info.GetSingle("center.z"));
            _size = new Vector3(info.GetSingle("size.x"), info.GetSingle("size.y"), info.GetSingle("size.z"));
            var euler = new Vector3(info.GetSingle("rot.x"), info.GetSingle("rot.y"), info.GetSingle("rot.z"));
            _orientation = Quaternion.Euler(euler);
        }

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("center.x", this.Center.x);
            info.AddValue("center.y", this.Center.y);
            info.AddValue("center.z", this.Center.z);
            info.AddValue("size.x", this.Size.x);
            info.AddValue("size.y", this.Size.y);
            info.AddValue("size.z", this.Size.z);
            var euler = this.Orientation.eulerAngles;
            info.AddValue("rot.x", euler.x);
            info.AddValue("rot.y", euler.y);
            info.AddValue("rot.z", euler.z);
        }

        #endregion


        #region Static Interface

        public static Box FromCollider(BoxCollider c, bool local = false)
        {
            if (c == null) return new Box();

            if (local)
            {
                //var pos = c.transform.localPosition;
                //var scale = c.transform.localScale;
                //var rot = c.transform.localRotation;
                //var cent = c.center;
                //var sz = c.size;

                //cent.x *= scale.x;
                //cent.y *= scale.y;
                //cent.z *= scale.z;
                //cent = rot * cent;
                //sz.x *= scale.x;
                //sz.y *= scale.y;
                //sz.z *= scale.z;
                //return new Box(pos + cent, sz, rot);

                return new Box(c.center, c.size, Quaternion.identity);
            }
            else
            {
                var pos = c.transform.position;
                var scale = c.transform.lossyScale;
                var rot = c.transform.rotation;
                var cent = c.center;
                var sz = c.size;

                cent.x *= scale.x;
                cent.y *= scale.y;
                cent.z *= scale.z;
                cent = rot * cent;
                sz.x *= scale.x;
                sz.y *= scale.y;
                sz.z *= scale.z;
                return new Box(pos + cent, sz, rot);
            }
        }

        #endregion

    }
}
