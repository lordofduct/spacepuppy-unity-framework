using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{
    public class SignalingCollection<T> : IList<T>, ISignalingCollection
    {

        #region Events

        public event EventHandler Clearing;
        public event EventHandler Cleared;

        public event SignalCollectionEventHandler Added;
        public event SignalCollectionEventHandler Removed;

        protected virtual void OnClearing(EventArgs e)
        {
            if (Clearing != null) this.Clearing(this, e);
        }

        protected virtual void OnCleared(EventArgs e)
        {
            if (Cleared != null) this.Cleared(this, e);
        }

        protected virtual void OnAdded(SignalCollectionEventArgs e)
        {
            if (Added != null) this.Added(this, e);
        }

        protected virtual void OnRemoved(SignalCollectionEventArgs e)
        {
            if (Removed != null) this.Removed(this, e);
        }

        #endregion

        #region Fields

        private List<T> _lst;
        private IEqualityComparer<T> _comparer;

        #endregion

        #region CONSTRUCTOR

        public SignalingCollection()
        {
            _lst = new List<T>();
            _comparer = null;
        }

        public SignalingCollection(int capacity)
        {
            _lst = new List<T>(capacity);
            _comparer = null;
        }

        public SignalingCollection(IEnumerable<T> collection)
        {
            _lst = new List<T>(collection);
            _comparer = null;
        }

        public SignalingCollection(IEqualityComparer<T> comparer)
        {
            _lst = new List<T>();
            _comparer = comparer;
        }

        public SignalingCollection(int capacity, IEqualityComparer<T> comparer)
        {
            _lst = new List<T>(capacity);
            _comparer = comparer;
        }

        public SignalingCollection(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _lst = new List<T>(collection);
            _comparer = comparer;
        }

        #endregion

        #region IList Interface

        public int IndexOf(T item)
        {
            if (_comparer == null)
            {
                return _lst.IndexOf(item);
            }
            else
            {
                for (int i = 0; i < _lst.Count; i++)
                {
                    if (_comparer.Equals(_lst[i], item)) return i;
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            _lst.Insert(index, item);
            this.OnAdded(new SignalCollectionEventArgs(item));
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _lst.Count) throw new IndexOutOfRangeException();

            var item = _lst[index];
            _lst.RemoveAt(index);
            this.OnRemoved(new SignalCollectionEventArgs(item));
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _lst.Count) throw new IndexOutOfRangeException();
                return _lst[index];
            }
            set
            {
                if (index < 0 || index >= _lst.Count) throw new IndexOutOfRangeException();
                var item = _lst[index];
                if (_comparer == null)
                {
                    if (Object.Equals(item, value)) return;
                }
                else
                {
                    if (_comparer.Equals(item, value)) return;
                }

                _lst[index] = value;
                this.OnRemoved(new SignalCollectionEventArgs(item));
                this.OnAdded(new SignalCollectionEventArgs(value));
            }
        }

        #endregion

        #region ICollection<T> Interface

        public void Add(T item)
        {
            _lst.Add(item);
            this.OnAdded(new SignalCollectionEventArgs(item));
        }

        public void Clear()
        {
            this.OnClearing(EventArgs.Empty);
            _lst.Clear();
            this.OnCleared(EventArgs.Empty);
        }

        public bool Contains(T item)
        {
            if (_comparer == null)
            {
                return _lst.Contains(item);
            }
            else
            {
                foreach (var obj in _lst)
                {
                    if (_comparer.Equals(obj, item)) return true;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lst.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _lst.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (_comparer == null)
            {
                if (_lst.Remove(item))
                {
                    this.OnRemoved(new SignalCollectionEventArgs(item));
                    return true;
                }
            }
            else
            {
                var index = this.IndexOf(item);
                if (index >= 0)
                {
                    this.RemoveAt(index);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region ICollection Interface

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            if (array is T[])
            {
                this.CopyTo(array as T[], index);
            }
            else
            {
                throw new ArrayTypeMismatchException();
            }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return _lst; }
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<T> GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        #endregion

    }
}
