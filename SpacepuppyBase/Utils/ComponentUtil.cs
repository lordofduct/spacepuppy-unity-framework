using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{
    public static class ComponentUtil
    {

        #region Fields

        private const int MAX_CAPACITY = 256;
        private static List<Component> _recycledList = new List<Component>(MAX_CAPACITY);
        private static void ResetRecycledList()
        {
            _recycledList.Clear();
            if (_recycledList.Capacity > MAX_CAPACITY) _recycledList.Capacity = MAX_CAPACITY;
        }

        #endregion



        public static bool IsComponentSource(object obj)
        {
            return (obj is GameObject || obj is Component);
        }

        public static bool IsEnabled(this Component comp)
        {
            if (comp == null) return false;
            if (comp is Behaviour) return (comp as Behaviour).enabled;
            if (comp is Collider) return (comp as Collider).enabled;
            return true;
        }

        public static bool IsActiveAndEnabled(this Component comp)
        {
            if (comp == null) return false;
            if (!comp.gameObject.activeInHierarchy) return false;
            if (comp is Behaviour) return (comp as Behaviour).enabled;
            if (comp is Collider) return (comp as Collider).enabled;
            return true;
        }

        public static void SetEnabled(this Component comp, bool enabled)
        {
            if (comp == null) return;
            else if (comp is Behaviour) (comp as Behaviour).enabled = enabled;
            else if (comp is Collider) (comp as Collider).enabled = enabled;
        }


        #region HasComponent

        public static bool HasComponent<T>(this GameObject obj, bool testIfEnabled = false) where T : class
        {
            return HasComponent(obj, typeof(T), testIfEnabled);
        }
        public static bool HasComponent<T>(this Component obj, bool testIfEnabled = false) where T : class
        {
            return HasComponent(obj, typeof(T), testIfEnabled);
        }

        public static bool HasComponent(this GameObject obj, System.Type tp, bool testIfEnabled = false)
        {
            if (obj == null) return false;

            if (testIfEnabled)
            {
                foreach (var c in obj.GetComponents(tp))
                {
                    if (c.IsEnabled()) return true;
                }
                return false;
            }
            else
            {
                return (obj.GetComponent(tp) != null);
            }
        }
        public static bool HasComponent(this Component obj, System.Type tp, bool testIfEnabled = false)
        {
            if (obj == null) return false;

            if (testIfEnabled)
            {
                foreach (var c in obj.GetComponents(tp))
                {
                    if (c.IsEnabled()) return true;
                }
                return false;
            }
            else
            {
                return (obj.GetComponent(tp) != null);
            }
        }

        #endregion

        #region AddComponent

        public static T AddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.AddComponent<T>();
        }
        public static Component AddComponent(this Component c, System.Type tp)
        {
            return c.gameObject.AddComponent(tp);
        }

        public static T AddOrGetComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null) return null;

            T comp = obj.GetComponent<T>();
            if (comp == null)
            {
                comp = obj.AddComponent<T>();
            }

            return comp;
        }

        public static T AddOrGetComponent<T>(this Component obj) where T : Component
        {
            if (obj == null) return null;

            T comp = obj.GetComponent<T>();
            if (comp == null)
            {
                comp = obj.gameObject.AddComponent<T>();
            }

            return comp;
        }

        public static Component AddOrGetComponent(this GameObject obj, System.Type tp)
        {
            if (obj == null) return null;
            if (!TypeUtil.IsType(tp, typeof(Component))) return null;

            var comp = obj.GetComponent(tp);
            if (comp == null)
            {
                comp = obj.AddComponent(tp);
            }

            return comp;
        }

        public static Component AddOrGetComponent(this Component obj, System.Type tp)
        {
            if (obj == null) return null;
            if (!TypeUtil.IsType(tp, typeof(Component))) return null;

            var comp = obj.GetComponent(tp);
            if (comp == null)
            {
                comp = obj.gameObject.AddComponent(tp);
            }

            return comp;
        }



        public static T AddOrFindComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null) return null;

            T comp = obj.FindComponent<T>();
            if (comp == null)
            {
                comp = obj.AddComponent<T>();
            }

            return comp;
        }

        public static T AddOrFindComponent<T>(this Component obj) where T : Component
        {
            if (obj == null) return null;

            T comp = obj.FindComponent<T>();
            if (comp == null)
            {
                comp = obj.gameObject.AddComponent<T>();
            }

            return comp;
        }

        public static Component AddOrFindComponent(this GameObject obj, System.Type tp)
        {
            if (obj == null) return null;
            if (!TypeUtil.IsType(tp, typeof(Component))) return null;

            var comp = obj.FindComponent(tp);
            if (comp == null)
            {
                comp = obj.AddComponent(tp);
            }

            return comp;
        }

        public static Component AddOrFindComponent(this Component obj, System.Type tp)
        {
            if (obj == null) return null;
            if (!TypeUtil.IsType(tp, typeof(Component))) return null;

            var comp = obj.FindComponent(tp);
            if (comp == null)
            {
                comp = obj.gameObject.AddComponent(tp);
            }

            return comp;
        }

        #endregion

        #region GetComponent

        public static T GetComponentAlt<T>(this GameObject obj) where T : class
        {
            return obj.GetComponent(typeof(T)) as T;
        }

        public static T GetComponentAlt<T>(this Component obj) where T : class
        {
            return obj.GetComponent(typeof(T)) as T;
        }

        public static bool GetComponent<T>(this GameObject obj, out T comp) where T : class
        {
            comp = obj.GetComponent(typeof(T)) as T;
            return (comp != null);
        }
        public static bool GetComponent<T>(this Component obj, out T comp) where T : class
        {
            comp = obj.GetComponent(typeof(T)) as T;
            return (comp != null);
        }

        public static bool GetComponent(this GameObject obj, System.Type tp, out Component comp)
        {
            comp = obj.GetComponent(tp);
            return (comp != null);
        }
        public static bool GetComponent(this Component obj, System.Type tp, out Component comp)
        {
            comp = obj.GetComponent(tp);
            return (comp != null);
        }

        #endregion

        #region GetComponents

        public static T[] GetComponentsAlt<T>(this GameObject obj) where T : class
        {
            //if (obj == null) return Enumerable.Empty<T>();
            //return obj.GetComponents(typeof(T)).Cast<T>();

            if (obj == null) return ArrayUtil.Empty<T>();

            obj.GetComponents(typeof(T), _recycledList);
            T[] result = new T[_recycledList.Count];
            for (int i = 0; i < _recycledList.Count; i++)
            {
                result[i] = _recycledList[i] as T;
            }
            ResetRecycledList();
            return result;
        }

        public static T[] GetComponentsAlt<T>(this Component obj) where T : class
        {
            if (obj == null) return ArrayUtil.Empty<T>();
            return GetComponentsAlt<T>(obj.gameObject);
        }

        public static void GetComponentsAlt<T>(this GameObject obj, ICollection<T> lst) where T : class
        {
            if (obj == null) return;

            obj.GetComponents(typeof(T), _recycledList);
            var e = _recycledList.GetEnumerator();
            T c = null;
            while(e.MoveNext())
            {
                c = e.Current as T;
                if (c != null) lst.Add(c);
            }
            ResetRecycledList();
        }

        public static void GetComponentsAlt<T>(this Component obj, ICollection<T> lst) where T : class
        {
            if (obj == null) return;

            GetComponentsAlt<T>(obj.gameObject, lst);
        }

        #endregion

        #region ChildComponent

        public static T GetComponentInChildrenAlt<T>(this GameObject obj) where T : class
        {
            if (obj == null) return null;
            return obj.GetComponentInChildren(typeof(T)) as T;
        }

        public static T GetComponentInChildrenAlt<T>(this Component obj) where T : class
        {
            if (obj == null) return null;
            return obj.GetComponentInChildren(typeof(T)) as T;
        }

        #endregion

        #region Child Components

        public static IEnumerable<T> GetChildComponents<T>(this GameObject obj, bool bIncludeSelf = false, bool bIncludeInactive = false) where T : class
        {
            if(bIncludeSelf)
            {
                return obj.GetComponentsInChildren(typeof(T), bIncludeInactive).Cast<T>();
            }
            else
            {
                var temp = TempCollection<T>.GetCollection();
                GetChildComponents<T>(obj, temp, false, bIncludeInactive);
                var result = temp.ToArray();
                temp.Release();
                return result;
            }
        }

        public static IEnumerable<T> GetChildComponents<T>(this Component obj, bool bIncludeSelf = false, bool bIncludeInactive = false) where T : class
        {
            if (obj == null) return Enumerable.Empty<T>();
            return GetChildComponents<T>(obj.gameObject, bIncludeSelf, bIncludeInactive);
        }

        public static void GetChildComponents<T>(this GameObject obj, ICollection<T> coll, bool bIncludeSelf = false, bool bIncludeInactive = false) where T : class
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            if (bIncludeSelf)
            {
                obj.GetComponentsInChildren<Component>(bIncludeInactive, _recycledList);
                var e = _recycledList.GetEnumerator();
                while(e.MoveNext())
                {
                    if (e.Current is T) coll.Add(e.Current as T);
                }
                ResetRecycledList();
            }
            else
            {
                obj.GetComponentsInChildren<Component>(bIncludeInactive, _recycledList);
                var e = _recycledList.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current is T && e.Current.gameObject != obj) coll.Add(e.Current as T);
                }
                ResetRecycledList();
            }
        }

        public static void GetChildComponents<T>(this Component obj, ICollection<T> coll, bool bIncludeSelf = false, bool bIncludeInactive = false) where T : class
        {
            if (obj == null) return;
            GetChildComponents<T>(obj.gameObject, coll, bIncludeSelf, bIncludeInactive);
        }




        public static IEnumerable<Component> GetChildComponents(this GameObject obj, System.Type tp, bool bIncludeSelf = false, bool bIncludeInactive = false)
        {
            if (bIncludeSelf)
            {
                return obj.GetComponentsInChildren(tp, bIncludeInactive);
            }
            else
            {
                var temp = TempCollection<Component>.GetCollection();
                GetChildComponents(obj, tp, temp, false, bIncludeInactive);
                var result = temp.ToArray();
                temp.Release();
                return result;
            }
        }

        public static IEnumerable<Component> GetChildComponents(this Component obj, System.Type tp, bool bIncludeSelf = false, bool bIncludeInactive = false)
        {
            if (obj == null) return Enumerable.Empty<Component>();
            return GetChildComponents(obj.gameObject, tp, bIncludeSelf, bIncludeInactive);
        }

        public static void GetChildComponents(this GameObject obj, System.Type tp, ICollection<Component> coll, bool bIncludeSelf = false, bool bIncludeInactive = false)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            if (bIncludeSelf)
            {
                obj.GetComponentsInChildren<Component>(bIncludeInactive, _recycledList);
                var e = _recycledList.GetEnumerator();
                while (e.MoveNext())
                {
                    if (TypeUtil.IsType(e.Current.GetType(), tp)) coll.Add(e.Current);
                }
                ResetRecycledList();
            }
            else
            {
                obj.GetComponentsInChildren<Component>(bIncludeInactive, _recycledList);
                var e = _recycledList.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.gameObject != obj && TypeUtil.IsType(e.Current.GetType(), tp)) coll.Add(e.Current);
                }
                ResetRecycledList();
            }
        }

        public static void GetChildComponents(this Component obj, System.Type tp, ICollection<Component> coll, bool bIncludeSelf = false, bool bIncludeInactive = false)
        {
            if (obj == null) return;
            GetChildComponents(obj.gameObject, tp, coll, bIncludeSelf, bIncludeInactive);
        }

        #endregion

        #region RemoveComponent

        //public static void RemoveComponent<T>(this GameObject obj) where T : class
        //{
        //    RemoveComponent(obj, typeof(T));
        //}

        //public static void RemoveComponent<T>(this Component obj) where T : class
        //{
        //    RemoveComponent(obj, typeof(T));
        //}

        //public static void RemoveComponent(this GameObject obj, System.Type tp)
        //{
        //    var comp = obj.GetComponent(tp);
        //    if (comp != null)
        //    {
        //        ObjUtil.SmartDestroy(comp);
        //    }
        //}

        //public static void RemoveComponent(this Component obj, System.Type tp)
        //{
        //    var comp = obj.GetComponent(tp);
        //    if (comp != null)
        //    {
        //        ObjUtil.SmartDestroy(comp);
        //    }
        //}

        public static void RemoveComponents<T>(this GameObject obj) where T : class
        {
            RemoveComponents(obj, typeof(T));
        }

        public static void RemoveComponents<T>(this Component obj) where T : class
        {
            RemoveComponents(obj, typeof(T));
        }

        public static void RemoveComponents(this GameObject obj, System.Type tp)
        {
            var arr = obj.GetComponents(tp);
            for (int i = 0; i < arr.Length; i++)
            {
                ObjUtil.SmartDestroy(arr[i]);
            }
        }

        public static void RemoveComponents(this Component obj, System.Type tp)
        {
            var arr = obj.GetComponents(tp);
            for (int i = 0; i < arr.Length; i++)
            {
                ObjUtil.SmartDestroy(arr[i]);
            }
        }

        public static void RemoveFromOwner(this Component comp)
        {
            if (comp != null)
            {
                ObjUtil.SmartDestroy(comp);
            }
        }

        #endregion


        #region EntityHasComponent

        public static bool EntityHasComponent<T>(this GameObject obj, bool testIfEnabled = false) where T : class
        {
            return EntityHasComponent(obj, typeof(T), testIfEnabled);
        }

        public static bool EntityHasComponent<T>(this Component obj, bool testIfEnabled = false) where T : class
        {
            return EntityHasComponent(obj.gameObject, typeof(T), testIfEnabled);
        }

        public static bool EntityHasComponent(this GameObject obj, System.Type tp, bool testIfEnabled = false)
        {
            var root = obj.FindRoot();

            //foreach (var t in root.GetAllChildrenAndSelf())
            //{
            //    if (t.HasComponent(tp, testIfEnabled)) return true;
            //}
            //return false;

            var c = root.GetComponentInChildren(tp);
            if (c == null) return false;
            return (testIfEnabled) ? c.IsEnabled() : true;
        }

        public static bool EntityHasComponent(this Component obj, System.Type tp, bool testIfEnabled = false)
        {
            return EntityHasComponent(obj.gameObject, tp, testIfEnabled);
        }

        #endregion

        #region FindComponent

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T FindComponent<T>(this GameObject go) where T : class
        {
            var root = go.FindRoot();

            var tp = typeof(T);

            //T c = null;
            //foreach (var t in root.GetAllChildrenAndSelf())
            //{
            //    c = t.GetComponent(tp) as T;
            //    if (c != null) return c;
            //}
            //return null;

            return root.GetComponentInChildren(tp) as T;
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static T FindComponent<T>(this Component c) where T : class
        {
            return FindComponent<T>(c.gameObject);
        }

        public static bool FindComponent<T>(this GameObject go, out T comp) where T : class
        {
            comp = FindComponent<T>(go);
            return comp != null;
        }

        public static bool FindComponent<T>(this Component c, out T comp) where T : class
        {
            comp = FindComponent<T>(c.gameObject);
            return comp != null;
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static Component FindComponent(this GameObject go, System.Type tp)
        {
            var root = go.FindRoot();

            //Component c = null;
            //foreach (var t in root.GetAllChildrenAndSelf())
            //{
            //    c = t.GetComponent(tp);
            //    if (c != null) return c;
            //}
            //return null;

            return root.GetComponentInChildren(tp);
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static Component FindComponent(this Component c, System.Type tp)
        {
            return FindComponent(c.gameObject, tp);
        }

        public static bool FindComponent(this GameObject go, System.Type tp, out Component comp)
        {
            comp = FindComponent(go, tp);
            return comp != null;
        }

        public static bool FindComponent(this Component c, System.Type tp, out Component comp)
        {
            comp = FindComponent(c.gameObject, tp);
            return comp != null;
        }

        #endregion

        #region FindComponents

        public static IEnumerable<T> FindComponents<T>(this GameObject go, bool bIncludeInactive = false) where T : class
        {
            var tp = typeof(T);
            var root = go.FindRoot();

            //foreach (var t in root.GetAllChildrenAndSelf())
            //{
            //    foreach (var c in t.GetComponents(tp))
            //    {
            //        yield return c as T;
            //    }
            //}

            return root.GetComponentsInChildren(tp, bIncludeInactive).Cast<T>();
        }
        public static IEnumerable<T> FindComponents<T>(this Component c, bool bIncludeInactive = false) where T : class
        {
            return FindComponents<T>(c.gameObject, bIncludeInactive);
        }

        public static IEnumerable<Component> FindComponents(this GameObject go, System.Type tp, bool bIncludeInactive = false)
        {
            var root = go.FindRoot();
            //foreach (var t in root.GetAllChildrenAndSelf())
            //{
            //    foreach (var c in t.GetComponents(tp))
            //    {
            //        yield return c;
            //    }
            //}
            return root.GetComponentsInChildren(tp, bIncludeInactive);
        }
        public static IEnumerable<Component> FindComponents(this Component c, System.Type tp, bool bIncludeInactive = false)
        {
            return FindComponents(c.gameObject, tp, bIncludeInactive);
        }


        public static void FindComponents<T>(this GameObject go, ICollection<T> coll, bool bIncludeInactive = false) where T : class
        {
            var root = go.FindRoot();
            GetChildComponents<T>(root, coll, true, bIncludeInactive);
        }
        public static void FindComponents<T>(this Component c, ICollection<T> coll, bool bIncludeInactive = false) where T : class
        {
            var root = c.FindRoot();
            GetChildComponents<T>(root, coll, true, bIncludeInactive);
        }

        public static void FindComponents(this GameObject go, System.Type tp, ICollection<Component> coll, bool bIncludeInactive = false)
        {
            var root = go.FindRoot();
            GetChildComponents(root, coll, true, bIncludeInactive);
        }
        public static void FindComponents(this Component c, System.Type tp, ICollection<Component> coll, bool bIncludeInactive = false)
        {
            var root = c.FindRoot();
            GetChildComponents(root, coll, true, bIncludeInactive);
        }

        #endregion


        #region GetHierarchyPath

        public static string GetHiearchyPathName(this Transform t)
        {
            var builder = StringUtil.GetTempStringBuilder();
            while(t.parent != null)
            {
                t = t.parent;
                builder.Insert(0, @"\");
                builder.Insert(0, t.name);
            }
            return StringUtil.Release(builder);
        }

        #endregion

    }
}
