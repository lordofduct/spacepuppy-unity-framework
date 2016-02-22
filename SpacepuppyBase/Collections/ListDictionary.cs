using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{
    public class ListDictionary<TKey, TValue> : IDictionary<TKey, IList<TValue>>
    {

        #region Fields

        private Dictionary<TKey, IList<TValue>> _dict;
        private Func<IList<TValue>> _listConstructor;
        private ListCollection _listRedirectColl;

        #endregion

        #region CONSTRUCTOR

        public ListDictionary()
        {
            _dict = new Dictionary<TKey, IList<TValue>>();
            _listConstructor = DefaultListConstructor;
            _listRedirectColl = new ListCollection(this);
        }

        public ListDictionary(Func<IList<TValue>> listConstructor)
        {
            _dict = new Dictionary<TKey, IList<TValue>>();
            _listConstructor = (listConstructor != null) ? listConstructor : DefaultListConstructor;
            _listRedirectColl = new ListCollection(this);
        }

        private static IList<TValue> DefaultListConstructor()
        {
            return new List<TValue>();
        }

        #endregion

        #region Properties

        public int Count { get { return _dict.Count; } }

        public TValue this[TKey key, int index]
        {
            get
            {
                //if (!_dict.ContainsKey(key)) throw new KeyNotFoundException();
                //var lst = _dict[key];
                //if (index < 0 || index >= lst.Count) throw new IndexOutOfRangeException();
                //return lst[index];
                IList<TValue> lst;
                if (_dict.TryGetValue(key, out lst))
                {
                    if (index < 0 || index >= lst.Count) throw new IndexOutOfRangeException();
                    return lst[index];
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            set
            {
                //if (!_dict.ContainsKey(key)) throw new KeyNotFoundException();
                //var lst = _dict[key];
                //if (index < 0 || index >= lst.Count) throw new IndexOutOfRangeException();
                //lst[index] = value;
                IList<TValue> lst;
                if (_dict.TryGetValue(key, out lst))
                {
                    if (index < 0 || index >= lst.Count) throw new IndexOutOfRangeException();
                    lst[index] = value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get { return _dict.Keys; }
        }

        public ListCollection Lists
        {
            get { return _listRedirectColl; }
        }

        #endregion

        #region Methods

        public void AddKey(TKey key)
        {
            if (!_dict.ContainsKey(key))
            {
                _dict.Add(key, _listConstructor());
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (!_dict.ContainsKey(key))
            {
                var lst = _listConstructor();
                lst.Add(value);
                _dict.Add(key, lst);
            }
            else
            {
                _dict[key].Add(value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Remove(TKey key)
        {
            return _dict.Remove(key);
        }

        public bool Remove(TKey key, TValue value)
        {
            if (!_dict.ContainsKey(key)) return false;
            return _dict[key].Remove(value);
        }

        /// <summary>
        /// Purges all keys that contain empty lists
        /// </summary>
        public void Purge()
        {
            //var keys = this.Keys.ToArray();
            //foreach (var key in keys)
            //{
            //    if (_dict[key] == null || _dict[key].Count == 0) _dict.Remove(key);
            //}

            using (var lst = TempCollection.GetList<TKey>(this.Keys))
            {
                var e = lst.GetEnumerator();
                while(e.MoveNext())
                {
                    var sublst = _dict[e.Current];
                    if (sublst == null || sublst.Count == 0) _dict.Remove(e.Current);
                }
            }
        }

        /// <summary>
        /// Purges key if the related list is empty.
        /// </summary>
        /// <param name="key"></param>
        public bool Purge(TKey key)
        {
            if (_dict.ContainsKey(key) && _dict[key].Count == 0)
            {
                return _dict.Remove(key);
            }
            return false;
        }

        public IEnumerable<TValue> GetValues()
        {
            foreach (var lst in _dict.Values)
            {
                foreach (var obj in lst)
                {
                    yield return obj;
                }
            }
        }

        public IEnumerable<TValue> GetValues(TKey key)
        {
            //if (!_dict.ContainsKey(key)) yield break;

            //foreach (var obj in _dict[key])
            //{
            //    yield return obj;
            //}

            return _dict[key];
        }

        #endregion

        #region IDictionary Interface

        void IDictionary<TKey, IList<TValue>>.Add(TKey key, IList<TValue> value)
        {
            throw new NotImplementedException("Mutating ListDictionary IList entry directly is not supported.");
        }

        bool IDictionary<TKey, IList<TValue>>.ContainsKey(TKey key)
        {
            return this.ContainsKey(key);
        }

        ICollection<TKey> IDictionary<TKey, IList<TValue>>.Keys
        {
            get { return this.Keys; }
        }

        bool IDictionary<TKey, IList<TValue>>.Remove(TKey key)
        {
            return this.Remove(key);
        }

        bool IDictionary<TKey, IList<TValue>>.TryGetValue(TKey key, out IList<TValue> value)
        {
            return _dict.TryGetValue(key, out value);
        }

        ICollection<IList<TValue>> IDictionary<TKey, IList<TValue>>.Values
        {
            get { return this.Lists; }
        }

        IList<TValue> IDictionary<TKey, IList<TValue>>.this[TKey key]
        {
            get
            {
                return _dict[key];
            }
            set
            {
                throw new NotImplementedException("Mutating ListDictionary IList entry directly is not supported.");
            }
        }

        #endregion

        #region ICollection Interface

        void ICollection<KeyValuePair<TKey, IList<TValue>>>.Add(KeyValuePair<TKey, IList<TValue>> item)
        {
            throw new NotImplementedException("Mutating ListDictionary IList entry directly is not supported.");
        }

        void ICollection<KeyValuePair<TKey, IList<TValue>>>.Clear()
        {
            this.Clear();
        }

        bool ICollection<KeyValuePair<TKey, IList<TValue>>>.Contains(KeyValuePair<TKey, IList<TValue>> item)
        {
            var e = _dict.GetEnumerator();
            while(e.MoveNext())
            {
                if (object.ReferenceEquals(e.Current, item)) return true;
            }
            return false;
        }

        void ICollection<KeyValuePair<TKey, IList<TValue>>>.CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex)
        {
            (_dict as ICollection<KeyValuePair<TKey, IList<TValue>>>).CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<TKey, IList<TValue>>>.Count
        {
            get { return this.Count; }
        }

        bool ICollection<KeyValuePair<TKey, IList<TValue>>>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<KeyValuePair<TKey, IList<TValue>>>.Remove(KeyValuePair<TKey, IList<TValue>> item)
        {
            throw new NotImplementedException("Mutating ListDictionary IList entry directly is not supported.");
        }

        #endregion

        #region IEnum Interface

        public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        #endregion

        #region Special Types

        public class ListCollection : ICollection<IList<TValue>>
        {

            private ListDictionary<TKey, TValue> _listDict;

            internal ListCollection(ListDictionary<TKey, TValue> listDict)
            {
                _listDict = listDict;
            }

            public IList<TValue> this[TKey key]
            {
                get
                {
                    IList<TValue> result;
                    if (_listDict._dict.TryGetValue(key, out result))
                    {
                        return result;
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
            }

            public bool TryGetList(TKey key, out IList<TValue> lst)
            {
                return _listDict._dict.TryGetValue(key, out lst);
            }

            #region ICollection Interface

            void ICollection<IList<TValue>>.Add(IList<TValue> item)
            {
                throw new NotImplementedException("Mutating a value collection derived from a dictionary is not allowed.");
            }

            void ICollection<IList<TValue>>.Clear()
            {
                throw new NotImplementedException("Mutating a value collection derived from a dictionary is not allowed.");
            }

            public bool Contains(IList<TValue> item)
            {
                var e = _listDict._dict.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Value == item) return true;
                }
                return false;
            }

            public void CopyTo(IList<TValue>[] array, int arrayIndex)
            {
                _listDict._dict.Values.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return _listDict._dict.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<IList<TValue>>.Remove(IList<TValue> item)
            {
                throw new NotImplementedException("Mutating a value collection derived from a dictionary is not allowed.");
            }

            #endregion

            #region IEnumerable Interface

            //TODO - implement propert Enumerator, remember dict.Values allocates mem in mono... ugh

            public IEnumerator<IList<TValue>> GetEnumerator()
            {
                return _listDict._dict.Values.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _listDict._dict.Values.GetEnumerator();
            }

            #endregion

        }

        #endregion

    }
}
