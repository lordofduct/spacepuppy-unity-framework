using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// Should only be inherited from by EnumKeyedCollection
    /// </summary>
    public abstract class DrawableEnumKeyedCollection
    {

    }

    [System.Serializable()]
    public class EnumKeyedCollection<TEnumKey, TValue> : DrawableEnumKeyedCollection, IDictionary<TEnumKey, TValue> where TEnumKey : struct, IConvertible
    {

        #region Static Methods

        private static int _keyCount;
        private static TEnumKey[] _keys;
        private static KeyCollection _keyColl;
        private static int _low;
        private static int _high;

        static EnumKeyedCollection()
        {
            var tp = typeof(TEnumKey);
            if (!tp.IsEnum) throw new NotSupportedException("Key type must be an enum type.");

            _keys = System.Enum.GetValues(tp) as TEnumKey[];
            _keyCount = _keys.Length;

            if(_keyCount > 0)
            {
                Array.Sort(_keys);
                _low = Convert.ToInt32(_keys[0]);
                _high = Convert.ToInt32(_keys[_keys.Length - 1]);

                var diff = _high - _low;
                if(diff != _keyCount - 1)
                {
                    throw new NotSupportedException("Key type must be an enum type that is sequential.");
                }
            }
        }

        public static Type KeyType
        {
            get { return typeof(TEnumKey); }
        }

        #endregion

        #region Fields

        [UnityEngine.SerializeField()]
        private TValue[] _values = new TValue[_keyCount];
        [System.NonSerialized()]
        private ValueCollection _valueColl;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public int Count { get { return _keyCount; } }

        public TValue this[TEnumKey key]
        {
            get
            {
                int i = Convert.ToInt32(key) - _low;
                if (i < 0 || i >= _keyCount) throw new KeyNotFoundException();
                return _values[i];
            }
            set
            {
                int i = Convert.ToInt32(key) - _low;
                if (i < 0 || i >= _keyCount) throw new KeyNotFoundException();
                _values[i] = value;
            }
        }
        
        public ValueCollection Values
        {
            get
            {
                if (_valueColl == null) _valueColl = new ValueCollection(this);
                return _valueColl;
            }
        }

        public KeyCollection Keys
        {
            get
            {
                if (_keyColl == null) _keyColl = new KeyCollection();
                return _keyColl;
            }
        }

        #endregion

        #region Methods

        #endregion

        #region IDictionary Interface

        bool ICollection<KeyValuePair<TEnumKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        ICollection<TEnumKey> IDictionary<TEnumKey, TValue>.Keys
        {
            get
            {
                return this.Keys;
            }
        }

        ICollection<TValue> IDictionary<TEnumKey, TValue>.Values
        {
            get
            {
                return this.Values;
            }
        }

        void ICollection<KeyValuePair<TEnumKey, TValue>>.Add(KeyValuePair<TEnumKey, TValue> item)
        {
            this[item.Key] = item.Value;
        }

        void IDictionary<TEnumKey, TValue>.Add(TEnumKey key, TValue value)
        {
            this[key] = value;
        }

        public void Clear()
        {
            Array.Clear(_values, 0, _keyCount);
        }

        public bool TryGetValue(TEnumKey key, out TValue value)
        {
            int i = Convert.ToInt32(key) - _low;
            if (i >= 0 && i < _keyCount)
            {
                value = _values[i];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        bool ICollection<KeyValuePair<TEnumKey, TValue>>.Contains(KeyValuePair<TEnumKey, TValue> item)
        {
            TValue value;
            return this.TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public bool ContainsKey(TEnumKey key)
        {
            return Array.IndexOf(_keys, key) >= 0;
        }

        void ICollection<KeyValuePair<TEnumKey, TValue>>.CopyTo(KeyValuePair<TEnumKey, TValue>[] array, int arrayIndex)
        {
            for (int i = 0; i < _keyCount; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<TEnumKey, TValue>(_keys[i], _values[i]);
            }
        }

        bool ICollection<KeyValuePair<TEnumKey, TValue>>.Remove(KeyValuePair<TEnumKey, TValue> item)
        {
            TValue value;
            if(this.TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                this[item.Key] = default(TValue);
                return true;
            }

            return false;
        }

        bool IDictionary<TEnumKey, TValue>.Remove(TEnumKey key)
        {
            this[key] = default(TValue);
            return true;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<KeyValuePair<TEnumKey, TValue>> IEnumerable<KeyValuePair<TEnumKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<KeyValuePair<TEnumKey, TValue>>
        {

            private EnumKeyedCollection<TEnumKey, TValue> _coll;
            private int _index;

            internal Enumerator(EnumKeyedCollection<TEnumKey, TValue> coll)
            {
                if (coll == null) throw new System.ArgumentNullException("coll");
                _coll = coll;
                _index = -1;
            }


            public KeyValuePair<TEnumKey, TValue> Current
            {
                get
                {
                    if (_coll == null || _index < 0 || _index >= _keyCount) return default(KeyValuePair<TEnumKey, TValue>);

                    return new KeyValuePair<TEnumKey, TValue>(_keys[_index], _coll._values[_index]);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Dispose()
            {
                _coll = null;
            }

            public bool MoveNext()
            {
                if (_coll == null) return false;
                _index++;
                return (_index >= 0 && _index <= _keyCount);
            }

            void IEnumerator.Reset()
            {
                _index = -1;
            }
        }

        public class KeyCollection : ICollection<TEnumKey>
        {
            public int Count
            {
                get
                {
                    return _keyCount;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            void ICollection<TEnumKey>.Add(TEnumKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TEnumKey>.Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TEnumKey item)
            {
                return Array.IndexOf(_keys, item) >= 0;
            }

            public void CopyTo(TEnumKey[] array, int arrayIndex)
            {
                Array.Copy(_keys, 0, array, arrayIndex, _keyCount);
            }

            bool ICollection<TEnumKey>.Remove(TEnumKey item)
            {
                throw new NotSupportedException();
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator();
            }

            IEnumerator<TEnumKey> IEnumerable<TEnumKey>.GetEnumerator()
            {
                return new Enumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator();
            }


            public struct Enumerator : IEnumerator<TEnumKey>
            {

                private int _index;

                public TEnumKey Current
                {
                    get
                    {
                        if (_index < 1 || _index > _keyCount) return default(TEnumKey);

                        return _keys[_index - 1];
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return this.Current;
                    }
                }

                public void Dispose()
                {
                    _index = 0;
                }

                public bool MoveNext()
                {
                    _index++;
                    return _index > 0 && _index <= _keyCount;
                }

                public void Reset()
                {
                    _index = 0;
                }
            }

        }

        public class ValueCollection : IList<TValue>
        {

            #region Fields

            private EnumKeyedCollection<TEnumKey, TValue> _owner;

            #endregion

            #region CONSTRUCTOR

            internal ValueCollection(EnumKeyedCollection<TEnumKey, TValue> coll)
            {
                if (coll == null) throw new ArgumentNullException("coll");

                _owner = coll;
            }

            #endregion

            public TValue this[int index]
            {
                get
                {
                    return _owner._values[index];
                }

                set
                {
                    _owner._values[index] = value;
                }
            }

            public int Count
            {
                get
                {
                    return _keyCount;
                }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                _owner.Clear();
            }

            public bool Contains(TValue item)
            {
                return Array.IndexOf(_owner._values, item) >= 0;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                Array.Copy(_owner._values, 0, array, arrayIndex, _keyCount);
            }

            public int IndexOf(TValue item)
            {
                return Array.IndexOf(_owner._values, item);
            }

            void IList<TValue>.Insert(int index, TValue item)
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            void IList<TValue>.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }


            public Enumerator GetEnumerator()
            {
                return new Enumerator(_owner);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_owner);
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(_owner);
            }


            public struct Enumerator : IEnumerator<TValue>
            {

                private EnumKeyedCollection<TEnumKey, TValue> _coll;
                private int _index;

                internal Enumerator(EnumKeyedCollection<TEnumKey, TValue> coll)
                {
                    _coll = coll;
                    _index = -1;
                }

                public TValue Current
                {
                    get
                    {
                        if (_coll == null || _index < 0 || _index >= _keyCount) return default(TValue);
                        return _coll._values[_index];
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return this.Current;
                    }
                }

                public void Dispose()
                {
                    _coll = null;
                    _index = -1;
                }

                public bool MoveNext()
                {
                    if (_coll == null) return false;
                    _index++;
                    return (_index >= 0 && _index <= _keyCount);
                }

                void IEnumerator.Reset()
                {
                    //do nothing
                }

            }

        }

        #endregion

    }

}
