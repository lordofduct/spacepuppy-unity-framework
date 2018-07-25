using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{

    public class PointOctree<T> where T : class
    {

        public const float DEFAULT_MINSIZE = 1f;

        const int CHILD_COUNT = 8;

        #region Fields

        private System.Func<T, Vector3> _getPositionFunc;
        private IEqualityComparer<T> _comparer;

        private float _minSize;

        private int _count;
        private Octant _root;

        #endregion

        #region CONSTRUCTOR

        public PointOctree(System.Func<T, Vector3> getPosFunc)
        {
            if (getPosFunc == null) throw new System.ArgumentNullException("getPosFunc");
            _minSize = DEFAULT_MINSIZE;
            _getPositionFunc = getPosFunc;
            _comparer = EqualityComparer<T>.Default;

            _root = new Octant(this, _minSize, Vector3.zero);
        }

        public PointOctree(System.Func<T, Vector3> getPosFunc, IEqualityComparer<T> comparer)
        {
            if (getPosFunc == null) throw new System.ArgumentNullException("getPosFunc");
            _minSize = DEFAULT_MINSIZE;
            _getPositionFunc = getPosFunc;
            _comparer = comparer ?? EqualityComparer<T>.Default;

            _root = new Octant(this, _minSize, Vector3.zero);
        }

        public PointOctree(float minSize, System.Func<T, Vector3> getPosFunc)
        {
            if (getPosFunc == null) throw new System.ArgumentNullException("getPosFunc");
            _minSize = minSize;
            _getPositionFunc = getPosFunc;
            _comparer = EqualityComparer<T>.Default;

            _root = new Octant(this, _minSize, Vector3.zero);
        }

        public PointOctree(float minSize, System.Func<T, Vector3> getPosFunc, IEqualityComparer<T> comparer)
        {
            if (getPosFunc == null) throw new System.ArgumentNullException("getPosFunc");
            _minSize = minSize;
            _getPositionFunc = getPosFunc;
            _comparer = comparer ?? EqualityComparer<T>.Default;

            _root = new Octant(this, _minSize, Vector3.zero);
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _count; }
        }

        #endregion

        #region Methods

        public void Add(T obj)
        {
            // Add object or expand the octree until it can be added
            int count = 0; // Safety check against infinite/excessive growth
            var pos = _getPositionFunc(obj);
            while (!_root.Add(obj, pos))
            {
                Grow(pos - _root.center);
                if (++count > 20)
                {
                    Debug.LogError("Aborted Add operation as it seemed to be going on forever (" + (count - 1) + ") attempts at growing the octree.");
                    return;
                }
            }
            _count++;
        }

        public bool Contains(T obj)
        {
            return _root.Contains(obj);
        }

        public bool Remove(T obj)
        {
            bool removed = _root.Remove(obj);

            // See if we can shrink the octree down now that we've removed the item
            if (removed)
            {
                _count--;
                Shrink();
            }

            return removed;
        }

        /// <summary>
        /// Recalculates the entire octree by re-polling the positions of all members.
        /// </summary>
        public void Resync()
        {
            //TODO - do this less naively, need a simple working fix for now
            //TODO - we should probably store the 'pos' of the object in the 'objects' list so that way we can test if it's moved
            using (var lst = TempCollection.GetList<T>())
            {
                if(_root.GetAll(lst) > 0)
                {
                    foreach(var obj in lst)
                    {
                        _root.Remove(obj);
                        _root.Add(obj, _getPositionFunc(obj));
                    }
                }
            }
        }

        private void Grow(Vector3 direction)
        {
            int xDirection = direction.x >= 0 ? 1 : -1;
            int yDirection = direction.y >= 0 ? 1 : -1;
            int zDirection = direction.z >= 0 ? 1 : -1;
            Octant oldRoot = _root;
            float half = _root.sideLength / 2;
            float newLength = _root.sideLength * 2;
            Vector3 newCenter = _root.center + new Vector3(xDirection * half, yDirection * half, zDirection * half);

            // Create a new, bigger octree root node
            _root = new Octant(this, newLength, newCenter);

            // Create 7 new octree children to go with the old root as children of the new root
            int rootPos = GetRootPosIndex(xDirection, yDirection, zDirection);
            Octant[] children = new Octant[8];
            for (int i = 0; i < 8; i++)
            {
                if (i == rootPos)
                {
                    children[i] = oldRoot;
                }
                else
                {
                    xDirection = i % 2 == 0 ? -1 : 1;
                    yDirection = i > 3 ? -1 : 1;
                    zDirection = (i < 2 || (i > 3 && i < 6)) ? -1 : 1;
                    children[i] = new Octant(this, _root.sideLength, newCenter + new Vector3(xDirection * half, yDirection * half, zDirection * half));
                }
            }

            // Attach the new children to the new root node
            _root.children = children;
        }

        private void Shrink()
        {
            _root = _root.ShrinkIfPossible(_minSize);
        }


        public T[] GetNearby(Vector3 position, float maxDistance)
        {
            using (var lst = TempCollection.GetList<T>())
            {
                _root.GetNearby(lst, ref position, ref maxDistance);
                return lst.ToArray();
            }
        }

        public int GetNearby(ICollection<T> coll, Vector3 position, float maxDistance)
        {
            return _root.GetNearby(coll, ref position, ref maxDistance);
        }

        public T[] GetNearby(Ray ray, float maxDistance)
        {
            using (var lst = TempCollection.GetList<T>())
            {
                _root.GetNearby(lst, ref ray, ref maxDistance);
                return lst.ToArray();
            }
        }

        public int GetNearby(ICollection<T> coll, Ray ray, float maxDistance)
        {
            return _root.GetNearby(coll, ref ray, ref maxDistance);
        }

        #endregion

        #region Draw Gizmos

        private static int _nestCount;

        /// <summary>
        /// Draw gizmos for all bounds.
        /// </summary>
        public void DrawAllBounds()
        {
            _nestCount = 0;
            _root.DrawAllBounds();
        }

        /// <summary>
        /// Draws gizmos for the bounds of all objects in the tree visually for debugging.
        /// Must be called from OnDrawGizmos externally. See also: DrawAllBounds.
        /// </summary>
        public void DrawAllObjects()
        {
            _root.DrawAllObjects();
        }

        #endregion

        #region Static Utils

        /// <summary>
        /// Used when growing the octree. Works out where the old root node would fit inside a new, larger root node.
        /// </summary>
        /// <param name="xDir">X direction of growth. 1 or -1.</param>
        /// <param name="yDir">Y direction of growth. 1 or -1.</param>
        /// <param name="zDir">Z direction of growth. 1 or -1.</param>
        /// <returns>Octant where the root node should be.</returns>
        static int GetRootPosIndex(int xDir, int yDir, int zDir)
        {
            int result = xDir > 0 ? 1 : 0;
            if (yDir < 0) result += 4;
            if (zDir > 0) result += 2;
            return result;
        }

        #endregion


        #region Special Types

        private class Octant
        {

            #region Fields

            private PointOctree<T> _owner;

            public Bounds bounds;
            public Vector3 center;
            public float sideLength;

            public readonly List<T> objects = new List<T>();
            public Octant[] children;

            #endregion

            #region CONSTRUCTOR

            public Octant(PointOctree<T> owner, float side, Vector3 cent)
            {
                _owner = owner;
                this.SetValues(side, cent);

            }

            #endregion

            #region Methods

            public bool Add(T obj, Vector3 pos)
            {
                if (!this.bounds.Contains(pos))
                {
                    return false;
                }
                this.SubAdd(obj, pos);
                return true;
            }

            public bool Contains(T obj)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    if (_owner._comparer.Equals(objects[i], obj)) return true;
                }

                if(children != null)
                {
                    for (int i = 0; i < children.Length; i++)
                    {
                        if (children[i] != null && children[i].Contains(obj)) return true;
                    }
                }

                return false;
            }

            public bool Remove(T obj)
            {
                bool removed = false;

                for (int i = 0; i < objects.Count; i++)
                {
                    if (_owner._comparer.Equals(objects[i], obj))
                    {
                        objects.RemoveAt(i);
                        removed = true;
                        break;
                    }
                }

                if (!removed && children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        removed = children[i].Remove(obj);
                        if (removed) break;
                    }
                }

                if (removed && children != null)
                {
                    // Check if we should merge nodes now that we've removed an item
                    if (ShouldMerge())
                    {
                        Merge();
                    }
                }

                return removed;
            }

            public Octant ShrinkIfPossible(float minLength)
            {
                if (this.sideLength < (2 * minLength))
                {
                    return this;
                }
                if (objects.Count == 0 && (children == null || children.Length == 0))
                {
                    return this;
                }

                // Check objects in root
                int bestFit = -1;
                for (int i = 0; i < objects.Count; i++)
                {
                    int newBestFit = BestFitChild(_owner._getPositionFunc(objects[i]));
                    if (i == 0 || newBestFit == bestFit)
                    {
                        if (bestFit < 0)
                        {
                            bestFit = newBestFit;
                        }
                    }
                    else
                    {
                        return this; // Can't reduce - objects fit in different octants
                    }
                }

                // Check objects in children if there are any
                if (children != null)
                {
                    // Check objects in children if there are any
                    bool childHadContent = false;
                    for (int i = 0; i < children.Length; i++)
                    {
                        if (children[i].HasAnyObjects())
                        {
                            if (childHadContent)
                            {
                                return this; // Can't shrink - another child had content already
                            }
                            if (bestFit >= 0 && bestFit != i)
                            {
                                return this; // Can't reduce - objects in root are in a different octant to objects in child
                            }
                            childHadContent = true;
                            bestFit = i;
                        }
                    }
                }

                //can reduce
                if(children == null || bestFit < 0)
                {
                    // We don't have any children, so just shrink this node to the new size
                    // We already know that everything will still fit in it
                    this.SetValues(this.sideLength / 2, this.GetChildCenter(bestFit));
                    return this;
                }

                // We have children. Use the appropriate child as the new root node
                return children[bestFit];
            }

            public int GetNearby(ICollection<T> coll, ref Vector3 position, ref float maxDistance)
            {
                int cnt = 0;

                var b = bounds;
                b.Expand(new Vector3(maxDistance * 2, maxDistance * 2, maxDistance * 2));
                if (!b.Contains(position))
                {
                    return cnt;
                }
                bounds.size = new Vector3(this.sideLength, this.sideLength, this.sideLength);

                // Check against any objects in this node
                float sqrDist = maxDistance * maxDistance;
                for (int i = 0; i < objects.Count; i++)
                {
                    if ((position - _owner._getPositionFunc(objects[i])).sqrMagnitude <= sqrDist)
                    {
                        coll.Add(objects[i]);
                        cnt++;
                    }
                }

                // Check children
                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (children[i] != null) cnt += children[i].GetNearby(coll, ref position, ref maxDistance);
                    }
                }

                return cnt;
            }

            public int GetNearby(ICollection<T> coll, ref Ray ray, ref float maxDistance)
            {
                int cnt = 0;

                var b = bounds;
                b.Expand(new Vector3(maxDistance * 2, maxDistance * 2, maxDistance * 2));
                if (!b.IntersectRay(ray))
                {
                    return cnt;
                }

                // Check against any objects in this node
                float sqrDist = maxDistance * maxDistance;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (Vector3.Cross(ray.direction, _owner._getPositionFunc(objects[i]) - ray.origin).sqrMagnitude <= sqrDist)
                    {
                        coll.Add(objects[i]);
                        cnt++;
                    }
                }

                // Check children
                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (children[i] != null) cnt += children[i].GetNearby(coll, ref ray, ref maxDistance);
                    }
                }

                return cnt;
            }

            public int GetAll(ICollection<T> coll)
            {
                int cnt = 0;
                for(int i = 0; i < objects.Count; i++)
                {
                    cnt++;
                    coll.Add(objects[i]);
                }

                if(children != null)
                {
                    for (int i = 0; i < children.Length; i++)
                    {
                        if (children[i] != null)
                        {
                            cnt += children[i].GetAll(coll);
                        }
                    }
                }
                return cnt;
            }

            #endregion

            #region Internal Utils

            void SetValues(float side, Vector3 cent)
            {
                this.sideLength = side;
                this.center = cent;

                // Create the bounding box.
                this.bounds = new Bounds(center, new Vector3(side, side, side));
            }


            private void SubAdd(T obj, Vector3 pos)
            {
                // We know it fits at this level if we've got this far
                // Just add if few objects are here, or children would be below min size
                if (objects.Count < CHILD_COUNT || (this.sideLength / 2) < _owner._minSize)
                {
                    objects.Add(obj);
                }
                else
                { // Enough objects in this node already: Create new children
                  // Create the 8 children
                    int bestFitChild;
                    if (children == null)
                    {
                        Split();
                        if (children == null)
                        {
                            Debug.Log("Child creation failed for an unknown reason. Early exit.");
                            return;
                        }

                        // Now that we have the new children, see if this node's existing objects would fit there
                        for (int i = objects.Count - 1; i >= 0; i--)
                        {
                            var p = _owner._getPositionFunc(objects[i]);
                            // Find which child the object is closest to based on where the
                            // object's center is located in relation to the octree's center.
                            bestFitChild = BestFitChild(p);
                            children[bestFitChild].SubAdd(objects[i], p); // Go a level deeper					
                            objects.RemoveAt(i);
                        }
                    }

                    // Now handle the new object we're adding now
                    bestFitChild = BestFitChild(pos);
                    children[bestFitChild].SubAdd(obj, pos);
                }
            }

            private void Split()
            {
                float quarter = this.sideLength / 4f;
                float newLength = this.sideLength / 2;
                children = new Octant[8];
                children[0] = new Octant(_owner, newLength, center + new Vector3(-quarter, quarter, -quarter));
                children[1] = new Octant(_owner, newLength, center + new Vector3(quarter, quarter, -quarter));
                children[2] = new Octant(_owner, newLength, center + new Vector3(-quarter, quarter, quarter));
                children[3] = new Octant(_owner, newLength, center + new Vector3(quarter, quarter, quarter));
                children[4] = new Octant(_owner, newLength, center + new Vector3(-quarter, -quarter, -quarter));
                children[5] = new Octant(_owner, newLength, center + new Vector3(quarter, -quarter, -quarter));
                children[6] = new Octant(_owner, newLength, center + new Vector3(-quarter, -quarter, quarter));
                children[7] = new Octant(_owner, newLength, center + new Vector3(quarter, -quarter, quarter));
            }


            private int BestFitChild(Vector3 pos)
            {
                return (pos.x <= center.x ? 0 : 1) + (pos.y >= center.y ? 0 : 4) + (pos.z <= center.z ? 0 : 2);
            }

            private bool ShouldMerge()
            {
                int totalObjects = objects.Count;
                if (children != null)
                {
                    foreach (Octant child in children)
                    {
                        if (child != null && child.children != null)
                        {
                            // If any of the *children* have children, there are definitely too many to merge,
                            // or the child woudl have been merged already
                            return false;
                        }
                        totalObjects += child.objects.Count;
                    }
                }
                return totalObjects <= CHILD_COUNT;
            }

            private void Merge()
            {
                // Note: We know children != null or we wouldn't be merging
                foreach (var child in children)
                {
                    if (child == null) continue;

                    int numObjects = child.objects.Count;
                    for (int j = numObjects - 1; j >= 0; j--)
                    {
                        objects.Add(child.objects[j]);
                    }
                }
                // Remove the child nodes (and the objects in them - they've been added elsewhere now)
                children = null;
            }

            private bool HasAnyObjects()
            {
                if (objects.Count > 0) return true;

                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (children[i] != null && children[i].HasAnyObjects()) return true;
                    }
                }

                return false;
            }


            private Vector3 GetChildCenter(int i)
            {
                float quarter = sideLength / 4f;
                float childActualLength = sideLength / 2f;
                Vector3 size = new Vector3(childActualLength, childActualLength, childActualLength);
                switch (i)
                {
                    case 0:
                        return center + new Vector3(-quarter, quarter, -quarter);
                    case 1:
                        return center + new Vector3(quarter, quarter, -quarter);
                    case 2:
                        return center + new Vector3(-quarter, quarter, quarter);
                    case 3:
                        return center + new Vector3(quarter, quarter, quarter);
                    case 4:
                        return center + new Vector3(-quarter, -quarter, -quarter);
                    case 5:
                        return center + new Vector3(quarter, -quarter, -quarter);
                    case 6:
                        return center + new Vector3(-quarter, -quarter, quarter);
                    case 7:
                        return center + new Vector3(quarter, -quarter, quarter);
                    default:
                        return center;
                }
            }

            private Bounds GetChildBounds(int i)
            {
                float quarter = sideLength / 4f;
                float childActualLength = sideLength / 2f;
                Vector3 size = new Vector3(childActualLength, childActualLength, childActualLength);
                switch (i)
                {
                    case 0:
                        return new Bounds(center + new Vector3(-quarter, quarter, -quarter), size);
                    case 1:
                        return new Bounds(center + new Vector3(quarter, quarter, -quarter), size);
                    case 2:
                        return new Bounds(center + new Vector3(-quarter, quarter, quarter), size);
                    case 3:
                        return new Bounds(center + new Vector3(quarter, quarter, quarter), size);
                    case 4:
                        return new Bounds(center + new Vector3(-quarter, -quarter, -quarter), size);
                    case 5:
                        return new Bounds(center + new Vector3(quarter, -quarter, -quarter), size);
                    case 6:
                        return new Bounds(center + new Vector3(-quarter, -quarter, quarter), size);
                    case 7:
                        return new Bounds(center + new Vector3(quarter, -quarter, quarter), size);
                    default:
                        return default(Bounds);
                }
            }

            #endregion

            #region Gizmos

            public void DrawAllBounds(float depth = 0)
            {
                _nestCount++;
                float tintVal = depth / 7; // Will eventually get values > 1. Color rounds to 1 automatically
                Gizmos.color = new Color(tintVal, 0, 1.0f - tintVal);

                Bounds thisBounds = new Bounds(this.center, new Vector3(this.sideLength, this.sideLength, this.sideLength));
                Gizmos.DrawWireCube(thisBounds.center, thisBounds.size);

                if (children != null)
                {
                    depth++;
                    for (int i = 0; i < 8; i++)
                    {
                        children[i].DrawAllBounds(depth);
                    }
                }
                Gizmos.color = Color.white;
            }

            public void DrawAllObjects()
            {
                float tintVal = this.sideLength / 20;
                Gizmos.color = new Color(0, 1.0f - tintVal, tintVal, 0.25f);

                foreach (var obj in objects)
                {
                    Gizmos.DrawIcon(_owner._getPositionFunc(obj), "marker.tif", true);
                }

                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (children != null) children[i].DrawAllObjects();
                    }
                }

                Gizmos.color = Color.white;
            }
            #endregion

        }

        #endregion

    }

}
