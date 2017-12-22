using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;
using System;
using System.Collections;

namespace com.spacepuppy.Collections
{

    public interface IMultitonPool<T> : IEnumerable<T> where T : class
    {

        int Count { get; }

        void AddReference(T obj);
        bool RemoveReference(T obj);
        bool Contains(T obj);

        T Find(System.Func<T, bool> predicate);

        T[] FindAll(System.Func<T, bool> predicate);
        int FindAll(ICollection<T> coll, System.Func<T, bool> predicate);
        int FindAll<TSub>(ICollection<TSub> coll, System.Func<TSub, bool> predicate) where TSub : class, T;

        /// <summary>
        /// Use for linq if you want to enumerate a specific subtype.
        /// </summary>
        /// <typeparam name="TSub"></typeparam>
        /// <returns></returns>
        IEnumerable<TSub> Enumerate<TSub>() where TSub : class, T;

    }

    public class MultitonPool<T> : IMultitonPool<T> where T : class
    {

        #region Fields

        private HashSet<T> _pool;
        private bool _querying;
        private System.Action _queryCompleteAction;

        #endregion

        #region CONSTRUCTOR

        public MultitonPool()
        {
            _pool = new HashSet<T>();
        }

        public MultitonPool(IEqualityComparer<T> comparer)
        {
            _pool = new HashSet<T>(comparer);
        }

        #endregion
        
        #region IMultitonPool Interface

        public int Count
        {
            get { return _pool.Count; }
        }

        public void AddReference(T obj)
        {
            if (object.ReferenceEquals(obj, null)) throw new System.ArgumentNullException();

            if (_querying)
            {
                if (!_pool.Contains(obj)) _queryCompleteAction += () => _pool.Add(obj);
            }
            else
            {
                _pool.Add(obj);
            }
        }

        public bool RemoveReference(T obj)
        {
            if (object.ReferenceEquals(obj, null)) throw new System.ArgumentNullException();

            if (_querying)
            {
                if (_pool.Contains(obj))
                {
                    _queryCompleteAction += () => _pool.Remove(obj);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return _pool.Remove(obj);
            }
        }

        public bool Contains(T obj)
        {
            return _pool.Contains(obj);
        }

        public T Find(System.Func<T, bool> predicate)
        {
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    if (e.MoveNext())
                        return e.Current;
                    else
                        return null;
                }

                while (e.MoveNext())
                {
                    if (predicate(e.Current)) return e.Current;
                }
                return null;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public T[] FindAll(System.Func<T, bool> predicate)
        {
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");

            try
            {
                if (predicate == null)
                {
                    _querying = true;

                    T[] arr = new T[_pool.Count];
                    var e = _pool.GetEnumerator();
                    int i = 0;
                    while (e.MoveNext())
                    {
                        arr[i] = e.Current;
                        i++;
                    }
                    return arr;
                }
                else
                {
                    using (var lst = TempCollection.GetList<T>())
                    {
                        FindAll(lst, predicate);
                        return lst.ToArray();
                    }
                }
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public int FindAll(ICollection<T> coll, System.Func<T, bool> predicate)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                int cnt = 0;
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    while (e.MoveNext())
                    {
                        coll.Add(e.Current);
                        cnt++;
                    }
                }
                else
                {
                    while (e.MoveNext())
                    {
                        if (predicate(e.Current))
                        {
                            coll.Add(e.Current);
                            cnt++;
                        }
                    }
                }
                return cnt;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public int FindAll<TSub>(ICollection<TSub> coll, System.Func<TSub, bool> predicate) where TSub : class, T
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                int cnt = 0;
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    while (e.MoveNext())
                    {
                        if (e.Current is TSub)
                        {
                            coll.Add((TSub)e.Current);
                            cnt++;
                        }
                    }
                }
                else
                {
                    while (e.MoveNext())
                    {
                        if (e.Current is TSub && predicate((TSub)e.Current))
                        {
                            coll.Add((TSub)e.Current);
                            cnt++;
                        }
                    }
                }

                return cnt;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public IEnumerable<TSub> Enumerate<TSub>() where TSub : class, T
        {
            var e = _pool.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current is TSub) yield return e.Current as TSub;
            }
        }

        #endregion

        #region IEnumerable Interface

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<T>
        {

            #region Fields

            private HashSet<T>.Enumerator _e;

            #endregion

            #region CONSTRUCTOR

            public Enumerator(MultitonPool<T> multi)
            {
                if (multi == null) throw new System.ArgumentNullException();
                _e = multi._pool.GetEnumerator();
            }

            #endregion

            public T Current
            {
                get
                {
                    return _e.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _e.Current;
                }
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void IEnumerator.Reset()
            {
                (_e as IEnumerator).Reset();
            }
        }

        #endregion

    }

