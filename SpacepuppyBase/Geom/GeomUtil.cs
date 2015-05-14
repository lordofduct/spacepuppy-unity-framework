using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Geom
{
    public static class GeomUtil
    {
		
		public const float DOT_EPSILON = 0.0001f;
        public static BoundingSphereAlgorithm DefaultBoundingSphereAlgorithm = BoundingSphereAlgorithm.FromBounds;
		
		
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
            return GetGlobalBoundingSphere(rend, GeomUtil.DefaultBoundingSphereAlgorithm);
        }

        public static Sphere GetGlobalBoundingSphere(this Renderer rend, BoundingSphereAlgorithm algorithm)
        {
            if (algorithm != BoundingSphereAlgorithm.FromBounds && rend is SkinnedMeshRenderer)
            {
                return Sphere.FromMesh((rend as SkinnedMeshRenderer).sharedMesh, algorithm, Trans.GetGlobal(rend.transform));
            }
            else if (algorithm != BoundingSphereAlgorithm.FromBounds && rend is MeshRenderer && rend.HasComponent<MeshFilter>())
            {
                return Sphere.FromMesh((rend as MeshRenderer).GetComponent<MeshFilter>().sharedMesh, algorithm, Trans.GetGlobal(rend.transform));
            }
            else
            {
                var bounds = rend.bounds;
                return new Sphere(bounds.center, bounds.extents.magnitude);
            }
        }

        public static Sphere GetGlobalBoundingSphere(this Collider c)
        {
            return Sphere.FromCollider(c, GeomUtil.DefaultBoundingSphereAlgorithm, false);
        }

        public static Sphere GetGlobalBoundingSphere(this Collider c, BoundingSphereAlgorithm algorithm)
        {
            return Sphere.FromCollider(c, algorithm, false);
        }

        public static Sphere GetLocalBoundingSphere(this Renderer rend)
        {
            return GetLocalBoundingSphere(rend, GeomUtil.DefaultBoundingSphereAlgorithm);
        }

        public static Sphere GetLocalBoundingSphere(this Renderer rend, BoundingSphereAlgorithm algorithm)
        {
            if (algorithm != BoundingSphereAlgorithm.FromBounds && rend is SkinnedMeshRenderer)
            {
                return Sphere.FromMesh((rend as SkinnedMeshRenderer).sharedMesh, algorithm);
            }
            else if (algorithm != BoundingSphereAlgorithm.FromBounds && rend is MeshRenderer && rend.HasComponent<MeshFilter>())
            {
                return Sphere.FromMesh((rend as MeshRenderer).GetComponent<MeshFilter>().sharedMesh, algorithm);
            }
            else
            {
                var bounds = rend.bounds;
                var c = rend.transform.InverseTransformPoint(bounds.center);
                var v = rend.transform.InverseTransformDirection(bounds.extents);
                return new Sphere(c, v.magnitude);
            }
        }

        public static Sphere GetLocalBoundingSphere(this Collider c)
        {
            return Sphere.FromCollider(c, GeomUtil.DefaultBoundingSphereAlgorithm, true);
        }

        public static Sphere GetLocalBoundingSphere(this Collider c, BoundingSphereAlgorithm algorithm)
        {
            return Sphere.FromCollider(c, algorithm, true);
        }

        public static Sphere GetGlobalBoundingSphere(this GameObject go, bool bRecurseChildren = true)
        {
            return GetGlobalBoundingSphere(go, GeomUtil.DefaultBoundingSphereAlgorithm);
        }

        public static Sphere GetGlobalBoundingSphere(this GameObject go, BoundingSphereAlgorithm algorithm, bool bRecurseChildren = true)
        {
            var s = new Sphere(go.transform.position, 0.0f);
            if (go.renderer != null) s.Encapsulate(go.renderer.GetGlobalBoundingSphere(algorithm));
            if (go.collider != null) s.Encapsulate(go.collider.GetGlobalBoundingSphere(algorithm));

            if (bRecurseChildren)
            {
                foreach (Transform child in go.transform)
                {
                    s.Encapsulate(GetGlobalBoundingSphere(child.gameObject, algorithm, bRecurseChildren));
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
        public static IGeom GetGeom(this Collider c, bool local = false)
        {
            return GetGeom(c, GeomUtil.DefaultBoundingSphereAlgorithm, local);
        }

        /// <summary>
        /// Attempts to calculate geometry for a collider. Not tested yet.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IGeom GetGeom(this Collider c, BoundingSphereAlgorithm algorithm, bool local = false)
        {
            if(c == null) return null;

            if (c is CharacterController)
            {
                return Capsule.FromCollider(c as CharacterController, local);
            }
            if (c is CapsuleCollider)
            {
                return Capsule.FromCollider(c as CapsuleCollider, local);
            }
            else if (c is BoxCollider)
            {
                return AABBox.FromCollider(c as BoxCollider, local);
            }
            else if (c is SphereCollider)
            {
                return Sphere.FromCollider(c as SphereCollider, local);
            }
            else if (algorithm != BoundingSphereAlgorithm.FromBounds && c is MeshCollider)
            {
                if(local)
                {
                    return Sphere.FromMesh((c as MeshCollider).sharedMesh, algorithm);
                }
                else
                {
                    return Sphere.FromMesh((c as MeshCollider).sharedMesh, algorithm, Trans.GetGlobal(c.transform));
                }
            }
            else
            {
                //otherwise just return bounds as AABBox
                return AABBox.FromCollider(c, local);
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
