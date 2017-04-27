using UnityEngine;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{
    public static class Messaging
    {

        public static void Execute<T>(GameObject go, System.Action<T> functor)
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

        public static void Execute<TInterface, TArg>(GameObject go, TArg arg, System.Action<TInterface, TArg> functor)
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

        public static void Broadcast<T>(GameObject go, System.Action<T> functor, bool includeInactive = false)
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

        public static void Broadcast<TInterface, TArg>(GameObject go, TArg arg, System.Action<TInterface, TArg> functor, bool includeInactive = false)
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

        public static void ExecuteUpwards<T>(GameObject go, System.Action<T> functor)
        {
            var p = go.transform;
            while(p != null)
            {
                Execute<T>(p.gameObject, functor);
                p = p.parent;
            }
        }

        public static void ExecuteUpwards<TInterface, TArg>(GameObject go, TArg arg, System.Action<TInterface, TArg> functor)
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
