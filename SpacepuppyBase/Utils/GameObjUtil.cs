using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Geom;

namespace com.spacepuppy.Utils
{
    public static class GameObjectUtil
    {
        
        public static GameObject CreateRoot(string name)
        {
            var go = new GameObject(name);
            go.AddTag(SPConstants.TAG_ROOT);
            return go;
        }

        public static GameObject CreateRoot(string name, params System.Type[] components)
        {
            var go = new GameObject(name);
            go.AddTag(SPConstants.TAG_ROOT);
            foreach (var tp in components)
            {
                go.AddComponent(tp);
            }
            return go;
        }
        
#region Get*FromSource

        public static bool IsGameObjectSource(object obj)
        {
            return (obj is GameObject || obj is Component || obj is IGameObjectSource);
        }

        public static bool IsGameObjectSource(object obj, bool respectProxy)
        {
            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return false;
            }

            return (obj is GameObject || obj is Component || obj is IGameObjectSource);
        }

        public static GameObject GetGameObjectFromSource(object obj)
        {
            if (obj == null) return null;
            if (obj is GameObject)
                return obj as GameObject;
            if (obj is Component)
                return (obj as Component).gameObject;
            if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).gameObject;

            return null;
        }

        public static GameObject GetGameObjectFromSource(object obj, bool respectProxy)
        {
            if (obj == null) return null;

            if(respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return null;
            }

            if (obj is GameObject)
                return obj as GameObject;
            if (obj is Component)
                return (obj as Component).gameObject;
            if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).gameObject;

            return null;
        }

        public static Transform GetTransformFromSource(object obj)
        {
            if (obj == null) return null;
            if (obj is Transform)
                return obj as Transform;
            if (obj is GameObject)
                return (obj as GameObject).transform;
            if (obj is Component)
                return (obj as Component).transform;
            if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).transform;

            return null;
        }

        public static Transform GetTransformFromSource(object obj, bool respectProxy)
        {
            if (obj == null) return null;

            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return null;
            }

            if (obj is Transform)
                return obj as Transform;
            if (obj is GameObject)
                return (obj as GameObject).transform;
            if (obj is Component)
                return (obj as Component).transform;
            if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).transform;

            return null;
        }

        public static GameObject GetRootFromSource(object obj)
        {
            if (obj.IsNullOrDestroyed()) return null;

            if (obj is IComponent) obj = (obj as IComponent).component;

            if (obj is Transform)
                return (obj as Transform).FindRoot();
            else if (obj is GameObject)
                return (obj as GameObject).FindRoot();
            else if (obj is SPComponent)
            {
                return (obj as SPComponent).entityRoot;
            }
            else if (obj is Component)
                return (obj as Component).FindRoot();
            else if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).gameObject.FindRoot();

            return null;
        }

        public static GameObject GetTrueRootFromSource(object obj)
        {
            if (obj.IsNullOrDestroyed()) return null;

            if (obj is IComponent) obj = (obj as IComponent).component;

            if (obj is Transform)
                return (obj as Transform).FindTrueRoot();
            else if (obj is GameObject)
                return (obj as GameObject).FindTrueRoot();
            else if (obj is SPComponent)
            {
                var r = (obj as SPComponent).entityRoot;
                if (r.HasTag(SPConstants.TAG_ROOT))
                    return r;
                else
                    return null;
            }
            else if (obj is Component)
                return (obj as Component).FindTrueRoot();
            else if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).gameObject.FindTrueRoot();

            return null;
        }

#endregion



