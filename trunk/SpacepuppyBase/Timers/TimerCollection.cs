using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Timers
{

    public class TimerCollection : IEnumerable<ITimer>, ITimer
    {

        #region Fields

        private List<ITimer> _timers = new List<ITimer>();
        private List<ITimer> _toRemove = new List<ITimer>();

        private bool _inUpdate;
        private System.Action _lateAction;

        #endregion

        #region CONSTRUCTOR

        public TimerCollection()
        {
            
        }

        #endregion

        #region Properties

        public int Count { get { return _timers.Count; } }

        #endregion

        #region Methods

        public void Start(ITimer t)
        {
            if(_inUpdate)
            {
                _lateAction += () =>
                {
                    if (!_timers.Contains(t)) _timers.Add(t);
                };
            }
            else
            {
                if (!_timers.Contains(t)) _timers.Add(t);
            }
        }

        public bool IsActive(ITimer t)
        {
            return _timers.Contains(t);
        }

        public void Stop(ITimer t)
        {
            if (_inUpdate)
            {
                if (_timers.Contains(t)) _lateAction += () => _timers.Remove(t);
            }
            else
            {
                _timers.Remove(t);
            }
        }

        public void StopAll()
        {
            if (_inUpdate)
            {
                _lateAction += () => _timers.Clear();
            }
            else
            {
                _timers.Clear();
            }
        }

        #endregion

        #region ICollection Interface

        public IEnumerator<ITimer> GetEnumerator()
        {
            return _timers.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _timers.GetEnumerator();
        }

        #endregion

        #region ITimer Interface

        public bool Update(float dt)
        {
            _inUpdate = true;

            //update timers
            _toRemove.Clear();
            if(_timers.Count > 0)
            {
                //if (_timers is WeakList<ITimer>) (_timers as WeakList<ITimer>).Clean();
                foreach(var t in _timers)
                {
                    if(!t.Update(dt))
                    {
                        _toRemove.Add(t);
                    }
                }
            }

            foreach(var t in _toRemove)
            {
                _timers.Remove(t);
            }
            _toRemove.Clear();

            //perform late actions
            var a = _lateAction;
            _lateAction = null;
            if (a != null)
            {
                a();
            }

            //exit
            _inUpdate = false;
            return _timers.Count > 0f;
        }

        #endregion


    }

}