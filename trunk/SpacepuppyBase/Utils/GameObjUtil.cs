using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

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

        public static bool IsGameObjectSource(object obj)
        {
            return (obj is GameObject || obj is Component || obj is IGameObjectSource);
        }

        public static GameObject GetGameObjectFromSource(object obj)
        {
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

        #region Kill Extension Methods

        /// <summary>
        /// Tests if the object is either destroyed or killed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsKilled(this GameObject obj)
        {
            if (obj.IsNullOrDestroyed()) return true;

            var comp = obj.FindRoot().GetComponent<KillableEntityProxy>();
            if (!comp.IsNullOrDestroyed())
            {
                return comp.IsDead;
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
            if (obj.IsNullOrDestroyed()) return true;

            var comp = obj.FindRoot().GetComponent<KillableEntityProxy>();
            if (!comp.IsNullOrDestroyed())
            {
                return comp.IsDead;
            }

            return false;
        }

        /// <summary>
        /// Destroys the entire entity, if the entity contains a KillableEntity component that will handle death first and foremost.
        /// </summary>
        /// <param name="obj"></param>
        public static void Kill(this GameObject obj)
        {
            if (obj.IsNullOrDestroyed()) return;

            var root = obj.FindRoot();
            var comp = root.GetComponent<KillableEntityProxy>();
            if (comp != null)
            {
                comp.Kill();
                return;
            }
            else
            {
                root.DestroyAll();
            }
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

            foreach (Transform child in go.GetAllChildren())
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

        #region Activation Methods

        /// <summary>
        /// Destroys self and all children
        /// </summary>
        /// <param name="obj"></param>
        public static void DestroyAll(this GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                DestroyAll(child.gameObject);
            }

            Object.Destroy(obj);
        }

        /// <summary>
        /// Sets 'active' property of self and all children to false
        /// </summary>
        /// <param name="obj"></param>
        public static void DeactivateAll(this GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                DeactivateAll(child.gameObject);
            }

            obj.SetActive(false);
        }

        /// <summary>
        /// Sets 'active' property of self and all children to true
        /// </summary>
        /// <param name="obj"></param>
        public static void ActivateAll(this GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                ActivateAll(child.gameObject);
            }

            obj.SetActive(true);
        }

        #endregion

        #region Search/Find

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
            foreach (var child in trans.GetAllChildren())
            {
                if (StringUtil.Equals(child.name, sname, bIgnoreCase)) return child;
            }

            return null;
        }

        #endregion

        #region Tags

        /**
         * Find
         */

        public static IEnumerable<GameObject> FindGameObjectsWithMultiTag(string tag)
        {
            if (tag == SPConstants.TAG_MULTITAG)
            {
                foreach (var go in GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG))
                {
                    yield return go;
                }
            }
            else
            {
                foreach (var go in GameObject.FindGameObjectsWithTag(tag))
                {
                    yield return go;
                }

                MultiTag comp;
                foreach (var go in GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG))
                {
                    if (go.GetComponent<MultiTag>(out comp))
                    {
                        if (comp.ContainsTag(tag)) yield return go;
                    }
                }
            }
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

                MultiTag comp;
                foreach (var go in GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG))
                {
                    if (go.GetComponent<MultiTag>(out comp))
                    {
                        if (comp.ContainsTag(tag)) return go;
                    }
                }
            }

            return null;
        }

        public static GameObject FindWithMultiTag(this GameObject go, string tag)
        {
            if (go.tag == tag) return go;
            if (go.HasComponent<MultiTag>() && go.GetComponent<MultiTag>().ContainsTag(tag)) return go;

            foreach (var child in go.transform.GetAllChildren())
            {
                if (child.tag == tag) return go;
                if (child.HasComponent<MultiTag>() && child.GetComponent<MultiTag>().ContainsTag(tag)) return child.gameObject;
            }

            return null;
        }

        public static IEnumerable<GameObject> FindAllWithMultiTag(this GameObject go, string tag)
        {
            if (go.tag == tag || (go.HasComponent<MultiTag>() && go.GetComponent<MultiTag>().ContainsTag(tag))) yield return go;

            foreach (var child in go.transform.GetAllChildren())
            {
                if (child.tag == tag || (child.HasComponent<MultiTag>() && child.GetComponent<MultiTag>().ContainsTag(tag))) yield return child.gameObject;
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
            if (t.tag == stag) return t.gameObject;
            MultiTag multitag = t.GetComponent<MultiTag>();
            if (multitag != null && multitag.ContainsTag(stag)) return t.gameObject;

            Transform p = t.parent;
            while (p != null)
            {
                if (p.tag == stag) return p.gameObject;

                multitag = p.GetComponent<MultiTag>();
                if (multitag != null && multitag.ContainsTag(stag)) return p.gameObject;

                p = p.parent;
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

            if (go.tag == SPConstants.TAG_ROOT) return true;
            if (go.HasComponent<MultiTag>() && go.GetComponent<MultiTag>().ContainsTag(SPConstants.TAG_ROOT)) return true;

            foreach (var p in go.GetParents())
            {
                if (p.tag == SPConstants.TAG_ROOT) return true;
                if (p.HasComponent<MultiTag>() && p.GetComponent<MultiTag>().ContainsTag(SPConstants.TAG_ROOT)) return true;
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

            if (go.tag == SPConstants.TAG_ROOT)
            {
                root = go;
                return true;
            }
            if (go.HasComponent<MultiTag>() && go.GetComponent<MultiTag>().ContainsTag(SPConstants.TAG_ROOT))
            {
                root = go;
                return true;
            }

            foreach (var p in go.GetParents())
            {
                if (p.tag == SPConstants.TAG_ROOT)
                {
                    root = p.gameObject;
                    return true;
                }
                if (p.HasComponent<MultiTag>() && p.GetComponent<MultiTag>().ContainsTag(SPConstants.TAG_ROOT))
                {
                    root = p.gameObject;
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
            return FindTrueRoot(c.gameObject);
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

            if (go.rigidbody != null) return go.rigidbody;

            foreach (var p in go.GetParents(stopAtRoot))
            {
                if (p.rigidbody != null) return p.rigidbody;
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

        public static IEnumerable<Transform> GetAllChildren(this GameObject go)
        {
            if (go == null) return null;
            return GetAllChildren(go.transform);
        }

        public static IEnumerable<Transform> GetAllChildren(this Component c)
        {
            if (c == null) return null;
            return GetAllChildren(c.transform);
        }

        public static IEnumerable<Transform> GetAllChildren(this Transform t)
        {
            //we do the first children first
            foreach (Transform trans in t.transform)
            {
                yield return trans;
            }

            //then grandchildren
            foreach (Transform trans in t.transform)
            {
                foreach (var child in GetAllChildren(trans))
                {
                    yield return child;
                }
            }
        }

        public static IEnumerable<Transform> GetAllChildrenAndSelf(this GameObject go)
        {
            if (go == null) return null;
            return GetAllChildrenAndSelf(go.transform);
        }

        public static IEnumerable<Transform> GetAllChildrenAndSelf(this Component c)
        {
            if (c == null) return null;
            return GetAllChildrenAndSelf(c.transform);
        }

        public static IEnumerable<Transform> GetAllChildrenAndSelf(this Transform t)
        {
            yield return t;

            //we do the first children first
            foreach (Transform trans in t.transform)
            {
                yield return trans;
            }

            //then grandchildren
            foreach (Transform trans in t.transform)
            {
                foreach (var child in GetAllChildren(trans))
                {
                    yield return child;
                }
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
        // Contains Child
        // ##########

        public static bool ContainsAsChild(this GameObject obj, GameObject child)
        {
            return ContainsAsChild(obj.transform, child.transform);
        }

        public static bool ContainsAsChild(this GameObject obj, Transform child)
        {
            return ContainsAsChild(obj.transform, child);
        }

        public static bool ContainsAsChild(this Transform obj, GameObject child)
        {
            return ContainsAsChild(obj, child.transform);
        }

        public static bool ContainsAsChild(this Transform obj, Transform child)
        {
            foreach (Transform trans in obj.transform)
            {
                if (trans == child) return true;
            }

            foreach (Transform trans in obj.transform)
            {
                if (ContainsAsChild(trans, child)) return true;
            }

            return false;
        }

        // ##############
        // Is Parent
        // ##########

        public static bool IsParentOf(this GameObject parent, GameObject possibleChild)
        {
            return IsParentOf(parent.transform, possibleChild.transform);
        }

        public static bool IsParentOf(this Transform parent, GameObject possibleChild)
        {
            return IsParentOf(parent, possibleChild.transform);
        }

        public static bool IsParentOf(this GameObject parent, Transform possibleChild)
        {
            return IsParentOf(parent.transform, possibleChild);
        }

        public static bool IsParentOf(this Transform parent, Transform possibleChild)
        {
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

        public static void AddChild(this GameObject obj, GameObject child)
        {
            child.transform.parent = obj.transform;
        }

        public static void AddChild(this GameObject obj, Transform child)
        {
            child.parent = obj.transform;
        }

        public static void AddChild(this Transform obj, GameObject child)
        {
            child.transform.parent = obj;
        }

        public static void AddChild(this Transform obj, Transform child)
        {
            child.parent = obj;
        }

        public static void PlaceInGlobalSpace(this GameObject go, bool bSetScale = false)
        {
            var trans = Trans.GetGlobal(go.transform);
            go.transform.parent = null;
            trans.SetToGlobal(go.transform, bSetScale);
        }

        public static void AddChildAdjusted(this GameObject obj, GameObject child, bool bSetScale = false)
        {
            var trans = Trans.GetGlobal(child.transform);
            child.transform.parent = obj.transform;
            trans.SetToGlobal(child.transform, bSetScale);
        }

        public static void AddChildAdjusted(this GameObject obj, Transform child, bool bSetScale = false)
        {
            var trans = Trans.GetGlobal(child.transform);
            child.parent = obj.transform;
            trans.SetToGlobal(child.transform, bSetScale);
        }

        public static void AddChildAdjusted(this Transform obj, GameObject child, bool bSetScale = false)
        {
            var trans = Trans.GetGlobal(child.transform);
            child.transform.parent = obj;
            trans.SetToGlobal(child.transform, bSetScale);
        }

        public static void AddChildAdjusted(this Transform obj, Transform child, bool bSetScale = false)
        {
            var trans = Trans.GetGlobal(child.transform);
            child.parent = obj;
            trans.SetToGlobal(child.transform, bSetScale);
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

            if (go.rigidbody != null && !go.rigidbody.isKinematic)
            {
                go.rigidbody.velocity = Vector3.zero;
                go.rigidbody.angularVelocity = Vector3.zero;

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

        public static Vector3 ParentTransformPoint(this Transform t, Vector3 pnt)
        {
            if (t.parent == null) return pnt;
            return t.parent.TransformPoint(pnt);
        }

        public static Vector3 ParentInverseTransformPoint(this Transform t, Vector3 pnt)
        {
            if (t.parent == null) return pnt;
            return t.parent.InverseTransformPoint(pnt);
        }

        #endregion




        public static bool Equals(this object obj, params object[] any)
        {
            return System.Array.IndexOf(any, obj) >= 0;
        }

    }
}