#region Kill Extension Methods

        /// <summary>
        /// Object is not null, dead/killed, and is active in hierarchy.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsAliveAndActive(this GameObject obj)
        {
            return obj != null && obj.activeInHierarchy && !obj.IsKilled();
        }

        /// <summary>
        /// Object is not null, dead/killed, and is active in hierarchy as well as enabled.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsAliveAndActive(this Component obj)
        {
            return obj != null && obj.IsActiveAndEnabled() && !obj.IsKilled();
        }

        /// <summary>
        /// Object is not null, dead/killed, and is active in hierarchy as well as enabled.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsAliveAndActive(this Behaviour obj)
        {
            return obj != null && obj.isActiveAndEnabled && !obj.IsKilled();
        }
        
        /// <summary>
        /// Tests if the object is either destroyed or killed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsKilled(this GameObject obj)
        {
            if (obj == null) return true;
            
            using (var lst = TempCollection.GetList<IKillableEntity>())
            {
                obj.GetComponents<IKillableEntity>(lst);
                if (lst.Count > 0)
                {
                    var e = lst.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.IsDead) return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if the object is either destroyed or killed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsKilled(this Component obj)
        {
            if (obj == null) return true;
            
            using (var lst = TempCollection.GetList<IKillableEntity>())
            {
                obj.GetComponents<IKillableEntity>(lst);
                if (lst.Count > 0)
                {
                    var e = lst.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.IsDead) return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Destroys the GameObject and its children, if the GameObject contains a KillableEntity component that will handle the death first and foremost.
        /// </summary>
        /// <param name="obj"></param>
        public static void Kill(this GameObject obj)
        {
            if (obj.IsNullOrDestroyed()) return;

            if(Application.isPlaying)
            {
                using (var lst = TempCollection.GetList<IKillableEntity>())
                {
                    //this returns in the order from top down, we will loop backwards to kill bottom up
                    obj.GetComponentsInChildren<IKillableEntity>(true, lst);
                    if (lst.Count > 0)
                    {
                        for (int i = lst.Count - 1; i > -1; i--)
                        {
                            lst[i].Kill();
                        }

                        if (lst[0].gameObject != obj)
                        {
                            UnityEngine.Object.Destroy(obj);
                        }
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                }
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }

        }

        /// <summary>
        /// Destroys the entire entity, if the entity contains a KillableEntity component that will handle death first and foremost.
        /// </summary>
        /// <param name="obj"></param>
        public static void KillEntity(this GameObject obj)
        {
            Kill(obj.FindRoot());
        }
        
#endregion

#region Layer Methods

        public static IEnumerable<GameObject> FindGameObjectOnLayer(int mask)
        {
            var arr = GameObject.FindObjectsOfType(typeof(GameObject));

            foreach (GameObject go in arr)
            {
                if (((1 << go.layer) & mask) != 0) yield return go;
            }
        }

        public static IEnumerable<GameObject> FindGameObjectOnLayer(this GameObject go, int mask)
        {
            if (go == null) yield break;
            if (((1 << go.layer) & mask) != 0) yield return go;

            foreach (Transform child in go.transform.IterateAllChildren())
            {
                if (((1 << child.gameObject.layer) & mask) != 0) yield return child.gameObject;
            }
        }

        public static void ChangeLayer(this GameObject obj, int layer, bool recursive, params string[] ignoreNames)
        {
            obj.layer = layer;

            if (recursive)
            {
                foreach (Transform child in obj.transform)
                {
                    if (!StringUtil.Equals(child.name, ignoreNames))
                    {
                        ChangeLayer(child.gameObject, layer, recursive, ignoreNames);
                    }
                }
            }
        }

        public static bool IntersectsLayerMask(this GameObject obj, int layerMask)
        {
            if (obj == null) return false;

            return ((1 << obj.layer) & layerMask) != 0;
        }

#endregion
        
#region Search/Find

        public static bool CompareName(this GameObject go, string name)
        {
            if (go == null) return false;

            return go.name == name;
        }

        public static bool CompareName(this Component c, string name)
        {
            if (c == null) return false;

            if (c is SPEntity)
                return (c as SPEntity).CompareName(name);
            else
                return c.name == name;
        }

        public static GameObject Find(this GameObject go, string spath)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            var child = go.transform.Find(spath);
            return (child != null) ? child.gameObject : null;
        }

        /// <summary>
        /// Finds a gameobject based on some path. This works just like Transform.Find, but added a case sensitivity option.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="spath"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static GameObject Find(this GameObject go, string spath, bool bIgnoreCase)
        {
            var result = Find(go.transform, spath, bIgnoreCase);
            if (result != null)
                return result.gameObject;
            else
                return null;
        }

        /// <summary>
        /// Finds a gameobject based on some path. This works just like Transform.Find, but added a case sensitivity option.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="spath"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static Transform Find(this Transform trans, string spath, bool bIgnoreCase)
        {
            if (bIgnoreCase)
            {
                var arr = spath.Split('/');
                if (arr == null || arr.Length == 0) return null;

                foreach (string sname in arr)
                {
                    bool foundNext = false;
                    foreach (Transform child in trans)
                    {
                        if (StringUtil.Equals(sname, child.name, true))
                        {
                            foundNext = true;
                            trans = child;
                            break;
                        }
                    }
                    if (!foundNext) return null;
                }

                return trans;
            }
            else
            {
                return trans.Find(spath);
            }
        }

        /// <summary>
        /// Attempts to find a gameobject based on some name, if it isn't found a gameobject is created with the name and added as a child. 
        /// Note, this is unlike Transform.Find in that it only searches for direct children, and does not traverse the hierarchy.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static GameObject FindOrAddChild(this GameObject go, string name, bool bIgnoreCase)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            GameObject child = (from Transform c in go.transform where StringUtil.Equals(c.name, name, bIgnoreCase) select c.gameObject).FirstOrDefault();

            if (child == null)
            {
                child = new GameObject(name);
                child.transform.parent = go.transform;
                child.transform.ZeroOut(false);
            }

            return child;
        }

        /// <summary>
        /// Attempts to find a gameobject based on some name, if it isn't found a gameobject is created with the name and added as a child. 
        /// Note, this is unlike Transform.Find in that it only searches for direct children, and does not traverse the hierarchy.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static Transform FindOrAddChild(this Transform trans, string name, bool bIgnoreCase)
        {
            if (trans == null) throw new System.ArgumentNullException("trans");

            Transform child = (from Transform c in trans where StringUtil.Equals(c.name, name, bIgnoreCase) select c).FirstOrDefault();

            if (child == null)
            {
                var childGo = new GameObject(name);
                child = childGo.transform;
                child.parent = trans;
                child.ZeroOut(false);
            }

            return child;
        }

        /// <summary>
        /// This is similar to Find, but allows for arbitrary path definitions using the '*'.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="spath"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static GameObject Search(this GameObject go, string spath, bool bIgnoreCase = false)
        {
            var result = Search(go.transform, spath, bIgnoreCase);
            if (result != null)
                return result.gameObject;
            else
                return null;
        }

        /// <summary>
        /// This is similar to Find, but allows for arbitrary path definitions using the '*'.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="spath"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static Transform Search(this Transform trans, string spath, bool bIgnoreCase = false)
        {
            if (!spath.Contains("*"))
            {
                return trans.Find(spath, bIgnoreCase);
            }
            else
            {
                var arr = spath.Split('/');
                string sval;
                spath = "";

                for (int i = 0; i < arr.Length; i++)
                {
                    sval = arr[i];

                    if (sval == "*")
                    {
                        if (spath != "")
                        {
                            trans = trans.Find(spath, bIgnoreCase);
                            if (trans == null) return null;
                            spath = "";
                        }

                        i++;
                        if (i >= arr.Length)
                            return (trans.childCount > 0) ? trans.GetChild(0) : null;
                        else
                        {
                            sval = arr[i];
                            //now we're going to do our recursing search
                            trans = FindByName(trans, sval, bIgnoreCase);
                            if (trans == null) return null;
                        }
                    }
                    else
                    {
                        if (spath != "") spath += "/";
                        spath += sval;
                    }

                }

                return trans;
            }
        }

        /// <summary>
        /// Recurses through all children until a child of some name is found.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="sname"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static GameObject FindByName(this GameObject go, string sname, bool bIgnoreCase = false)
        {
            if (go == null) return null;
            var result = FindByName(go.transform, sname, bIgnoreCase);
            if (result != null)
                return result.gameObject;
            else
                return null;
        }

        /// <summary>
        /// Recurses through all children until a child of some name is found.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="sname"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static Transform FindByName(this Transform trans, string sname, bool bIgnoreCase = false)
        {
            if (trans == null) return null;
            foreach (var child in trans.IterateAllChildren())
            {
                if (StringUtil.Equals(child.name, sname, bIgnoreCase)) return child;
            }
            return null;
        }

        public static IEnumerable<GameObject> FindAllByName(string sname, bool bIgnoreCase = false)
        {
            foreach (var go in Object.FindObjectsOfType<GameObject>())
            {
                if (StringUtil.Equals(go.name, sname, bIgnoreCase)) yield return go;
            }
        }

        public static void FindAllByName(string sname, ICollection<GameObject> results, bool bIgnoreCase = false)
        {
            if (results == null) throw new System.ArgumentNullException("results");
            
            foreach (var go in Object.FindObjectsOfType<GameObject>())
            {
                if (StringUtil.Equals(go.name, sname, bIgnoreCase)) results.Add(go);
            }
        }


        public static Transform[] FindAllByName(this Transform trans, string sname, bool bIgnoreCase = false)
        {
            if (trans == null) return ArrayUtil.Empty<Transform>();

            using (var results = TempCollection.GetList<Transform>())
            {
                FindAllByName(trans, sname, results, bIgnoreCase);
                return results.ToArray();
            }
        }

        public static void FindAllByName(this Transform trans, string sname, ICollection<Transform> results, bool bIgnoreCase = false)
        {
            if (results == null) throw new System.ArgumentNullException("results");
            if (trans == null) return;
            
            using (var lst = TempCollection.GetList<Transform>())
            {
                trans.GetAllChildrenAndSelf(lst);
                var e = lst.GetEnumerator();
                while (e.MoveNext())
                {
                    if (StringUtil.Equals(e.Current.name, sname, bIgnoreCase))
                    {
                        results.Add(e.Current);
                    }
                }
            }
        }

        public static GameObject FindParentWithName(this GameObject go, string sname, bool bIgnoreCase = false)
        {
            if (go == null) return null;
            var result = FindParentWithName(go.transform, sname, bIgnoreCase);
            if (result != null)
                return result.gameObject;
            else
                return null;
        }

        public static Transform FindParentWithName(this Transform trans, string sname, bool bIgnoreCase = false)
        {
            var p = trans.parent;
            while(p != null)
            {
                if (StringUtil.Equals(p.name, sname, bIgnoreCase)) return p;
                p = p.parent;
            }
            return null;
        }

        public static string GetPathNameRelativeTo(this GameObject go, Transform parent)
        {
            if (go == null) return null;
            return GetPathNameRelativeTo(go.transform, parent);
        }

        public static string GetPathNameRelativeTo(this Transform t, Transform parent)
        {
            if (t == null) return null;
            if (t == parent) return null;

            var bldr = StringUtil.GetTempStringBuilder();
            bldr.Append(t.name);
            t = t.parent;
            while(t != null && t != parent)
            {
                bldr.Insert(0, '/');
                bldr.Insert(0, t.name);
            }
            return StringUtil.Release(bldr);
        }

        public static string GetFullPathName(this Transform t)
        {
            var builder = StringUtil.GetTempStringBuilder();
            while (t.parent != null)
            {
                t = t.parent;
                builder.Insert(0, @"\");
                builder.Insert(0, t.name);
            }
            return StringUtil.Release(builder);
        }

#endregion

#region Tags

        /**
         * Find
         */

        public static GameObject[] FindGameObjectsWithMultiTag(string tag)
        {
            if (tag == SPConstants.TAG_MULTITAG)
            {
                return GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG);
            }
            else
            {
                using (var tmp = TempList<GameObject>.GetList())
                {
                    foreach (var go in GameObject.FindGameObjectsWithTag(tag)) tmp.Add(go);

                    MultiTag.FindAll(tag, tmp);

                    return tmp.ToArray();
                }
            }
        }

        public static int FindGameObjectsWithMultiTag(string tag, ICollection<UnityEngine.GameObject> coll)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            int cnt = coll.Count;
            if(tag == SPConstants.TAG_MULTITAG)
            {
                coll.AddRange(GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG));
            }
            else
            {
                foreach (var go in GameObject.FindGameObjectsWithTag(tag)) coll.Add(go);

                MultiTag.FindAll(tag, coll);
            }
            return coll.Count - cnt;
        }

        public static GameObject FindWithMultiTag(string tag)
        {
            if (tag == SPConstants.TAG_MULTITAG)
            {
                return GameObject.FindWithTag(SPConstants.TAG_MULTITAG);
            }
            else
            {
                var directHit = GameObject.FindWithTag(tag);
                if (directHit != null) return directHit;

                //MultiTag comp;
                //foreach (var go in GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG))
                //{
                //    if (go.GetComponent<MultiTag>(out comp))
                //    {
                //        if (comp.ContainsTag(tag)) return go;
                //    }
                //}

                var comp = MultiTag.Find(tag);
                return (comp != null) ? comp.gameObject : null;
            }
        }

        public static GameObject FindWithMultiTag(this GameObject go, string tag)
        {
            if (MultiTagHelper.HasTag(go, tag)) return go;

            foreach (var child in go.transform.IterateAllChildren())
            {
                if (MultiTagHelper.HasTag(child.gameObject, tag)) return child.gameObject;
            }

            return null;
        }

        public static IEnumerable<GameObject> FindAllWithMultiTag(this GameObject go, string tag)
        {
            if (MultiTagHelper.HasTag(go)) yield return go;

            foreach(var child in go.transform.IterateAllChildren())
            {
                if (MultiTagHelper.HasTag(child.gameObject, tag)) yield return child.gameObject;
            }
        }

        /**
         * FindParentWithTag
         */

        public static GameObject FindParentWithTag(this GameObject go, string stag)
        {
            if (go == null) return null;
            return FindParentWithTag(go.transform, stag);
        }

        public static GameObject FindParentWithTag(this Component c, string stag)
        {
            if (c == null) return null;
            return FindParentWithTag(c.transform, stag);
        }

        public static GameObject FindParentWithTag(this Transform t, string stag)
        {
            while(t != null)
            {
                if (MultiTagHelper.HasTag(t, stag)) return t.gameObject;
                t = t.parent;
            }

            return null;
        }

        /*
         * ReduceToParent
         */

        public static IEnumerable<GameObject> ReduceToParentWithTag(this IEnumerable<GameObject> e, string tag, bool bdistinct = true)
        {
            if (bdistinct)
            {
                foreach (var obj in ReduceToParentWithTag(e, tag, false).Distinct()) yield return obj;
            }
            else
            {
                foreach (var obj in e)
                {
                    var o = FindParentWithTag(obj, tag);
                    if (o != null) yield return o;
                }
            }
        }

        public static IEnumerable<GameObject> ReduceToParentWithTag(this IEnumerable<Component> e, string tag, bool bdistinct = true)
        {
            if (bdistinct)
            {
                foreach (var obj in ReduceToParentWithTag(e, tag, false).Distinct()) yield return obj;
            }
            else
            {
                foreach (var obj in e)
                {
                    var o = FindParentWithTag(obj, tag);
                    if (o != null) yield return o;
                }
            }
        }

#endregion

#region Find Root

        /**
         * HasTrueRoot
         */

        /// <summary>
        /// Returns true if 
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool HasTrueRoot(this GameObject go)
        {
            if (go == null) return false;

            var t = go.transform;
            while(t != null)
            {
                if (MultiTagHelper.HasTag(t.gameObject, SPConstants.TAG_ROOT)) return true;
            }

            return false;
        }

        public static bool HasTrueRoot(this Component c)
        {
            if (c == null) return false;
            return HasTrueRoot(c.gameObject);
        }

        public static bool HasTrueRoot(this GameObject go, out GameObject root)
        {
            root = null;
            if (go == null) return false;

            var t = go.transform;
            while(t != null)
            {
                if(MultiTagHelper.HasTag(t.gameObject, SPConstants.TAG_ROOT))
                {
                    root = t.gameObject;
                    return true;
                }
            }

            return false;
        }

        public static bool HasTrueRoot(this Component c, out GameObject root)
        {
            if (c == null)
            {
                root = null;
                return false;
            }
            return HasTrueRoot(c.gameObject, out root);
        }

        /**
         * FindTrueRoot
         */

        /// <summary>
        /// Attempts to find a parent with a tag of 'Root', if none is found, null is returned.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static GameObject FindTrueRoot(this GameObject go)
        {
            if (go == null) return null;

            return FindParentWithTag(go.transform, SPConstants.TAG_ROOT);
        }

        public static GameObject FindTrueRoot(this Component c)
        {
            if (c == null) return null;
            return FindParentWithTag(c.transform, SPConstants.TAG_ROOT);
        }

        /**
         * FindRoot
         */

        /// <summary>
        /// Attempts to find a parent with a tag of 'Root', if none is found, self is returned.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static GameObject FindRoot(this GameObject go)
        {
            if (go == null) return null;

            var root = FindParentWithTag(go.transform, SPConstants.TAG_ROOT);
            return (root != null) ? root : go; //we return self if no root was found...
        }

        public static GameObject FindRoot(this Component c)
        {
            if (c == null) return null;

            //return FindRoot(c.gameObject);
            var root = FindParentWithTag(c.transform, SPConstants.TAG_ROOT);
            return (root != null) ? root : c.gameObject;
        }


        /**
         * ReduceToRoot IEnum
         */

        public static IEnumerable<GameObject> ReduceToRoot(this IEnumerable<GameObject> e, bool bdistinct = true)
        {
            if (bdistinct)
            {
                foreach (var obj in ReduceToRoot(e, false).Distinct()) yield return obj;
            }
            else
            {
                foreach (var obj in e)
                {
                    yield return FindRoot(obj);
                }
            }
        }

        public static IEnumerable<GameObject> ReduceToRoot(this IEnumerable<Component> e, bool bdistinct = true)
        {
            if (bdistinct)
            {
                foreach (var obj in ReduceToRoot(e, false).Distinct()) yield return obj;
            }
            else
            {
                foreach (var obj in e)
                {
                    yield return FindRoot(obj);
                }
            }
        }

#endregion

#region RigidBody Parenting

        public static Rigidbody FindRigidbody(this GameObject go, bool stopAtRoot = false)
        {
            if (go == null) return null;

            var rb = go.GetComponent<Rigidbody>();
            if (rb != null) return rb;

            foreach (var p in go.GetParents(stopAtRoot))
            {
                rb = p.GetComponent<Rigidbody>();
                if (rb != null) return rb;
            }

            return null;
        }

        public static Rigidbody FindRigidbody(this Component comp, bool stopAtRoot = false)
        {
            if (comp == null) return null;
            return FindRigidbody(comp.gameObject, stopAtRoot);
        }

#endregion

#region Parenting

        public static IEnumerable<Transform> IterateAllChildren(this Transform trans)
        {
            for(int i = 0; i < trans.childCount; i++)
            {
                yield return trans.GetChild(i);
            }
            
            for(int i = 0; i < trans.childCount; i++)
            {
                foreach (var c in IterateAllChildren(trans.GetChild(i)))
                    yield return c;
            }
        }

        public static Transform[] GetAllChildren(this GameObject go)
        {
            if (go == null) return null;
            return GetAllChildren(go.transform);
        }

        public static Transform[] GetAllChildren(this Component c)
        {
            if (c == null) return null;
            return GetAllChildren(c.transform);
        }

        public static Transform[] GetAllChildren(this Transform t)
        {
            using (var lst = TempCollection.GetList<Transform>())
            {
                GetAllChildren(t, lst);

                return lst.ToArray();
            }
        }

        public static void GetAllChildren(this Transform t, ICollection<Transform> coll)
        {
            if(coll is IList<Transform>)
            {
                GetAllChildren(t, coll as IList<Transform>);
            }
            else
            {
                using (var lst = TempCollection.GetList<Transform>())
                {
                    GetAllChildren(t, lst);
                    var e = lst.GetEnumerator();
                    while (e.MoveNext()) coll.Add(e.Current);
                }
            }
        }

        public static void GetAllChildren(this Transform t, IList<Transform> lst)
        {
            int i = lst.Count;
            
            for(int j = 0; j < t.childCount; j++)
            {
                lst.Add(t.GetChild(j));
            }

            while (i < lst.Count)
            {
                t = lst[i];
                for (int j = 0; j < t.childCount; j++)
                {
                    lst.Add(t.GetChild(j));
                }
                i++;
            }
        }

        public static Transform[] GetAllChildrenAndSelf(this GameObject go)
        {
            if (go == null) return null;
            return GetAllChildrenAndSelf(go.transform);
        }

        public static Transform[] GetAllChildrenAndSelf(this Component c)
        {
            if (c == null) return null;
            return GetAllChildrenAndSelf(c.transform);
        }

        public static Transform[] GetAllChildrenAndSelf(this Transform t)
        {
            using (var lst = TempCollection.GetList<Transform>())
            {
                GetAllChildrenAndSelf(t, lst);
                return lst.ToArray();
            }
        }

        public static void GetAllChildrenAndSelf(this Transform t, ICollection<Transform> coll)
        {
            if (coll is IList<Transform>)
            {
                GetAllChildrenAndSelf(t, coll as IList<Transform>);
            }
            else
            {
                using (var lst = TempCollection.GetList<Transform>())
                {
                    GetAllChildrenAndSelf(t, lst);
                    var e = lst.GetEnumerator();
                    while (e.MoveNext()) coll.Add(e.Current);
                }
            }
        }

        public static void GetAllChildrenAndSelf(this Transform t, IList<Transform> lst)
        {
            int i = lst.Count;
            lst.Add(t);

            while (i < lst.Count)
            {
                t = lst[i];
                for(int j = 0; j < t.childCount; j++)
                {
                    lst.Add(t.GetChild(j));
                }
                i++;
            }
        }

        public static IEnumerable<Transform> GetParents(this GameObject go, bool stopAtRoot = false)
        {
            if (go == null) return null;
            return GetParents(go.transform, stopAtRoot);
        }

        public static IEnumerable<Transform> GetParents(this Component c, bool stopAtRoot = false)
        {
            if (c == null) return null;
            return GetParents(c.transform, stopAtRoot);
        }

        public static IEnumerable<Transform> GetParents(this Transform t, bool stopAtRoot = false)
        {
            if (stopAtRoot)
            {
                if (t.HasTag(SPConstants.TAG_ROOT)) yield break;

                t = t.parent;
                while (t != null)
                {
                    yield return t;
                    if (t.HasTag(SPConstants.TAG_ROOT)) yield break;
                    t = t.parent;
                }
            }
            else
            {
                t = t.parent;
                while (t != null)
                {
                    yield return t;
                    t = t.parent;
                }
            }
        }

        // ##############
        // Is Parent
        // ##########

        public static bool IsParentOf(this GameObject parent, GameObject possibleChild)
        {
            if (parent == null || possibleChild == null) return false;
            return IsParentOf(parent.transform, possibleChild.transform);
        }

        public static bool IsParentOf(this Transform parent, GameObject possibleChild)
        {
            if (parent == null || possibleChild == null) return false;
            return IsParentOf(parent, possibleChild.transform);
        }

        public static bool IsParentOf(this GameObject parent, Transform possibleChild)
        {
            if (parent == null || possibleChild == null) return false;
            return IsParentOf(parent.transform, possibleChild);
        }

        public static bool IsParentOf(this Transform parent, Transform possibleChild)
        {
            if (parent == null || possibleChild == null) return false;
            while (possibleChild != null)
            {
                if (parent == possibleChild.parent) return true;
                possibleChild = possibleChild.parent;
            }
            return false;
        }

        // ##############
        // Add Child
        // ##########

        /// <summary>
        /// Set the parent of some GameObject to this GameObject. The 'OnTransformHierarchyChanged' message will be broadcasted 
        /// to child and all its children signaling it that the change occurred, unless suppressed by the suppress parameter. 
        /// Note that changing the parent of complex hierarchies is expensive regardless of the suppress, if you're calling this 
        /// method frequently, you probably have a design flaw in your game. Reparenting shouldn't occur that frequently.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="child"></param>
        /// <param name="suppressChangeHierarchyMessage"></param>
        public static void AddChild(this GameObject obj, GameObject child)
        {
            var p = (obj != null) ? obj.transform : null;
            var t = (child != null) ? child.transform : null;
            AddChild(p, t);
        }
        
        /// <summary>
        /// Set the parent of some GameObject to this GameObject. The 'OnTransformHierarchyChanged' message will be broadcasted 
        /// to child and all its children signaling it that the change occurred, unless suppressed by the suppress parameter. 
        /// Note that changing the parent of complex hierarchies is expensive regardless of the suppress, if you're calling this 
        /// method frequently, you probably have a design flaw in your game. Reparenting shouldn't occur that frequently.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="child"></param>
        /// <param name="suppressChangeHierarchyMessage">Don't send the OnTransformHierarchyChanged message.</param>
        public static void AddChild(this GameObject obj, Transform child)
        {
            var p = (obj != null) ? obj.transform : null;
            AddChild(p, child);
        }

        /// <summary>
        /// Set the parent of some GameObject to this GameObject. The 'OnTransformHierarchyChanged' message will be broadcasted 
        /// to child and all its children signaling it that the change occurred, unless suppressed by the suppress parameter. 
        /// Note that changing the parent of complex hierarchies is expensive regardless of the suppress, if you're calling this 
        /// method frequently, you probably have a design flaw in your game. Reparenting shouldn't occur that frequently.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="child"></param>
        /// <param name="suppressChangeHierarchyMessage">Don't send the OnTransformHierarchyChanged message.</param>
        public static void AddChild(this Transform obj, GameObject child)
        {
            var t = (child != null) ? child.transform : null;
            AddChild(obj, t);
        }

        /// <summary>
        /// Set the parent of some GameObject to this GameObject. The 'OnTransformHierarchyChanged' message will be broadcasted 
        /// to child and all its children signaling it that the change occurred, unless the new child is already a child of the GameObject. 
        /// Note that changing the parent of complex hierarchies is expensive regardless of the suppress, if you're calling this 
        /// method frequently, you probably have a design flaw in your game. Reparenting shouldn't occur that frequently.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="child"></param>
        /// <param name="suppressChangeHierarchyMessage">Don't send the OnTransformHierarchyChanged message.</param>
        public static void AddChild(this Transform obj, Transform child)
        {
            if (child == null) throw new System.ArgumentNullException("child");

            if (child.parent == obj) return;
            child.parent = obj;
        }

        /// <summary>
        /// Sets the parent property of this GameObject to null. The 'OnTrasformHieararchyChanged' message will be 
        /// broadcasted to it and all of its children signaling this change occurred, unless the parent is already null.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="suppressChangeHierarchyMessage">Don't send the OnTransformHierarchyChanged message.</param>
        public static void RemoveFromParent(this GameObject obj)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");

            var t = obj.transform;
            if (t.parent == null) return;
            t.parent = null;
        }

        /// <summary>
        /// Sets the parent property of this GameObject to null. The 'OnTrasformHieararchyChanged' message will be 
        /// broadcasted to it and all of its children signaling this change occurred, unless the parent is already null.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="suppressChangeHierarchyMessage">Don't send the OnTransformHierarchyChanged message.</param>
        public static void RemoveFromParent(this Transform obj)
        {
            if (obj == null) throw new System.ArgumentNullException("t");

            if (obj.parent == null) return;
            obj.parent = null;
        }

