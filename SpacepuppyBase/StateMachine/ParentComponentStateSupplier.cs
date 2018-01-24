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
        private bool _isStatic;

        private List<T> _states = new List<T>();
        private bool _clean;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Create a ParentComponentStateSupplier
        /// </summary>
        /// <param name="container"></param>
        /// <param name="includeStatesOnContainer"></param>
        /// <param name="isStatic">Set true if the hierarchy doesn't change.</param>
        public ParentComponentStateSupplier(GameObject container, bool includeStatesOnContainer, bool isStatic)
        {
            if (container == null) throw new System.ArgumentNullException("container");
            _container = container;
            _includeStatesOnContainer = includeStatesOnContainer;
            _isStatic = isStatic;
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

        /// <summary>
        /// Set true if the hierarchy doesn't change.
        /// </summary>
        public bool IsStatic
        {
            get { return _isStatic; }
            set
            {
                if (_isStatic == value) return;
                _isStatic = value;
                if (!_isStatic) this.SetDirty();
            }
        }

        #endregion

        #region Methods

        public void SetDirty()
        {
            _clean = false;
        }

        private void SyncStates()
        {
            if (_isStatic)
            {
                if(!_clean)
                {
                    _states.Clear();
                    if(_container != null) ParentComponentStateSupplier<T>.GetComponentsOnTarg(_container, _states, _includeStatesOnContainer);
                    _clean = true;
                }
            }
            else
            {
                _states.Clear();
                if (_container != null) ParentComponentStateSupplier<T>.GetComponentsOnTarg(_container, _states, _includeStatesOnContainer);
            }
        }

        private IEnumerable<TSub> GetStates<TSub>() where TSub : class, T
        {
            if (_isStatic)
            {
                this.SyncStates();
                return (from s in _states where s is TSub select s as TSub);
            }
            else
            {
                return ParentComponentStateSupplier<T>.GetComponentsOnTarg<TSub>(_container, _includeStatesOnContainer);
            }
        }

        private IEnumerable<T> GetStates(System.Type tp)
        {
            if (_isStatic)
            {
                this.SyncStates();
                return (from s in _states where tp.IsAssignableFrom(s.GetType()) select s);
            }
            else
            {
                return ParentComponentStateSupplier<T>.GetComponentsOnTarg(tp, _container, _includeStatesOnContainer);
            }
        }

        #endregion

        #region ITypedStateSupplier Interface

        public int Count
        {
            get
            {
                this.SyncStates();
                return _states.Count;
            }
        }

        public T GetStateAt(int index)
        {
            if (index < 0) throw new System.IndexOutOfRangeException();

            this.SyncStates();
            if (index < _states.Count) return _states[index];
            else throw new System.IndexOutOfRangeException();
        }

        public bool Contains<TSub>() where TSub : class, T
        {
            //if (_container == null) return false;
            //T comp = this.GetStates<TSub>().FirstOrDefault();
            //return !comp.IsNullOrDestroyed();

            this.SyncStates();
            var e = _states.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current is TSub && !e.Current.IsNullOrDestroyed()) return true;
            }
            return false;
        }

        public bool Contains(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            //if (_container == null) return false;
            //var comp = this.GetStates(tp).FirstOrDefault();
            //return !comp.IsNullOrDestroyed();

            this.SyncStates();
            var e = _states.GetEnumerator();
            while(e.MoveNext())
            {
                if (TypeUtil.IsType(e.Current.GetType(), tp) && !e.Current.IsNullOrDestroyed()) return true;
            }
            return false;
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
            //if (_container == null) return null;
            //TSub comp = this.GetStates<TSub>().FirstOrDefault();
            //if (!comp.IsNullOrDestroyed())
            //    return comp;
            //else
            //    return null;

            this.SyncStates();
            var e = _states.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current is TSub && !e.Current.IsNullOrDestroyed()) return e.Current as TSub;
            }
            return null;
        }

        public T GetState(System.Type tp)
        {
            if (tp == null) return null;

            //if (_container == null) return null;
            //T comp = this.GetStates(tp).FirstOrDefault();
            //if (!comp.IsNullOrDestroyed())
            //    return comp;
            //else
            //    return null;

            this.SyncStates();
            var e = _states.GetEnumerator();
            while (e.MoveNext())
            {
                if (TypeUtil.IsType(e.Current.GetType(), tp) && !e.Current.IsNullOrDestroyed()) return e.Current;
            }
            return null;
        }

        public T GetNext(T current)
        {
            //if (_container == null) return null;
            //return this.GetValueAfterOrDefault(current, true);

            this.SyncStates();
            return _states.GetValueAfterOrDefault(current, true);
        }

        public void GetStates(ICollection<T> coll)
        {
            GetComponentsOnTarg(_container, coll, _includeStatesOnContainer);
        }

        public void Foreach(System.Action<T> callback)
        {
            if (callback == null) return;
            if (_container == null) return;
            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<T>())
            {
                _container.GetComponents<T>(lst);
                var e = lst.GetEnumerator();
                while (e.MoveNext())
                {
                    callback(e.Current);
                }
            }
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<T> GetEnumerator()
        {
            if (_container == null) return System.Linq.Enumerable.Empty<T>().GetEnumerator();
            return (GetComponentsOnTarg(_container, _includeStatesOnContainer) as IEnumerable<T>).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_container == null) return System.Linq.Enumerable.Empty<T>().GetEnumerator();
            return GetComponentsOnTarg(_container, _includeStatesOnContainer).GetEnumerator();
        }

        #endregion

        #region Static Interface

        public static T[] GetComponentsOnTarg(GameObject container, bool includeComponentsOnContainer)
        {
            using (var set = com.spacepuppy.Collections.TempCollection.GetSet<T>())
            {
                GetComponentsOnTarg(container, set, includeComponentsOnContainer);
                return set.ToArray();
            }
        }

        public static void GetComponentsOnTarg(GameObject container, ICollection<T> coll, bool includeComponentsOnContainer)
        {
            if (includeComponentsOnContainer)
            {
                ComponentUtil.GetComponentsAlt<T>(container, coll);
            }

            for (int i = 0; i < container.transform.childCount; i++)
            {
                ComponentUtil.GetComponentsAlt<T>(container.transform.GetChild(i), coll);
            }
        }

        public static TSub[] GetComponentsOnTarg<TSub>(GameObject container, bool includeComponentsOnContainer) where TSub : class, T
        {
            using (var set = com.spacepuppy.Collections.TempCollection.GetSet<TSub>())
            {
                if (includeComponentsOnContainer)
                {
                    ComponentUtil.GetComponentsAlt<TSub>(container, set);
                }

                for (int i = 0; i < container.transform.childCount; i++)
                {
                    ComponentUtil.GetComponentsAlt<TSub>(container.transform.GetChild(i), set);
                }

                return set.ToArray();
            }
        }

        public static T[] GetComponentsOnTarg(System.Type tp, GameObject container, bool includeComponentsOnContainer)
        {
            if (!TypeUtil.IsType(tp, typeof(T))) throw new TypeArgumentMismatchException(tp, typeof(T), "tp");

            using (var set = com.spacepuppy.Collections.TempCollection.GetSet<T>())
            {
                System.Func<Component, T> filter = (c) =>
                {
                    if (object.ReferenceEquals(c, null)) return null;

                    if (tp.IsAssignableFrom(c.GetType())) return c as T;
                    else return null;
                };

                if (includeComponentsOnContainer)
                {
                    ComponentUtil.GetComponents<T>(container, set, filter);
                }

                for (int i = 0; i < container.transform.childCount; i++)
                {
                    ComponentUtil.GetComponents<T>(container.transform.GetChild(i), set, filter);
                }

                return set.ToArray();
            }
        }

        #endregion

    }

}
