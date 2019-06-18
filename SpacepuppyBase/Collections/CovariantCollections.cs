using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{

    public class CovariantCollection<TInner, TOuter> : ICollection<TOuter> where TInner : class, TOuter
    {

        #region Fields

        private ICollection<TInner> _coll;

        #endregion

        #region CONSTRUCTOR

        public CovariantCollection(ICollection<TInner> coll)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            _coll = coll;
        }

        #endregion

        #region ICollection Interface

        public ICollection<TInner> InnerCollection { get { return _coll; } }

        public int Count { get { return _coll.Count; } }

        public bool IsReadOnly { get { return _coll.IsReadOnly; } }

        public void Add(TOuter item)
        {
            if (item != null && !(item is TInner)) throw new System.ArgumentException("item must be of type " + typeof(TInner).Name + " to be added to this collection.", "item");

            _coll.Add(item as TInner);
        }

        public void Clear()
        {
            _coll.Clear();
        }

        public bool Contains(TOuter item)
        {
            if (item != null && !(item is TInner)) return false;

            return _coll.Contains(item as TInner);
        }

        public void CopyTo(TOuter[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach(var item in _coll)
            {
                if (i >= array.Length) return;

                array[i] = item;
                i++;
            }
        }

        public bool Remove(TOuter item)
        {
            if (item != null && !(item is TInner)) return false;

            return _coll.Remove(item as TInner);
        }

        public IEnumerator<TOuter> GetEnumerator()
        {
            return _coll.Cast<TOuter>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _coll.GetEnumerator();
        }

        #endregion



        public static CovariantCollection<TInner, TOuter> Sync(ref CovariantCollection<TInner, TOuter> covariantColl, ICollection<TInner> innerColl)
        {
            if (covariantColl == null || covariantColl.InnerCollection != innerColl)
            {
                covariantColl = new CovariantCollection<TInner, TOuter>(innerColl);
            }

            return covariantColl;
        }

    }

    public class CovariantList<TInner, TOuter> : IList<TOuter> where TInner : class, TOuter
    {

        #region Fields

        private IList<TInner> _coll;

        #endregion

        #region CONSTRUCTOR

        public CovariantList(IList<TInner> coll)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            _coll = coll;
        }

        #endregion

        #region IList Interface

        public IList<TInner> InnerCollection { get { return _coll; } }

        public int Count { get { return _coll.Count; } }

        public bool IsReadOnly { get { return _coll.IsReadOnly; } }

        public TOuter this[int index]
        {
            get { return _coll[index]; }
            set
            {
                if(value != null && !(value is TInner)) throw new System.ArgumentException("item must be of type " + typeof(TInner).Name + " to be added to this collection.", "value");
                _coll[index] = value as TInner;
            }
        }

        public void Add(TOuter item)
        {
            if (item != null && !(item is TInner)) throw new System.ArgumentException("item must be of type " + typeof(TInner).Name + " to be added to this collection.", "item");

            _coll.Add(item as TInner);
        }

        public void Clear()
        {
            _coll.Clear();
        }

        public bool Contains(TOuter item)
        {
            if (item != null && !(item is TInner)) return false;

            return _coll.Contains(item as TInner);
        }

        public void CopyTo(TOuter[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (var item in _coll)
            {
                if (i >= array.Length) return;

                array[i] = item;
                i++;
            }
        }
        
        public int IndexOf(TOuter item)
        {
            if (item != null && !(item is TInner)) return -1;

            return _coll.IndexOf(item as TInner);
        }

        public void Insert(int index, TOuter item)
        {
            if (item != null && !(item is TInner)) throw new System.ArgumentException("item must be of type " + typeof(TInner).Name + " to be added to this collection.", "item");
        }

        public bool Remove(TOuter item)
        {
            if (!(item is TInner)) return false;

            return _coll.Remove(item as TInner);
        }

        public void RemoveAt(int index)
        {
            _coll.RemoveAt(index);
        }

        public IEnumerator<TOuter> GetEnumerator()
        {
            return _coll.Cast<TOuter>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _coll.GetEnumerator();
        }

        #endregion

        public static CovariantList<TInner, TOuter> Sync(ref CovariantList<TInner, TOuter> covariantColl, IList<TInner> innerColl)
        {
            if (covariantColl == null || covariantColl.InnerCollection != innerColl)
            {
                covariantColl = new CovariantList<TInner, TOuter>(innerColl);
            }

            return covariantColl;
        }

    }

}
