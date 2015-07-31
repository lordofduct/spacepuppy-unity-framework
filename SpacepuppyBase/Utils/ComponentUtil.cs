using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Utils
{
    public static class ComponentUtil
    {

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

        public static bool HasComponent<T>(this GameObject obj, bool testIfEnabled = false) where T : Component
        {
            if (obj == null) return false;
            if (testIfEnabled)
            {
                foreach (var c in obj.GetComponents<T>())
                {
                    if (c.IsEnabled()) return true;
                }
                return false;
            }
            else
            {
                return (obj.GetComponent<T>() != null);
            }
        }
        public static bool HasComponent<T>(this Component obj, bool testIfEnabled = false) where T : Component
        {
            if (obj == null) return false;
            if (testIfEnabled)
            {
                foreach (var c in obj.GetComponents<T>())
                {
                    if (c.IsEnabled()) return true;
                }
                return false;
            }
            else
            {
                return (obj.GetComponent<T>() != null);
            }
        }

        public static bool HasComponent(this GameObject obj, string tp, bool testIfEnabled = false)
        {
            if (obj == null) return false;
            if (testIfEnabled)
            {
                var c = obj.GetComponent(tp);
                if (c == null) return false;
                return c.IsEnabled();
            }
            else
            {
                return (obj.GetComponent(tp) != null);
            }
        }
        public static bool HasComponent(this Component obj, string tp, bool testIfEnabled = false)
        {
            if (obj == null) return false;
            if (testIfEnabled)
            {
                var c = obj.GetComponent(tp);
                if (c == null) return false;
                return c.IsEnabled();
            }
            else
            {
                return (obj.GetComponent(tp) != null);
            }
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

        public static bool HasLikeComponent<T>(this GameObject obj, bool testIfEnabled = false)
        {
            foreach (var comp in obj.GetComponents<Component>())
            {
                if (comp is T && (!testIfEnabled || comp.IsEnabled())) return true;
            }

            return false;
        }
        public static bool HasLikeComponent<T>(this Component obj, bool testIfEnabled = false)
        {
            foreach (var comp in obj.GetComponents<Component>())
            {
                if (comp is T && (!testIfEnabled || comp.IsEnabled())) return true;
            }

            return false;
        }

        public static bool HasLikeComponent(this GameObject obj, System.Type tp, bool testIfEnabled = false)
        {
            foreach (var comp in obj.GetComponents<Component>())
            {
                if (TypeUtil.IsType(comp.GetType(), tp) && (!testIfEnabled || comp.IsEnabled())) return true;
            }

            return false;
        }
        public static bool HasLikeComponent(this Component obj, System.Type tp, bool testIfEnabled = false)
        {
            foreach (var comp in obj.GetComponents<Component>())
            {
                if (TypeUtil.IsType(comp.GetType(), tp) && (!testIfEnabled || comp.IsEnabled())) return true;
            }

            return false;
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
        public static Component AddComponent(this Component c, string tp)
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
            if (!TypeUtil.IsType(tp, typeof(ComponentUtil))) return null;

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
            if (!TypeUtil.IsType(tp, typeof(ComponentUtil))) return null;

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
            if (!TypeUtil.IsType(tp, typeof(ComponentUtil))) return null;

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
            if (!TypeUtil.IsType(tp, typeof(ComponentUtil))) return null;

            var comp = obj.FindComponent(tp);
            if (comp == null)
            {
                comp = obj.gameObject.AddComponent(tp);
            }

            return comp;
        }

        #endregion

        #region GetComponent

        // ############
        // GetComponent
        // #########

        public static bool GetComponent<T>(this GameObject obj, out T comp) where T : Component
        {
            comp = obj.GetComponent<T>();
            return (comp != null);
        }
        public static bool GetComponent<T>(this Component obj, out T comp) where T : Component
        {
            comp = obj.GetComponent<T>();
            return (comp != null);
        }

        public static bool GetComponent(this GameObject obj, string tp, out Component comp)
        {
            comp = obj.GetComponent(tp);
            return (comp != null);
        }
        public static bool GetComponent(this Component obj, string tp, out Component comp)
        {
            comp = obj.GetComponent(tp);
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

        // ############
        // GetFirstComponent
        // #########

        public static T GetFirstComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null) return default(T);

            var arr = obj.GetComponents<T>();
            if (arr != null && arr.Length > 0)
                return arr[0];
            else
                return default(T);
        }

        public static T GetFirstComponent<T>(this Component obj) where T : Component
        {
            if (obj == null) return default(T);

            var arr = obj.GetComponents<T>();
            if (arr != null && arr.Length > 0)
                return arr[0];
            else
                return default(T);
        }

        public static Component GetFirstComponent(this GameObject obj, System.Type tp)
        {
            if (obj == null) return null;

            var arr = obj.GetComponents(tp);
            if (arr != null && arr.Length > 0)
                return arr[0];
            else
                return null;
        }

        public static Component GetFirstComponent(this Component obj, System.Type tp)
        {
            if (obj == null) return null;

            var arr = obj.GetComponents(tp);
            if (arr != null && arr.Length > 0)
                return arr[0];
            else
                return null;
        }

        public static T GetFirstLikeComponent<T>(this GameObject obj)
        {
            if (obj == null) return default(T);

            foreach (object comp in obj.GetComponents<Component>())
            {
                if (comp is T) return (T)comp;
            }

            return default(T);
        }

        public static T GetFirstLikeComponent<T>(this Component obj)
        {
            if (obj == null) return default(T);

            foreach (object comp in obj.GetComponents<Component>())
            {
                if (comp is T) return (T)comp;
            }

            return default(T);
        }

        public static Component GetFirstLikeComponent(this GameObject obj, System.Type tp)
        {
            if (obj == null) return null;

            foreach (Component comp in obj.GetComponents<Component>())
            {
                if (TypeUtil.IsType(comp.GetType(), tp)) return comp;
            }

            return null;
        }

        public static Component GetFirstLikeComponent(this Component obj, System.Type tp)
        {
            if (obj == null) return null;

            foreach (Component comp in obj.GetComponents<Component>())
            {
                if (TypeUtil.IsType(comp.GetType(), tp)) return comp;
            }

            return null;
        }

        public static bool GetFirstLikeComponent<T>(this GameObject obj, out T comp)
        {
            comp = default(T);
            if (obj == null) return false;

            foreach (object c in obj.GetComponents<Component>())
            {
                if (c is T)
                {
                    comp = (T)c;
                    return true;
                }
            }

            return false;
        }

        public static bool GetFirstLikeComponent<T>(this Component obj, out T comp)
        {
            comp = default(T);
            if (obj == null) return false;

            foreach (object c in obj.GetComponents<Component>())
            {
                if (c is T)
                {
                    comp = (T)c;
                    return true;
                }
            }

            return false;
        }

        public static bool GetFirstLikeComponent(this GameObject obj, System.Type tp, out Component comp)
        {
            comp = null;
            if (obj == null) return false;

            foreach (Component possibleComp in obj.GetComponents<Component>())
            {
                if (TypeUtil.IsType(possibleComp.GetType(), tp))
                {
                    comp = possibleComp;
                    return true;
                }
            }

            return false;
        }

        public static bool GetFirstLikeComponent(this Component obj, System.Type tp, out Component comp)
        {
            comp = null;
            if (obj == null) return false;

            foreach (Component possibleComp in obj.GetComponents<Component>())
            {
                if (TypeUtil.IsType(possibleComp.GetType(), tp))
                {
                    comp = possibleComp;
                    return true;
                }
            }

            return false;
        }

        // ############
        // GetComponents
        // #########

        public static IEnumerable<T> GetLikeComponents<T>(this GameObject obj)
        {
            if (obj == null) yield break;

            foreach (var comp in obj.GetComponents<Component>())
            {
                //if (comp is T) yield return comp as T;
                if (comp is T) yield return (T)(comp as object);
            }
        }

        public static IEnumerable<T> GetLikeComponents<T>(this Component obj)
        {
            if (obj == null) yield break;

            foreach (var comp in obj.GetComponents<Component>())
            {
                //if (comp is T) yield return comp as T;
                if (comp is T) yield return (T)(comp as object);
            }
        }

        public static IEnumerable<Component> GetLikeComponents(this GameObject obj, System.Type tp)
        {
            if (obj == null) yield break;

            foreach (var comp in obj.GetComponents<Component>())
            {
                if (TypeUtil.IsType(comp.GetType(), tp)) yield return comp;
            }
        }

        public static IEnumerable<Component> GetLikeComponents(this Component obj, System.Type tp)
        {
            if (obj == null) yield break;

            foreach (var comp in obj.GetComponents<Component>())
            {
                if (TypeUtil.IsType(comp.GetType(), tp)) yield return comp;
            }
        }

        #endregion

        #region RemoveComponent

        public static void RemoveComponent<T>(this GameObject obj) where T : Component
        {
            var comp = obj.GetComponent<T>();
            if (comp != null)
            {
                ObjUtil.SmartDestroy(comp);
            }
        }

        public static void RemoveComponent<T>(this Component obj) where T : Component
        {
            var comp = obj.GetComponent<T>();
            if (comp != null)
            {
                ObjUtil.SmartDestroy(comp);
            }
        }

        public static void RemoveComponent(this GameObject obj, string tp)
        {
            var comp = obj.GetComponent(tp);
            if (comp != null)
            {
                ObjUtil.SmartDestroy(comp);
            }
        }

        public static void RemoveComponent(this Component obj, string tp)
        {
            var comp = obj.GetComponent(tp);
            if (comp != null)
            {
                ObjUtil.SmartDestroy(comp);
            }
        }

        public static void RemoveComponent(this GameObject obj, System.Type tp)
        {
            var comp = obj.GetComponent(tp);
            if (comp != null)
            {
                ObjUtil.SmartDestroy(comp);
            }
        }

        public static void RemoveComponent(this Component obj, System.Type tp)
        {
            var comp = obj.GetComponent(tp);
            if (comp != null)
            {
                ObjUtil.SmartDestroy(comp);
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

        public static bool EntityHasComponent<T>(this GameObject obj, bool testIfEnabled = false) where T : Component
        {
            var root = obj.FindRoot();

            foreach (var t in root.GetAllChildrenAndSelf())
            {
                if (t.HasComponent<T>(testIfEnabled)) return true;
            }

            return false;
        }
        public static bool EntityHasComponent<T>(this Component obj, bool testIfEnabled = false) where T : Component
        {
            return EntityHasComponent<T>(obj.gameObject, testIfEnabled);
        }

        public static bool EntityHasComponent(this GameObject obj, string tp, bool testIfEnabled = false)
        {
            var root = obj.FindRoot();

            foreach (var t in root.GetAllChildrenAndSelf())
            {
                if (t.HasComponent(tp, testIfEnabled)) return true;
            }

            return false;
        }
        public static bool EntityHasComponent(this Component obj, string tp, bool testIfEnabled = false)
        {
            return EntityHasComponent(obj.gameObject, tp, testIfEnabled);
        }

        public static bool EntityHasComponent(this GameObject obj, System.Type tp, bool testIfEnabled = false)
        {
            var root = obj.FindRoot();

            foreach (var t in root.GetAllChildrenAndSelf())
            {
                if (t.HasComponent(tp, testIfEnabled)) return true;
            }

            return false;
        }
        public static bool EntityHasComponent(this Component obj, System.Type tp, bool testIfEnabled = false)
        {
            return EntityHasComponent(obj.gameObject, tp, testIfEnabled);
        }

        public static bool EntityHasLikeComponent<T>(this GameObject obj, bool testIfEnabled = false)
        {
            var root = obj.FindRoot();

            foreach (var t in root.GetAllChildrenAndSelf())
            {
                if (t.HasLikeComponent<T>(testIfEnabled)) return true;
            }

            return false;
        }
        public static bool EntityHasLikeComponent<T>(this Component obj, bool testIfEnabled = false)
        {
            return EntityHasLikeComponent<T>(obj.gameObject, testIfEnabled);
        }

        public static bool EntityHasLikeComponent(this GameObject obj, System.Type tp, bool testIfEnabled = false)
        {
            var root = obj.FindRoot();

            foreach (var t in root.GetAllChildrenAndSelf())
            {
                if (t.HasLikeComponent(tp, testIfEnabled)) return true;
            }

            return false;
        }
        public static bool EntityHasLikeComponent(this Component obj, System.Type tp, bool testIfEnabled = false)
        {
            return EntityHasLikeComponent(obj.gameObject, tp, testIfEnabled);
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
        public static T FindComponent<T>(this GameObject go) where T : Component
        {
            var root = go.FindRoot();

            T c = null;
            foreach (var t in root.GetAllChildrenAndSelf())
            {
                c = t.GetComponent<T>();
                if (c != null) return c;
            }

            return null;
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static T FindComponent<T>(this Component c) where T : Component
        {
            return FindComponent<T>(c.gameObject);
        }

        public static bool FindComponent<T>(this GameObject go, out T comp) where T : Component
        {
            comp = FindComponent<T>(go);
            return comp != null;
        }

        public static bool FindComponent<T>(this Component c, out T comp) where T : Component
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

            Component c = null;
            foreach (var t in root.GetAllChildrenAndSelf())
            {
                c = t.GetComponent(tp);
                if (c != null) return c;
            }

            return null;
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

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static Component FindComponent(this GameObject go, string tp)
        {
            var root = go.FindRoot();

            Component c = null;
            foreach (var t in root.GetAllChildrenAndSelf())
            {
                c = t.GetComponent(tp);
                if (c != null) return c;
            }

            return null;
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static Component FindComponent(this Component c, string tp)
        {
            return FindComponent(c.gameObject, tp);
        }

        public static bool FindComponent(this GameObject go, string tp, out Component comp)
        {
            comp = FindComponent(go, tp);
            return comp != null;
        }

        public static bool FindComponent(this Component c, string tp, out Component comp)
        {
            comp = FindComponent(c.gameObject, tp);
            return comp != null;
        }




        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T FindLikeComponent<T>(this GameObject go)
        {
            var root = go.FindRoot();

            T c = default(T);
            foreach (var t in root.GetAllChildrenAndSelf())
            {
                c = t.GetFirstLikeComponent<T>();
                if (c != null) return c;
            }

            return default(T);
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static T FindLikeComponent<T>(this Component c)
        {
            return FindLikeComponent<T>(c.gameObject);
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static Component FindLikeComponent(this GameObject go, System.Type tp)
        {
            var root = go.FindRoot();

            Component c = null;
            foreach (var t in root.GetAllChildrenAndSelf())
            {
                c = t.GetFirstLikeComponent(tp);
                if (c != null) return c;
            }

            return null;
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static Component FindLikeComponent(this Component c, System.Type tp)
        {
            return FindLikeComponent(c.gameObject, tp);
        }

        #endregion

        #region FindComponents

        public static IEnumerable<T> FindComponents<T>(this GameObject go) where T : Component
        {
            var root = go.FindRoot();
            foreach (var t in root.GetAllChildrenAndSelf())
            {
                foreach (var c in t.GetComponents<T>())
                {
                    yield return c;
                }
            }
            //return root.GetComponentsInChildren<T>();
        }
        public static IEnumerable<T> FindComponents<T>(this Component c) where T : Component
        {
            return FindComponents<T>(c.gameObject);
        }

        public static IEnumerable<Component> FindComponents(this GameObject go, System.Type tp)
        {
            var root = go.FindRoot();
            foreach (var t in root.GetAllChildrenAndSelf())
            {
                foreach (var c in t.GetComponents(tp))
                {
                    yield return c;
                }
            }
            //return root.GetComponentsInChildren(tp);
        }
        public static IEnumerable<Component> FindComponents(this Component c, System.Type tp)
        {
            return FindComponents(c.gameObject, tp);
        }




        public static IEnumerable<T> FindLikeComponents<T>(this GameObject go)
        {
            var root = go.FindRoot();
            foreach (var t in root.GetAllChildrenAndSelf())
            {
                foreach (var c in t.GetLikeComponents<T>())
                {
                    yield return c;
                }
            }
        }
        public static IEnumerable<T> FindLikeComponents<T>(this Component c)
        {
            return FindLikeComponents<T>(c.gameObject);
        }

        public static IEnumerable<Component> FindLikeComponents(this GameObject go, System.Type tp)
        {
            var root = go.FindRoot();
            foreach (var t in root.GetAllChildrenAndSelf())
            {
                foreach (var c in t.GetLikeComponents(tp))
                {
                    yield return c;
                }
            }
        }
        public static IEnumerable<Component> FindLikeComponents(this Component c, System.Type tp)
        {
            return FindLikeComponents(c.gameObject, tp);
        }

        #endregion



        #region Child Components

        public static IEnumerable<T> GetChildComponents<T>(this GameObject obj, bool bIncludeSelf = false) where T : Component
        {
            var e = (bIncludeSelf) ? obj.GetAllChildrenAndSelf() : obj.GetAllChildren();
            foreach (var child in e)
            {
                //var comp = child.GetComponent<T>();
                //if (comp != null) yield return comp;
                var arr = child.GetComponents<T>();
                for(int i = 0; i < arr.Length; i++)
                {
                    yield return arr[i];
                }
            }
        }

        public static IEnumerable<T> GetChildComponents<T>(this Component obj, bool bIncludeSelf = false) where T : Component
        {
            var e = (bIncludeSelf) ? obj.GetAllChildrenAndSelf() : obj.GetAllChildren();
            foreach (var child in e)
            {
                //var comp = child.GetComponent<T>();
                //if (comp != null) yield return comp;
                var arr = child.GetComponents<T>();
                for (int i = 0; i < arr.Length; i++)
                {
                    yield return arr[i];
                }
            }
        }

        public static IEnumerable<Component> GetChildComponents(this GameObject obj, System.Type tp, bool bIncludeSelf = false)
        {
            var e = (bIncludeSelf) ? obj.GetAllChildrenAndSelf() : obj.GetAllChildren();
            foreach (var child in e)
            {
                //var comp = child.GetComponent(tp);
                //if (comp != null) yield return comp;
                var arr = child.GetComponents(tp);
                for (int i = 0; i < arr.Length; i++)
                {
                    yield return arr[i];
                }
            }
        }

        public static IEnumerable<Component> GetChildComponents(this Component obj, System.Type tp, bool bIncludeSelf = false)
        {
            var e = (bIncludeSelf) ? obj.GetAllChildrenAndSelf() : obj.GetAllChildren();
            foreach (var child in e)
            {
                //var comp = child.GetComponent(tp);
                //if (comp != null) yield return comp;
                var arr = child.GetComponents(tp);
                for (int i = 0; i < arr.Length; i++)
                {
                    yield return arr[i];
                }
            }
        }

        public static IEnumerable<T> GetChildLikeComponents<T>(this GameObject obj, bool bIncludeSelf = false)
        {
            var e = (bIncludeSelf) ? obj.GetAllChildrenAndSelf() : obj.GetAllChildren();
            foreach (var child in e)
            {
                foreach (var comp in child.GetLikeComponents<T>())
                {
                    yield return comp;
                }
            }
        }

        public static IEnumerable<T> GetChildLikeComponents<T>(this Component obj, bool bIncludeSelf = false)
        {
            var e = (bIncludeSelf) ? obj.GetAllChildrenAndSelf() : obj.GetAllChildren();
            foreach (var child in e)
            {
                foreach (var comp in child.GetLikeComponents<T>())
                {
                    yield return comp;
                }
            }
        }

        public static IEnumerable<Component> GetChildLikeComponents(this GameObject obj, System.Type tp, bool bIncludeSelf = false)
        {
            var e = (bIncludeSelf) ? obj.GetAllChildrenAndSelf() : obj.GetAllChildren();
            foreach (var child in e)
            {
                foreach (var comp in child.GetLikeComponents(tp))
                {
                    yield return comp;
                }
            }
        }

        public static IEnumerable<Component> GetChildLikeComponents(this Component obj, System.Type tp, bool bIncludeSelf = false)
        {
            var e = (bIncludeSelf) ? obj.GetAllChildrenAndSelf() : obj.GetAllChildren();
            foreach (var child in e)
            {
                foreach (var comp in child.GetLikeComponents(tp))
                {
                    yield return comp;
                }
            }
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
