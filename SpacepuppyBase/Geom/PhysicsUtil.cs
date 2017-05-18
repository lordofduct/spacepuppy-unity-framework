using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Geom
{
    public static class PhysicsUtil
    {

        #region Fields/Properties

        public static float Detail = 0.1f;

        private const int MAX_BUFFER = 1024;
        private static Collider[] _nonallocColliderBuffer;
        private static Collider[] NonAllocColliderBuffer
        {
            get
            {
                if(_nonallocColliderBuffer == null)
                    _nonallocColliderBuffer = new Collider[MAX_BUFFER];
                return _nonallocColliderBuffer;
            }
        }
        private static RaycastHit[] _nonallocRaycastBuffer;
        private static RaycastHit[] NonAllocRaycastBuffer
        {
            get
            {
                if (_nonallocRaycastBuffer == null)
                    _nonallocRaycastBuffer = new RaycastHit[MAX_BUFFER];
                return _nonallocRaycastBuffer;
            }
        }

        public class RaycastHitDistanceComparer : IComparer<RaycastHit>
        {
            private static RaycastHitDistanceComparer _default;
            public static RaycastHitDistanceComparer Default
            {
                get
                {
                    if (_default == null)
                        _default = new RaycastHitDistanceComparer();
                    return _default;
                }
            }

            public int Compare(RaycastHit x, RaycastHit y)
            {
                return x.distance.CompareTo(y.distance);
            }
        }
        
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


        #region Physics Forwards

        public static int RaycastAll(Vector3 origin, Vector3 dir, ICollection<RaycastHit> results, float maxDistance = float.PositiveInfinity, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return RaycastAll(new Ray(origin, dir), results, maxDistance, layerMask, query);
        }

        public static int RaycastAll(Ray ray, ICollection<RaycastHit> results, float maxDistance = float.PositiveInfinity, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            if (results is RaycastHit[])
            {
                return Physics.RaycastNonAlloc(ray, results as RaycastHit[], maxDistance, layerMask, query);
            }
            else
            {
                var nonAllocArr = NonAllocRaycastBuffer;
                int cnt = Physics.RaycastNonAlloc(ray, nonAllocArr, maxDistance, layerMask, query);
                if (results is List<RaycastHit>)
                {
                    var lst = results as List<RaycastHit>;
                    var num = cnt + lst.Count;
                    if (lst.Capacity < num) lst.Capacity = num;
                }

                for (int i = 0; i < cnt; i++)
                {
                    results.Add(nonAllocArr[i]);
                    nonAllocArr[i] = default(RaycastHit);
                }
                return cnt;
            }
        }

        public static int OverlapLine(Vector3 start, Vector3 end, ICollection<Collider> results, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            var nonAllocArr = NonAllocRaycastBuffer;

            var dir = end - start;
            var dist = dir.magnitude;
            int cnt = Physics.RaycastNonAlloc(new Ray(start, dir), nonAllocArr, dist, layerMask, query);

            if (results is Collider[])
            {
                var arr = results as Collider[];
                if (arr.Length < cnt) cnt = arr.Length;

                for (int i = 0; i < cnt; i++)
                {
                    arr[i] = nonAllocArr[i].collider;
                    nonAllocArr[i] = default(RaycastHit);
                }
                return cnt;
            }
            else
            {
                if (results is List<Collider>)
                {
                    var lst = results as List<Collider>;
                    var num = cnt + lst.Count;
                    if (lst.Capacity < num) lst.Capacity = num;
                }

                for (int i = 0; i < cnt; i++)
                {
                    results.Add(nonAllocArr[i].collider);
                    nonAllocArr[i] = default(RaycastHit);
                }
                return cnt;
            }
        }


        /// <summary>
        /// Find all colliders touching or inside the given box, and store them into the buffer.
        /// 
        /// If the buffer is an array, the array is filled from index 0, up to either capacity or the number of colliders found, whichever is smaller.
        /// If the buffer is a List or or collection, all found colliders are appended to the collection.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="halfExtents"></param>
        /// <param name="results"></param>
        /// <param name="orientation"></param>
        /// <param name="layerMask"></param>
        /// <param name="query"></param>
        /// <returns>The amount of colliders stored in results</returns>
        public static int OverlapBox(Vector3 center, Vector3 halfExtents, ICollection<Collider> results, 
                                     Quaternion orientation, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            if (results is Collider[])
            {
                return Physics.OverlapBoxNonAlloc(center, halfExtents, results as Collider[], orientation, layerMask, query);
            }
            else
            {
                var nonAllocArr = NonAllocColliderBuffer;

                int cnt = Physics.OverlapBoxNonAlloc(center, halfExtents, nonAllocArr, orientation, layerMask, query);
                if (results is List<Collider>)
                {
                    var lst = results as List<Collider>;
                    var num = cnt + lst.Count;
                    if (lst.Capacity < num) lst.Capacity = num;
                }

                for (int i = 0; i < cnt; i++)
                {
                    results.Add(nonAllocArr[i]);
                    nonAllocArr[i] = null;
                }
                return cnt;
            }
        }

        public static int BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, ICollection<RaycastHit> results, 
                                  Quaternion orientation, float maxDistance = float.PositiveInfinity, 
                                  int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            if (results is RaycastHit[])
            {
                return Physics.BoxCastNonAlloc(center, halfExtents, direction, results as RaycastHit[], orientation, maxDistance, layerMask, query);
            }
            else
            {
                var nonAllocArr = NonAllocRaycastBuffer;

                int cnt = Physics.BoxCastNonAlloc(center, halfExtents, direction, nonAllocArr, orientation, maxDistance, layerMask);
                if (results is List<RaycastHit>)
                {
                    var lst = results as List<RaycastHit>;
                    var num = cnt + lst.Count;
                    if (lst.Capacity < num) lst.Capacity = num;
                }

                for (int i = 0; i < cnt; i++)
                {
                    results.Add(nonAllocArr[i]);
                    nonAllocArr[i] = default(RaycastHit);
                }
                return cnt;
            }
        }

        public static int OverlapSphere(Vector3 center, float radius, ICollection<Collider> results,
                                        int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            if (results is Collider[])
            {
                return Physics.OverlapSphereNonAlloc(center, radius, results as Collider[], layerMask, query);
            }
            else
            {
                var nonAllocArr = NonAllocColliderBuffer;

                int cnt = Physics.OverlapSphereNonAlloc(center, radius, nonAllocArr, layerMask, query);
                if (results is List<Collider>)
                {
                    var lst = results as List<Collider>;
                    var num = cnt + lst.Count;
                    if (lst.Capacity < num) lst.Capacity = num;
                }

                for (int i = 0; i < cnt; i++)
                {
                    results.Add(nonAllocArr[i]);
                    nonAllocArr[i] = null;
                }
                return cnt;
            }
        }

        public static int SphereCastAll(Vector3 center, float radius, Vector3 direction, ICollection<RaycastHit> results,
                                     float maxDistance = float.PositiveInfinity,
                                     int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            if (results is RaycastHit[])
            {
                return Physics.SphereCastNonAlloc(center, radius, direction, results as RaycastHit[], maxDistance, layerMask, query);
            }
            else
            {
                var nonAllocArr = NonAllocRaycastBuffer;

                int cnt = Physics.SphereCastNonAlloc(center, radius, direction, nonAllocArr, maxDistance, layerMask, query);
                if (results is List<RaycastHit>)
                {
                    var lst = results as List<RaycastHit>;
                    var num = cnt + lst.Count;
                    if (lst.Capacity < num) lst.Capacity = num;
                }

                for (int i = 0; i < cnt; i++)
                {
                    results.Add(nonAllocArr[i]);
                    nonAllocArr[i] = default(RaycastHit);
                }
                return cnt;
            }
        }

        public static int OverlapCapsule(Vector3 point1, Vector3 point2, float radius, ICollection<Collider> results,
                                         int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            //note, HashSets are inherently unique collections... so no need to check if contains during second and third overlaps
            using (var tmpSet = TempCollection.GetSet<Collider>())
            {
                if (VectorUtil.FuzzyEquals(point1, point2))
                {
                    var nonAllocArr = NonAllocColliderBuffer;
                    int cnt = Physics.OverlapSphereNonAlloc(point1, radius, nonAllocArr, layerMask, query);
                    for (int i = 0; i < cnt; i++)
                    {
                        tmpSet.Add(nonAllocArr[i]);
                        nonAllocArr[i] = null;
                    }
                }
                else
                {
                    var nonAllocCollArr = NonAllocColliderBuffer;
                    var nonAllocRayArr = NonAllocRaycastBuffer;

                    int cnt;
                    cnt = Physics.OverlapSphereNonAlloc(point1, radius, nonAllocCollArr, layerMask, query);
                    for (int i = 0; i < cnt; i++)
                    {
                        tmpSet.Add(nonAllocCollArr[i]);
                        nonAllocCollArr[i] = null;
                    }

                    cnt = Physics.OverlapSphereNonAlloc(point2, radius, nonAllocCollArr, layerMask, query);
                    for (int i = 0; i < cnt; i++)
                    {
                        tmpSet.Add(nonAllocCollArr[i]);
                        nonAllocCollArr[i] = null;
                    }

                    var dir = point2 - point1;
                    var dist = dir.magnitude;
                    cnt = Physics.SphereCastNonAlloc(point1, radius, dir.normalized, nonAllocRayArr, dist, layerMask, query);
                    for (int i = 0; i < cnt; i++)
                    {
                        tmpSet.Add(nonAllocRayArr[i].collider);
                        nonAllocRayArr[i] = default(RaycastHit);
                    }
                }

                //done, now fill collection
                if (results is Collider[])
                {
                    var arr = results as Collider[];
                    int cnt = Mathf.Min(arr.Length, tmpSet.Count);
                    int i = -1;
                    var e = tmpSet.GetEnumerator();
                    while (e.MoveNext() && ++i < cnt)
                    {
                        arr[i] = e.Current;
                    }
                    return cnt;
                }
                else
                {
                    if (results is List<Collider>)
                    {
                        var lst = results as List<Collider>;
                        var num = tmpSet.Count + lst.Count;
                        if (lst.Capacity < num) lst.Capacity = num;
                    }

                    var e = tmpSet.GetEnumerator();
                    while (e.MoveNext())
                    {
                        results.Add(e.Current);
                    }
                    return tmpSet.Count;
                }
            }
        }

        public static int CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, ICollection<RaycastHit> results,
                                     float maxDistance = float.PositiveInfinity,
                                     int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            if (results is RaycastHit[])
            {
                return Physics.CapsuleCastNonAlloc(point1, point2, radius, direction, results as RaycastHit[], maxDistance, layerMask, query);
            }
            else
            {
                var nonAllocArr = NonAllocRaycastBuffer;

                int cnt = Physics.CapsuleCastNonAlloc(point1, point2, radius, direction, nonAllocArr, maxDistance, layerMask, query);
                if (results is List<RaycastHit>)
                {
                    var lst = results as List<RaycastHit>;
                    var num = cnt + lst.Count;
                    if (lst.Capacity < num) lst.Capacity = num;
                }

                for (int i = 0; i < cnt; i++)
                {
                    results.Add(nonAllocArr[i]);
                    nonAllocArr[i] = default(RaycastHit);
                }
                return cnt;
            }
        }

        #endregion

        #region CheckGeom

        public static bool CheckGeom(IPhysicsGeom geom, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            if (geom is Sphere)
            {
                var s = (Sphere)geom;
                return Physics.CheckSphere(s.Center, s.Radius, Physics.AllLayers,  query);
            }
            else if (geom is Capsule)
            {
                var c = (Capsule)geom;
                return Physics.CheckCapsule(c.Start, c.End, c.Radius, Physics.AllLayers, query);
            }
            else
            {
                return geom.TestOverlap(Physics.AllLayers, query);
            }
        }

        public static bool CheckGeom(IPhysicsGeom geom, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            if (geom is Sphere)
            {
                var s = (Sphere)geom;
                return Physics.CheckSphere(s.Center, s.Radius, layerMask, query);
            }
            else if (geom is Capsule)
            {
                var c = (Capsule)geom;
                return Physics.CheckCapsule(c.Start, c.End, c.Radius, layerMask, query);
            }
            else
            {
                return geom.TestOverlap(layerMask, query);
            }
        }

        #endregion

        #region OverlapGeom
        
        public static Collider[] OverlapGeom(IPhysicsGeom geom, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");
            
            using (var lst = TempCollection.GetList<Collider>())
            {
                geom.Overlap(lst, layerMask, query);
                return lst.ToArray();
            }
        }
        
        public static int OverlapGeom(IPhysicsGeom geom, IList<Collider> results, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");
            
            return geom.Overlap(results, layerMask, query);
        }

        #endregion
        
        #region Cast

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            RaycastHit hit;
            return geom.Cast(dir, out hit, float.PositiveInfinity, Physics.AllLayers, query);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, float dist, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            RaycastHit hit;
            return geom.Cast(dir, out hit, dist, Physics.AllLayers, query);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, out RaycastHit hitInfo, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.Cast(dir, out hitInfo, float.PositiveInfinity, Physics.AllLayers, query);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, float dist, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            RaycastHit hit;
            return geom.Cast(dir, out hit, dist, layerMask, query);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, out RaycastHit hitInfo, float dist, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.Cast(dir, out hitInfo, dist, Physics.AllLayers, query);
        }

        public static bool Cast(IPhysicsGeom geom, Vector3 dir, out RaycastHit hitInfo, float dist, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.Cast(dir, out hitInfo, dist, layerMask, query);
        }

        #endregion

        #region CastAll

        public static RaycastHit[] CastAll(IPhysicsGeom geom, Vector3 dir, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            //return geom.CastAll(dir, float.PositiveInfinity, Physics.AllLayers, query).ToArray();

            using (var lst = TempCollection.GetList<RaycastHit>())
            {
                geom.CastAll(dir, lst, float.PositiveInfinity, Physics.AllLayers, query);
                return lst.ToArray();
            }
        }

        public static RaycastHit[] CastAll(IPhysicsGeom geom, Vector3 dir, float dist, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            //return geom.CastAll(dir, dist, Physics.AllLayers, query).ToArray();

            using (var lst = TempCollection.GetList<RaycastHit>())
            {
                geom.CastAll(dir, lst, dist, Physics.AllLayers, query);
                return lst.ToArray();
            }
        }

        public static RaycastHit[] CastAll(IPhysicsGeom geom, Vector3 dir, float dist, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            //return geom.CastAll(dir, dist, layerMask, query).ToArray();

            using (var lst = TempCollection.GetList<RaycastHit>())
            {
                geom.CastAll(dir, lst, dist, layerMask, query);
                return lst.ToArray();
            }
        }

        public static int CastAll(IPhysicsGeom geom, Vector3 dir, ICollection<RaycastHit> results, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");
            
            return geom.CastAll(dir, results, float.PositiveInfinity, Physics.AllLayers, query);
        }

        public static int CastAll(IPhysicsGeom geom, Vector3 dir, ICollection<RaycastHit> results, float dist, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.CastAll(dir, results, dist, Physics.AllLayers, query);
        }

        public static int CastAll(IPhysicsGeom geom, Vector3 dir, ICollection<RaycastHit> results, float dist, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) throw new System.ArgumentNullException("geom");

            return geom.CastAll(dir, results, dist, layerMask, query);
        }

        #endregion

        #region RadialCast

        public static RaycastHit[] RadialCast(IPhysicsGeom geom, float dist, int detail, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return RadialCast(geom, dist, detail, layerMask, Vector2.right, query);
        }

        public static RaycastHit[] RadialCast(IPhysicsGeom geom, float dist, int detail, int layerMask, Vector2 initialAxis, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            using (var lst = TempCollection.GetList<RaycastHit>())
            {
                RadialCast(geom, lst, dist, detail, layerMask, initialAxis, query);
                return lst.ToArray();
            }
        }

        public static RaycastHit[] RadialCast(IPhysicsGeom geom, float dist, int detail, int layerMask, Vector3 initialAxis, Vector3 rotationalAxis, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            using (var lst = TempCollection.GetList<RaycastHit>())
            {
                RadialCast(geom, lst, dist, detail, layerMask, initialAxis, rotationalAxis, query);
                return lst.ToArray();
            }
        }

        public static bool RadialCast(IPhysicsGeom geom, ICollection<RaycastHit> results, float dist, int detail, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return RadialCast(geom, results, dist, detail, layerMask, Vector2.right, query);
        }

        public static bool RadialCast(IPhysicsGeom geom, ICollection<RaycastHit> results, float dist, int detail, int layerMask, Vector2 initialAxis, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (VectorUtil.NearZeroVector(initialAxis))
                initialAxis = Vector2.right;
            else
                initialAxis.Normalize();
            var a = 360f / (float)detail;

            int cnt = results.Count;
            for (int i = 0; i < detail; i++)
            {
                var v = VectorUtil.RotateBy(initialAxis, a * i);
                geom.CastAll(v, results, dist, layerMask, query);
            }
            return results.Count - cnt > 0;
        }

        public static bool RadialCast(IPhysicsGeom geom, ICollection<RaycastHit> results, float dist, int detail, int layerMask, Vector3 initialAxis, Vector3 rotationalAxis, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (VectorUtil.NearZeroVector(initialAxis))
                initialAxis = Vector2.right;
            else
                initialAxis.Normalize();
            var a = 360f / (float)detail;

            int cnt = results.Count;
            for (int i = 0; i < detail; i++)
            {
                var v = VectorUtil.RotateAroundAxis(initialAxis, a * i, rotationalAxis);
                geom.CastAll(v, results, dist, layerMask, query);
            }
            return results.Count - cnt > 0;
        }

        #endregion

        #region Selective Overlap

        public static bool CheckGeomAgainst(IPhysicsGeom geom, IEnumerable<Collider> colliders, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) return false;
            if (colliders == null) return false;

            using (var set = TempCollection.GetSet<Collider>())
            {
                if(geom.Overlap(set, layerMask, query) > 0)
                {
                    var e = LightEnumerator.Create<Collider>(colliders);
                    while (e.MoveNext())
                    {
                        if (set.Contains(e.Current)) return true;
                    }
                }
            }

            return false;
        }

        public static bool CheckGeomIgnoring(IPhysicsGeom geom, IEnumerable<Collider> ignoredColliders, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (geom == null) return false;

            using (var set = TempCollection.GetSet<Collider>())
            {
                if(geom.Overlap(set, layerMask, query) > 0)
                {
                    var e = set.GetEnumerator();
                    while(e.MoveNext())
                    {
                        if (!ignoredColliders.Contains(e.Current)) return true;
                    }
                }
            }

            return false;
        }

        public static int OverlapGeomAgainst(IPhysicsGeom geom, ICollection<Collider> results, IEnumerable<Collider> colliders, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");
            if (geom == null) return 0;
            if (colliders == null) return 0;

            int cnt = 0;
            using (var set = TempCollection.GetSet<Collider>())
            {
                if (geom.Overlap(set, layerMask, query) > 0)
                {
                    var e = LightEnumerator.Create<Collider>(colliders);
                    while (e.MoveNext())
                    {
                        if (set.Contains(e.Current))
                        {
                            cnt++;
                            results.Add(e.Current);
                        }
                    }
                }
            }

            return cnt;
        }

        public static int OverlapGeomIgnoring(IPhysicsGeom geom, ICollection<Collider> results, IEnumerable<Collider> ignoredColliders, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            if (results == null) throw new System.ArgumentNullException("results");
            if (geom == null) return 0;

            int cnt = 0;
            using (var set = TempCollection.GetSet<Collider>())
            {
                if (geom.Overlap(set, layerMask, query) > 0)
                {
                    var e = set.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (!ignoredColliders.Contains(e.Current))
                        {
                            cnt++;
                            results.Add(e.Current);
                        }
                    }
                }
            }

            return cnt;
        }

        #endregion



        #region Selective Cast

        public static bool RaycastAgainst(Ray ray, Collider coll, float maxDistance = float.PositiveInfinity)
        {
            RaycastHit hit;
            return RaycastAgainst(ray, out hit, coll, maxDistance);
        }

        public static bool RaycastAgainst(Ray ray, out RaycastHit hit, Collider coll, float maxDistance = float.PositiveInfinity)
        {
            var nonAllocArr = NonAllocRaycastBuffer;
            LayerMask mask = 1 << coll.gameObject.layer;

            int cnt = Physics.RaycastNonAlloc(ray, nonAllocArr, maxDistance, mask, QueryTriggerInteraction.Collide);
            if (cnt > 0)
            {
                System.Array.Sort(nonAllocArr, 0, cnt, RaycastHitDistanceComparer.Default);

                for (int i = 0; i < cnt; i++)
                {
                    if (coll == nonAllocArr[i].collider)
                    {
                        hit = nonAllocArr[i];
                        System.Array.Clear(nonAllocArr, 0, cnt);
                        return true;
                    }
                }

                System.Array.Clear(nonAllocArr, 0, cnt);
            }

            hit = default(RaycastHit);
            return false;
        }

        /// <summary>
        /// Raycast against a collection of colliders.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="colliders"></param>
        /// <returns></returns>
        public static bool RaycastAgainst(Vector3 pos, Vector3 dir, IEnumerable<Collider> colliders, float maxDistance = float.PositiveInfinity)
        {
            return RaycastAgainst(new Ray(pos, dir), colliders, maxDistance);
        }

        /// <summary>
        /// Raycast against a collection of colliders.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="colliders"></param>
        /// <returns></returns>
        public static bool RaycastAgainst(Ray ray, IEnumerable<Collider> colliders, float maxDistance = float.PositiveInfinity)
        {
            RaycastHit hit;
            var e = LightEnumerator.Create<Collider>(colliders);
            while (e.MoveNext())
            {
                if (e.Current.Raycast(ray, out hit, maxDistance)) return true;
            }

            return false;
        }

        /// <summary>
        /// Raycast against a collection of colliders.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="colliders"></param>
        /// <returns></returns>
        public static bool RaycastAgainst(Vector3 pos, Vector3 dir, out RaycastHit hit, IEnumerable<Collider> colliders, float maxDistance = float.PositiveInfinity)
        {
            return RaycastAgainst(new Ray(pos, dir), out hit, colliders, maxDistance);
        }

        /// <summary>
        /// Raycast against a collection of colliders.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="colliders"></param>
        /// <returns></returns>
        public static bool RaycastAgainst(Ray ray, out RaycastHit hit, IEnumerable<Collider> colliders, float maxDistance = float.PositiveInfinity)
        {
            if (colliders == null)
            {
                hit = default(RaycastHit);
                return false;
            }

            var nonAllocArr = NonAllocRaycastBuffer;

            int cnt = Physics.RaycastNonAlloc(ray, nonAllocArr, maxDistance, Physics.AllLayers, QueryTriggerInteraction.Collide);
            if(cnt > 0)
            {
                System.Array.Sort(nonAllocArr, 0, cnt, RaycastHitDistanceComparer.Default);
                
                for(int i = 0; i < cnt; i++)
                {
                    if(colliders.Contains(nonAllocArr[i].collider))
                    {
                        hit = nonAllocArr[i];
                        System.Array.Clear(nonAllocArr, 0, cnt);
                        return true;
                    }
                }

                System.Array.Clear(nonAllocArr, 0, cnt);
            }

            hit = default(RaycastHit);
            return false;
        }

        public static bool RaycastIgnoring(Vector3 pos, Vector3 dir, IEnumerable<Collider> ignoredColliders, float maxDistance = float.PositiveInfinity, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            RaycastHit hit;
            return RaycastIgnoring(new Ray(pos, dir), out hit, ignoredColliders, maxDistance, layerMask, query);
        }
        
        public static bool RaycastIgnoring(Ray ray, IEnumerable<Collider> ignoredColliders, float maxDistance = float.PositiveInfinity, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            RaycastHit hit;
            return RaycastIgnoring(ray, out hit, ignoredColliders, maxDistance, layerMask, query);
        }
        
        public static bool RaycastIgnoring(Vector3 pos, Vector3 dir, out RaycastHit hit, IEnumerable<Collider> ignoredColliders, float maxDistance = float.PositiveInfinity, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            return RaycastIgnoring(new Ray(pos, dir), out hit, ignoredColliders, maxDistance, layerMask, query);
        }
        
        public static bool RaycastIgnoring(Ray ray, out RaycastHit hit, IEnumerable<Collider> ignoredColliders, float maxDistance = float.PositiveInfinity, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            var nonAllocArr = NonAllocRaycastBuffer;

            int cnt = Physics.RaycastNonAlloc(ray, nonAllocArr, maxDistance, layerMask, query);

            if (cnt > 0)
            {
                System.Array.Sort<RaycastHit>(nonAllocArr, 0, cnt, RaycastHitDistanceComparer.Default);

                for (int i = 0; i < cnt; i++)
                {
                    if (!ignoredColliders.Contains(nonAllocArr[i].collider))
                    {
                        hit = nonAllocArr[i];
                        System.Array.Clear(nonAllocArr, 0, cnt);
                        return true;
                    }
                }

                System.Array.Clear(nonAllocArr, 0, cnt);
            }

            hit = default(RaycastHit);
            return false;
        }

        #endregion

        #region Selective CastAll

        public static RaycastHit[] RaycastAllAgainst(Vector3 origin, Vector3 dir, IEnumerable<Collider> colliders, float maxDistance = float.PositiveInfinity)
        {
            using (var lst = TempCollection.GetList<RaycastHit>())
            {
                RaycastAllAgainst(new Ray(origin, dir), lst, colliders, maxDistance);
                return lst.ToArray();
            }
        }

        public static RaycastHit[] RaycastAllAgainst(Ray ray, IEnumerable<Collider> colliders, float maxDistance = float.PositiveInfinity)
        {
            using (var lst = TempCollection.GetList<RaycastHit>())
            {
                RaycastAllAgainst(ray, lst, colliders, maxDistance);
                return lst.ToArray();
            }
        }

        public static int RaycastAllAgainst(Vector3 origin, Vector3 dir, ICollection<RaycastHit> results, IEnumerable<Collider> colliders, float maxDistance = float.PositiveInfinity)
        {
            return RaycastAllAgainst(new Ray(origin, dir), results, colliders, maxDistance);
        }

        public static int RaycastAllAgainst(Ray ray, ICollection<RaycastHit> results, IEnumerable<Collider> colliders, float maxDistance = float.PositiveInfinity)
        {
            var nonAllocArr = NonAllocRaycastBuffer;

            int cnt = Physics.RaycastNonAlloc(ray, nonAllocArr, maxDistance, Physics.AllLayers, QueryTriggerInteraction.Collide);
            
            if(results is RaycastHit[])
            {
                var arr = results as RaycastHit[];
                int j = 0;
                for (int i = 0; i < cnt; i++)
                {
                    if(j < arr.Length && colliders.Contains(nonAllocArr[i].collider))
                    {
                        arr[j] = nonAllocArr[i];
                        j++;
                    }
                    nonAllocArr[i] = default(RaycastHit);
                }
            }
            else
            {
                for (int i = 0; i < cnt; i++)
                {
                    if (colliders.Contains(nonAllocArr[i].collider))
                    {
                        results.Add(nonAllocArr[i]);
                    }
                    nonAllocArr[i] = default(RaycastHit);
                }
            }

            return cnt;
        }

        public static RaycastHit[] RaycastAllIgnoring(Vector3 origin, Vector3 dir, IEnumerable<Collider> ignoredColliders, float maxDistance = float.PositiveInfinity)
        {
            using (var lst = TempCollection.GetList<RaycastHit>())
            {
                RaycastAllIgnoring(new Ray(origin, dir), lst, ignoredColliders, maxDistance);
                return lst.ToArray();
            }
        }

        public static RaycastHit[] RaycastAllIgnoring(Ray ray, IEnumerable<Collider> ignoredColliders, float maxDistance = float.PositiveInfinity)
        {
            using (var lst = TempCollection.GetList<RaycastHit>())
            {
                RaycastAllIgnoring(ray, lst, ignoredColliders, maxDistance);
                return lst.ToArray();
            }
        }

        public static int RaycastAllIgnoring(Vector3 origin, Vector3 dir, ICollection<RaycastHit> results, IEnumerable<Collider> ignoredColliders, float maxDistance = float.PositiveInfinity)
        {
            return RaycastAllIgnoring(new Ray(origin, dir), results, ignoredColliders, maxDistance);
        }

        public static int RaycastAllIgnoring(Ray ray, ICollection<RaycastHit> results, IEnumerable<Collider> ignoredColliders, float maxDistance = float.PositiveInfinity)
        {
            var nonAllocArr = NonAllocRaycastBuffer;

            int cnt = Physics.RaycastNonAlloc(ray, nonAllocArr, maxDistance, Physics.AllLayers, QueryTriggerInteraction.Collide);

            if (results is RaycastHit[])
            {
                var arr = results as RaycastHit[];
                int j = 0;
                for (int i = 0; i < cnt; i++)
                {
                    if (j < arr.Length && !ignoredColliders.Contains(nonAllocArr[i].collider))
                    {
                        arr[j] = nonAllocArr[i];
                        j++;
                    }
                    nonAllocArr[i] = default(RaycastHit);
                }
            }
            else
            {
                for (int i = 0; i < cnt; i++)
                {
                    if (!ignoredColliders.Contains(nonAllocArr[i].collider))
                    {
                        results.Add(nonAllocArr[i]);
                    }
                    nonAllocArr[i] = default(RaycastHit);
                }
            }

            return cnt;
        }

        #endregion



        #region RepairHitSurfaceNormal

        public static Vector3 RepairHitSurfaceNormal(RaycastHit hit, int layerMask)
        {
            /*
             * Naive attempt to save a Raycast calculation... but ends up with a GC overhead 
             * retrieving the tris and verts. Stick with just a Raycast, no garbage.
             *
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
             */

            var p = hit.point + hit.normal * 0.01f;
            Physics.Raycast(p, -hit.normal, out hit, 0.011f, layerMask);
            return hit.normal;
        }

        #endregion
        
    }
}
