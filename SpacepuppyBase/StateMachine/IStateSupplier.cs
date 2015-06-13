using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.StateMachine
{

    public interface IStateSupplier<T> : IEnumerable<T> where T : class
    {

        bool Contains(T state);

    }

    public interface ITypedStateSupplier<T> : IStateSupplier<T> where T : class
    {

        bool Contains<TSub>() where TSub : class, T;
        bool Contains(System.Type tp);

        TSub GetState<TSub>() where TSub : class, T;
        T GetState(System.Type tp);

    }

    public class StateSupplierCollection<T> : com.spacepuppy.Collections.UniqueList<T>, ITypedStateSupplier<T> where T : class
    {

        #region CONSTRUCTOR
        
        public StateSupplierCollection()
            : base(null as IEqualityComparer<T>)
        {
        }

        public StateSupplierCollection(int capacity)
            : base(capacity, null as IEqualityComparer<T>)
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

        public StateSupplierCollection(int capacity, IEqualityComparer<T> comparer)
            : base(capacity, comparer)
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
            for(int i = 0; i < this.Count; i++)
            {
                if (this[i] is TSub) return true;
            }
            return false;
        }

        public bool Contains(System.Type tp)
        {
            T obj;
            for (int i = 0; i < this.Count; i++)
            {
                obj = this[i];
                if (obj != null && TypeUtil.IsType(obj.GetType(), tp)) return true;
            }
            return false;
        }

        public TSub GetState<TSub>() where TSub : class, T
        {
            T obj;
            for (int i = 0; i < this.Count; i++)
            {
                obj = this[i];
                if (obj is TSub) return obj as TSub;
            }
            return null;
        }

        public T GetState(System.Type tp)
        {
            T obj;
            for (int i = 0; i < this.Count; i++)
            {
                obj = this[i];
                if (obj != null && TypeUtil.IsType(obj.GetType(), tp)) return obj;
            }
            return null;
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

        #endregion

    }

}
