using UnityEngine;
using System.Collections.Generic;

using MathUtil = com.spacepuppy.Utils.MathUtil;

namespace com.spacepuppy.Geom
{
    public class ViewCone : IGeom, IPhysicsGeom
    {

        #region Fields

        private Vector3 _loc;
        private Vector3 _dir;
        private float _dist;
        private float _fov;

        #endregion

        #region CONSTRUCTOR

        public ViewCone(Vector3 loc, Vector3 dir, float dist, float fov)
        {
            _loc = loc;
            _dir = dir.normalized;
            _dist = dist;
            _fov = fov;
        }

        #endregion

        #region Properties

        public Vector3 Location
        {
            get { return _loc; }
            set { _loc = value; }
        }

        public Vector3 Direction
        {
            get { return _dir; }
            set { _dir = value.normalized; }
        }

        public float Distance
        {
            get { return _dist; }
            set { _dist = value; }
        }

        public float FOV
        {
            get { return _fov; }
            set { _fov = value; }
        }

        public float FarRadius
        {
            get { return Mathf.Tan(MathUtil.RAD_TO_DEG * _fov / 2.0f) * _dist; }
        }

        #endregion

        #region IGeom Interface

        public void Move(Vector3 mv)
        {
            _loc += mv;
        }

        public void RotateAroundPoint(Vector3 point, Quaternion rot)
        {
            _loc = point + rot * (_loc - point);
            _dir = rot * _dir;
        }

        public AxisInterval Project(Vector3 axis)
        {
            //TODO
            return new AxisInterval();
        }

        public Bounds GetBounds()
        {
            var x = Project(Vector3.right);
            var y = Project(Vector3.up);
            var z = Project(Vector3.forward);

            var c = ((x.Axis * (x.Max + x.Min) / 2.0f) + (y.Axis * (y.Max + y.Min) / 2.0f) + (z.Axis * (z.Max + z.Min) / 2.0f)) / 3.0f;
            var s = new Vector3(x.Length / 2.0f, y.Length / 2.0f, z.Length / 2.0f);

            return new Bounds(c, s);
        }

        public Sphere GetBoundingSphere()
        {
			Vector3 up = (Mathf.Abs(Vector3.Dot(_dir, Vector3.right)) - 1.0f < GeomUtil.DOT_EPSILON) ? Vector3.up : Vector3.Cross(_dir, Vector3.right);
			Vector3 floc = _loc + _dir * _dist;
			
			float theta = (_fov * Mathf.PI / 180.0f) / 2.0f;
			float h = _dist / Mathf.Cos(theta);
			float l = h * Mathf.Sin(theta);
			
            Vector3 a = _loc;
			Vector3 b = floc + up * l;
			Vector3 c = floc - up * l;
			
			Vector3 cent = GeomUtil.CircumCenter(a,b,c);
			float r = (a - cent).magnitude;
			return new Sphere(cent, r);
        }

        public IEnumerable<Vector3> GetAxes()
        {
            //TODO
            return System.Linq.Enumerable.Empty<Vector3>();
        }

        public bool Contains(Vector3 pos)
        {
            var dir = pos - _loc;
            float dp = Vector3.Dot(Direction, dir);
            return (dp <= _dist && Mathf.Acos(dp / dir.magnitude) * MathUtil.RAD_TO_DEG < FOV / 2.0f);
        }

        #endregion

        #region IPhysicsGeom Interface

        public bool TestOverlap(int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            throw new System.NotImplementedException();
        }

        public int Overlap(ICollection<Collider> results, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            throw new System.NotImplementedException();
        }

        public bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            throw new System.NotImplementedException();
        }

        public int CastAll(Vector3 direction, ICollection<RaycastHit> results, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            throw new System.NotImplementedException();
        }

        private IEnumerable<RaycastInfo> GetRays(float detail)
        {
            if (_dist == 0) yield break;

            yield return new RaycastInfo(_loc, _dir, _dist);

            float farRad = this.FarRadius;
            Vector3 right = (_dir == Vector3.forward) ? Vector3.right : Vector3.Cross(_dir, Vector3.forward);

            int rp = Mathf.CeilToInt(farRad / detail); //amount of radial moves
            float rd = farRad / (float)rp; //adjusted detail for the radial move

            for (int i = 1; i <= rp; i++)
            {
                float r = i * rd;//current shell radius
                float c = Mathf.PI * 2.0f * r; //current shell circumference

                int cp = Mathf.CeilToInt(c / detail); //amount of circular moves
                float cd = c / (float)cp; //adjusted detail for the circular moves
                float ad = 360.0f / (float)cd;

                Vector3 v = (_dir * _dist) + (right * rd * i);
                for (int j = 0; j < cp; j++)
                {
                    Vector3 ray = Quaternion.AngleAxis(j * ad, _dir) * v;
                    yield return new RaycastInfo(_loc, ray.normalized, ray.magnitude);
                }
            }
        }

        #endregion

    }

}