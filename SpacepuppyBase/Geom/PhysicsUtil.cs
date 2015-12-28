using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Geom
{
    public class PhysicsUtil
    {

        #region Properties

        public static float Detail = 0.1f;

        #endregion

        public static int CalculateLayerMaskAgainst(int layer)
        {
            int mask = 0;

            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i)) mask = mask | (1 << i);
            }

            return mask;
        }

        #region CheckGeom

        public static bool CheckGeom(IPhysicsGeom geom)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            if (geom is Sphere)
            {
                var s = (Sphere)geom;
                return Physics.CheckSphere(s.Center, s.Radius);
            }
            else if (geom is Capsule)
            {
                var c = (Capsule)geom;
                return Physics.CheckCapsule(c.Start, c.End, c.Radius);
            }
            else
            {
                return geom.TestOverlap(Physics.AllLayers);
            }
        }

        public static bool CheckGeom(IPhysicsGeom geom, int layerMask)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            if (geom is Sphere)
            {
                var s = (Sphere)geom;
                return Physics.CheckSphere(s.Center, s.Radius, layerMask);
            }
            else if (geom is Capsule)
            {
                var c = (Capsule)geom;
                return Physics.CheckCapsule(c.Start, c.End, c.Radius, layerMask);
            }
            else
            {
                return geom.TestOverlap(layerMask);
            }
        }

        #endregion

        #region OverlapGeom

        public static Collider[] OverlapGeom(IPhysicsGeom geom)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.Overlap(Physics.AllLayers).ToArray();
        }

        public static Collider[] OverlapGeom(IPhysicsGeom geom, int layerMask)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.Overlap(layerMask).ToArray();
        }

        public static bool OverlapGeom(IPhysicsGeom geom, IList<Collider> lst)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            var e = LightEnumerator.Create(geom.Overlap(Physics.AllLayers));
            bool added = false;
            while (e.MoveNext())
            {
                added = true;
                lst.Add(e.Current);
            }
            return added;
        }

        public static bool OverlapGeom(IPhysicsGeom geom, int layerMask, IList<Collider> lst)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            var e = LightEnumerator.Create(geom.Overlap(layerMask));
            bool added = false;
            while (e.MoveNext())
            {
                added = true;
                lst.Add(e.Current);
            }
            return added;
        }

        #endregion

        #region Cast

        public static bool Cast(IPhysicsGeom geom, Vector3 dir)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            RaycastHit hit;
            return geom.Cast(dir, out hit, float.PositiveInfinity, Physics.AllLayers);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, float dist)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            RaycastHit hit;
            return geom.Cast(dir, out hit, dist, Physics.AllLayers);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, out RaycastHit hitInfo)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.Cast(dir, out hitInfo, float.PositiveInfinity, Physics.AllLayers);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, float dist, int layerMask)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            RaycastHit hit;
            return geom.Cast(dir, out hit, dist, layerMask);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, out RaycastHit hitInfo, float dist)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.Cast(dir, out hitInfo, dist, Physics.AllLayers);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, out RaycastHit hitInfo, float dist, int layerMask)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.Cast(dir, out hitInfo, dist, layerMask);
        }

        #endregion

        #region CastAll

        public static RaycastHit[] CastAll(IPhysicsGeom geom, Vector3 dir)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.CastAll(dir, float.PositiveInfinity, Physics.AllLayers).ToArray();
        }

        public static RaycastHit[] CastAll(IPhysicsGeom geom, Vector3 dir, float dist)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.CastAll(dir, dist, Physics.AllLayers).ToArray();
        }

        public static RaycastHit[] CastAll(IPhysicsGeom geom, Vector3 dir, float dist, int layerMask)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.CastAll(dir, dist, layerMask).ToArray();
        }

        #endregion

        #region RadialCast

        public static IEnumerable<RaycastHit> RadialCast(IPhysicsGeom geom, float dist, int detail, int layerMask)
        {
            return RadialCast(geom, dist, detail, layerMask, Vector2.right);
        }

        public static IEnumerable<RaycastHit> RadialCast(IPhysicsGeom geom, float dist, int detail, int layerMask, Vector2 initialAxis)
        {
            if (VectorUtil.NearZeroVector(initialAxis))
                initialAxis = Vector2.right;
            else
                initialAxis.Normalize();
            var a = 360f / (float)detail;

            for (int i = 0; i < detail; i++)
            {
                var v = VectorUtil.RotateBy(initialAxis, a * i);
                foreach (var h in geom.CastAll(v, dist, layerMask))
                {
                    yield return h;
                }
            }
        }

        public static IEnumerable<RaycastHit> RadialCast(IPhysicsGeom geom, float dist, int detail, int layerMask, Vector3 initialAxis, Vector3 rotationalAxis)
        {
            if (VectorUtil.NearZeroVector(initialAxis))
                initialAxis = Vector2.right;
            else
                initialAxis.Normalize();
            var a = 360f / (float)detail;

            for (int i = 0; i < detail; i++)
            {
                var v = VectorUtil.RotateAroundAxis(initialAxis, a * i, rotationalAxis);
                foreach (var h in geom.CastAll(v, dist, layerMask))
                {
                    yield return h;
                }
            }
        }

        #endregion


        #region RepairHitSurfaceNormal

        public static Vector3 RepairHitSurfaceNormal(RaycastHit hit, int layerMask)
        {
            if(hit.collider is MeshCollider && !(hit.collider as MeshCollider).convex)
            {
                var collider = hit.collider as MeshCollider;
                var mesh = collider.sharedMesh;
                var tris = mesh.triangles;
                var verts = mesh.vertices;

                var v0 = verts[tris[hit.triangleIndex * 3]];
                var v1 = verts[tris[hit.triangleIndex * 3 + 1]];
                var v2 = verts[tris[hit.triangleIndex * 3 + 2]];

                var n = Vector3.Cross(v1 - v0, v2 - v1).normalized;
                //var n =  Vector3.Cross(v2 - v1, v1 - v0).normalized;

                return hit.transform.TransformDirection(n);
            }
            else
            {
                var p = hit.point + hit.normal * 0.01f;
                Physics.Raycast(p, -hit.normal, out hit, 0.011f, layerMask);
                return hit.normal;
            }
        }

        #endregion

    }
}
