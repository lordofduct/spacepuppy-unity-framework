using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Geom
{
    public struct LineSegment : IGeom, IPhysicsGeom
    {

        private Vector3 _start;
        private Vector3 _end;

        public LineSegment(Vector3 s, Vector3 e)
        {
            _start = s;
            _end = e;
        }

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

        public float Length
        {
            get { return Vector3.Distance(_start, _end); }
        }

        public Vector3 Direction
        {
            get { return (_end - _start).normalized; }
        }

        public Line Line
        {
            get { return new Line(_start, this.Direction); }
        }

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
            var a = Vector3.Dot(_start, axis);
            var b = Vector3.Dot(_end, axis);
            return new AxisInterval(axis, a, b);
        }

        public Bounds GetBounds()
        {
            var bounds = new Bounds();
            bounds.SetMinMax(_start, _end);
            return bounds;
        }

        public Sphere GetBoundingSphere()
        {
            var c = _start + (_end - _start) / 2.0f;
            var r = (_end - c).magnitude;
            return new Sphere(c, r);
        }

        public IEnumerable<Vector3> GetAxes()
        {
            //TODO
            return System.Linq.Enumerable.Empty<Vector3>();
        }

        public bool Contains(Vector3 pos)
        {
            const float EPSILON = 0.0001f;

            var to = pos - _start;
            var tlen = to.magnitude;
            if (tlen < EPSILON) return true; //start and pos are so close they're nearly the same point

            var dir = _end - _start;
            var dlen = dir.magnitude;
            var d = Vector3.Dot(to, dir / dlen);
            if (d < 0) return false;

            //if d < dlen it's shorter then dir
            //if the (d/tlen) almost equals 1.0f, then dir and to are nearly equal
            return (d < dlen) && (Mathf.Abs((d / tlen) - 1.0f) < EPSILON);
        }

        #endregion

        #region IPhysicsGeom Interface

        public bool TestOverlap(int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.Linecast(_start, _end, layerMask, query);
        }

        public int Overlap(ICollection<Collider> results, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return PhysicsUtil.OverlapLine(_start, _end, results, layerMask, query);
        }

        public bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            throw new System.NotImplementedException();
        }

        public int CastAll(Vector3 direction, ICollection<RaycastHit> results, float distance, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            throw new System.NotImplementedException();
        }


        /*
        public System.Collections.Generic.IEnumerable<RaycastInfo> GetRays(float detail)
        {
            var dir = _end - _start;
            yield return new RaycastInfo(_start, dir.normalized, dir.magnitude);
        }

        public System.Collections.Generic.IEnumerable<RaycastInfo> GetRays(Vector3 dir, float detail)
        {
            yield return new RaycastInfo(_start, dir, 0);

            var v = _end - _start;
            var len = v.magnitude;
            v.Normalize();

            int vp = Mathf.CeilToInt(len / detail);
            float vd = len / (float)vp;

            for (int i = 1; i <= vp; i++)
            {
                float s = vd * i;
                yield return new RaycastInfo(_start + v * s, dir, 0);
            }
        }

        public System.Collections.Generic.IEnumerable<RaycastInfo> GetRays(Vector3 dir, float dist, float detail)
        {
            yield return new RaycastInfo(_start, dir, dist);

            var v = _end - _start;
            var len = v.magnitude;
            v.Normalize();

            int vp = Mathf.CeilToInt(len / detail);
            float vd = len / (float)vp;

            for (int i = 1; i <= vp; i++)
            {
                float s = vd * i;
                yield return new RaycastInfo(_start + v * s, dir, dist);
            }
        }
         */

        #endregion

    }
}
