using System;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Collections
{
    public class UniqueList<T> : IList<T>, System.Collections.IList
    {

        #region Fields

        private IList<T> _lst;
        private IEqualityComparer<T> _comparer;

        private bool _bMoveDuplicatesToTopOnAdd = true;

        #endregion

        #region CONSTRUCTOR

        public UniqueList()
            : this(null as IEqualityComparer<T>)
        {
        }

        public UniqueList(int capacity)
            : this(capacity, null as IEqualityComparer<T>)
        {
        }

        public UniqueList(IEnumerable<T> collection)
            : this(collection, null as IEqualityComparer<T>)
        {
        }

        public UniqueList(IEqualityComparer<T> comparer)
        {
            _lst = new List<T>();
            _comparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;
        }

        public UniqueList(int capacity, IEqualityComparer<T> comparer)
        {
            _lst = new List<T>(capacity);
            _comparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;
        }

        public UniqueList(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _lst = new List<T>(collection);
            _comparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;

            this.AddRange(collection);
        }

        #endregion

        #region Properties

        public bool MoveDuplicatesToTopOnAdd
        {
            get { return _bMoveDuplicatesToTopOnAdd; }
            set { _bMoveDuplicatesToTopOnAdd = value; }
        }

        public bool Empty
        {
            get { return _lst.Count == 0; }
        }

        #endregion

        #region UniqueList Methods

        public T[] ToArray()
        {
            if (_lst is List<T>)
                return (_lst as List<T>).ToArray();
            else
                return System.Linq.Enumerable.ToArray(_lst);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var obj in collection)
            {
                this.Add(obj);
            }
        }

        /// <summary>
        /// Removes any duplicates in a wrapped IList that may have creeped in.
        /// </summary>
        public void Clean()
        {
            for (int i = 0; i < _lst.Count; i++)
            {
                for (int j = i + 1; j < _lst.Count; j++)
                {
                    if (_comparer.Equals(_lst[i], _lst[j]))
                    {
                        _lst.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        public void RemoveAll(Predicate<T> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            if (_lst is List<T>)
                (_lst as List<T>).RemoveAll(func);
            else
            {
                for (int i = 0; i < _lst.Count; i++)
                {
                    if (func(_lst[i]))
                    {
                        _lst.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        #endregion

        #region IList<T> Members

        public int IndexOf(T item)
        {
            for (int i = 0; i < _lst.Count; i++)
            {
                if (_comparer.Equals(item, _lst[i])) return i;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            int i = this.IndexOf(item);
            if (i >= 0)
            {
                this.RemoveAt(i);
                if (i < index) index--;
            }

            _lst.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _lst.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _lst[index];
            }
            set
            {
                if (this.Contains(value) && !_comparer.Equals(_lst[index], value))
                {
                    int i = this.IndexOf(value);
                    if (i < index) index--;
                    _lst.RemoveAt(i);
                }

                _lst[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public int Count
        {
            get { return _lst.Count; }
        }

        public bool IsReadOnly
        {
            get { return _lst.IsReadOnly; }
        }

        public void Add(T item)
        {
            if (_bMoveDuplicatesToTopOnAdd)
            {
                if (this.Contains(item)) this.Remove(item);

                _lst.Add(item);
            }
            else
            {
                if (!this.Contains(item)) _lst.Add(item);
            }
        }

        public void Clear()
        {
            _lst.Clear();
        }

        public bool Contains(T item)
        {
            foreach (var obj in _lst)
            {
                if (_comparer.Equals(obj, item)) return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lst.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            //use indexof using comparer, then remove at that index
            int index = this.IndexOf(item);
            if (index < 0) return false;
            _lst.RemoveAt(index);
            return true;
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator<T> GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        #endregion


        #region IList Interface

        int System.Collections.IList.Add(object value)
        {
            if (!(value is T)) throw new ArgumentException("item is of a type that is not assignable to this IList.");

            this.Add((T)value);
            return this.IndexOf((T)value);
        }

        void System.Collections.IList.Clear()
        {
            this.Clear();
        }

        bool System.Collections.IList.Contains(object value)
        {
            if (!(value is T)) return false;

            return this.Contains((T)value);
        }

        int System.Collections.IList.IndexOf(object value)
        {
            if (!(value is T)) return -1;

            return this.IndexOf((T)value);
        }

        void System.Collections.IList.Insert(int index, object value)
        {
            if (!(value is T)) throw new ArgumentException("item is of a type that is not assignable to this IList.");

            this.Insert(index, (T)value);
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { return false; }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get { return this.IsReadOnly; }
        }

        void System.Collections.IList.Remove(object value)
        {
            if (!(value is T)) throw new ArgumentException("item is of a type that is not assignable to this IList.");

            this.Remove((T)value);
        }

        void System.Collections.IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (!(value is T)) throw new ArgumentException("item is of a type that is not assignable to this IList.");

                this[index] = (T)value;
            }
        }

        #endregion

        #region ICollection Interface

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            ArrayUtil.Copy(_lst, array, index);
        }

        int System.Collections.ICollection.Count
        {
            get { return this.Count; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get
            {
                if (_lst is System.Collections.ICollection)
                    return (_lst as System.Collections.ICollection).IsSynchronized;
                else
                    return false;
            }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get
            {
                if (_lst is System.Collections.ICollection)
                    return (_lst as System.Collections.ICollection).SyncRoot;
                else
                    return _lst;
            }
        }

        #endregion

    }
}
