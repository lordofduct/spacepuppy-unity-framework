using System;
using System.Collections.Generic;

namespace com.spacepuppy.Timers
{

    public enum SchedulingStyle
    {
        FromZero = 0,
        FromNow = 1,
        FromRelative = 2
    }

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

        /// <summary>
        /// Reschedule an event that was previously cancelled based on the time that schedule was initially started. 
        /// This is useful for saving and loading scheduled events. You can serialize the start time, and calculate 
        /// and restart the event based on that.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="originalStartTime"></param>
        /// <param name="time"></param>
        /// <param name="repeatInterval"></param>
        /// <param name="repeatCount"></param>
        /// <param name="callback"></param>
        /// <returns>Returns the ScheduledEvent toke. Will return null if no event will occur, for example, if the current time is pass the time the event would have occurred.</returns>
        public ScheduledEvent Reschedule(SchedulingStyle style, double originalStartTime, double time, double repeatInterval, int repeatCount, System.Action<ScheduledEvent> callback)
        {
            switch(style)
            {
                case SchedulingStyle.FromZero:
                    {
                        //calculate next time to schedule at
                        var currentTime = _time.TotalPrecise;
                        var next = System.Math.Ceiling((currentTime - time) / repeatInterval) * repeatInterval + time;


                        //determine how many repeats are left based on the _startTime, this is necessary incase this was loaded and started again
                        if (repeatCount < 0)
                        {
                            repeatCount = -1; //negative is infinite
                        }
                        else
                        {
                            repeatCount = repeatCount - (int)System.Math.Round((next - time) / repeatInterval);
                            if (repeatCount < 0) return null; //completed, so don't start
                        }

                        this.Schedule(next, repeatInterval, repeatCount, callback);
                    }
                    break;
                case SchedulingStyle.FromNow:
                    {
                        //calculate next time to schedule at
                        var currentTime = _time.TotalPrecise;
                        var firstTime = originalStartTime + time;
                        var next = System.Math.Ceiling((currentTime - firstTime) / repeatInterval) * repeatInterval + firstTime;


                        //determine how many repeats are left based on the _startTime, this is necessary incase this was loaded and started again
                        if (repeatCount < 0)
                        {
                            repeatCount = -1; //negative is infinite
                        }
                        else
                        {
                            repeatCount = repeatCount - (int)System.Math.Round((next - firstTime) / repeatInterval);
                            if (repeatCount < 0) return null; //completed, so don't start
                        }

                        this.Schedule(next, repeatInterval, repeatCount, callback);
                    }
                    break;
                case SchedulingStyle.FromRelative:
                    {
                        //calculate next time to schedule at
                        var currentTime = _time.TotalPrecise;
                        var next = System.Math.Ceiling((currentTime - time) / repeatInterval) * repeatInterval + time;


                        //determine how many repeats are left based on the _startTime, this is necessary incase this was loaded and started again
                        if (repeatCount < 0)
                        {
                            repeatCount = -1; //negative is infinite
                        }
                        else
                        {
                            var first = System.Math.Ceiling((originalStartTime - time) / repeatInterval) * repeatInterval + time;
                            repeatCount = repeatCount - (int)System.Math.Round((next - first) / repeatInterval);
                            if (repeatCount < 0) return null; //completed, so don't start
                        }

                        this.Schedule(next, repeatInterval, repeatCount, callback);
                    }
                    break;
            }

            return null;
        }
        
        /// <summary>
        /// Schedule an event to occur at some point in time.
        /// </summary>
        /// <param name="time">The time of the event</param>
        /// <param name="callback">Callback to respond to the event</param>
        /// <returns></returns>
        public ScheduledEvent Schedule(double time, System.Action<ScheduledEvent> callback)
        {
            var ev = new ScheduledEvent(time, 0d, 0, callback);
            this.Add(ev);
            return ev;
        }
        /// <summary>
        /// Create an event that occurs on some interval.
        /// </summary>
        /// <param name="time">The time at which the interval begins counting, good for repeating intervals</param>
        /// <param name="interval">The interval of the event</param>
        /// <param name="repeatCount">Number of times to repeat, negative values are treated as infinite</param>
        /// <param name="callback">Callback to respond to the event</param>
        /// <returns></returns>
        public ScheduledEvent Schedule(double time, double interval, int repeatCount, System.Action<ScheduledEvent> callback)
        {
            var ev = new ScheduledEvent(time, interval, repeatCount, callback);
            this.Add(ev);
            return ev;
        }

        /// <summary>
        /// Schedule an event that raises at some duration from the current time.
        /// </summary>
        /// <param name="duration">Amount of time from now the event occurs</param>
        /// <param name="callback">Callback to respond to the event</param>
        /// <returns></returns>
        public ScheduledEvent ScheduleFromNow(double duration, System.Action<ScheduledEvent> callback)
        {
            var ev = new ScheduledEvent(_time.TotalPrecise + duration, 0d, 0, callback);
            this.Add(ev);
            return ev;
        }

        /// <summary>
        /// Schedule an event that raises at some duration from the current time.
        /// </summary>
        /// <param name="duration">Amount of time from now the first event occurs</param>
        /// <param name="repeatFrequency">Frequency of the event</param>
        /// <param name="repeatCount">Number of times the event occurs</param>
        /// <param name="callback">Callback to respond to the event</param>
        /// <returns></returns>
        public ScheduledEvent ScheduleFromNow(double duration, double repeatFrequency, int repeatCount, System.Action<ScheduledEvent> callback)
        {
            var ev = new ScheduledEvent(_time.TotalPrecise + duration, repeatFrequency, repeatCount, callback);
            this.Add(ev);
            return ev;
        }

