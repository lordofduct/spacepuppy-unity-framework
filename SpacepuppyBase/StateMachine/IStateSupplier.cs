using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.StateMachine
{

    public interface IStateSupplier<T> : IEnumerable<T> where T : class
    {

        bool Contains(T state);
        T GetNext(T current);

    }

    public interface ITypedStateSupplier<T> : IStateSupplier<T> where T : class
    {

        bool Contains<TSub>() where TSub : class, T;
        bool Contains(System.Type tp);

        TSub GetState<TSub>() where TSub : class, T;
        T GetState(System.Type tp);

    }

    public class StateSupplierCollection<T> : System.Collections.Generic.HashSet<T>, ITypedStateSupplier<T> where T : class
    {

        #region CONSTRUCTOR

        public StateSupplierCollection()
            : base(null as IEqualityComparer<T>)
        {
        }

        public StateSupplierCollection(IEnumerable<T> collection)
            : base(collection, null as IEqualityComparer<T>)
        {
        }

        public StateSupplierCollection(IEqualityComparer<T> comparer)
            : base(comparer)
        {
        }

        public StateSupplierCollection(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : base(collection, comparer)
        {
        }

        #endregion

        #region ITypedStateSupplier Interface

        public bool Contains<TSub>() where TSub : class, T
        {
            if (this.Count == 0) return false;

            var e = this.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current is TSub) return true;
            }
            return false;
        }

        public bool Contains(System.Type tp)
        {
            if (this.Count == 0) return false;

            var e = this.GetEnumerator();
            T obj;
            while (e.MoveNext())
            {
                obj = e.Current;
                if (obj != null && TypeUtil.IsType(obj.GetType(), tp)) return true;
            }
            return false;
        }

        public TSub GetState<TSub>() where TSub : class, T
        {
            if (this.Count == 0) return null;

            var e = this.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current is TSub) return e.Current as TSub;
            }
            return null;
        }

        public T GetState(System.Type tp)
        {
            if (this.Count == 0) return null;

            var e = this.GetEnumerator();
            T obj;
            while (e.MoveNext())
            {
                obj = e.Current;
                if (obj != null && TypeUtil.IsType(obj.GetType(), tp)) return obj;
            }
            return null;
        }

        public T GetNext(T current)
        {
            if (this.Count == 0) return null;

            //NOTE - if current doesn't exist in collection, we return the first entry

            HashSet<T>.Enumerator e;
            if (this.Contains(current))
            {
                e = this.GetEnumerator();
                while (e.MoveNext())
                {
                    if (this.Comparer.Equals(current, e.Current))
                    {
                        if (e.MoveNext())
                            return e.Current;
                        else
                            break;
                    }
                }
            }

            e = this.GetEnumerator();
            e.MoveNext();
            return e.Current;
        }

        #endregion

    }

    public class TypeStateSupplierCollection<T> : com.spacepuppy.Collections.ComponentCollection<T>, ITypedStateSupplier<T> where T : class
    {

        #region Fields

        private bool _allowIndirectHit;

        #endregion

        #region CONSTRUCTOR

        public TypeStateSupplierCollection()
        {

        }

        public TypeStateSupplierCollection(bool allowIndirectHit)
        {
            _allowIndirectHit = allowIndirectHit;
        }

        #endregion

        #region Properties

        public bool AllowIndirectHit { get { return _allowIndirectHit; } }

        #endregion

        #region ITypedStateSupplier Interface

        public bool Contains<TSub>() where TSub : class, T
        {
            return this.Has<TSub>(_allowIndirectHit);
        }

        public bool Contains(System.Type tp)
        {
            return this.Has(tp, _allowIndirectHit);
        }

        public TSub GetState<TSub>() where TSub : class, T
        {
            return this.Get<TSub>(_allowIndirectHit);
        }

        public T GetState(System.Type tp)
        {
            return this.Get(tp, _allowIndirectHit);
        }

        public T GetNext(T current)
        {
            if (this.Count == 0) return null;

            return this.GetValueAfterOrDefault(current, true);
        }

        #endregion

    }

}