    public class UniqueToGameObjectMultitonPool<T> : IMultitonPool<T> where T : class, IComponent
    {

        #region Fields

        private Dictionary<GameObject, T> _pool;
        private bool _querying;
        private System.Action _queryCompleteAction;

        #endregion

        #region CONSTRUCTOR

        public UniqueToGameObjectMultitonPool()
        {
            _pool = new Dictionary<GameObject, T>(ObjectInstanceIDEqualityComparer<GameObject>.Default);
        }

        #endregion

        #region MultitonPool Methods

        public int Count
        {
            get { return _pool.Count; }
        }

        public void AddReference(T obj)
        {
            if (object.ReferenceEquals(obj, null)) throw new System.ArgumentNullException();

            if (_querying)
            {
                //if (!_pool.Contains(obj)) _queryCompleteAction += () => _pool.Add(obj.gameObject, obj);
                if (!_pool.Contains(obj)) _queryCompleteAction += () => _pool[obj.gameObject] = obj;
            }
            else
            {
                //_pool.Add(obj.gameObject, obj);
                _pool[obj.gameObject] = obj;
            }
        }

        public bool RemoveReference(T obj)
        {
            if (object.ReferenceEquals(obj, null)) throw new System.ArgumentNullException();

            if (_querying)
            {
                if (_pool.Contains(obj))
                {
                    _queryCompleteAction += () => _pool.Remove(obj.gameObject);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return _pool.Remove(obj.gameObject);
            }
        }

        public bool Contains(T obj)
        {
            return _pool.Contains(obj);
        }

        public T Find(System.Func<T, bool> predicate)
        {
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    if (e.MoveNext())
                        return e.Current.Value;
                    else
                        return null;
                }

                while (e.MoveNext())
                {
                    if (predicate(e.Current.Value)) return e.Current.Value;
                }
                return null;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public T[] FindAll(System.Func<T, bool> predicate)
        {
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");

            try
            {
                if (predicate == null)
                {
                    _querying = true;

                    T[] arr = new T[_pool.Count];
                    var e = _pool.GetEnumerator();
                    int i = 0;
                    while (e.MoveNext())
                    {
                        arr[i] = e.Current.Value;
                        i++;
                    }
                    return arr;
                }
                else
                {
                    using (var lst = TempCollection.GetList<T>())
                    {
                        FindAll(lst, predicate);
                        return lst.ToArray();
                    }
                }
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public int FindAll(ICollection<T> coll, System.Func<T, bool> predicate)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                int cnt = 0;
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    while (e.MoveNext())
                    {
                        coll.Add(e.Current.Value);
                        cnt++;
                    }
                }
                else
                {
                    while (e.MoveNext())
                    {
                        if (predicate(e.Current.Value))
                        {
                            coll.Add(e.Current.Value);
                            cnt++;
                        }
                    }
                }
                return cnt;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public int FindAll<TSub>(ICollection<TSub> coll, System.Func<TSub, bool> predicate) where TSub : class, T
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                int cnt = 0;
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    while (e.MoveNext())
                    {
                        if (e.Current.Value is TSub)
                        {
                            coll.Add((TSub)e.Current.Value);
                            cnt++;
                        }
                    }
                }
                else
                {
                    while (e.MoveNext())
                    {
                        if (e.Current.Value is TSub && predicate((TSub)e.Current.Value))
                        {
                            coll.Add((TSub)e.Current.Value);
                            cnt++;
                        }
                    }
                }
                return cnt;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public IEnumerable<TSub> Enumerate<TSub>() where TSub : class, T
        {
            var e = _pool.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value is TSub) yield return e.Current.Value as TSub;
            }
        }

        #endregion

        #region GameObjectMultiton Interface

        public bool TryGet(GameObject go, out T c)
        {
            if (_pool.TryGetValue(go, out c))
            {
                return true;
            }
            else if (go.CompareTag(SPConstants.TAG_MULTITAG))
            {
                c = go.GetComponent<T>();
                return c != null;
            }
            return false;
        }

        #endregion

