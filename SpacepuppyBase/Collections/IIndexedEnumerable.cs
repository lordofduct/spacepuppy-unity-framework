using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{

    public interface IIndexedEnumerable<T> : IEnumerable<T>
    {

        int Count { get; }
        T this[int index] { get; }

        bool Contains(T item);
        void CopyTo(T[] array, int startIndex);
        int IndexOf(T item);

    }

    public class ReadOnlyList<T> : IIndexedEnumerable<T>, IList<T>
    {

        #region Fields

        private IList<T> _lst;

        #endregion

        #region CONSTRUCTOR

        public ReadOnlyList(IList<T> lst)
        {
            if (lst == null) throw new ArgumentNullException("lst");
            _lst = lst;
        }

        #endregion

        #region IIndexedEnumerable Interface

        public T this[int index] { get { return _lst[index]; } }

        public int Count { get { return _lst.Count; } }

        public bool Contains(T item)
        {
            return _lst.Contains(item);
        }

        public void CopyTo(T[] array, int startIndex)
        {
            _lst.CopyTo(array, startIndex);
        }

        public int IndexOf(T item)
        {
            return _lst.IndexOf(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList Interface

        T IList<T>.this[int index] { get { return _lst[index]; } set { throw new NotSupportedException(); } }

        bool ICollection<T>.IsReadOnly { get { return true; } }

        int IIndexedEnumerable<T>.Count { get { return _lst.Count; } }

        T IIndexedEnumerable<T>.this[int index] { get { return _lst[index]; } }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Utils

        public static ReadOnlyList<T> Validate(ref ReadOnlyList<T> pointer, IList<T> consumable)
        {
            if (pointer == null) pointer = new ReadOnlyList<T>(consumable);
            else if (pointer._lst != consumable) pointer._lst = consumable;
            return pointer;
        }

        #endregion

    }

    public class ReadOnlyMemberList<TOwner, TMember> : IIndexedEnumerable<TMember>, IList<TMember>
    {

        #region Fields

        private IList<TOwner> _lst;
        private System.Func<TOwner, TMember> _getter;

        #endregion

        #region CONSTRUCTOR

        public ReadOnlyMemberList(IList<TOwner> lst, System.Func<TOwner, TMember> getter)
        {
            if (lst == null) throw new ArgumentNullException("lst");
            if (getter == null) throw new ArgumentNullException("getter");
            _lst = lst;
            _getter = getter;
        }

        #endregion

        #region IIndexedEnumerable Interface

        public TMember this[int index] { get { return _getter(_lst[index]); } }

        public int Count { get { return _lst.Count; } }

        public bool Contains(TMember item)
        {
            var comparer = EqualityComparer<TMember>.Default;
            for(int i = 0; i < _lst.Count; i++)
            {
                if (comparer.Equals(item, _getter(_lst[i]))) return true;
            }
            return false;
        }

        public void CopyTo(TMember[] array, int startIndex)
        {
            for (int i = 0; i < _lst.Count; i++)
            {
                array[startIndex] = _getter(_lst[i]);
                startIndex++;
            }
        }

        public int IndexOf(TMember item)
        {
            var comparer = EqualityComparer<TMember>.Default;
            for (int i = 0; i < _lst.Count; i++)
            {
                if (comparer.Equals(item, _getter(_lst[i]))) return i;
                i++;
            }

            return -1;
        }

        public IEnumerator<TMember> GetEnumerator()
        {
            return _lst.Select(o => _getter(o)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList Interface

        TMember IList<TMember>.this[int index] { get { return _getter(_lst[index]); } set { throw new NotSupportedException(); } }

        bool ICollection<TMember>.IsReadOnly { get { return true; } }

        int IIndexedEnumerable<TMember>.Count { get { return _lst.Count; } }

        TMember IIndexedEnumerable<TMember>.this[int index]
        {
            get { return _getter(_lst[index]); }
        }

        void ICollection<TMember>.Add(TMember item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TMember>.Clear()
        {
            throw new NotSupportedException();
        }

        void IList<TMember>.Insert(int index, TMember item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<TMember>.Remove(TMember item)
        {
            throw new NotSupportedException();
        }

        void IList<TMember>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Utils

        public static ReadOnlyMemberList<TOwner, TMember> Validate(ref ReadOnlyMemberList<TOwner, TMember> pointer, IList<TOwner> consumable, Func<TOwner, TMember> getter)
        {
            if (pointer == null) pointer = new ReadOnlyMemberList<TOwner, TMember>(consumable, getter);
            else if (pointer._lst != consumable) pointer._lst = consumable;
            return pointer;
        }

        #endregion

    }

}
