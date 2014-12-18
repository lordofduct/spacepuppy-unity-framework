using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Timers
{
    public class BulkTimer : ITimer
    {

        #region Fields

        private bool _stopWhenEmpty = false;

        private float _currentTime;
        private List<TimerEntry> _lst = new List<TimerEntry>();

        #endregion

        #region Properties

        public bool StopWhenEmpty
        {
            get { return _stopWhenEmpty; }
        }

        #endregion

        #region Methods

        public void Add(float dur, System.Action callback)
        {
            if (callback == null) throw new System.ArgumentNullException("callback");

            var endTime = _currentTime + dur;
            var entry = new TimerEntry(endTime, callback);

            for(int i = 0; i < _lst.Count; i++)
            {
                if(endTime < _lst[i].EndTime)
                {
                    _lst.Insert(i, entry);
                    return;
                }
            }

            _lst.Add(entry);
        }

        #endregion

        #region ITimer Interface

        public bool Update(float dt)
        {
            _currentTime += dt;

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
            public float EndTime;
            public System.Action Callback;

            public TimerEntry(float endTime, System.Action callback)
            {
                this.EndTime = endTime;
                this.Callback = callback;
            }
        }

        #endregion

    }
}
