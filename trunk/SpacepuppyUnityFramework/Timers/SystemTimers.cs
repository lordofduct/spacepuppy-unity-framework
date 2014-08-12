using UnityEngine;
using System.Collections.Generic;
using com.spacepuppy.Collections;

namespace com.spacepuppy.Timers
{

    /// <summary>
    /// Timers that operate on a system thread independent of the game thread. The update events of these timers are not thread safe with the main thread.
    /// </summary>
    public static class SystemTimers
    {

        #region Fields

        private static TimerCollection _activeTimers = new TimerCollection();
        private static System.Timers.Timer _hookTimer;

        #endregion
        
        #region CONSTRUCTOR

        static SystemTimers()
        {
            _hookTimer = new System.Timers.Timer(10); //every 10 milliseconds
            _hookTimer.Elapsed += OnUpdate;
            _hookTimer.Start();

            //GameLoopEntry.EarlyUpdate += OnUpdate;
        }

        #endregion

        #region Event Handlers

        private static void OnUpdate(object sender, System.EventArgs e)
        {
//#if UNITY_EDITOR
//            if (!Application.isPlaying) return;
//#endif

            _activeTimers.Update((float)_hookTimer.Interval / 1000f);
            //_activeTimers.Update(Time.unscaledDeltaTime);
        }

        #endregion

        #region Methods

        public static void Start(ITimer t)
        {
            _activeTimers.Start(t);
        }

        public static bool IsActive(ITimer t)
        {
            return _activeTimers.IsActive(t);
        }

        public static void Stop(ITimer t)
        {
            _activeTimers.Stop(t);
        }

        public static void CreateGypsyTimer(float delay, System.Action<Timer> complete)
        {
            if (complete == null) throw new System.ArgumentNullException("complete", "complete event handler must be non-null");

            var t = new Timer(delay, 1);
            t.TimerComplete += complete;
            Start(t);
        }

        public static void CreateGypsyTimer(float delay, int repeatCount, System.Action<Timer> complete, System.Action<Timer> count)
        {
            if (complete == null) throw new System.ArgumentNullException("complete", "complete event handler must be non-null");

            var t = new Timer(delay, repeatCount);
            t.TimerComplete += complete;
            if (count != null) t.TimerCount += count;
            Start(t);
        }

        #endregion

    }

}