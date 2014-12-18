using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Timers
{

    public class BulkTimer : ITimer
    {

        #region Fields

        private bool _stopWhenEmpty = true;

        //NOTE - we use a double for better precission over long periods of time
        private double _currentTime = 0d;
        private List<TimerEntry> _lst = new List<TimerEntry>();

        #endregion

        #region Properties

        public bool StopWhenEmpty
        {
            get { return _stopWhenEmpty; }
            set { _stopWhenEmpty = value; }
        }

        #endregion

        #region Methods

        public void Add(float dur, System.Action callback)
        {
            if (callback == null) throw new System.ArgumentNullException("callback");

            double endTime = _currentTime + dur;
            var entry = new TimerEntry(endTime, callback);

            if(_lst.Count > 0 && endTime < _lst.Last().EndTime)
            {
                for (int i = 0; i < _lst.Count; i++)
                {
                    if (endTime < _lst[i].EndTime)
                    {
                        _lst.Insert(i, entry);
                        return;
                    }
                }
            }

            _lst.Add(entry);
        }

        public void Reset()
        {
            _lst.Clear();
            _currentTime = 0d;
        }

        #endregion

        #region ITimer Interface

        public bool Update(float dt)
        {
            _currentTime += (double)dt;

            while(_lst.Count > 0 && _currentTime >= _lst[0].EndTime)
            {
                var entry = _lst[0];
                _lst.RemoveAt(0);
                entry.Callback();
            }

            if(_stopWhenEmpty)
            {
                return _lst.Count > 0;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Special Types

        private struct TimerEntry
        {
            public double EndTime;
            public System.Action Callback;

            public TimerEntry(double endTime, System.Action callback)
            {
                this.EndTime = endTime;
                this.Callback = callback;
            }
        }

        #endregion

    }
}
