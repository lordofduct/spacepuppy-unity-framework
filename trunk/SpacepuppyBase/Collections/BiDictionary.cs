﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{

    public class BiDictionary<TFirst, TSecond> : IDictionary<TFirst, TSecond>, IDictionary
    {

        private readonly IDictionary<TFirst, TSecond> _firstToSecond = new Dictionary<TFirst, TSecond>();
        private readonly IDictionary<TSecond, TFirst> _secondToFirst = new Dictionary<TSecond, TFirst>();
        private readonly ReverseDictionary _reverseDictionary;

        public BiDictionary()
        {
            _reverseDictionary = new ReverseDictionary(this);
        }

        public IDictionary<TSecond, TFirst> Reverse
        {
            get { return _reverseDictionary; }
        }

        public int Count
        {
            get { return _firstToSecond.Count; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_firstToSecond).SyncRoot; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_firstToSecond).IsSynchronized; }
        }

        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)_firstToSecond).IsFixedSize; }
        }

        public bool IsReadOnly
        {
            get { return _firstToSecond.IsReadOnly || _secondToFirst.IsReadOnly; }
        }

        public TSecond this[TFirst key]
        {
            get { return _firstToSecond[key]; }
            set
            {
                _firstToSecond[key] = value;
                _secondToFirst[value] = key;
            }
        }

        object IDictionary.this[object key]
        {
            get { return ((IDictionary)_firstToSecond)[key]; }
            set
            {
                ((IDictionary)_firstToSecond)[key] = value;
                ((IDictionary)_secondToFirst)[value] = key;
            }
        }

        public ICollection<TFirst> Keys
        {
            get { return _firstToSecond.Keys; }
        }

        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)_firstToSecond).Keys; }
        }

        public ICollection<TSecond> Values
        {
            get { return _firstToSecond.Values; }
        }

        ICollection IDictionary.Values
        {
            get { return ((IDictionary)_firstToSecond).Values; }
        }

        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator()
        {
            return _firstToSecond.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_firstToSecond).GetEnumerator();
        }

        public void Add(TFirst key, TSecond value)
        {
            _firstToSecond.Add(key, value);
            _secondToFirst.Add(value, key);
        }

        void IDictionary.Add(object key, object value)
        {
            ((IDictionary)_firstToSecond).Add(key, value);
            ((IDictionary)_secondToFirst).Add(value, key);
        }

        public void Add(KeyValuePair<TFirst, TSecond> item)
        {
            _firstToSecond.Add(item);
            _secondToFirst.Add(item.Reverse());
        }

        public bool ContainsKey(TFirst key)
        {
            return _firstToSecond.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<TFirst, TSecond> item)
        {
            return _firstToSecond.Contains(item);
        }

        public bool TryGetValue(TFirst key, out TSecond value)
        {
            return _firstToSecond.TryGetValue(key, out value);
        }

        public bool Remove(TFirst key)
        {
            TSecond value;
            if (_firstToSecond.TryGetValue(key, out value))
            {
                _firstToSecond.Remove(key);
                _secondToFirst.Remove(value);
                return true;
            }
            else
                return false;
        }

        void IDictionary.Remove(object key)
        {
            var firstToSecond = (IDictionary)_firstToSecond;
            if (!firstToSecond.Contains(key))
                return;
            var value = firstToSecond[key];
            firstToSecond.Remove(key);
            ((IDictionary)_secondToFirst).Remove(value);
        }

        public bool Remove(KeyValuePair<TFirst, TSecond> item)
        {
            return _firstToSecond.Remove(item);
        }

        public bool Contains(object key)
        {
            return ((IDictionary)_firstToSecond).Contains(key);
        }

        public void Clear()
        {
            _firstToSecond.Clear();
            _secondToFirst.Clear();
        }

        public void CopyTo(KeyValuePair<TFirst, TSecond>[] array, int arrayIndex)
        {
            _firstToSecond.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)_firstToSecond).CopyTo(array, index);
        }

        private class ReverseDictionary : IDictionary<TSecond, TFirst>, IDictionary
        {
            private readonly BiDictionary<TFirst, TSecond> _owner;

            public ReverseDictionary(BiDictionary<TFirst, TSecond> owner)
            {
                _owner = owner;
            }

            public int Count
            {
                get { return _owner._secondToFirst.Count; }
            }

            object ICollection.SyncRoot
            {
                get { return ((ICollection)_owner._secondToFirst).SyncRoot; }
            }

            bool ICollection.IsSynchronized
            {
                get { return ((ICollection)_owner._secondToFirst).IsSynchronized; }
            }

            bool IDictionary.IsFixedSize
            {
                get { return ((IDictionary)_owner._secondToFirst).IsFixedSize; }
            }

            public bool IsReadOnly
            {
                get { return _owner._secondToFirst.IsReadOnly || _owner._firstToSecond.IsReadOnly; }
            }

            public TFirst this[TSecond key]
            {
                get { return _owner._secondToFirst[key]; }
                set
                {
                    _owner._secondToFirst[key] = value;
                    _owner._firstToSecond[value] = key;
                }
            }

            object IDictionary.this[object key]
            {
                get { return ((IDictionary)_owner._secondToFirst)[key]; }
                set
                {
                    ((IDictionary)_owner._secondToFirst)[key] = value;
                    ((IDictionary)_owner._firstToSecond)[value] = key;
                }
            }

            public ICollection<TSecond> Keys
            {
                get { return _owner._secondToFirst.Keys; }
            }

            ICollection IDictionary.Keys
            {
                get { return ((IDictionary)_owner._secondToFirst).Keys; }
            }

            public ICollection<TFirst> Values
            {
                get { return _owner._secondToFirst.Values; }
            }

            ICollection IDictionary.Values
            {
                get { return ((IDictionary)_owner._secondToFirst).Values; }
            }

            public IEnumerator<KeyValuePair<TSecond, TFirst>> GetEnumerator()
            {
                return _owner._secondToFirst.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                return ((IDictionary)_owner._secondToFirst).GetEnumerator();
            }

            public void Add(TSecond key, TFirst value)
            {
                _owner._secondToFirst.Add(key, value);
                _owner._firstToSecond.Add(value, key);
            }

            void IDictionary.Add(object key, object value)
            {
                ((IDictionary)_owner._secondToFirst).Add(key, value);
                ((IDictionary)_owner._firstToSecond).Add(value, key);
            }

            public void Add(KeyValuePair<TSecond, TFirst> item)
            {
                _owner._secondToFirst.Add(item);
                _owner._firstToSecond.Add(item.Reverse());
            }

            public bool ContainsKey(TSecond key)
            {
                return _owner._secondToFirst.ContainsKey(key);
            }

            public bool Contains(KeyValuePair<TSecond, TFirst> item)
            {
                return _owner._secondToFirst.Contains(item);
            }

            public bool TryGetValue(TSecond key, out TFirst value)
            {
                return _owner._secondToFirst.TryGetValue(key, out value);
            }

            public bool Remove(TSecond key)
            {
                TFirst value;
                if (_owner._secondToFirst.TryGetValue(key, out value))
                {
                    _owner._secondToFirst.Remove(key);
                    _owner._firstToSecond.Remove(value);
                    return true;
                }
                else
                    return false;
            }

            void IDictionary.Remove(object key)
            {
                var firstToSecond = (IDictionary)_owner._secondToFirst;
                if (!firstToSecond.Contains(key))
                    return;
                var value = firstToSecond[key];
                firstToSecond.Remove(key);
                ((IDictionary)_owner._firstToSecond).Remove(value);
            }

            public bool Remove(KeyValuePair<TSecond, TFirst> item)
            {
                return _owner._secondToFirst.Remove(item);
            }

            public bool Contains(object key)
            {
                return ((IDictionary)_owner._secondToFirst).Contains(key);
            }

            public void Clear()
            {
                _owner._secondToFirst.Clear();
                _owner._firstToSecond.Clear();
            }

            public void CopyTo(KeyValuePair<TSecond, TFirst>[] array, int arrayIndex)
            {
                _owner._secondToFirst.CopyTo(array, arrayIndex);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((IDictionary)_owner._secondToFirst).CopyTo(array, index);
            }
        }
    }

    internal static class KeyValuePairExts
    {
        public static KeyValuePair<TValue, TKey> Reverse<TKey, TValue>(this KeyValuePair<TKey, TValue> @this)
        {
            return new KeyValuePair<TValue, TKey>(@this.Value, @this.Key);
        }
    }
}
