using UnityEngine;
using System.Collections.Generic;
using com.spacepuppy.Collections;

namespace com.spacepuppy.Timers
{
    public static class GameTimers
    {

        #region Fields

        private static TimerCollection _activeTimers = new TimerCollection();

        #endregion

        #region CONSTRUCTOR

        static GameTimers()
        {
            GameLoopEntry.OnUpdate += OnUpdate;
        }

        #endregion

        #region Event Handlers

        private static void OnUpdate(object sender, System.EventArgs e)
        {
            _activeTimers.Update(Time.deltaTime);
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