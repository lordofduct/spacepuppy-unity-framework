using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.StateMachine
{

    public class ParentComponentStateSupplier<T> : ITypedStateSupplier<T> where T : class
    {

        #region Fields

        private GameObject _container;
        private bool _includeStatesOnContainer;

        #endregion

        #region CONSTRUCTOR

        public ParentComponentStateSupplier(GameObject container, bool includeStatesOnContainer)
        {
            if (container == null) throw new System.ArgumentNullException("container");
            _container = container;
            _includeStatesOnContainer = includeStatesOnContainer;
        }

        #endregion

        #region Properties

        public GameObject Container
        {
            get { return _container; }
        }

        public bool IncludeStatesOnContainer
        {
            get { return _includeStatesOnContainer; }
        }

        #endregion

        #region ITypedStateSupplier Interface

        public bool Contains<TSub>() where TSub : class, T
        {
            if (_container == null) return false;
            T comp = ParentComponentStateSupplier<T>.GetComponentsOnTarg<TSub>(_container, _includeStatesOnContainer).FirstOrDefault();
            return !comp.IsNullOrDestroyed();
        }

        public bool Contains(System.Type tp)
        {
            if (_container == null) return false;
            var comp = ParentComponentStateSupplier<T>.GetComponentsOnTarg(tp, _container, _includeStatesOnContainer).FirstOrDefault();
            return !comp.IsNullOrDestroyed();
        }

        public bool Contains(T state)
        {
            if (_container == null) return false;
            var go = GameObjectUtil.GetGameObjectFromSource(state);
            if (go != null)
            {
                if (_includeStatesOnContainer && _container == go) return true;
                if (_container.transform == go.transform.parent) return true;
            }

            return false;
        }

        public TSub GetState<TSub>() where TSub : class, T
        {
            if (_container == null) return null;
            TSub comp = ParentComponentStateSupplier<T>.GetComponentsOnTarg<TSub>(_container, _includeStatesOnContainer).FirstOrDefault();
            if (!comp.IsNullOrDestroyed())
                return comp;
            else
                return null;
        }

        public T GetState(System.Type tp)
        {
            if (_container == null) return null;
            T comp = ParentComponentStateSupplier<T>.GetComponentsOnTarg(tp, _container, _includeStatesOnContainer).FirstOrDefault();
            if (!comp.IsNullOrDestroyed())
                return comp;
            else
                return null;
        }

        public T GetNext(T current)
        {
            if (_container == null) return null;
            return this.GetValueAfter(current, true);
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<T> GetEnumerator()
        {
            if (_container == null) return System.Linq.Enumerable.Empty<T>().GetEnumerator();
            return ParentComponentStateSupplier<T>.GetComponentsOnTarg(_container, _includeStatesOnContainer).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_container == null) return System.Linq.Enumerable.Empty<T>().GetEnumerator();
            return ParentComponentStateSupplier<T>.GetComponentsOnTarg(_container, _includeStatesOnContainer).GetEnumerator();
        }

        #endregion

        #region Static Interface

        public static IEnumerable<T> GetComponentsOnTarg(GameObject container, bool includeComponentsOnContainer)
        {
            if (includeComponentsOnContainer)
            {
                foreach (var s in container.GetLikeComponents<T>())
                {
                    yield return s;
                }
            }

            foreach (Transform t in container.transform)
            {
                foreach (var s in t.GetLikeComponents<T>())
                {
                    yield return s;
                }
            }
        }

        public static IEnumerable<TSub> GetComponentsOnTarg<TSub>(GameObject container, bool includeComponentsOnContainer) where TSub : class, T
        {
            if (includeComponentsOnContainer)
            {
                foreach (var s in container.GetLikeComponents<TSub>())
                {
                    yield return s;
                }
            }

            foreach (Transform t in container.transform)
            {
                foreach (var s in t.GetLikeComponents<TSub>())
                {
                    yield return s;
                }
            }
        }

        public static IEnumerable<T> GetComponentsOnTarg(System.Type tp, GameObject container, bool includeComponentsOnContainer)
        {
            if (!TypeUtil.IsType(tp, typeof(T))) throw new TypeArgumentMismatchException(tp, typeof(T), "tp");

            if (includeComponentsOnContainer)
            {
                foreach (var s in container.GetLikeComponents(tp))
                {
                    yield return s as T;
                }
            }

            foreach (Transform t in container.transform)
            {
                foreach (var s in t.GetLikeComponents(tp))
                {
                    yield return s as T;
                }
            }
        }

        #endregion

    }

}