        #region IEnumerable Interface

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<T>
        {

            #region Fields

            private Dictionary<GameObject, T>.Enumerator _e;

            #endregion

            #region CONSTRUCTOR

            public Enumerator(UniqueToGameObjectMultitonPool<T> multi)
            {
                if (multi == null) throw new System.ArgumentNullException();
                _e = multi._pool.GetEnumerator();
            }

            #endregion

            public T Current
            {
                get
                {
                    return _e.Current.Value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _e.Current.Value;
                }
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void IEnumerator.Reset()
            {
                (_e as IEnumerator).Reset();
            }
        }

        #endregion

    }

    public class UniqueToEntityMultitonPool<T> : IMultitonPool<T> where T : class, IComponent
    {

        #region Fields

        private Dictionary<GameObject, T> _pool;
        private bool _querying;
        private System.Action _queryCompleteAction;

        #endregion

        #region CONSTRUCTOR

        public UniqueToEntityMultitonPool()
        {
            _pool = new Dictionary<GameObject, T>(ObjectInstanceIDEqualityComparer<GameObject>.Default);
        }

        #endregion

        #region Multiton Methods

        public int Count
        {
            get { return _pool.Count; }
        }

        public void AddReference(T obj)
        {
            if (object.ReferenceEquals(obj, null)) throw new System.ArgumentNullException();

            var go = GameObjectUtil.FindRoot(obj.gameObject);
            if(_querying)
            {
                //_queryCompleteAction += () => _pool.Add(go, obj);
                _queryCompleteAction += () => _pool[go] = obj;
            }
            else
            {
                //_pool.Add(go, obj);
                _pool[go] = obj;
            }
        }

        public bool RemoveReference(T obj)
        {
            if (object.ReferenceEquals(obj, null)) throw new System.ArgumentNullException();

            var go = GameObjectUtil.FindRoot(obj.gameObject);
            T cont;
            if(_pool.TryGetValue(go, out cont) && object.ReferenceEquals(cont, obj))
            {
                if(_querying)
                {
                    _queryCompleteAction += () => _pool.Remove(go);
                    return true;
                }
                else
                {
                    return _pool.Remove(go);
                }
            }
            else
            {
                var e = _pool.GetEnumerator();
                while(e.MoveNext())
                {
                    if(object.ReferenceEquals(e.Current.Value, obj))
                    {
                        if(_querying)
                        {
                            go = e.Current.Key;
                            _queryCompleteAction += () => _pool.Remove(go);
                        }
                        else
                        {
                            return _pool.Remove(e.Current.Key);
                        }
                    }
                }
            }

            return false;
        }

        public bool Contains(T obj)
        {
            return _pool.Contains(obj);
        }

        public T Find(System.Func<T, bool> predicate)
        {
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    if (e.MoveNext())
                        return e.Current.Value;
                    else
                        return null;
                }

                while (e.MoveNext())
                {
                    if (predicate(e.Current.Value)) return e.Current.Value;
                }
                return null;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public TSub Find<TSub>(System.Func<TSub, bool> predicate) where TSub : class, T
        {
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    while(e.MoveNext())
                    {
                        if (e.Current.Value is TSub) return e.Current.Value as TSub;
                    }
                }

                while (e.MoveNext())
                {
                    if (e.Current.Value is TSub && predicate(e.Current.Value as TSub)) return e.Current.Value as TSub;
                }
                return null;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public T[] FindAll(System.Func<T, bool> predicate)
        {
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");

            try
            {
                if (predicate == null)
                {
                    _querying = true;

                    T[] arr = new T[_pool.Count];
                    var e = _pool.GetEnumerator();
                    int i = 0;
                    while (e.MoveNext())
                    {
                        arr[i] = e.Current.Value;
                        i++;
                    }
                    return arr;
                }
                else
                {
                    using (var lst = TempCollection.GetList<T>())
                    {
                        FindAll(lst, predicate);
                        return lst.ToArray();
                    }
                }
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public int FindAll(ICollection<T> coll, System.Func<T, bool> predicate)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                int cnt = 0;
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    while (e.MoveNext())
                    {
                        coll.Add(e.Current.Value);
                        cnt++;
                    }
                }
                else
                {
                    while (e.MoveNext())
                    {
                        if (predicate(e.Current.Value))
                        {
                            coll.Add(e.Current.Value);
                            cnt++;
                        }
                    }
                }
                return cnt;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public TSub[] FindAll<TSub>(System.Func<TSub, bool> predicate) where TSub : class, T
        {
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");

            try
            {
                using (var lst = TempCollection.GetList<TSub>())
                {
                    FindAll<TSub>(lst, predicate);
                    return lst.ToArray();
                }
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public int FindAll<TSub>(ICollection<TSub> coll, System.Func<TSub, bool> predicate) where TSub : class, T
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                int cnt = 0;
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    while (e.MoveNext())
                    {
                        if (e.Current.Value is TSub)
                        {
                            coll.Add((TSub)e.Current.Value);
                            cnt++;
                        }
                    }
                }
                else
                {
                    while (e.MoveNext())
                    {
                        if (e.Current.Value is TSub && predicate((TSub)e.Current.Value))
                        {
                            coll.Add((TSub)e.Current.Value);
                            cnt++;
                        }
                    }
                }
                return cnt;
            }
            finally
            {
                if (_queryCompleteAction != null)
                {
                    _queryCompleteAction();
                    _queryCompleteAction = null;
                }
                _querying = false;
            }
        }

