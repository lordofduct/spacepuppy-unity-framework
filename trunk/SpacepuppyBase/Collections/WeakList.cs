using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{
    /// <summary>
    /// A generic collection of weak references to a type of object. The objects are stored weakly allowing them to still be garbage collected. 
    /// If an object is garbage collected its position in the list is made null, you can call WeakList.Clean() to clear out all dead objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakList<T> : IList<T>
    {

        #region Fields

        private List<WeakReference> _lst;
        private bool _trackResurrection;

        #endregion

        #region CONSTRUCTOR

        public WeakList()
        {
            _lst = new List<WeakReference>();
            _trackResurrection = false;
        }

        public WeakList(int capacity)
        {
            _lst = new List<WeakReference>(capacity);
            _trackResurrection = false;
        }

        public WeakList(IEnumerable<T> collection)
        {
            _lst = new List<WeakReference>();
            _trackResurrection = false;

            this.AddRange(collection);
        }

        public WeakList(bool trackResurrection)
        {
            _lst = new List<WeakReference>();
            _trackResurrection = trackResurrection;
        }

        public WeakList(int capacity, bool trackResurrection)
        {
            _lst = new List<WeakReference>(capacity);
            _trackResurrection = trackResurrection;
        }

        public WeakList(IEnumerable<T> collection, bool trackResurrection)
        {
            _lst = new List<WeakReference>();
            _trackResurrection = trackResurrection;

            this.AddRange(collection);
        }

        #endregion

        #region Properties

        public bool Empty
        {
            get { return _lst.Count == 0; }
        }

        #endregion

        #region WeakList Methods

        public T[] ToArray()
        {
            var arr = new T[_lst.Count];
            for (int i = 0; i < arr.Length; i++)
            {
                if (_lst[i].IsAlive)
                    arr[i] = (T)_lst[i].Target;
                else
                    arr[i] = default(T);
            }
            return arr;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var obj in collection)
            {
                _lst.Add(new WeakReference(obj, _trackResurrection));
            }
        }

        /// <summary>
        /// removes all contained objects that are dead
        /// </summary>
        public bool Clean()
        {
            bool result = false;
            foreach (var weak in _lst.ToArray())
            {
                if (!weak.IsAlive)
                {
                    _lst.Remove(weak);
                    result = true;
                }
            }
            return result;
        }

        public bool IsAlive(int index)
        {
            return _lst[index].IsAlive;
        }

        public bool TrackResurrection(int index)
        {
            return _lst[index].TrackResurrection;
        }

        #endregion


        #region IList<T> Members

        public int IndexOf(T item)
        {
            for (int i = 0; i < _lst.Count; i++)
            {
                if (_lst[i].IsAlive && _lst[i].Target.Equals(item)) return i;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            _lst.Insert(index, new WeakReference(item, _trackResurrection));
        }

        public void RemoveAt(int index)
        {
            _lst.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                if (_lst[index].IsAlive)
                    return (T)_lst[index].Target;
                else
                    return default(T);
            }
            set
            {
                _lst[index].Target = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            _lst.Add(new WeakReference(item, _trackResurrection));
        }

        public void Clear()
        {
            _lst.Clear();
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _lst.Count; i++)
            {
                if (_lst[i].IsAlive && _lst[i].Target.Equals(item)) return true;
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < _lst.Count) throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");

            for (int i = 0; i < _lst.Count; i++)
            {
                if (_lst[i].IsAlive)
                    array[i + arrayIndex] = (T)_lst[i].Target;
                else
                    array[i + arrayIndex] = default(T);
            }

        }

        public int Count
        {
            get { return _lst.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return ((IList<T>)_lst).IsReadOnly; }
        }

        public bool Remove(T item)
        {
            int i = this.IndexOf(item);
            if (i >= 0)
            {
                _lst.RemoveAt(i);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            T[] arr = new T[_lst.Count];

            for (int i = 0; i < _lst.Count; i++)
            {
                if (_lst[i].IsAlive)
                    arr[i] = (T)_lst[i].Target;
                else
                    arr[i] = default(T);
            }

            return ((IEnumerable<T>)arr).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            T[] arr = new T[_lst.Count];

            for (int i = 0; i < _lst.Count; i++)
            {
                if (_lst[i].IsAlive)
                    arr[i] = (T)_lst[i].Target;
                else
                    arr[i] = default(T);
            }

            return arr.GetEnumerator();
        }

        #endregion
    }
}
