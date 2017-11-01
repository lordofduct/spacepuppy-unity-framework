using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{
    public static class Messaging
    {

        #region Standard Execute Methods

        public static void Execute<T>(this GameObject go, System.Action<T> functor, bool includeDisabledComponents = false) where T : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<T>())
            {
                go.GetComponents<T>(lst);
                if (lst.Count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if(includeDisabledComponents || TargetIsValid(lst[i]))
                            functor(lst[i]);
                    }
                }
            }
        }

        public static void Execute<TInterface, TArg>(this GameObject go, TArg arg, System.Action<TInterface, TArg> functor, bool includeDisabledComponents = false) where TInterface : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<TInterface>())
            {
                go.GetComponents<TInterface>(lst);
                if (lst.Count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (includeDisabledComponents || TargetIsValid(lst[i]))
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
        public static void Broadcast<T>(this GameObject go, System.Action<T> functor, bool includeInactiveObjects = false, bool includeDisabledComponents = false) where T : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<T>())
            {
                go.GetComponentsInChildren<T>(includeInactiveObjects, lst);
                if(lst.Count > 0)
                {
                    for(int i = 0; i < lst.Count; i++)
                    {
                        if (includeDisabledComponents || TargetIsValid(lst[i]))
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
        public static void Broadcast<TInterface, TArg>(this GameObject go, TArg arg, System.Action<TInterface, TArg> functor, bool includeInactiveObjects = false, bool includeDisabledComponents = false) where TInterface : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetList<TInterface>())
            {
                go.GetComponentsInChildren<TInterface>(includeInactiveObjects, lst);
                if (lst.Count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (includeDisabledComponents || TargetIsValid(lst[i]))
                            functor(lst[i], arg);
                    }
                }
            }
        }

        public static void ExecuteUpwards<T>(this GameObject go, System.Action<T> functor, bool includeDisabledComponents = false) where T : class
        {
            var p = go.transform;
            while(p != null)
            {
                Execute<T>(p.gameObject, functor, includeDisabledComponents);
                p = p.parent;
            }
        }

        public static void ExecuteUpwards<TInterface, TArg>(this GameObject go, TArg arg, System.Action<TInterface, TArg> functor, bool includeDisabledComponents = false) where TInterface : class
        {
            var p = go.transform;
            while (p != null)
            {
                Execute<TInterface, TArg>(p.gameObject, arg, functor, includeDisabledComponents);
                p = p.parent;
            }
        }

        #endregion

        #region Global Execute

        /// <summary>
        /// Register a listener for a global Broadcast.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        public static void RegisterGlobal<T>(T listener) where T : class
        {
            if (listener == null) throw new System.ArgumentNullException("listener");
            GlobalMessagePool<T>.Add(listener);
        }

        /// <summary>
        /// Register a listener for a global broadcast.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        public static void UnregisterGlobal<T>(T listener) where T : class
        {
            if(object.ReferenceEquals(listener, null)) throw new System.ArgumentNullException("listener");
            GlobalMessagePool<T>.Remove(listener);
        }

        /// <summary>
        /// Broadcast a message globally to all registered for T. This is faster than FindAndBroadcast, but requires manuall registering/unregistering.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functor"></param>
        /// <param name="includeDisabledComponents"></param>
        public static void Broadcast<T>(System.Action<T> functor) where T : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            GlobalMessagePool<T>.Execute(functor);
        }

        /// <summary>
        /// Broadcast a message globally to all registered for T. This is faster than FindAndBroadcast, but requires manuall registering/unregistering.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="arg"></param>
        /// <param name="functor"></param>
        /// <param name="includeDisabledComponents"></param>
        public static void Broadcast<TInterface, TArg>(TArg arg, System.Action<TInterface, TArg> functor) where TInterface : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            GlobalMessagePool<TInterface>.Execute<TArg>(arg, functor);
        }
        
        /// <summary>
        /// Broadcast a message globally to all that match T. This can be slow, use sparingly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functor"></param>
        public static void FindAndBroadcast<T>(System.Action<T> functor, bool includeDisabledComponents = false) where T : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetSet<T>())
            {
                ObjUtil.FindObjectsOfInterface<T>(lst);
                var e = lst.GetEnumerator();
                while (e.MoveNext())
                {
                    if (includeDisabledComponents || TargetIsValid(e.Current))
                        functor(e.Current);
                }
            }
        }

        /// <summary>
        /// Broadcast a message globally to all that match T. This can be slow, use sparingly.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="arg"></param>
        /// <param name="functor"></param>
        public static void FindAndBroadcast<TInterface, TArg>(TArg arg, System.Action<TInterface, TArg> functor, bool includeDisabledComponents = false) where TInterface : class
        {
            if (functor == null) throw new System.ArgumentNullException("functor");

            using (var lst = TempCollection.GetSet<TInterface>())
            {
                ObjUtil.FindObjectsOfInterface<TInterface>(lst);
                var e = lst.GetEnumerator();
                while (e.MoveNext())
                {
                    if (includeDisabledComponents || TargetIsValid(e.Current))
                        functor(e.Current, arg);
                }
            }
        }

        #endregion




        #region Internal Utils

        private static bool TargetIsValid(object obj)
        {
            if (obj is Behaviour) return (obj as Behaviour).isActiveAndEnabled;
            return true;
        }

        private static class GlobalMessagePool<T> where T : class
        {

            private static HashSet<T> _receivers;
            private static bool _executing;
            private static TempHashSet<T> _toAdd;
            private static TempHashSet<T> _toRemove;

            public static void Add(T listener)
            {
                if (_receivers == null) _receivers = new HashSet<T>();

                if(_executing)
                {
                    if (_toAdd == null) _toAdd = TempCollection.GetSet<T>();
                    _toAdd.Add(listener);
                }
                else
                {
                    _receivers.Add(listener);
                }
            }

            public static void Remove(T listener)
            {
                if (_receivers == null || _receivers.Count == 0) return;

                if (_executing)
                {
                    if (_toRemove == null) _toRemove = TempCollection.GetSet<T>();
                    _toRemove.Add(listener);
                }
                else
                {
                    _receivers.Remove(listener);
                }
            }

            public static void Execute(System.Action<T> functor)
            {
                if (_executing) throw new System.InvalidOperationException("Can not globally broadcast a message currently executing.");
                if (_receivers == null || _receivers.Count == 0) return;

                _executing = true;
                try
                {
                    var e = _receivers.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current is UnityEngine.Object && (e.Current as UnityEngine.Object) == null)
                        {
                            //skip & remove destroyed objects
                            Remove(e.Current);
                        }
                        try
                        {
                            functor(e.Current);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }
                }
                finally
                {
                    _executing = false;

                    if (_toRemove != null)
                    {
                        var e = _toRemove.GetEnumerator();
                        while (e.MoveNext())
                        {
                            _receivers.Remove(e.Current);
                        }
                        _toRemove.Dispose();
                        _toRemove = null;
                    }

                    if (_toAdd != null)
                    {
                        var e = _toAdd.GetEnumerator();
                        while (e.MoveNext())
                        {
                            _receivers.Add(e.Current);
                        }
                        _toAdd.Dispose();
                        _toAdd = null;
                    }
                }
            }

            public static void Execute<TArg>(TArg arg, System.Action<T, TArg> functor)
            {
                if (_executing) throw new System.InvalidOperationException("Can not globally broadcast a message currently executing.");
                if (_receivers == null || _receivers.Count == 0) return;

                _executing = true;
                try
                {
                    var e = _receivers.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current is UnityEngine.Object && (e.Current as UnityEngine.Object) == null)
                        {
                            //skip & remove destroyed objects
                            Remove(e.Current);
                        }
                        try
                        {
                            functor(e.Current, arg);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }
                }
                finally
                {
                    _executing = false;

                    if (_toRemove != null)
                    {
                        var e = _toRemove.GetEnumerator();
                        while (e.MoveNext())
                        {
                            _receivers.Remove(e.Current);
                        }
                        _toRemove.Dispose();
                        _toRemove = null;
                    }

                    if (_toAdd != null)
                    {
                        var e = _toAdd.GetEnumerator();
                        while (e.MoveNext())
                        {
                            _receivers.Add(e.Current);
                        }
                        _toAdd.Dispose();
                        _toAdd = null;
                    }
                }
            }

        }

        #endregion

    }
}
