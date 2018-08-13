using UnityEngine;
using System.Collections.Generic;
using com.spacepuppy.Collections;

namespace com.spacepuppy.ClassicTimers
{

    public class GameTimer : Timer
    {

        #region Events

        public event System.Action<GameTimer> TimerCount;
        public event System.Action<GameTimer> TimerComplete;

        #endregion

        #region Fields

        private float _delay;
        private int _repeatCount;

        private float _currentTime;
        private int _currentCount;

        //if true, when the timer completes it will attempt to clean the timer pool of gypsy timers
        private bool _isGypsy;

        #endregion

        #region CONSTRUCTOR

        public GameTimer()
        {
            
        }

        /// <summary>
        /// Create a timer with a delay in seconds and a certain repeat count. A repeat count of 0 continues forever.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="repeatCount"></param>
        public GameTimer(float delay, int repeatCount)
        {
            _delay = delay;
            _repeatCount = repeatCount;
        }

        #endregion

        #region Properties

        public float CurrentTime
        {
            get { return _currentTime; }
        }

        public int CurrentCount
        {
            get { return _currentCount; }
        }

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        /// <summary>
        /// Number of times the interval should repeat before completing. Negative values repeat forever.
        /// </summary>
        public int RepeatCount
        {
            get { return _repeatCount; }
            set { _repeatCount = value; }
        }

        #endregion

        #region Methods

        public override void Start()
        {
            if (_repeatCount > 0 && _currentCount >= _repeatCount) return;

            base.Start();
        }

        public void Start(float delay, int repeatCount)
        {
            this.Delay = delay;
            this.RepeatCount = repeatCount;

            this.Start();
        }

        public void Complete()
        {
            _currentCount = _repeatCount;
            _currentTime = 0;

            if (this.TimerCount != null) this.TimerCount(this);
            if (this.TimerComplete != null) this.TimerComplete(this);

            this.Stop();
        }

        public void Reset()
        {
            this.Stop();

            _currentCount = 0;
            _currentTime = 0;
        }

        protected override void UpdateTimer(float dt)
        {
            if (!this.IsRunning) return;

            _currentTime += dt;

            while (_currentTime > _delay)
            {
                _currentTime -= _delay;
                _currentCount++;

                if (this.TimerCount != null) this.TimerCount(this);

                if (_repeatCount >= 0 && _currentCount > _repeatCount)
                {
                    //manually stop, don't call stop
                    this.IsRunning = false;
                    if (this.TimerComplete != null) this.TimerComplete(this);

                    //manually remove timer after calling TimerComplete event
                    //this way if it's a gypsy timer it doesn't get cleaned before the event is fired
                    Timer.RemoveReference(this);
                    if (_isGypsy)
                    {
                        this.Dispose();
                    }
                    return;
                }
            }
        }

        #endregion

        #region IDisposable Interface

        protected override void Dispose(bool disposing)
        {
            base.Dispose();

            if(disposing && _isGypsy)
            {
                _timerPool.Release(this);
            }
        }

        #endregion

        #region Static Interface

        /// <summary>
        /// these timers are used as gypsy timers, said timers only exist for a short period of time and are in a 
        /// state of constant play (they should not ever be stopped). When they finish playing they're returned to 
        /// the pool for future use. They're pooled for quick propogation.
        /// </summary>
        private static ObjectCachePool<GameTimer> _timerPool = new ObjectCachePool<GameTimer>(-1,
        () =>
        {
            var t = new GameTimer();
            t._isGypsy = true;
            return t;
        },
        (t) =>
        {
            t._delay = 0;
            t._repeatCount = 0;
            t._currentTime = 0;
            t._currentCount = 0;
            t.TimerComplete = null;
            t.TimerCount = null;
            t.IsRunning = false;
            t.IsDisposed = false;
        });

        /// <summary>
        /// quickly generates a timer for a specific duration, when accessing the count event do NOT attempt to store 
        /// a reference to the timer as it is a pooled asset and can quickly change to an unstable state.
        /// </summary>
        /// <param name="delay">Duration in seconds.</param>
        /// <param name="complete">Delegate fired when the timer completes.</param>
        /// <param name="count">Delegate fired when the time ticks.</param>
        public static void CreateGypsyTimer(float delay, System.Action<GameTimer> complete, System.Action<GameTimer> count)
        {
            var t = _timerPool.GetInstance();
            t.Delay = delay;
            t.RepeatCount = 0;

            if (complete == null) throw new System.ArgumentNullException("complete", "complete event handler must be non-null");

            t.TimerComplete += complete;
            if (count != null) t.TimerCount += count;

            t.Start();
        }

        public static void CreateGypsyTimer(float delay, System.Action<GameTimer> complete)
        {
            CreateGypsyTimer(delay, complete, null);
        }

        #endregion

    }

}