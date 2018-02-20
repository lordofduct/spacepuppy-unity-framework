using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput
{

    public class InputSignatureCollection : IInputSignatureCollection
    {

        #region Fields

        private Dictionary<string, IInputSignature> _table = new Dictionary<string, IInputSignature>();
        private List<IInputSignature> _sortedList = new List<IInputSignature>();

        #endregion

        #region Methods
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IInputSignatureCollection Interface

        public virtual IInputSignature GetSignature(string id)
        {
            IInputSignature result;
            if (_table.TryGetValue(id, out result) && result != null && result.Id == id)
            {
                return result;
            }
            return null;
        }

        public virtual bool Contains(string id)
        {
            IInputSignature result;
            if (_table.TryGetValue(id, out result) && result != null && result.Id == id)
            {
                return true;
            }
            return false;
        }

        public virtual bool Remove(string id)
        {
            IInputSignature sig;
            if(_table.TryGetValue(id, out sig) && _table.Remove(id))
            {
                _sortedList.Remove(sig);
                return true;
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

        public void Add(IInputSignature item)
        {
            if (item == null) throw new System.ArgumentNullException("item");
            if (_table.ContainsKey(item.Id)) throw new System.ArgumentException("A signature already exists with this Id and/or Hash.", "item");

            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Id == item.Id)
                {
                    throw new System.ArgumentException("A signature already exists with this Id and/or Hash.", "item");
                }
            }

            _table[item.Id] = item;
            _sortedList.Add(item);
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

            public Enumerator(InputSignatureCollection coll)
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
