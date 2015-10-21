using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{
    public class WeakValueDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : class
    {

        #region Fields

        private Dictionary<TKey, WeakReference<TValue>> _dict;
        private KeyCollection _keyColl;
        private ValueCollection _valueColl;

        #endregion

        #region CONSTRUCTOR

        public WeakValueDictionary()
            : this(0, null)
        {

        }

        public WeakValueDictionary(int capacity)
            : this(capacity, null)
        {

        }

        public WeakValueDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {

        }

        public WeakValueDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, WeakReference<TValue>>(capacity, comparer);
            _keyColl = new KeyCollection(this);
            _valueColl = new ValueCollection(this);
        }

        #endregion

        #region Weak Methods

        public void Clean()
        {
            using (var lst = TempCollection.GetList<TKey>())
            {
                var e = _dict.GetEnumerator();
                while(e.MoveNext())
                {
                    if (!e.Current.Value.IsAlive) lst.Add(e.Current.Key);
                }

                var e2 = lst.GetEnumerator();
                while(e2.MoveNext())
                {
                    _dict.Remove(e2.Current);
                }
            }
        }

        #endregion

        #region IDictionary Interface

        //******************
        // Props

        public TValue this[TKey key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException("key");
                try
                {
                    var weakValue = _dict[key];
                    if (weakValue.IsAlive)
                        return weakValue.Target;
                    else
                        return null;
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException();
                }
                catch (Exception ex)
                {
                    throw new KeyNotFoundException("The given key resulted in corrupted information.", ex);
                }
            }
            set
            {
                _dict[key] = WeakReference<TValue>.Create(value);
            }
        }

        public ICollection<TKey> Keys
        {
            get { return _keyColl; }
        }

        public ICollection<TValue> Values
        {
            get { return _valueColl; }
        }

        //*****************
        // Methods

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");
            _dict.Add(key, WeakReference<TValue>.Create(value));
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return _dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dict.ContainsKey(key))
            {
                var weakValue = _dict[key];
                if (weakValue.IsAlive)
                {
                    value = weakValue.Target;
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }
            else
            {
                value = null;
                return false;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var pair in _dict)
            {
                if (pair.Value.IsAlive)
                {
                    yield return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.Target);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _dict.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        public void Clear()
        {
            _dict.Clear();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            foreach (var pair in _dict)
            {
                if (pair.Value.IsAlive)
                {
                    if (_dict.Comparer.Equals(item.Key, pair.Key) && item.Value == pair.Value.Target) return true;
                }
            }
            return false;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var pair in _dict)
            {
                if (pair.Value.IsAlive)
                {
                    var item = new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.Target);
                    array[arrayIndex] = item;
                }
                else
                    array[arrayIndex] = default(KeyValuePair<TKey, TValue>);

                arrayIndex++;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if ((this as ICollection<KeyValuePair<TKey, TValue>>).Contains(item))
            {
                return _dict.Remove(item.Key);
            }
            return false;
        }

        #endregion

        #region Special Types

        private class KeyCollection : ICollection<TKey>
        {
            private WeakValueDictionary<TKey, TValue> _dict;

            public KeyCollection(WeakValueDictionary<TKey, TValue> dict)
            {
                _dict = dict;
            }

            #region ICollection Interface

            public void Add(TKey item)
            {
                throw new NotImplementedException("Mutating a key collection derived from a dictionary is not allowed.");
            }

            public void Clear()
            {
                throw new NotImplementedException("Mutating a key collection derived from a dictionary is not allowed.");
            }

            public bool Contains(TKey item)
            {
                return _dict.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                foreach (var pair in _dict._dict)
                {
                    if (pair.Value.IsAlive)
                        array[arrayIndex] = pair.Key;
                    else
                        array[arrayIndex] = default(TKey);

                    arrayIndex++;
                }
            }

            public int Count
            {
                get { return _dict.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(TKey item)
            {
                throw new NotImplementedException("Mutating a key collection derived from a dictionary is not allowed.");
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                foreach (var pair in _dict)
                {
                    yield return pair.Key;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

        }

        private class ValueCollection : ICollection<TValue>
        {

            private WeakValueDictionary<TKey, TValue> _dict;

            public ValueCollection(WeakValueDictionary<TKey, TValue> dict)
            {
                _dict = dict;
            }

            #region ICollection Interface

            public void Add(TValue item)
            {
                throw new NotImplementedException("Mutating a value collection derived from a dictionary is not allowed.");
            }

            public void Clear()
            {
                throw new NotImplementedException("Mutating a value collection derived from a dictionary is not allowed.");
            }

            public bool Contains(TValue item)
            {
                foreach (var pair in _dict)
                {
                    if (object.Equals(pair.Value, item)) return true;
                }
                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                foreach (var pair in _dict._dict)
                {
                    if (pair.Value.IsAlive)
                        array[arrayIndex] = pair.Value.Target;
                    else
                        array[arrayIndex] = null;

                    arrayIndex++;
                }
            }

            public int Count
            {
                get { return _dict.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(TValue item)
            {
                throw new NotImplementedException("Mutating a value collection derived from a dictionary is not allowed.");
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach (var pair in _dict)
                {
                    yield return pair.Value;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

        }

        #endregion

    }
}