        /// <summary>
        /// Schedules an event relative to current time so that it would have been at the repeated time of 'interval' of a scheduled time at 'offset'. 
        /// 
        /// For example if you wanted to schedule an event for on the half hour past the current hour. You'd pass in 30 minutes for 'firstTime' and 'hour' 
        /// for interval. It'll calculate the next half hour on the hour from now, and schedule an event for it.
        /// </summary>
        /// <param name="firstTime"></param>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ScheduledEvent ScheduleOnIntervalFromNow(double firstTime, double interval,  System.Action<ScheduledEvent> callback)
        {
            var t = Math.Ceiling((_time.TotalPrecise - firstTime) / interval) * interval + firstTime;
            var ev = new ScheduledEvent(t, 0d, 0, callback);
            this.Add(ev);
            return ev;
        }

        /// <summary>
        /// Schedules an event relative to current time so that it would have been at the repeated time of 'interval' of a scheduled time at 'offset'. 
        /// And then repeat that interval 'repeatCount' times.
        /// 
        /// For example if you wanted to schedule an event for on the half hour past the current hour. You'd pass in 30 minutes for 'firstTime' and 'hour' 
        /// for interval. It'll calculate the next half hour on the hour from now, and schedule an event for it.
        /// </summary>
        /// <param name="firstTime"></param>
        /// <param name="interval"></param>
        /// <param name="repeatCount"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ScheduledEvent ScheduleOnIntervalFromNow(double firstTime, double interval, int repeatCount, System.Action<ScheduledEvent> callback)
        {
            var t = Math.Ceiling((_time.TotalPrecise - firstTime) / interval) * interval + firstTime;
            var ev = new ScheduledEvent(t, interval, repeatCount, callback);
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
            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<ScheduledEvent>(_nodeCount))
            {
                var node = _firstNode;
                _firstNode = null;
                while (node != null)
                {
                    node._nextScheduledTime = node.GetNextScheduledTime(total);
                    if (double.IsNaN(node._nextScheduledTime))
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
            }
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

        private double _time;
        private double _interval;
        private int _repeatCount;
        private int _currentCount;
        private System.Action<ScheduledEvent> _callback;

        internal Scheduler _owner;
        internal ScheduledEvent _prevNode;
        internal ScheduledEvent _nextNode;
        internal double _nextScheduledTime;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Creates a Scheduled Event
        /// </summary>
        /// <param name="time">The time at which the interval begins counting, good for repeating intervals</param>
        /// <param name="interval">The interval of the event</param>
        /// <param name="repeatCount">Number of times to repeat, negative values are treated as infinite</param>
        /// <param name="callback">Callback to respond to the event</param>
        public ScheduledEvent(double time, double interval, int repeatCount, System.Action<ScheduledEvent> callback)
        {
            _time = time;
            _interval = interval;
            _callback = callback;
            _repeatCount = repeatCount;
        }

        #endregion

        #region #region Properties

        /// <summary>
        /// The time of the scheduled event.
        /// </summary>
        public double Time { get { return _time; } }

        /// <summary>
        /// The interval after 'Time' that the event should repeat, if it repeats.
        /// </summary>
        public double Interval { get { return _interval; } }

        /// <summary>
        /// Number of times the interval should repeat before completing, values &lt 0 repeat forever.
        /// </summary>
        public int RepeatCount { get { return _repeatCount; } }

        /// <summary>
        /// Number of times the event has occurred.
        /// </summary>
        public int CurrentCount { get { return _currentCount; } }

        /// <summary>
        /// Has all the events finished.
        /// </summary>
        public bool Complete { get { return _repeatCount >= 0 && _currentCount > _repeatCount; } }

        /// <summary>
        /// The Scheduler with which this event is registered.
        /// </summary>
        public Scheduler Scheduler { get { return _owner; } }

        #endregion

        #region Methods


        public void Cancel()
        {
            if(_owner != null)
            {
                _owner.Remove(this);
            }
        }


        public double GetNextScheduledTime()
        {
            double t = (_owner != null && _owner.TimeSupplier != null) ? _owner.TimeSupplier.TotalPrecise : 0d;
            return GetNextScheduledTime(t);
        }

        /// <summary>
        /// Returns the next time after 'time' that this event aught to be raised.
        /// </summary>
        /// <param name="time">The time after which to get the next scheduled time.</param>
        /// <returns></returns>
        public double GetNextScheduledTime(double time)
        {
            //if (this.Complete) return double.NaN;
            //if (_interval <= 0f) return double.NaN;

            //if (time < _time) return _time + _interval;
            //var t = time - _time;
            //int cnt = (int)Math.Floor(t / _interval);
            //if (_repeatCount >= 0 && cnt > _repeatCount) return double.NaN;

            //return _time + (_interval * (cnt + 1));

            if (this.Complete) return double.NaN;

            if (time < _time) return _time;
            if (_repeatCount == 0) return double.NaN;

            var t = time - _time;
            int cnt = (int)Math.Floor(t / _interval);
            if (_repeatCount >= 0 && cnt > _repeatCount) return double.NaN;

            return _time + (_interval * (cnt + 1));
        }

        //public float GetProgress()
        //{
        //    double t = (_owner != null && _owner.TimeSupplier != null) ? _owner.TimeSupplier.TotalPrecise : 0d;
        //    double tf = GetNextScheduledTime(t);
        //    double t0 = tf - _interval;

        //    float p = com.spacepuppy.Utils.MathUtil.Clamp01((float)((t - t0) / (tf - t0)));
        //    if (com.spacepuppy.Utils.MathUtil.IsReal(p))
        //        return p;
        //    else
        //        return 1f;
        //}

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
            _time = 0f;
            _callback = null;
            _repeatCount = 0;
        }

        #endregion

    }

}
