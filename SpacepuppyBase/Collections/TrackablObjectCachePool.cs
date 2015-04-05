using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{
    public class TrackablObjectCachePool<T> : ICachePool<T> where T : class
    {

        #region Fields

        private Stack<T> _inactive = new Stack<T>();
        private List<T> _active = new List<T>();

        private Func<T> _constructorDelegate;
        private Action<T> _resetObjectDelegate;

        #endregion

        #region CONSTRUCTOR

        public TrackablObjectCachePool(int cacheSize)
        {
            this.CacheSize = cacheSize;
            _constructorDelegate = this.SimpleConstructor;
        }

        public TrackablObjectCachePool(int cacheSize, Func<T> constructorDelegate)
        {
            this.CacheSize = cacheSize;
            _constructorDelegate = (constructorDelegate != null) ? constructorDelegate : this.SimpleConstructor;
        }

        public TrackablObjectCachePool(int cacheSize, Func<T> constructorDelegate, Action<T> resetObjectDelegate)
        {
            this.CacheSize = cacheSize;
            _constructorDelegate = (constructorDelegate != null) ? constructorDelegate : this.SimpleConstructor;
            _resetObjectDelegate = resetObjectDelegate;
        }

        private T SimpleConstructor()
        {
            return Activator.CreateInstance<T>();
        }

        #endregion

        #region Properties

        public int CacheSize { get; set; }

        public IEnumerable<T> ActiveMembers { get { return _active; } }

        #endregion

        #region Methods

        public T GetInstance()
        {
            if(_inactive.Count > 0)
            {
                var o = _inactive.Pop();
                _active.Add(o);
                return o;
            }
            else
            {
                var o = _constructorDelegate();
                _active.Add(o);
                return o;
            }
        }

        public void Release(T obj)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");
            if (!_active.Contains(obj)) throw new System.ArgumentException("ObjectCachePool does not consider this object to be an active member.", "obj");

            _active.Remove(obj);
            if(_active.Count + _inactive.Count < this.CacheSize)
            {
                if (_resetObjectDelegate != null) _resetObjectDelegate(obj);
                _inactive.Push(obj);
            }
        }

        public bool Manages(T obj)
        {
            return _active.Contains(obj) || _inactive.Contains(obj);
        }

        #endregion

    }
}
