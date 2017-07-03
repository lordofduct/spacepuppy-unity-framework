using UnityEngine;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{
    public static class Messaging
    {
        
        public static void Execute<T>(GameObject go, System.Action<T> functor) where T : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<T>())
            {
                go.GetComponents<T>(lst);
                if (lst.Count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        functor(lst[i]);
                    }
                }
            }
        }

        public static void Execute<TInterface, TArg>(GameObject go, TArg arg, System.Action<TInterface, TArg> functor) where TInterface : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<TInterface>())
            {
                go.GetComponents<TInterface>(lst);
                if (lst.Count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        functor(lst[i], arg);
                    }
                }
            }
        }

        /// <summary>
        /// Broadcast message to all children of a GameObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="functor"></param>
        /// <param name="includeInactive"></param>
        public static void Broadcast<T>(GameObject go, System.Action<T> functor, bool includeInactive = false) where T : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<T>())
            {
                go.GetComponentsInChildren<T>(includeInactive, lst);
                if(lst.Count > 0)
                {
                    for(int i = 0; i < lst.Count; i++)
                    {
                        functor(lst[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Broadcast message to all children of a GameObject
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="go"></param>
        /// <param name="arg"></param>
        /// <param name="functor"></param>
        /// <param name="includeInactive"></param>
        public static void Broadcast<TInterface, TArg>(GameObject go, TArg arg, System.Action<TInterface, TArg> functor, bool includeInactive = false) where TInterface : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<TInterface>())
            {
                go.GetComponentsInChildren<TInterface>(includeInactive, lst);
                if (lst.Count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        functor(lst[i], arg);
                    }
                }
            }
        }

        /// <summary>
        /// Broadcast a message globally. This can be slow, use sparingly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functor"></param>
        public static void Broadcast<T>(System.Action<T> functor) where T : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<T>())
            {
                ObjUtil.FindObjectsOfInterface<T>(lst);
                var e = lst.GetEnumerator();
                while(e.MoveNext())
                {
                    functor(e.Current);
                }
            }
        }

        /// <summary>
        /// Broadcast a message globally. This can be slow, use sparingly.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="arg"></param>
        /// <param name="functor"></param>
        public static void Broadcast<TInterface, TArg>(TArg arg, System.Action<TInterface, TArg> functor) where TInterface : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<TInterface>())
            {
                ObjUtil.FindObjectsOfInterface<TInterface>(lst);
                var e = lst.GetEnumerator();
                while (e.MoveNext())
                {
                    functor(e.Current, arg);
                }
            }
        }

        public static void ExecuteUpwards<T>(GameObject go, System.Action<T> functor) where T : class
        {
            var p = go.transform;
            while(p != null)
            {
                Execute<T>(p.gameObject, functor);
                p = p.parent;
            }
        }

        public static void ExecuteUpwards<TInterface, TArg>(GameObject go, TArg arg, System.Action<TInterface, TArg> functor) where TInterface : class
        {
            var p = go.transform;
            while (p != null)
            {
                Execute<TInterface, TArg>(p.gameObject, arg, functor);
                p = p.parent;
            }
        }

    }
}
