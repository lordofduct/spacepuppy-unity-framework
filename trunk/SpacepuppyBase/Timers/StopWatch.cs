using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Timers
{

    public class StopWatch : ITimer
    {

        #region Fields

        private float _currentTime;

        #endregion

        #region CONSTRUCTOR

        public StopWatch()
        {

        }

        #endregion

        #region Properties

        public float CurrentTime { get { return _currentTime; } }

        #endregion

        #region ITimer Interface

        public bool Update(float dt)
        {
            _currentTime += dt;
            return true;
        }

        #endregion

    }

}