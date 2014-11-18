using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Timers
{

    public class DelayTimer : ITimer
    {

        #region Fields

        private float _delay;
        private float _t;

        #endregion

        #region CONSTRUCTOR

        public DelayTimer()
        {

        }

        public DelayTimer(float delay)
        {
            _delay = delay;
        }

        #endregion

        #region Properties

        public float Delay { get { return _delay; } set { _delay = value; } }

        public bool Blocking { get { return _t > 0f; } }

        #endregion

        #region Methods

        public void Set()
        {
            _t = _delay;
        }

        public void Stop()
        {
            _t = 0f;
        }

        #endregion


        #region ITimer Interface

        public bool Update(float dt)
        {
            if(_t > 0f)
            {
                _t -= dt;
                if (_t < 0f) _t = 0f;
                return _t > 0f;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }

}