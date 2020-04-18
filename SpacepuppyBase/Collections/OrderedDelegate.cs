using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{

    public class OrderedDelegate : IEnumerable<OrderedDelegate.DelegateEntry>
    {

        #region Fields

        private List<DelegateEntry> _delegates = new List<DelegateEntry>();
        private Delegate _callback;

        #endregion

        #region CONSTRUCTOR

        public OrderedDelegate()
        {

        }

        public OrderedDelegate(Delegate del)
        {
            this.Add(del);
        }

        public OrderedDelegate(Delegate del, int precedence)
        {
            this.Add(del, precedence);
        }

        #endregion

        #region Properties

        public bool HasEntries
        {
            get { return _delegates.Count > 0; }
        }

        public float HighestPrecedence
        {
            get
            {
                if (_delegates.Count == 0) return float.NaN;
                return _delegates[0].Precedence;
            }
        }

        public float MinPrecedence
        {
            get
            {
                if (_delegates.Count == 0) return float.NaN;
                return _delegates[_delegates.Count - 1].Precedence;
            }
        }
        
        #endregion

        #region Methods

        protected Delegate GetDelegate()
        {
            if(_callback == null && _delegates.Count > 0)
            {
                Delegate[] arr = new Delegate[_delegates.Count];
                for(int i = 0; i < _delegates.Count; i++)
                {
                    arr[i] = _delegates[i].Delegate;
                }
                _callback = Delegate.Combine(arr);
            }
            return _callback;
        }

        public void DynamicInvoke(params object[] args)
        {
            var d = this.GetDelegate();
            if (d != null) d.DynamicInvoke(args);
        }

        public void Add(Delegate del)
        {
            if (del == null) throw new System.ArgumentNullException("del");

            float prec = (_delegates.Count > 0) ? _delegates[_delegates.Count - 1].Precedence : 0f;
            _delegates.Add(new DelegateEntry(del, prec, null));
            _callback = null;
        }

        public void Add(Delegate del, float precedence, object tag = null)
        {
            if (del == null) throw new System.ArgumentNullException("del");

            int i = this.FindInsertLocationOfPrecedence(precedence);
            _delegates.Insert(i, new DelegateEntry(del, precedence, tag));
            _callback = null;
        }

        public bool Contains(Delegate del)
        {
            return FindIndexOf(del) >= 0;
        }

        public void Clear()
        {
            _delegates.Clear();
            _callback = null;
        }

        public bool Remove(Delegate del)
        {
            int index = FindIndexOf(del);
            if (index >= 0)
            {
                _delegates.RemoveAt(index);
                _callback = null;
                return true;
            }
            else
            {
                return false;
            }
        }



        private int FindIndexOf(Delegate del)
        {
            for (int i = 0; i < _delegates.Count; i++)
            {
                if (_delegates[i].Delegate == del) return i;
            }
            return -1;
        }

        private int FindInsertLocationOfPrecedence(float precedence)
        {
            for (int i = 0; i < _delegates.Count; i++)
            {
                if (precedence > _delegates[i].Precedence)
                {
                    return i;
                }
            }
            return _delegates.Count;
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<OrderedDelegate.DelegateEntry> GetEnumerator()
        {
            return _delegates.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Operators

        public static OrderedDelegate operator +(OrderedDelegate a, Delegate b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");
            a.Add(b);
            return a;
        }

        public static OrderedDelegate operator -(OrderedDelegate a, Delegate b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");
            a.Remove(b);
            return a;
        }

        public static implicit operator OrderedDelegate(Delegate d)
        {
            return new OrderedDelegate(d);
        }

        //public static explicit operator OrderedDelegate(Delegate d)
        //{
        //    return new OrderedDelegate(d);
        //}

        #endregion

        #region Special Types

        public struct DelegateEntry
        {
            public Delegate Delegate;
            public float Precedence;
            public object Tag;

            public DelegateEntry(Delegate del, float prec, object tag)
            {
                this.Delegate = del;
                this.Precedence = prec;
                this.Tag = tag;
            }
        }

        #endregion

    }

    public class OrderedDelegate<T> : OrderedDelegate where T : class
    {

        #region CONSTRUCTOR

        public OrderedDelegate()
        {
            if (!com.spacepuppy.Utils.TypeUtil.IsType(typeof(T), typeof(Delegate)))
            {
                throw new System.InvalidOperationException("Cannot create an OrderedDelegate<T> where T does not inherit from System.Delegate.");
            }
        }

        public OrderedDelegate(T del)
            : this()
        {
            this.Add(del);
        }

        public OrderedDelegate(T del, int precedence)
            : this()
        {
            this.Add(del, precedence);
        }

        #endregion

        #region Properties

        public T Invoke
        {
            get
            {
                return this.GetDelegate() as T;
            }
        }

        #endregion

        #region Methods

        public void Add(T del)
        {
            base.Add(del as Delegate);
        }

        public void Add(T del, float precedence, object tag = null)
        {
            base.Add(del as Delegate, precedence, tag);
        }

        public bool Remove(T del)
        {
            return base.Remove(del as Delegate);
        }

        #endregion

        #region Operators

        public static OrderedDelegate<T> operator +(OrderedDelegate<T> a, T b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");
            a.Add(b);
            return a;
        }

        public static T operator +(T a, OrderedDelegate<T> b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");
            foreach (var d in b)
            {
                //a = Delegate.Combine(a as Delegate, d) as T;
            }
            return a;
        }

        public static OrderedDelegate<T> operator -(OrderedDelegate<T> a, T b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");
            a.Remove(b);
            return a;
        }

        public static T operator -(T a, OrderedDelegate<T> b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");
            foreach (var d in b)
            {
                //a = Delegate.Remove(a as Delegate, d) as T;
            }
            return a;
        }

        public static implicit operator OrderedDelegate<T>(T d)
        {
            return new OrderedDelegate<T>(d);
        }

        //public static explicit operator OrderedDelegate<T>(T d)
        //{
        //    return new OrderedDelegate<T>(d);
        //}

        #endregion

    }
    
}
