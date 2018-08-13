using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{
    /// <summary>
    /// A collection that acts like a self-keyed unique-value dictionary. When adding values you add just the value, and its key is generated.
    /// </summary>
    public class KeyedCollection<TKey, TValue> : ICollection<TValue>
    {

        public delegate TKey KeyCalculator(TValue value);

        #region Fields

        private KeyCalculator _calc;
        private DuplicateKeyAddAction _addAction;

        private Dictionary<TKey, TValue> _dict;

        #endregion

        #region CONSTRUCTOR

        public KeyedCollection(KeyCalculator calc)
        {
            if (calc == null) throw new ArgumentNullException("calc");
            _dict = new Dictionary<TKey, TValue>();
            _calc = calc;
            _addAction = DuplicateKeyAddAction.Replace;
        }

        public KeyedCollection(KeyCalculator calc, DuplicateKeyAddAction addAction)
        {
            if (calc == null) throw new ArgumentNullException("calc");
            _dict = new Dictionary<TKey, TValue>();
            _calc = calc;
            _addAction = addAction;
        }

        #endregion

        #region Properties

        public TValue this[TKey key]
        {
            get
            {
                if (_dict.ContainsKey(key))
                    return _dict[key];
                else
                    return default(TValue);
            }
        }

        public DuplicateKeyAddAction AddAction { get { return _addAction; } }

        public ICollection<TKey> Keys { get { return _dict.Keys; } }

        #endregion

        #region Methods

        public TValue[] ToArray()
        {
            return _dict.Values.ToArray();
        }

        public void AddRange(IEnumerable<TValue> collection)
        {
            if (collection == null) return;
            foreach (var v in collection)
            {
                this.Add(v);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        #endregion

        #region ICollection

        public void Add(TValue item)
        {
            if (_dict.Values.Contains(item)) return;

            var key = _calc(item);
            if (_dict.ContainsKey(key))
            {
                switch (_addAction)
                {
                    case DuplicateKeyAddAction.Replace:
                        _dict[key] = item;
                        break;
                    case DuplicateKeyAddAction.Nothing:
                        //do nothing
                        break;
                    case DuplicateKeyAddAction.Exception:
                        throw new DuplicateKeyException();
                }
            }
            else
            {
                _dict.Add(key, item);
            }
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(TValue item)
        {
            return _dict.Values.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _dict.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TValue item)
        {
            var key = _calc(item);
            return _dict.Remove(key);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<TValue>
        {

            private KeyedCollection<TKey, TValue> _coll;
            private Dictionary<TKey, TValue>.Enumerator _e;

            public Enumerator(KeyedCollection<TKey, TValue> coll)
            {
                if (coll == null) throw new System.ArgumentNullException("coll");
                _coll = coll;
                _e = coll._dict.GetEnumerator();
            }

            public TValue Current
            {
                get { return _e.Current.Value; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _e.Current.Value; }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                _e = _coll._dict.GetEnumerator();
            }

            public void Dispose()
            {
                _e.Dispose();
            }
        }

        #endregion

    }
}
