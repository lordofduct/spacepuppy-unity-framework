using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Geom
{
    public class Frustum : IGeom
    {

        #region Fields

        private const int LEFT = 0;
        private const int RIGHT = 1;
        private const int BOTTOM = 2;
        private const int TOP = 3;
        private const int NEAR = 4;
        private const int FAR = 5;

        public const int NEAR_UL = 0;
        public const int NEAR_UR = 1;
        public const int NEAR_LR = 2;
        public const int NEAR_LL = 3;
        public const int FAR_UL = 4;
        public const int FAR_UR = 5;
        public const int FAR_LR = 6;
        public const int FAR_LL = 7;

        private Matrix4x4 _mat;
        private Plane[] _planes;

        #endregion

        #region CONSTRUCTOR

        public Frustum(Matrix4x4 mat)
        {
            this.SetFromMatrix(mat);
        }

        public Frustum(Vector3 pos, Quaternion rot, float near, float far, float fov, float aspect)
        {
            this.SetFromPerspective(pos, rot, near, far, fov, aspect);
        }

        #endregion

        #region Properties

        public Matrix4x4 Matrix
        {
            get { return _mat; }
        }

        public Plane Bottom
        {
            get { return _planes[BOTTOM]; }
        }

        public Plane Far
        {
            get { return _planes[FAR]; }
        }

        public Plane Left
        {
            get { return _planes[LEFT]; }
        }

        public Plane Near
        {
            get { return _planes[NEAR]; }
        }

        public Plane Right
        {
            get { return _planes[RIGHT]; }
        }

        public Plane Top
        {
            get { return _planes[TOP]; }
        }

        #endregion

        #region Methods

        public void SetFromMatrix(Matrix4x4 m)
        {
            _mat = m;
            _planes = GeometryUtility.CalculateFrustumPlanes(_mat);
        }

        public void SetFromPerspective(Vector3 pos, Quaternion rot, float near, float far, float fov, float aspect)
        {
            var m = Matrix4x4.Perspective(fov, aspect, near, far) * Matrix4x4.TRS(pos, rot, Vector3.one);
            this.SetFromMatrix(m);
        }

        public Vector3[] GetCorners()
        {
            Vector3[] arr = new Vector3[8];

            arr[NEAR_UL] = GeomUtil.IntersectionOfPlanes(this.Near, this.Left, this.Top);
            arr[NEAR_UR] = GeomUtil.IntersectionOfPlanes(this.Near, this.Right, this.Top);
            arr[NEAR_LR] = GeomUtil.IntersectionOfPlanes(this.Near, this.Right, this.Bottom);
            arr[NEAR_LL] = GeomUtil.IntersectionOfPlanes(this.Near, this.Left, this.Bottom);

            arr[FAR_UL] = GeomUtil.IntersectionOfPlanes(this.Far, this.Left, this.Top);
            arr[FAR_UR] = GeomUtil.IntersectionOfPlanes(this.Far, this.Right, this.Top);
            arr[FAR_LR] = GeomUtil.IntersectionOfPlanes(this.Far, this.Right, this.Bottom);
            arr[FAR_LL] = GeomUtil.IntersectionOfPlanes(this.Far, this.Left, this.Bottom);

            return arr;
        }

        #endregion

        #region IGeom Methods

        public void Move(Vector3 mv)
        {
            throw new System.NotImplementedException();
        }

        public void RotateAroundPoint(Vector3 point, Quaternion rot)
        {
            throw new System.NotImplementedException();
        }

        public AxisInterval Project(Vector3 axis)
        {
            //TODO
            return new AxisInterval();
        }

        public Bounds GetBounds()
        {
            var pnts = this.GetCorners();

            Vector3 max = new Vector3();
            max.x = Mathf.Max(pnts[0].x, pnts[1].x, pnts[2].x, pnts[3].x, pnts[4].x, pnts[5].x, pnts[6].x, pnts[7].x);
            max.y = Mathf.Max(pnts[0].y, pnts[1].y, pnts[2].y, pnts[3].y, pnts[4].y, pnts[5].y, pnts[6].y, pnts[7].y);
            max.z = Mathf.Max(pnts[0].z, pnts[1].z, pnts[2].z, pnts[3].z, pnts[4].z, pnts[5].z, pnts[6].z, pnts[7].z);

            Vector3 min = new Vector3();
            min.x = Mathf.Min(pnts[0].x, pnts[1].x, pnts[2].x, pnts[3].x, pnts[4].x, pnts[5].x, pnts[6].x, pnts[7].x);
            min.y = Mathf.Min(pnts[0].y, pnts[1].y, pnts[2].y, pnts[3].y, pnts[4].y, pnts[5].y, pnts[6].y, pnts[7].y);
            min.z = Mathf.Min(pnts[0].z, pnts[1].z, pnts[2].z, pnts[3].z, pnts[4].z, pnts[5].z, pnts[6].z, pnts[7].z);

            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        public Sphere GetBoundingSphere()
        {
            var pnts = this.GetCorners();

            Vector3 max = new Vector3();
            max.x = Mathf.Max(pnts[0].x, pnts[1].x, pnts[2].x, pnts[3].x, pnts[4].x, pnts[5].x, pnts[6].x, pnts[7].x);
            max.y = Mathf.Max(pnts[0].y, pnts[1].y, pnts[2].y, pnts[3].y, pnts[4].y, pnts[5].y, pnts[6].y, pnts[7].y);
            max.z = Mathf.Max(pnts[0].z, pnts[1].z, pnts[2].z, pnts[3].z, pnts[4].z, pnts[5].z, pnts[6].z, pnts[7].z);

            Vector3 min = new Vector3();
            min.x = Mathf.Min(pnts[0].x, pnts[1].x, pnts[2].x, pnts[3].x, pnts[4].x, pnts[5].x, pnts[6].x, pnts[7].x);
            min.y = Mathf.Min(pnts[0].y, pnts[1].y, pnts[2].y, pnts[3].y, pnts[4].y, pnts[5].y, pnts[6].y, pnts[7].y);
            min.z = Mathf.Min(pnts[0].z, pnts[1].z, pnts[2].z, pnts[3].z, pnts[4].z, pnts[5].z, pnts[6].z, pnts[7].z);

            var c = min + (max - min) / 2.0f;
            float r = (max - c).magnitude;
            return new Sphere(c, r);
        }

        public IEnumerable<Vector3> GetAxes()
        {
            //TODO
            return System.Linq.Enumerable.Empty<Vector3>();
        }

        public bool Contains(Vector3 pos)
        {
            return GeometryUtility.TestPlanesAABB(_planes, new Bounds(pos, Vector3.zero));
        }

        /*
        public IEnumerable<RaycastInfo> GetRays(float detail)
        {
            if (this.Near.distance == this.Far.distance) yield break;

            var pnts = this.GetCorners();

            float nearx = Vector3.Distance(pnts[NEAR_UL], pnts[NEAR_UR]);
            float neary = Vector3.Distance(pnts[NEAR_UL], pnts[NEAR_LL]);
            float farx = Vector3.Distance(pnts[FAR_UL], pnts[FAR_UR]);
            float fary = Vector3.Distance(pnts[FAR_UL], pnts[FAR_LL]);
            Vector3 dirx = (pnts[FAR_UR] - pnts[FAR_UL]).normalized;
            Vector3 diry = (pnts[FAR_UL] - pnts[FAR_LL]).normalized;

            int xp = Mathf.CeilToInt(nearx / detail);
            int yp = Mathf.CeilToInt(neary / detail);

            float nearxdt = nearx / (float)xp;
            float nearydt = neary / (float)yp;
            float farxdt = farx / (float)xp;
            float farydt = fary / (float)yp;

            int cnt = xp * yp;
            for (int i = 0; i < cnt; i++)
            {
                int ix = i % xp;
                int iy = i / yp;

                Vector3 s = pnts[NEAR_LL] + (dirx * ix * nearxdt) + (diry * iy * nearydt);
                Vector3 e = pnts[FAR_LL] + (dirx * ix * farxdt) + (diry * iy * farydt);
                Vector3 v = e - s;
                yield return new RaycastInfo(s, v.normalized, v.magnitude);
            }
        }

         */

        #endregion

    }
}
