using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput
{

    /// <summary>
    /// Store a group of IInputSignatures based on a mapping value instead of a hash. This mapping value should usually be an enum, you can also use an int/long/etc if you want.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MappedInputSignatureCollection<T> :  IInputSignatureCollection where T : struct, System.IConvertible
    {

        #region Fields

        private Dictionary<T, IInputSignature> _table;
        private List<IInputSignature> _sortedList = new List<IInputSignature>();

        #endregion

        #region CONSTRUCTOR

        public MappedInputSignatureCollection()
        {
            _table = new Dictionary<T, IInputSignature>();
        }

        public MappedInputSignatureCollection(IEqualityComparer<T> comparer)
        {
            _table = new Dictionary<T, IInputSignature>(comparer);
        }

        #endregion

        #region Properties

        public IEqualityComparer<T> Comparer
        {
            get { return _table.Comparer; }
        }

        #endregion

        #region Methods

        public void Add(T mapping, IInputSignature item)
        {
            if (item == null) throw new System.ArgumentNullException("item");
            if (_table.ContainsKey(mapping)) throw new System.ArgumentException("A signature already exists with this mapping.", "item");
            
            _table[mapping] = item;
            _sortedList.Add(item);
        }

        public bool Remove(T mapping)
        {
            IInputSignature sig;
            if(_table.TryGetValue(mapping, out sig))
            {
                _table.Remove(mapping);
                _sortedList.Remove(sig);
                return true;
            }

            return false;
        }

        public IInputSignature GetSignature(T mapping)
        {
            IInputSignature result;
            if (_table.TryGetValue(mapping, out result) && result != null)
            {
                return result;
            }
            return null;
        }

        public bool Contains(T mapping)
        {
            return _table.ContainsKey(mapping);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IInputSignatureCollection Interface

        public virtual IInputSignature GetSignature(string id)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Id == id) return e.Current.Value;
            }
            return null;
        }

        public virtual bool Contains(string id)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Id == id) return true;
            }
            return false;
        }

        public virtual bool Remove(string id)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Id == id)
                {
                    _table.Remove(e.Current.Key);
                    _sortedList.Remove(e.Current.Value);
                    return true;
                }
            }

            return false;
        }

        public void Sort()
        {
            _sortedList.Sort(SortOnPrecedence);
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _table.Count; }
        }

        void ICollection<IInputSignature>.Add(IInputSignature item)
        {
            throw new System.NotSupportedException();
        }

        public void Clear()
        {
            _table.Clear();
            _sortedList.Clear();
        }

        public bool Contains(IInputSignature item)
        {
            return _sortedList.Contains(item);
        }

        public void CopyTo(IInputSignature[] array, int arrayIndex)
        {
            _sortedList.CopyTo(array, arrayIndex);
        }

        bool ICollection<IInputSignature>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IInputSignature item)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value == item)
                {
                    if (_table.Remove(e.Current.Key))
                    {
                        _sortedList.Remove(item);
                        return true;
                    }
                    else
                        return false;
                }
            }

            return false;
        }

        IEnumerator<IInputSignature> IEnumerable<IInputSignature>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<IInputSignature>
        {

            private List<IInputSignature>.Enumerator _e;

            public Enumerator(MappedInputSignatureCollection<T> coll)
            {
                if (coll == null) throw new System.ArgumentNullException("coll");
                _e = coll._sortedList.GetEnumerator();
            }

            public IInputSignature Current
            {
                get { return _e.Current; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _e.Current; }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                (_e as System.Collections.IEnumerator).Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }
        }

        #endregion

        #region Sort Methods

        private static System.Comparison<IInputSignature> _sortOnPrecedence;
        private static System.Comparison<IInputSignature> SortOnPrecedence
        {
            get
            {
                if (_sortOnPrecedence == null)
                {
                    _sortOnPrecedence = (a, b) =>
                    {
                        if (a.Precedence > b.Precedence)
                            return 1;
                        if (a.Precedence < b.Precedence)
                            return -1;
                        else
                            return 0;
                    };
                }
                return _sortOnPrecedence;
            }
        }

        #endregion

    }

}