        public IEnumerable<TSub> Enumerate<TSub>() where TSub : class, T
        {
            var e = _pool.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value is TSub) yield return e.Current.Value as TSub;
            }
        }

        #endregion

        #region EntityMultiton Methods

        public bool IsSource(object obj)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go == null) return false;

            go = go.FindTrueRoot();
            if (go == null) return false;
            return _pool.Contains(go) || go.HasComponent<T>();
        }

        public bool IsSource<TSub>(object obj) where TSub : class, T
        {
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go == null) return false;

            go = go.FindTrueRoot();
            if (go == null) return false;
            T e;
            return (_pool.TryGetValue(go, out e) && e is TSub) || go.HasComponent<TSub>();
        }

        public T GetFromSource(object obj)
        {
            if (obj is T) return obj as T;

            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return null;

            T e;
            if (_pool.TryGetValue(go, out e)) return e;
            else return go.GetComponent<T>();
        }

        public T GetFromSource(System.Type tp, object obj)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");

            if (obj is T && TypeUtil.IsType(obj.GetType(), tp)) return obj as T;

            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return null;

            T e;
            if (_pool.TryGetValue(go, out e) && TypeUtil.IsType(e.GetType(), tp))
            {
                return e;
            }
            else if (TypeUtil.IsType(tp, typeof(T)))
            {
                return go.GetComponent(tp) as T;
            }

            return null;
        }

        public TSub GetFromSource<TSub>(object obj) where TSub : class, T
        {
            if (obj is TSub) return obj as TSub;

            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return null;

            T e;
            if (_pool.TryGetValue(go, out e) && e is TSub)
            {
                return e as TSub;
            }
            else
            {
                return go.GetComponent<TSub>();
            }
        }

        public bool GetFromSource(object obj, out T comp)
        {
            if(obj is T)
            {
                comp = obj as T;
                return true;
            }

            comp = null;
            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return false;

            T e;
            if (_pool.TryGetValue(go, out e))
            {
                comp = e;
                return true;
            }
            else
            {
                comp = go.GetComponent<T>();
                return comp != null;
            }
        }

        public bool GetFromSource(System.Type tp, object obj, out T comp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");

            if (obj is T && TypeUtil.IsType(obj.GetType(), tp))
            {
                comp = obj as T;
                return true;
            }

            comp = null;
            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return false;

            T e;
            if (_pool.TryGetValue(go, out e) && TypeUtil.IsType(e.GetType(), tp))
            {
                comp = e;
                return true;
            }
            else if (TypeUtil.IsType(tp, typeof(T)))
            {
                comp = go.GetComponent(tp) as T;
                return comp != null;
            }

            return false;
        }

        public bool GetFromSource<TSub>(object obj, out TSub comp) where TSub : class, T
        {
            if(obj is TSub)
            {
                comp = obj as TSub;
                return true;
            }

            comp = null;
            var go = GameObjectUtil.GetTrueRootFromSource(obj);
            if (go == null) return false;

            T e;
            if (_pool.TryGetValue(go, out e) && e is T)
            {
                comp = e as TSub;
                return true;
            }
            else
            {
                comp = go.GetComponent<TSub>();
                return comp != null;
            }
        }

        #endregion

        #region IEnumerable Interface

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<T>
        {

            #region Fields

            private Dictionary<GameObject, T>.Enumerator _e;

            #endregion

            #region CONSTRUCTOR

            public Enumerator(UniqueToEntityMultitonPool<T> multi)
            {
                if (multi == null) throw new System.ArgumentNullException();
                _e = multi._pool.GetEnumerator();
            }

            #endregion

            public T Current
            {
                get
                {
                    return _e.Current.Value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _e.Current.Value;
                }
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void IEnumerator.Reset()
            {
                (_e as IEnumerator).Reset();
            }
        }

        #endregion

    }

}
