using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Geom
{
    public static class GeomUtil
    {
		
		public const float DOT_EPSILON = 0.0001f;
		
		
        #region Matrix Methods

        public static Vector3 MatrixToTranslation(Matrix4x4 m)
        {
            var col = m.GetColumn(3);
            return new Vector3(col.x, col.y, col.z);
        }

        public static Quaternion MatrixToRotation(Matrix4x4 m)
        {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
        }

        public static Vector3 MatrixToScale(Matrix4x4 m)
        {
            //var xs = m.GetColumn(0);
            //var ys = m.GetColumn(1);
            //var zs = m.GetColumn(2);

            //var sc = new Vector3();
            //sc.x = Vector3.Magnitude(new Vector3(xs.x, xs.y, xs.z));
            //sc.y = Vector3.Magnitude(new Vector3(ys.x, ys.y, ys.z));
            //sc.z = Vector3.Magnitude(new Vector3(zs.x, zs.y, zs.z));

            //return sc;

            return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
        }

        #endregion

        #region Transform Methods

        /// <summary>
        /// Multiply a vector by only the scale part of a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 ScaleVector(this Matrix4x4 m, Vector3 v)
        {
            var sc = MatrixToScale(m);
            return Matrix4x4.Scale(sc).MultiplyPoint(v);
        }

        public static Vector3 ScaleVector(this Trans t, Vector3 v)
        {
            return Matrix4x4.Scale(t.Scale).MultiplyPoint(v);
        }

        public static Vector3 ScaleVector(this Transform t, Vector3 v)
        {
            var sc = MatrixToScale(t.localToWorldMatrix);
            return Matrix4x4.Scale(sc).MultiplyPoint(v);
        }

        /// <summary>
        /// Inverse multiply a vector by on the scale part of a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 InverseScaleVector(this Matrix4x4 m, Vector3 v)
        {
            var sc = MatrixToScale(m.inverse);
            return Matrix4x4.Scale(sc).MultiplyPoint(v);
        }

        public static Vector3 InvserScaleVector(this Trans t, Vector3 v)
        {
            var sc = MatrixToScale(t.Matrix.inverse);
            return Matrix4x4.Scale(sc).MultiplyPoint(v);
        }

        public static Vector3 InvserScaleVector(this Transform t, Vector3 v)
        {
            var sc = MatrixToScale(t.worldToLocalMatrix);
            return Matrix4x4.Scale(sc).MultiplyPoint(v);
        }

        /// <summary>
        /// Transform a ray by a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Ray TransformRay(this Matrix4x4 m, Ray r)
        {
            return new Ray(m.MultiplyPoint(r.origin), m.MultiplyVector(r.direction));
        }

        public static Ray TransformRay(this Trans t, Ray r)
        {
            return new Ray(t.TransformPoint(r.origin), t.TransformDirection(r.direction));
        }

        public static Ray TransformRay(this Transform t, Ray r)
        {
            return new Ray(t.TransformPoint(r.origin), t.TransformDirection(r.direction));
        }

        /// <summary>
        /// Inverse transform a ray by a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Ray InverseTransformRay(this Matrix4x4 m, Ray r)
        {
            m = m.inverse;
            return new Ray(m.MultiplyPoint(r.origin), m.MultiplyVector(r.direction));
        }

        public static Ray InverseTransformRay(this Trans t, Ray r)
        {
            return new Ray(t.InverseTransformPoint(r.origin), t.InverseTransformDirection(r.direction));
        }

        public static Ray InverseTransformRay(this Transform t, Ray r)
        {
            return new Ray(t.InverseTransformPoint(r.origin), t.InverseTransformDirection(r.direction));
        }

        /// <summary>
        /// Transform ray cast info by a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static RaycastInfo TransformRayCastInfo(this Matrix4x4 m, RaycastInfo r)
        {
            float dist = r.Distance;
            var sc = MatrixToScale(m);
            if (sc.sqrMagnitude != 1.0f)
            {
                var v = r.Direction * r.Distance;
                v = Matrix4x4.Scale(sc).MultiplyPoint(v);
                dist = v.magnitude;
            }

            return new RaycastInfo(m.MultiplyPoint(r.Origin), m.MultiplyVector(r.Direction), dist);
        }

        public static RaycastInfo TransformRayCastInfo(this Trans t, RaycastInfo r)
        {
            float dist = r.Distance;
            var sc = MatrixToScale(t.Matrix);
            if (sc.sqrMagnitude != 1.0f)
            {
                var v = r.Direction * r.Distance;
                v = Matrix4x4.Scale(sc).MultiplyPoint(v);
                dist = v.magnitude;
            }

            return new RaycastInfo(t.TransformPoint(r.Origin), t.TransformDirection(r.Direction), dist);
        }

        public static RaycastInfo TransformRayCastInfo(this Transform t, RaycastInfo r)
        {
            float dist = r.Distance;
            var sc = MatrixToScale(t.localToWorldMatrix);
            if (sc.sqrMagnitude != 1.0f)
            {
                var v = r.Direction * r.Distance;
                v = Matrix4x4.Scale(sc).MultiplyPoint(v);
                dist = v.magnitude;
            }

            return new RaycastInfo(t.TransformPoint(r.Origin), t.TransformDirection(r.Direction), dist);
        }

        #endregion

        #region Intersections

        public static bool Intersects(this IGeom geom1, IGeom geom2)
        {
            if (geom1 == null || geom2 == null) return false;

            var s1 = geom1.GetBoundingSphere();
            var s2 = geom2.GetBoundingSphere();
            if ((s1.Center - s2.Center).magnitude > (s1.Radius + s2.Radius)) return false;

            foreach(var a in geom1.GetAxes().Union(geom2.GetAxes()))
            {
                if (geom1.Project(a).Intersects(geom2.Project(a))) return true;
            }

            return false;
        }

        public static bool Intersects(this IGeom geom, Bounds bounds)
        {
            //TODO - re-implement independent of geom, may speed this up
            var geom2 = new AABBox(bounds);
            return Intersects(geom, geom2);
        }

        /// <summary>
        /// Find intersecting line of two planes
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Line IntersectionOfPlanes(Plane p1, Plane p2)
        {
            var line = new Line();
            line.Direction = Vector3.Cross(p1.normal, p2.normal);
            Vector3 ldir = Vector3.Cross(p2.normal, line.Direction);

            float d = Vector3.Dot(p1.normal, ldir);

            //if d is to close to 0, planes are parallel
            if (Mathf.Abs(d) > 0.005f)
            {
                Vector3 p1Top2 = (p1.normal * p1.distance) - (p2.normal * p2.distance);
                float t = Vector3.Dot(p1.normal, p1Top2) / d;
                line.Point = (p2.normal * p2.distance) + t * ldir;
                return line;
            }
            else
            {
                throw new System.Exception("both planes are parallel");
            }
        }
        public static bool IntersectionOfPlanes(Plane p1, Plane p2, out Line line)
        {
            line = new Line();
            line.Direction = Vector3.Cross(p1.normal, p2.normal);
            Vector3 ldir = Vector3.Cross(p2.normal, line.Direction);

            float d = Vector3.Dot(p1.normal, ldir);

            //if d is to close to 0, planes are parallel
            if (Mathf.Abs(d) > 0.005f)
            {
                Vector3 p1Top2 = (p1.normal * p1.distance) - (p2.normal * p2.distance);
                float t = Vector3.Dot(p1.normal, p1Top2) / d;
                line.Point = (p2.normal * p2.distance) + t * ldir;
                return true;
            }
            else
            {
                line = new Line();
                return false;
            }
        }

        /// <summary>
        /// Find point at which 3 planes intersect
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static Vector3 IntersectionOfPlanes(Plane p1, Plane p2, Plane p3)
        {
            try
            {
                var line = IntersectionOfPlanes(p1, p2);
                var intersection = IntersectionOfLineAndPlane(line, p3);
                return intersection;
            }
            catch (System.Exception)
            {
                throw new System.Exception("two or more planes are parallel, no intersection found");
            }
        }
        public static bool IntersectionOfPlanes(Plane p1, Plane p2, Plane p3, out Vector3 point)
        {
            try
            {
                var line = IntersectionOfPlanes(p1, p2);
                point = IntersectionOfLineAndPlane(line, p3);
                return true;
            }
            catch (System.Exception)
            {
                point = Vector3.zero;
                return false;
            }
        }

        /// <summary>
        /// Find interesection location of a line and a plane
        /// </summary>
        /// <param name="line"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Vector3 IntersectionOfLineAndPlane(Line line, Plane plane)
        {
            //calc dist...
            float dotnum = Vector3.Dot((plane.normal * plane.distance) - line.Point, plane.normal);
            float dotdenom = Vector3.Dot(line.Direction, plane.normal);

            if (dotdenom != 0.0f)
            {
                float len = dotnum / dotdenom;
                var v = line.Direction * len;
                return line.Point + v;
            }
            else
            {
                throw new System.Exception("line and plane are parallel");
            }
        }
        public static bool IntersectionOfLineAndPlane(Line line, Plane plane, out Vector3 point)
        {
            //calc dist...
            float dotnum = Vector3.Dot((plane.normal * plane.distance) - line.Point, plane.normal);
            float dotdenom = Vector3.Dot(line.Direction, plane.normal);

            if (dotdenom != 0.0f)
            {
                float len = dotnum / dotdenom;
                var v = line.Direction * len;
                point = line.Point + v;
                return true;
            }
            else
            {
                point = Vector3.zero;
                return false;
            }
        }

        #endregion

        #region Plane Extension Methods

        /// <summary>
        /// returns distance point is from a plane, sign reflects if it is above or below the facing side (direction of normal)
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static float DistanceOfPointFromPlane(this Plane pl, Vector3 p)
        {
            return Vector3.Dot(pl.normal, (p - pl.normal * pl.distance));
        }

        public static Vector3 ProjectVectorOnPlane(this Plane pl, Vector3 v)
        {
            return v - (Vector3.Dot(v, pl.normal) * pl.normal);
        }

        public static Vector3 ProjectPointOnPlane(this Plane pl, Vector3 p)
        {
            float dist = DistanceOfPointFromPlane(pl, p) *-1;
            var v = pl.normal * dist;
            return p + v;
        }

        public static float AngleBetweenVectorAndPlane(this Plane pl, Vector3 v)
        {
            float dot = Vector3.Dot(v.normalized, pl.normal);
            float a = Mathf.Acos(dot);
            return 1.5707963f - a;
        }

        #endregion

        #region GetBounds

        public static Sphere GetGlobalBoundingSphere(this Renderer rend)
        {
            var bounds = rend.bounds;
            return new Sphere(bounds.center, bounds.extents.magnitude);
        }

        public static Sphere GetGlobalBoundingSphere(this Collider coll)
        {
            var bounds = coll.bounds;
            return new Sphere(bounds.center, bounds.extents.magnitude);
        }

        public static Sphere GetLocalBoundingSphere(this Renderer rend)
        {
            var bounds = rend.bounds;
            var c = rend.transform.InverseTransformPoint(bounds.center);
            var v = rend.transform.InverseTransformDirection(bounds.extents);
            return new Sphere(c, v.magnitude);
        }

        public static Sphere GetLocalBoundingSphere(this Collider coll)
        {
            var bounds = coll.bounds;
            var c = coll.transform.InverseTransformPoint(bounds.center);
            var v = coll.transform.InverseTransformDirection(bounds.extents);
            return new Sphere(c, v.magnitude);
        }

        public static Sphere GetGlobalBoundingSphere(this GameObject go, bool bRecurseChildren = true)
        {
            var s = new Sphere(go.transform.position, 0.0f);
            if (go.renderer != null) s.Encapsulate(go.renderer.GetGlobalBoundingSphere());
            if (go.collider != null) s.Encapsulate(go.collider.GetGlobalBoundingSphere());

            if (bRecurseChildren)
            {
                foreach (Transform child in go.transform)
                {
                    s.Encapsulate(GetGlobalBoundingSphere(child.gameObject, bRecurseChildren));
                }
            }

            return s;
        }


        public static Bounds GetGlobalBounds(this GameObject go, bool bRecurseChildren = true)
        {
            var b = new Bounds(go.transform.position, Vector3.zero);

            if (go.renderer != null) b.Encapsulate(go.renderer.bounds);
            if (go.collider != null) b.Encapsulate(go.collider.bounds);

            if (bRecurseChildren)
            {
                foreach (Transform child in go.transform)
                {
                    b.Encapsulate(GetGlobalBounds(child.gameObject, bRecurseChildren));
                }
            }

            return b;
        }

        #endregion

        #region GetColliderGeom

        /// <summary>
        /// Attempts to calculate geometry for a collider. Not tested yet.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IGeom GetColliderGeom(Collider c)
        {
            if(c == null) return null;

            if (c is CharacterController)
            {
                return Capsule.FromCollider(c as CharacterController);
            }
            if (c is CapsuleCollider)
            {
                return Capsule.FromCollider(c as CapsuleCollider);
            }
            else if (c is BoxCollider)
            {
                //TODO - implement Box Geom
                return new AABBox(c.bounds);
            }
            else if (c is SphereCollider)
            {
                return Sphere.FromCollider(c as SphereCollider);
            }
            else
            {
                //otherwise just return bounds as AABBox
                return new AABBox(c.bounds);
            }
        }

        #endregion

        public static Vector3 CircumCenter(Vector3 a, Vector3 b, Vector3 c)
		{
			var a2b = Mathf.Pow(a.magnitude, 2.0f) * b;
			var b2a = Mathf.Pow(b.magnitude, 2.0f) * a;
			var aCrossb = Vector3.Cross(a, b);
			var numer = Vector3.Cross(a2b - b2a, aCrossb);
			var denom = 2.0f * Mathf.Pow(aCrossb.magnitude, 2.0f);
			return numer / denom + c;
		}
		
    }
}
