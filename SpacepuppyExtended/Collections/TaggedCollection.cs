using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TaggedCollection<TKey, TValue> : IEnumerable<TValue>, System.Collections.ICollection
    {

        #region Fields

        private Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();
        private ITagValidator<TKey, TValue> _validator;

        #endregion

        #region CONSTRUCTOR

        public TaggedCollection(ITagValidator<TKey, TValue> validator)
        {
            if (validator == null) throw new ArgumentNullException("validator");
            _validator = validator;
        }

        #endregion

        #region Properties

        public TValue this[int index]
        {
            get
            {
                if (index < 0 || index >= _dict.Count) throw new System.IndexOutOfRangeException();
                var key = _dict.Keys.Take(index + 1).Last();
                return _dict[key];
            }
            set
            {
                if (index < 0 || index >= _dict.Count) throw new System.IndexOutOfRangeException();
                var key = _dict.Keys.Take(index + 1).Last();
                if (!_validator.Validate(key, value)) throw new TaggedCollectionValidationException();

                this[key] = value;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _dict[key];
            }
            set
            {
                var item = _dict[key];
                if (Object.Equals(item, value)) return; //it's already part of it

                _dict[key] = value;
            }
        }

        public int Count
        {
            get { return _dict.Count; }
        }

        public IEnumerable<TKey> Tags { get { return _dict.Keys; } }

        #endregion

        #region Methods

        public void Add(TValue value)
        {
            var tag = _validator.GetTag(value);
            _dict[tag] = value;
        }

        public bool ContainsTag(TKey tag)
        {
            return _dict.ContainsKey(tag);
        }

        public bool ContainsValue(TValue value)
        {
            return _dict.Values.Contains(value);
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Remove(TKey tag)
        {
            return _dict.Remove(tag);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        #endregion

        #region IEnumerable Interface

        //TODO - implement propert Enumerator, remember dict.Values allocates mem in mono... ugh

        public IEnumerator<TValue> GetEnumerator()
        {
            return _dict.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict.Values.GetEnumerator();
        }

        #endregion

        #region ICollection Interface

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            if (array is TValue[])
            {
                _dict.Values.CopyTo(array as TValue[], index);
            }
            else
            {
                throw new System.ArrayTypeMismatchException();
            }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return (_dict as System.Collections.ICollection).IsSynchronized; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return (_dict as System.Collections.ICollection).SyncRoot; }
        }

        #endregion

    }

    public interface ITagValidator<TKey, TValue>
    {

        bool Validate(TKey tag, TValue value);
        TKey GetTag(TValue value);

    }
}
