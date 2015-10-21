using System;
using System.Collections.Generic;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Collections
{
    public class EventForwardingPool<T, TEvent> : ICollection<T> where TEvent : class
    {

        #region Fields

        private Delegate _handler;
        private EventInfo _event;
        private List<T> _lst = new List<T>();

        #endregion

        #region CONSTRUCTOR

        public EventForwardingPool(TEvent handler, string eventName)
        {
            if (!TypeUtil.IsType(typeof(TEvent), typeof(Delegate))) throw new InvalidOperationException("TEvent must be a Delegate of some type.");
            if (handler == null) throw new ArgumentNullException("handler");
            _handler = handler as Delegate;

            var tp = typeof(T);
            try
            {
                _event = tp.GetEvent(eventName);
            }
            catch
            {
                _event = null;
            }
            if (_event == null) throw new ArgumentException("No event of name '" + eventName + "' was found on type '" + tp.Name + "'.");
        }

        public EventForwardingPool(TEvent handler, string eventName, BindingFlags binding)
        {
            if (!TypeUtil.IsType(typeof(TEvent), typeof(Delegate))) throw new InvalidOperationException("TEvent must be a Delegate of some type.");
            if (handler == null) throw new ArgumentNullException("handler");
            _handler = handler as Delegate;

            var tp = typeof(T);
            try
            {
                _event = tp.GetEvent(eventName, binding);
            }
            catch
            {
                _event = null;
            }
            if (_event == null) throw new ArgumentException("No event of name '" + eventName + "' was found on type '" + tp.Name + "'.");
        }

        public EventForwardingPool(TEvent handler, EventInfo info)
        {
            if (!TypeUtil.IsType(typeof(TEvent), typeof(Delegate))) throw new InvalidOperationException("TEvent must be a Delegate of some type.");
            if (handler == null) throw new ArgumentNullException("handler");
            if (info == null) throw new ArgumentNullException("info");
            if (!TypeUtil.IsType(typeof(T), info.DeclaringType)) throw new ArgumentException("EventInfo must be from a type that T can be assigned to.");

            _handler = handler as Delegate;
            _event = info;
        }

        #endregion

        #region ICollection Interface

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (_lst.Contains(item)) return;

            _event.AddEventHandler(this, _handler);
            _lst.Add(item);
        }

        public void Clear()
        {
            var arr = _lst.ToArray();
            _lst.Clear();
            foreach(var item in arr)
            {
                _event.RemoveEventHandler(item, _handler);
            }
        }

        public bool Contains(T item)
        {
            return _lst.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lst.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _lst.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (_lst.Remove(item))
            {
                _event.RemoveEventHandler(item, _handler);
                return true;
            }
            else
                return false;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        #endregion

    }
}
