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

        #region Properties

        public bool IsQuerying
        {
            get { return _querying; }
        }

        protected System.Action QueryCompleteAction
        {
            get { return _queryCompleteAction; }
            set { _queryCompleteAction = value; }
        }

        #endregion

        #region IMultitonPool Interface

        public int Count
        {
            get { return _pool.Count; }
        }

        public virtual void AddReference(T obj)
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

        public virtual bool RemoveReference(T obj)
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

        public TSub Find<TSub>(System.Func<TSub, bool> predicate) where TSub : class, T
        {
            if (_querying) throw new System.InvalidOperationException("MultitonPool is already in the process of a query.");
            _querying = true;

            try
            {
                var e = _pool.GetEnumerator();
                if (predicate == null)
                {
                    while (e.MoveNext())
                    {
                        if (e.Current is TSub) return e.Current as TSub;
                    }
                }

                while (e.MoveNext())
                {
                    if (e.Current is TSub && predicate(e.Current as TSub)) return e.Current as TSub;
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
            while (e.MoveNext())
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

    public class UniqueToEntityMultitonPool<T> : MultitonPool<T> where T : class
    {

        #region CONSTRUCTOR

        public UniqueToEntityMultitonPool() : base()
        {
        }

        public UniqueToEntityMultitonPool(IEqualityComparer<T> comparer) : base(comparer)
        {
        }

        #endregion

        #region Methods

        public bool IsSource(object obj)
        {
            return GetFromSource(obj) != null;
        }

        public T GetFromSource(object obj)
        {
            if (obj == null) return null;
            if (obj is T) return this.Contains(obj as T) ? obj as T : null;

            var entity = SPEntity.Pool.GetFromSource(obj);
            if (entity == null) return null;
            //if (entity is T) return this.Contains(entity as T) ? entity as T : null;
            if (entity is T) return entity as T;

            var result = entity.GetComponentInChildren<T>();
            return this.Contains(result) ? result : null;
        }

        public bool GetFromSource(object obj, out T comp)
        {
            comp = GetFromSource(obj);
            return comp != null;
        }




        public bool IsSource<TSub>(object obj) where TSub : class, T
        {
            if (obj is TSub) return true;

            return GetFromSource<TSub>(obj) != null;
        }

        public TSub GetFromSource<TSub>(object obj) where TSub : class, T
        {
            if (obj == null) return null;
            if (obj is TSub) return obj as TSub;

            var entity = SPEntity.Pool.GetFromSource(obj);
            if (entity == null) return null;
            //if (entity is TSub) return this.Contains(entity as TSub) ? entity as TSub : null;
            if (entity is TSub) return entity as TSub;

            var result = entity.GetComponentInChildren<TSub>();
            return this.Contains(result) ? result : null;
        }
        
        public bool GetFromSource<TSub>(object obj, out TSub comp) where TSub : class, T
        {
            comp = GetFromSource<TSub>(obj);
            return comp != null;
        }

        #endregion

    }

}