#endregion

#region Transform

        public static void ZeroOut(this GameObject go, bool bIgnoreScale, bool bGlobal = false)
        {
            if (bGlobal)
            {
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.identity;
                if (!bIgnoreScale) go.transform.localScale = Vector3.one;
            }
            else
            {
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                if (!bIgnoreScale) go.transform.localScale = Vector3.one;
            }

            var rb = go.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

            }
        }

        public static void ZeroOut(this Transform trans, bool bIgnoreScale, bool bGlobal = false)
        {
            if (bGlobal)
            {
                trans.position = Vector3.zero;
                trans.rotation = Quaternion.identity;
                if (!bIgnoreScale) trans.localScale = Vector3.one;
            }
            else
            {
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
                if (!bIgnoreScale) trans.localScale = Vector3.one;
            }
        }

        public static void ZeroOut(this Rigidbody body)
        {
            if (body.isKinematic) return;

            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        public static void CopyTransform(GameObject src, GameObject dst, bool bSetScale = false)
        {
            if (src == null) throw new System.ArgumentNullException("src");
            if (dst == null) throw new System.ArgumentNullException("dst");
            CopyTransform(src.transform, dst.transform);
        }

        public static void CopyTransform(Transform src, Transform dst, bool bSetScale = false)
        {
            if (src == null) throw new System.ArgumentNullException("src");
            if (dst == null) throw new System.ArgumentNullException("dst");

            Trans.GetGlobal(src).SetToGlobal(dst, bSetScale);

            foreach (Transform child in dst)
            {
                //match the transform by name
                var srcChild = src.Find(child.name);
                if (srcChild != null) CopyTransform(srcChild, child);
            }
        }

#endregion




        public static bool Equals(this object obj, params object[] any)
        {
            return System.Array.IndexOf(any, obj) >= 0;
        }

    }
}

