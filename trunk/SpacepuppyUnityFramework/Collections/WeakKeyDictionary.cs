using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{
    public class WeakKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : class
    {

        #region Fields

        private Dictionary<object, TValue> _dict;
        private WeakKeyComparer<TKey> _comparer;
        private KeyCollection _keyColl;
        private ValueCollection _valueColl;

        #endregion

        #region CONSTRUCTOR

        public WeakKeyDictionary()
            : this(0, null)
        {

        }

        public WeakKeyDictionary(int capacity)
            : this(capacity, null)
        {

        }

        public WeakKeyDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {

        }

        public WeakKeyDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _comparer = new WeakKeyComparer<TKey>(comparer);
            _dict = new Dictionary<object, TValue>(capacity, _comparer);
            _keyColl = new KeyCollection(this);
            _valueColl = new ValueCollection(this);
        }

        #endregion

        #region Weak Methods

        public void Clean()
        {
            //List<object> lst;

            //foreach (var pair in _dict)
            //{
            //    var weakKey = pair.Key as WeakReference<TKey>;
            //    if (weakKey == null || !weakKey.IsAlive)
            //    {
            //        if (lst == null) lst = new List<object>();
            //        lst.Add(pair.Key);
            //    }
            //}

            //if (lst != null)
            //{
            //    foreach (var key in lst)
            //    {
            //        _dict.Remove(key);
            //    }
            //}

            var arr = _dict.Where((p) =>
            {
                var weakKey = p.Key as WeakReference<TKey>;
                return (weakKey == null || !weakKey.IsAlive);
            }).Select((p) => p.Key).ToArray();

            foreach (var key in arr)
            {
                _dict.Remove(key);
            }
        }

        #endregion

        #region IDictionary Interface

        // ********************
        // PROPS

        public TValue this[TKey key]
        {
            get
            {
                try
                {
                    return _dict[key];
                }
                catch
                {
                    throw new KeyNotFoundException();
                }
            }
            set
            {
                if (key == null) throw new ArgumentNullException("key");
                var weakKey = new WeakKeyReference<TKey>(key, _comparer);
                _dict[weakKey] = value;
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

        // ********************
        // Methods

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");
            var weakKey = new WeakKeyReference<TKey>(key, _comparer);
            _dict.Add(weakKey, value);
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
            return _dict.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var pair in _dict)
            {
                var weakKey = pair.Key as WeakKeyReference<TKey>;
                if (weakKey != null && weakKey.IsAlive)
                {
                    yield return new KeyValuePair<TKey, TValue>(weakKey.Target, pair.Value);
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
            get { throw new NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<TKey,TValue>>.IsReadOnly
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
                var weakKey = pair.Key as WeakKeyReference<TKey>;
                if (weakKey != null && weakKey.IsAlive)
                {
                    if (item.Key == weakKey.Target && object.Equals(item.Value, pair.Value)) return true;
                }
            }

            return false;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var pair in _dict)
            {
                var weakKey = pair.Key as WeakKeyReference<TKey>;
                if (weakKey != null && weakKey.IsAlive)
                {
                    var item = new KeyValuePair<TKey, TValue>(weakKey.Target, pair.Value);
                    array[arrayIndex] = item;
                }
                else
                    array[arrayIndex] = default(KeyValuePair<TKey, TValue>);

                arrayIndex++;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Special Types

        private class KeyCollection : ICollection<TKey>
        {
            private WeakKeyDictionary<TKey, TValue> _dict;

            public KeyCollection(WeakKeyDictionary<TKey, TValue> dict)
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
                    var weakKey = pair.Key as WeakKeyReference<TKey>;
                    if (weakKey != null && weakKey.IsAlive)
                        array[arrayIndex] = weakKey.Target;
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

            public bool Remove(TKey item)
            {
                throw new NotImplementedException("Mutating a key collection derived from a dictionary is not allowed.");
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                //foreach (var pair in _dict)
                //{
                //    yield return pair.Key;
                //}
                return (from p in _dict select p.Key).GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

        }

        private class ValueCollection : ICollection<TValue>
        {

            private WeakKeyDictionary<TKey, TValue> _dict;

            public ValueCollection(WeakKeyDictionary<TKey, TValue> dict)
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
                    if (pair.Key is WeakReference && (pair.Key as WeakReference).IsAlive)
                        array[arrayIndex] = pair.Value;
                    else
                        array[arrayIndex] = default(TValue);

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
                //foreach (var pair in _dict)
                //{
                //    yield return pair.Value;
                //}
                return (from p in _dict select p.Value).GetEnumerator();
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
