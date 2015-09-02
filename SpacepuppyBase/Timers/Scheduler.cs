using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Timers
{

    public class Scheduler : ICollection<ScheduledEvent>
    {

        #region Fields

        private ITimeSupplier _time;

        private int _nodeCount;
        private ScheduledEvent _firstNode;
        private int _version;

        #endregion

        #region CONSTRUCTOR

        public Scheduler(ITimeSupplier time)
        {
            if (time == null) throw new System.ArgumentNullException("time");
            _time = time;
        }

        #endregion

        #region Properties

        public ITimeSupplier TimeSupplier
        {
            get { return _time; }
        }

        #endregion

        #region Methods

        public ScheduledEvent Add(float interval, float offset, System.Action<ScheduledEvent> callback, int count = 0)
        {
            var ev = new ScheduledEvent(interval, offset, callback, count);
            this.Add(ev);
            return ev;
        }

        public void ChangeTimeSupplier(ITimeSupplier time)
        {
            if (time == null) throw new System.ArgumentNullException("time");
            if (time == _time) return;

            _time = time;
            _version++;
            if (_firstNode == null) return;

            var total = _time.TotalPrecise;
            var lst = com.spacepuppy.Collections.TempCollection<ScheduledEvent>.GetCollection(_nodeCount);
            var node = _firstNode;
            _firstNode = null;
            while(node != null)
            {
                node._nextScheduledTime = node.GetNextScheduledTime(total);
                if(double.IsNaN(node._nextScheduledTime))
                {
                    var tnode = node;
                    node = node._nextNode;
                    tnode._prevNode = null;
                    tnode._nextNode = null;
                    tnode._owner = null;
                }
                else
                {
                    lst.Add(node);
                }
            }

            _nodeCount = lst.Count;
            if (_nodeCount > 0)
            {
                lst.Sort((a, b) => a._nextScheduledTime.CompareTo(b._nextScheduledTime));

                _firstNode = lst[0];
                _firstNode._prevNode = null;
                for (int i = 1; i < lst.Count; i++)
                {
                    lst[i - 1]._nextNode = lst[i];
                    lst[i]._prevNode = lst[i - 1];
                }
                lst[lst.Count - 1]._nextNode = null;
            }

            lst.Release();
        }

        public void Update()
        {
            if (_firstNode == null) return;
            
            var total = _time.TotalPrecise;
            var node = _firstNode;
            while (node != null && total > node._nextScheduledTime)
            {
                node.Signal();
                if(node.Complete)
                {
                    _firstNode = node._nextNode;
                    if(_firstNode != null) _firstNode._prevNode = null;

                    node._prevNode = null;
                    node._nextNode = null;
                    node._owner = null;
                    _nodeCount--;
                    _version++;

                    node = _firstNode;
                    continue;
                }
                else
                {
                    node._nextScheduledTime += node.Interval;
                    if(double.IsNaN(node._nextScheduledTime) || double.IsInfinity(node._nextScheduledTime))
                    {
                        _firstNode = node._nextNode;
                        if (_firstNode != null) _firstNode._prevNode = null;

                        node._prevNode = null;
                        node._nextNode = null;
                        node._owner = null;
                        _nodeCount--;
                        _version++;

                        node = _firstNode;
                        continue;
                    }
                    else if(total < node._nextScheduledTime)
                    {
                        //set first node to next
                        _firstNode = node._nextNode;
                        if (_firstNode != null) _firstNode._prevNode = null;

                        //reinsert
                        node._nextNode = null;
                        this.Insert(node);

                        //set next test
                        if (_firstNode == node)
                            break;
                        else
                            node = _firstNode;
                    }
                }
            }
        }

        private void Insert(ScheduledEvent item)
        {
            _version++;

            if (_firstNode == null)
            {
                _firstNode = item;
                _firstNode._prevNode = null;
                _firstNode._nextNode = null;
                _nodeCount = 1;
                return;
            }

            var node = _firstNode;
            if (item._nextScheduledTime < node._nextScheduledTime)
            {
                //placed at front of list
                _firstNode = item;
                _firstNode._nextNode = node;
                node._prevNode = _firstNode;
                return;
            }

            while (node._nextNode != null)
            {
                if (item._nextScheduledTime < node._nextNode._nextScheduledTime)
                {
                    //inserted in middle of list
                    node._nextNode._prevNode = item;
                    item._nextNode = node._nextNode;
                    item._prevNode = node;
                    node._nextNode = item;
                    return;
                }

                node = node._nextNode;
            }

            //added to end of list
            node._nextNode = item;
            item._prevNode = node;
        }

        #endregion

        #region ICollection Interface

        public void Add(ScheduledEvent item)
        {
            if (item._owner == this) return;
            if (item._owner != null) throw new System.ArgumentException("Can not add an event that is a member of another Scheduler.");

            item._nextScheduledTime = item.GetNextScheduledTime(_time.TotalPrecise);
            if (double.IsNaN(item._nextScheduledTime)) return;

            _nodeCount++;
            item._owner = this;
            this.Insert(item);
        }

        public void Clear()
        {
            var node = _firstNode;
            ScheduledEvent lnode;
            while(node != null)
            {
                lnode = node;
                node = lnode._nextNode;
                lnode._owner = null;
                lnode._prevNode = null;
                lnode._nextNode = null;
            }
            _nodeCount = 0;
            _version++;
        }

        public bool Contains(ScheduledEvent item)
        {
            return item._owner == this;
        }

        public void CopyTo(ScheduledEvent[] array, int arrayIndex)
        {
            var e = this.GetEnumerator();
            while(e.MoveNext())
            {
                array[arrayIndex] = e.Current;
                arrayIndex++;
            }
        }

        public int Count
        {
            get { return _nodeCount; }
        }

        bool ICollection<ScheduledEvent>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ScheduledEvent item)
        {
            if (item == null) return false;
            if (item._owner != this) return false;

            var pnode = item._prevNode;
            var nnode = item._nextNode;
            item._owner = null;
            item._prevNode = null;
            item._nextNode = null;
            if (pnode == null)
                _firstNode = nnode;
            else
                pnode._nextNode = nnode;

            _nodeCount--;
            _version++;
            return true;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<ScheduledEvent> IEnumerable<ScheduledEvent>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<ScheduledEvent>
        {

            private Scheduler _scheduler;
            private ScheduledEvent _current;
            private int _version;

            public Enumerator(Scheduler scheduler)
            {
                _scheduler = scheduler;
                _current = null;
                _version = _scheduler._version;
            }

            public ScheduledEvent Current
            {
                get { return _current; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _current; }
            }

            public bool MoveNext()
            {
                if (_scheduler == null) return false;
                if (_scheduler._version != _version) return false;

                if(_current == null)
                {
                    _current = _scheduler._firstNode;
                }
                else
                {
                    _current = _current._nextNode;
                }

                if(_current == null)
                {
                    _version = _scheduler._version - 1;
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public void Reset()
            {
                _current = null;
                if (_scheduler != null) _version = _scheduler._version;
            }

            public void Dispose()
            {
                _scheduler = null;
                _current = null;
            }
        }

        #endregion

    }

    public class ScheduledEvent : IDisposable
    {

        #region Fields

        private float _interval;
        private float _offset;
        private int _count;
        private int _currentCount;
        private System.Action<ScheduledEvent> _callback;

        internal Scheduler _owner;
        internal ScheduledEvent _prevNode;
        internal ScheduledEvent _nextNode;
        internal double _nextScheduledTime;

        #endregion

        #region CONSTRUCTOR

        public ScheduledEvent(float interval, float offset, System.Action<ScheduledEvent> callback, int count = 0)
        {
            _interval = interval;
            _offset = offset;
            _callback = callback;
            _count = count;
        }

        #endregion

        #region #region Properties

        public float Interval { get { return _interval; } }

        public float Offset { get { return _offset; } }

        public int Count { get { return _count; } }

        public int CurrentCount { get { return _currentCount; } }

        public bool Complete { get { return _count > 0 && _currentCount >= _count; } }

        #endregion

        #region Methods

        public double GetNextScheduledTime(double total)
        {
            if (this.Complete) return double.NaN;
            if (_interval == 0f) return double.NaN;

            var loop = total % _interval;
            total = total - loop + _offset;
            if (loop >= _offset)
                total += _interval;

            return total;
        }

        protected internal virtual void Signal()
        {
            if (this.Complete) return;
            _currentCount++;
            if (_callback != null) _callback(this);
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            if (_owner != null)
                _owner.Remove(this);

            _interval = 0f;
            _offset = 0f;
            _callback = null;
            _count = 0;
        }

        #endregion

    }

}
