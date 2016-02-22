using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.StateMachine
{

    public class ComponentStateSupplier<T> : ITypedStateSupplier<T> where T : class
    {

        #region Fields

        private GameObject _container;

        #endregion

        #region CONSTRUCTOR

        public ComponentStateSupplier(GameObject container)
        {
            if (container == null) throw new System.ArgumentNullException("container");
            _container = container;
        }

        #endregion

        #region Properties

        public GameObject Container
        {
            get { return _container; }
        }

        #endregion

        #region ITypedStateSupplier Interface

        public int Count
        {
            get
            {
                using (var lst = com.spacepuppy.Collections.TempCollection.GetList<T>())
                {
                    _container.GetComponents<T>(lst);
                    return lst.Count;
                }
            }
        }

        public T GetStateAt(int index)
        {
            if (_container == null) return null;
            if (index < 0) throw new System.IndexOutOfRangeException();

            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<T>())
            {
                _container.GetComponents<T>(lst);
                if (index < lst.Count) return lst[index];
                else throw new System.IndexOutOfRangeException();
            }
        }

        public bool Contains<TSub>() where TSub : class, T
        {
            if (_container == null) return false;
            return _container.HasComponent<TSub>();
        }

        public bool Contains(System.Type tp)
        {
            if (_container == null) return false;
            return _container.HasComponent(tp);
        }

        public bool Contains(T state)
        {
            if (_container == null) return false;
            return _container == GameObjectUtil.GetGameObjectFromSource(state);
        }

        public TSub GetState<TSub>() where TSub : class, T
        {
            if (_container == null) return null;
            return _container.GetComponentAlt<TSub>();
        }

        public T GetState(System.Type tp)
        {
            if (!TypeUtil.IsType(tp, typeof(T))) return default(T);
            if (_container == null) return default(T);
            return _container.GetComponent(tp) as T;
        }

        public T GetNext(T current)
        {
            if (_container == null) return null;
            //return this.GetValueAfterOrDefault(current, true);
            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<T>())
            {
                _container.GetComponents<T>(lst);
                return lst.GetValueAfterOrDefault(current, true);
            }
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<T> GetEnumerator()
        {
            if (_container == null) return Enumerable.Empty<T>().GetEnumerator();
            return (_container.GetComponentsAlt<T>() as IEnumerable<T>).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_container == null) return Enumerable.Empty<T>().GetEnumerator();
            return _container.GetComponentsAlt<T>().GetEnumerator();
        }

        #endregion

        #region Static Interface

        public static IEnumerable<T> GetComponentsOnTarg(GameObject container)
        {
            return container.GetComponentsAlt<T>();
        }

        public static IEnumerable<TSub> GetComponentsOnTarg<TSub>(GameObject container) where TSub : class, T
        {
            return container.GetComponentsAlt<TSub>();
        }

        public static IEnumerable<T> GetComponentsOnTarg(System.Type tp, GameObject container)
        {
            if (!TypeUtil.IsType(tp, typeof(T))) throw new TypeArgumentMismatchException(tp, typeof(T), "tp");

            return container.GetComponents(tp).Cast<T>();
        }

        #endregion

    }

}
