using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{
    public static class ComponentUtil
    {

        #region Fields
        
        private static TempList<Component> _recycledList = new TempList<Component>();

        #endregion

        public static bool IsComponentType(System.Type tp)
        {
            if (tp == null) return false;
            return typeof(Component).IsAssignableFrom(tp) || typeof(IComponent).IsAssignableFrom(tp);
        }

        public static bool IsAcceptableComponentType(System.Type tp)
        {
            if (tp == null) return false;
            return tp.IsInterface || typeof(Component).IsAssignableFrom(tp);
        }

        public static bool IsComponentSource(object obj)
        {
            return (obj is GameObject || obj is Component);
        }

        public static T GetComponentFromSource<T>(object obj) where T : class
        {
            if (obj == null) return null;
            if (obj is T) return obj as T;
            if (obj is IComponent)
            {
                var c = (obj as IComponent).component;
                if (c is T) return c as T;
                else return c.GetComponentAlt<T>();
            }
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go != null)
                return go.GetComponentAlt<T>();

            return null;
        }

        public static Component GetComponentFromSource(System.Type tp, object obj)
        {
            if (obj == null) return null;
            if(TypeUtil.IsType(obj.GetType(), tp))
            {
                if (obj is Component) return obj as Component;
                else if (obj is IComponent) return (obj as IComponent).component;
            }

            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go != null)
                return go.GetComponent(tp);

            return null;
        }

        public static Component GetComponentFromSourceAsComponent<T>(object obj) where T : class
        {
            if (obj == null) return null;
            if(obj is T)
            {
                if (obj is Component) return obj as Component;
                else if (obj is IComponent) return (obj as IComponent).component;
                else return null;
            }

            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go != null)
                return go.GetComponent(typeof(T));

            return null;
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
            if (c == null) return null;
            return c.gameObject.AddComponent<T>();
        }
        public static Component AddComponent(this Component c, System.Type tp)
        {
            if (c == null) return null;
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
            if (obj == null) return null;
            return obj.GetComponent(typeof(T)) as T;
        }

        public static T GetComponentAlt<T>(this Component obj) where T : class
        {
            if (obj == null) return null;
            return obj.GetComponent(typeof(T)) as T;
        }

        public static bool GetComponent<T>(this GameObject obj, out T comp) where T : class
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponent(typeof(T)) as T;
            return (comp != null);
        }
        public static bool GetComponent<T>(this Component obj, out T comp) where T : class
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponent(typeof(T)) as T;
            return (comp != null);
        }

        public static bool GetComponent(this GameObject obj, System.Type tp, out Component comp)
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponent(tp);
            return (comp != null);
        }
        public static bool GetComponent(this Component obj, System.Type tp, out Component comp)
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
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

            using (_recycledList)
            {
                obj.GetComponents(typeof(T), _recycledList);
                T[] result = new T[_recycledList.Count];
                for (int i = 0; i < _recycledList.Count; i++)
                {
                    result[i] = _recycledList[i] as T;
                }
                return result;
            }
        }

        public static T[] GetComponentsAlt<T>(this Component obj) where T : class
        {
            if (obj == null) return ArrayUtil.Empty<T>();
            return GetComponentsAlt<T>(obj.gameObject);
        }

        public static void GetComponentsAlt<T>(this GameObject obj, ICollection<T> lst) where T : class
        {
            if (obj == null) return;

            using (_recycledList)
            {
                obj.GetComponents(typeof(T), _recycledList);
                var e = _recycledList.GetEnumerator();
                T c = null;
                while (e.MoveNext())
                {
                    c = e.Current as T;
                    if (c != null) lst.Add(c);
                }
            }
        }

        public static void GetComponentsAlt<T>(this Component obj, ICollection<T> lst) where T : class
        {
            if (obj == null) return;

            GetComponentsAlt<T>(obj.gameObject, lst);
        }

        public static void GetComponents<T>(this GameObject obj, ICollection<T> lst, System.Func<Component, T> filter)
        {
            if (obj == null) return;
            if (filter == null) return;

            using (_recycledList)
            {
                obj.GetComponents(typeof(Component), _recycledList);
                var e = _recycledList.GetEnumerator();
                T c;
                while (e.MoveNext())
                {
                    c = filter(e.Current);
                    if (c != null) lst.Add(c);
                }
            }
        }

        public static void GetComponents<T>(this Component obj, ICollection<T> lst, System.Func<Component, T> filter)
        {
            if (obj == null) return;

            GetComponents(obj.gameObject, lst, filter);
        }


        public static Component[] GetComponents(this GameObject obj, params System.Type[] types)
        {
            if (obj == null) return ArrayUtil.Empty<Component>();

            using (_recycledList)
            {
                foreach (var tp in types)
                {
                    obj.GetComponents(tp, _recycledList);
                }
                return _recycledList.ToArray();
            }
        }

        public static void GetComponents(this GameObject obj, System.Type[] types, ICollection<Component> lst)
        {
            if (obj == null) return;

            foreach (var tp in types)
            {
                obj.GetComponents(tp, _recycledList);
            }
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
            if (obj == null) return Enumerable.Empty<T>();
            if(bIncludeSelf)
            {
                return obj.GetComponentsInChildren(typeof(T), bIncludeInactive).Cast<T>();
            }
            else
            {
                using (var temp = TempCollection.GetList<T>())
                {
                    GetChildComponents<T>(obj, temp, false, bIncludeInactive);
                    return temp.ToArray();
                }
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
            if (obj == null) return;

            using (_recycledList)
            {
                if (bIncludeSelf)
                {
                    obj.GetComponentsInChildren<Component>(bIncludeInactive, _recycledList);
                    var e = _recycledList.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current is T) coll.Add(e.Current as T);
                    }
                }
                else
                {
                    obj.GetComponentsInChildren<Component>(bIncludeInactive, _recycledList);
                    var e = _recycledList.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current is T && e.Current.gameObject != obj) coll.Add(e.Current as T);
                    }
                }
            }
        }

        public static void GetChildComponents<T>(this Component obj, ICollection<T> coll, bool bIncludeSelf = false, bool bIncludeInactive = false) where T : class
        {
            if (obj == null) return;
            GetChildComponents<T>(obj.gameObject, coll, bIncludeSelf, bIncludeInactive);
        }




        public static IEnumerable<Component> GetChildComponents(this GameObject obj, System.Type tp, bool bIncludeSelf = false, bool bIncludeInactive = false)
        {
            if (obj == null) return Enumerable.Empty<Component>();

            if (bIncludeSelf)
            {
                return obj.GetComponentsInChildren(tp, bIncludeInactive);
            }
            else
            {
                using (var temp = TempCollection.GetList<Component>())
                {
                    GetChildComponents(obj, tp, temp, false, bIncludeInactive);
                    return temp.ToArray();
                }
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
            if (obj == null) return;

            using (_recycledList)
            {
                if (bIncludeSelf)
                {
                    obj.GetComponentsInChildren<Component>(bIncludeInactive, _recycledList);
                    var e = _recycledList.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (TypeUtil.IsType(e.Current.GetType(), tp)) coll.Add(e.Current);
                    }
                }
                else
                {
                    obj.GetComponentsInChildren<Component>(bIncludeInactive, _recycledList);
                    var e = _recycledList.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.gameObject != obj && TypeUtil.IsType(e.Current.GetType(), tp)) coll.Add(e.Current);
                    }
                }
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
            if (obj == null) return;
            var arr = obj.GetComponents(tp);
            for (int i = 0; i < arr.Length; i++)
            {
                ObjUtil.SmartDestroy(arr[i]);
            }
        }

        public static void RemoveComponents(this Component obj, System.Type tp)
        {
            if (obj == null) return;
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
            if (obj == null) return false;
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
            if (go == null) return null;

            var root = go.FindRoot();
            var tp = typeof(T);
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
            if (c == null) return null;
            return FindComponent<T>(c.gameObject);
        }

        public static bool FindComponent<T>(this GameObject go, out T comp) where T : class
        {
            comp = FindComponent<T>(go);
            return comp != null;
        }

        public static bool FindComponent<T>(this Component c, out T comp) where T : class
        {
            if (c == null)
            {
                comp = null;
                return false;
            }
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
            if (go == null) return null;
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
            if (c == null) return null;
            return FindComponent(c.gameObject, tp);
        }

        public static bool FindComponent(this GameObject go, System.Type tp, out Component comp)
        {
            comp = FindComponent(go, tp);
            return comp != null;
        }

        public static bool FindComponent(this Component c, System.Type tp, out Component comp)
        {
            if (c == null)
            {
                comp = null;
                return false;
            }
            comp = FindComponent(c.gameObject, tp);
            return comp != null;
        }

        #endregion

        #region FindComponents

        public static IEnumerable<T> FindComponents<T>(this GameObject go, bool bIncludeInactive = false) where T : class
        {
            if (go == null) return Enumerable.Empty<T>();

            var tp = typeof(T);
            var root = go.FindRoot();
            return root.GetComponentsInChildren(tp, bIncludeInactive).Cast<T>();
        }
        public static IEnumerable<T> FindComponents<T>(this Component c, bool bIncludeInactive = false) where T : class
        {
            if (c == null) return Enumerable.Empty<T>();
            return FindComponents<T>(c.gameObject, bIncludeInactive);
        }

        public static IEnumerable<Component> FindComponents(this GameObject go, System.Type tp, bool bIncludeInactive = false)
        {
            if (go == null) return Enumerable.Empty<Component>();
            return go.FindRoot().GetComponentsInChildren(tp, bIncludeInactive);
        }
        public static IEnumerable<Component> FindComponents(this Component c, System.Type tp, bool bIncludeInactive = false)
        {
            if (c == null) return Enumerable.Empty<Component>();
            return FindComponents(c.gameObject, tp, bIncludeInactive);
        }


        public static void FindComponents<T>(this GameObject go, ICollection<T> coll, bool bIncludeInactive = false) where T : class
        {
            if (go == null) return;
            GetChildComponents<T>(go.FindRoot(), coll, true, bIncludeInactive);
        }
        public static void FindComponents<T>(this Component c, ICollection<T> coll, bool bIncludeInactive = false) where T : class
        {
            if (c == null) return;
            GetChildComponents<T>(c.FindRoot(), coll, true, bIncludeInactive);
        }

        public static void FindComponents(this GameObject go, System.Type tp, ICollection<Component> coll, bool bIncludeInactive = false)
        {
            if (go == null) return;
            GetChildComponents(go.FindRoot(), coll, true, bIncludeInactive);
        }
        public static void FindComponents(this Component c, System.Type tp, ICollection<Component> coll, bool bIncludeInactive = false)
        {
            if (c == null) return;
            GetChildComponents(c.FindRoot(), coll, true, bIncludeInactive);
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
