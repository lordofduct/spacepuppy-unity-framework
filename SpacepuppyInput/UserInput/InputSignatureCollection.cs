using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.UserInput
{

    public class InputSignatureCollection : ICollection<IInputSignature>
    {

        #region Fields

        private Dictionary<int, IInputSignature> _table = new Dictionary<int, IInputSignature>();
        private List<IInputSignature> _sortedList = new List<IInputSignature>();

        #endregion

        #region Methods

        public IInputSignature GetSignature(string id)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Id == id) return e.Current.Value;
            }
            return null;
        }

        public IInputSignature GetSignature(int hash)
        {
            IInputSignature result;
            if (_table.TryGetValue(hash, out result) && result != null && result.Hash == hash)
            {
                return result;
            }
            return null;
        }

        public bool Contains(string id)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Id == id) return true;
            }
            return false;
        }

        public bool Contains(int hash)
        {
            return _table.ContainsKey(hash);
        }

        public bool Remove(string id)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Id == id)
                {
                    _table.Remove(e.Current.Key);
                    return true;
                }
            }

            return false;
        }

        public bool Remove(int hash)
        {
            return _table.Remove(hash);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
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
            if (_table.ContainsKey(item.Hash)) throw new System.ArgumentException("A signature already exists with this Id and/or Hash.", "item");

            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Id == item.Id || e.Current.Value.Hash == item.Hash)
                {
                    throw new System.ArgumentException("A signature already exists with this Id and/or Hash.", "item");
                }
            }

            _table[item.Hash] = item;
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



    /*

    public class InputSignatureCollection_Old : ICollection<IInputSignature>
    {

        #region Fields

        private List<IInputSignature> _lst = new List<IInputSignature>();

        #endregion

        #region Methods

        public IInputSignature GetSignature(string id)
        {
            var e = _lst.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Id == id) return e.Current;
            }
            return null;
        }

        public IInputSignature GetSignature(int hash)
        {
            var e = _lst.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Hash == hash) return e.Current;
            }
            return null;
        }

        public bool Contains(string id)
        {
            var e = _lst.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Id == id) return true;
            }
            return false;
        }

        public bool Contains(int hash)
        {
            var e = _lst.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Hash == hash) return true;
            }
            return false;
        }

        public bool Remove(string id)
        {
            var sig = this.GetSignature(id);
            if (sig != null)
            {
                return _lst.Remove(sig);
            }
            else
            {
                return false;
            }
        }

        public bool Remove(int hash)
        {
            var sig = this.GetSignature(hash);
            if (sig != null)
            {
                return _lst.Remove(sig);
            }
            else
            {
                return false;
            }
        }

        public void Sort()
        {
            _lst.Sort(SortOnPrecedence);
        }

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
        //private static int SortOnPrecedence(IInputSignature a, IInputSignature b)
        //{
        //    if (a.Precedence > b.Precedence)
        //        return 1;
        //    if (a.Precedence < b.Precedence)
        //        return -1;
        //    else
        //        return 0;
        //}

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _lst.Count; }
        }

        public void Add(IInputSignature item)
        {
            if (item == null) throw new System.ArgumentNullException("item");
            foreach (var s in _lst)
            {
                if (s.Id == item.Id || s.Hash == item.Hash)
                {
                    throw new System.ArgumentException("A signature already exists with this Id and/or Hash.", "item");
                }
            }

            _lst.Add(item);
        }

        public void Clear()
        {
            _lst.Clear();
        }

        public bool Contains(IInputSignature item)
        {
            return _lst.Contains(item);
        }

        public void CopyTo(IInputSignature[] array, int arrayIndex)
        {
            _lst.CopyTo(array, arrayIndex);
        }

        bool ICollection<IInputSignature>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IInputSignature item)
        {
            return _lst.Remove(item);
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
                _e = coll._lst.GetEnumerator();
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

    }

    */

}
