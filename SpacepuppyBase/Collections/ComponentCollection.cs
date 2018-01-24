using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// A collection of like objects that only one of can exist. This is similar to Unity's components (behaviors), but in its own framework.
    /// </summary>
    public class ComponentCollection<TBase> : IEnumerable<TBase>
    {

        #region Events

        public event ComponentCollectionChangedEventHandler ComponentAdded;
        public event ComponentCollectionChangedEventHandler ComponentRemoved;

        public delegate void ComponentCollectionChangedEventHandler(object sender, TBase component);

        protected virtual void OnComponentAdded(TBase comp)
        {
            if (ComponentAdded != null) ComponentAdded(this, comp);
        }

        protected virtual void OnComponentRemoved(TBase comp)
        {
            if (ComponentRemoved != null) ComponentRemoved(this, comp);
        }

        #endregion

        #region Fields

        private Dictionary<System.Type, TBase> _coll = new Dictionary<System.Type, TBase>();

        #endregion

        #region CONSTRUCTOR

        public ComponentCollection()
        {

        }

        #endregion

        #region Properties

        public int Count { get { return _coll.Count; } }

        #endregion

        #region Methods

        public void Add<T>(T comp) where T : class, TBase
        {
            if (comp == null) throw new System.ArgumentNullException("comp");
            var tp = typeof(T);

            if (_coll.ContainsKey(tp))
            {
                //remove old component
                var old = _coll[tp];
                _coll.Remove(tp);
                this.OnComponentRemoved(old);
            }

            _coll[tp] = comp;
            this.OnComponentAdded(comp);
        }

        public T Add<T>() where T : class, TBase
        {
            T comp = this.Get<T>();
            if (comp != null) return comp;

            try
            {
                comp = System.Activator.CreateInstance<T>();
            }
            catch (System.Exception ex)
            {
                throw new System.ArgumentException("Failed to create instance of component " + typeof(T).Name, ex);
            }

            var tp = typeof(T);
            if (_coll.ContainsKey(tp))
            {
                //remove old component
                var old = _coll[tp];
                _coll.Remove(tp);
                this.OnComponentRemoved(old);
            }

            _coll[tp] = comp;
            this.OnComponentAdded(comp);
            return comp;
        }

        public TBase Add(System.Type tp)
        {
            if (tp == null || !TypeUtil.IsType(tp, typeof(TBase))) throw new TypeArgumentMismatchException(tp, typeof(TBase), "tp");

            var comp = this.Get(tp);
            if (comp != null) return comp;

            try
            {
                comp = (TBase)System.Activator.CreateInstance(tp);
            }
            catch (System.Exception ex)
            {
                throw new System.ArgumentException("Failed to create instance of component " + tp.Name, ex);
            }

            if (_coll.ContainsKey(tp))
            {
                //remove old component
                var old = _coll[tp];
                _coll.Remove(tp);
                this.OnComponentRemoved(old);
            }

            _coll[tp] = comp;
            this.OnComponentAdded(comp);
            return comp;
        }

        public bool Contains(TBase comp)
        {
            return _coll.Values.Contains(comp);
        }

        public bool Has<T>(bool allowIndirectHit = false) where T : class, TBase
        {
            var tp = typeof(T);

            //direct type hit
            if (_coll.ContainsKey(tp)) return true;

            if (allowIndirectHit)
            {
                //indirect type hit second
                foreach (var obj in _coll.Values)
                {
                    if (obj is T) return true;
                }
            }

            return false;
        }

        public bool Has(System.Type tp, bool allowIndirectHit = false)
        {
            if (tp == null || !TypeUtil.IsType(tp, typeof(TBase))) throw new TypeArgumentMismatchException(tp, typeof(TBase), "tp");

            //direct type hit
            if (_coll.ContainsKey(tp)) return true;

            if (allowIndirectHit)
            {
                //indirect type hit second
                foreach (var obj in _coll.Values)
                {
                    if (TypeUtil.IsType(obj.GetType(), tp)) return true;
                }
            }

            return false;
        }

        public T Get<T>(bool allowIndirectHit = false) where T : class, TBase
        {
            var tp = typeof(T);

            //direct type hit first
            if (_coll.ContainsKey(tp)) return _coll[tp] as T;

            if (allowIndirectHit)
            {
                //indirect type hit second
                foreach (var obj in _coll.Values)
                {
                    if (obj is T) return obj as T;
                }
            }

            return null;
        }

        public TBase Get(System.Type tp, bool allowIndirectHit = false)
        {
            if (tp == null || !TypeUtil.IsType(tp, typeof(TBase))) throw new TypeArgumentMismatchException(tp, typeof(TBase), "tp");

            //direct type hit first
            if (_coll.ContainsKey(tp)) return _coll[tp];

            if (allowIndirectHit)
            {
                //indirect type hit second
                foreach (var obj in _coll.Values)
                {
                    if (TypeUtil.IsType(obj.GetType(), tp)) return obj;
                }
            }

            return default(TBase);
        }

        public bool Remove<T>() where T : class, TBase
        {
            var tp = typeof(T);

            //direct type hit only
            if (_coll.ContainsKey(tp))
            {
                var old = _coll[tp];
                if (_coll.Remove(tp))
                {
                    this.OnComponentRemoved(old);
                    return true;
                }
            }

            return false;
        }

        public bool Remove(System.Type tp)
        {
            if (tp == null || !TypeUtil.IsType(tp, typeof(TBase))) throw new TypeArgumentMismatchException(tp, typeof(TBase), "tp");

            //direct type hit only
            if (_coll.ContainsKey(tp))
            {
                var old = _coll[tp];
                if (_coll.Remove(tp))
                {
                    this.OnComponentRemoved(old);
                    return true;
                }
            }

            return false;
        }

        public bool Remove(TBase comp)
        {
            System.Type hit = null;
            foreach (var pair in _coll)
            {
                if(object.Equals(pair.Value, comp))
                {
                    hit = pair.Key;
                    break;
                }
            }

            if (hit != null)
            {
                if (_coll.Remove(hit))
                {
                    this.OnComponentRemoved(comp);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region IEnumerable Interface

        //TODO - implement propert Enumerator, remember dict.Values allocates mem in mono... ugh
        protected Dictionary<System.Type, TBase>.ValueCollection.Enumerator GetEnumeratorDirect()
        {
            return _coll.Values.GetEnumerator();
        }

        public IEnumerator<TBase> GetEnumerator()
        {
            return _coll.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _coll.Values.GetEnumerator();
        }

        #endregion
    }
}
