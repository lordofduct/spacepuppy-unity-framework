using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    [System.Serializable]
    public class SightGeom : IGeom, IPhysicsGeom
    {

        public enum SightType
        {
            Cone = 0,
            Frustum = 1
        }

        #region Fields

        private Transform _targ;
        private IGeom _geom;

        public SightType Type;
        public Vector3 EyeOffset = Vector3.zero;
        public float SeeDistance = 1.0f;
        public float SeeFOV = 60.0f;

        #endregion

        #region CONSTRUCTOR

        public SightGeom()
        {
        }

        public SightGeom(Transform targ)
        {
            this.Init(targ);
        }

        public void Init(Transform targ)
        {
            _targ = targ;
        }

        #endregion

        #region Properties

        public Transform Target { get { return _targ; } }

        public Vector3 Origin
        {
            get
            {
                return _targ.position + (_targ.rotation * EyeOffset);
            }
        }

        #endregion

        #region Methods

        public void Invalidate()
        {
            _geom = null;
        }

        public void UpdateGeom()
        {
            switch (Type)
            {
                case SightType.Cone:
                    _geom = new ViewCone(this.Origin, _targ.forward, SeeDistance, SeeFOV);
                    break;
                case SightType.Frustum:
                    _geom = new Frustum(this.Origin, _targ.rotation, 0.0f, SeeDistance, SeeFOV, 1.0f);
                    break;

            }
        }

        #endregion

        #region IGeom Interface

        public void Move(Vector3 mv)
        {
            _geom.Move(mv);
        }

        public void RotateAroundPoint(Vector3 point, Quaternion rot)
        {
            _geom.RotateAroundPoint(point, rot);
        }

        public AxisInterval Project(Vector3 axis)
        {
            if (_geom == null)
            {
                this.UpdateGeom();
                if (_geom == null) return AxisInterval.NoHit(axis);
            }

            return _geom.Project(axis);
        }

        public Bounds GetBounds()
        {
            if (_geom == null)
            {
                this.UpdateGeom();
                if (_geom == null) return new Bounds();
            }

            return _geom.GetBounds();
        }

        public Sphere GetBoundingSphere()
        {
            if (_geom == null)
            {
                this.UpdateGeom();
                if (_geom == null) return new Sphere();
            }

            return _geom.GetBoundingSphere();
        }

        public IEnumerable<Vector3> GetAxes()
        {
            if(_geom == null)
            {
                this.UpdateGeom();
                if (_geom == null) return System.Linq.Enumerable.Empty<Vector3>();
            }
            return _geom.GetAxes();
        }

        public bool Contains(Vector3 pos)
        {
            if (_geom == null)
            {
                this.UpdateGeom();
                if (_geom == null) return false;
            }

            return _geom.Contains(pos);
        }

        #endregion

        #region IPhysicsGeom Interface

        public bool TestOverlap(int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (_geom == null)
            {
                this.UpdateGeom();
                if (_geom == null) return false;
            }
            if (!(_geom is IPhysicsGeom)) return false;

            return (_geom as IPhysicsGeom).TestOverlap(layerMask, query);
        }

        public int Overlap(ICollection<Collider> results, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (_geom == null)
            {
                this.UpdateGeom();
                if (_geom == null) return 0;
            }
            if (!(_geom is IPhysicsGeom)) return 0;
            return (_geom as IPhysicsGeom).Overlap(results, layerMask, query);
        }

        public bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (_geom == null)
            {
                this.UpdateGeom();
                if (_geom == null)
                {
                    hitinfo = new RaycastHit();
                    return false;
                }
            }
            if (!(_geom is IPhysicsGeom))
            {
                hitinfo = new RaycastHit();
                return false;
            }

            return (_geom as IPhysicsGeom).Cast(direction, out hitinfo, distance, layerMask, query);
        }

        public int CastAll(Vector3 direction, ICollection<RaycastHit> results, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (_geom == null)
            {
                this.UpdateGeom();
                if (_geom == null) return 0;
            }
            if (!(_geom is IPhysicsGeom)) return 0;

            return (_geom as IPhysicsGeom).CastAll(direction, results, distance, layerMask, query);
        }

        #endregion

    }
}
