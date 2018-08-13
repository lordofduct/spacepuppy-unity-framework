using UnityEngine;
using System.Collections.Generic;
using com.spacepuppy.Collections;


namespace com.spacepuppy.ClassicTimers
{

    public abstract class Timer : System.IDisposable
    {

        #region Properties

        public bool IsRunning
        {
            get;
            protected set;
        }

        #endregion

        #region CONSTRUCTOR

        public Timer()
        {
            IsRunning = false;
        }

        #endregion

        #region Methods

        public virtual void Start()
        {
            this.IsRunning = true;
            Timer.AddReference(this);
        }

        public virtual void Stop()
        {
            this.IsRunning = false;
            Timer.RemoveReference(this);
        }

        protected abstract void UpdateTimer(float dt);

        #endregion

        #region IDisposable Members

        public bool IsDisposed
        {
            get;
            protected set;
        }

        public void Dispose()
        {
            Dispose(true);

            //System.GC.SuppressFinalize(this);

            IsDisposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Timer.RemoveReference(this);
            }
        }

        #endregion

        #region Static Timer Entry

        private static WeakList<Timer> _runningTimers = new WeakList<Timer>();

        static Timer()
        {
            GameLoopEntry.OnUpdate += (s, e) => Update();
        }

        private static void Update()
        {
            float dt = Time.deltaTime;
            if (_runningTimers.Count > 0)
            {
                _runningTimers.Clean();

                foreach (var timer in _runningTimers)
                {
                    if (timer != null && timer.IsRunning) timer.UpdateTimer(dt);
                }
            }
        }

        protected static void AddReference(Timer timer)
        {
            _runningTimers.Add(timer);
        }

        protected static void RemoveReference(Timer timer)
        {
            _runningTimers.Remove(timer);
        }

        #endregion

        #region DECONSTRUCTOR

        ~Timer()
        {
            try
            {
                Timer.RemoveReference(this);
            }
            catch
            {
                //do nothing if we failed...
            }
        }

        #endregion

    }

}