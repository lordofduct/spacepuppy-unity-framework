using UnityEngine;
using System.Collections;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public class WaitUntilTime : IRadicalEnumerator
    {

        #region Fields

        private ITimeSupplier _timeSupplier;
        private float _finishTime;

        #endregion

        #region CONSTRUCTOR

        protected WaitUntilTime()
        {
            //for resusable factory
        }

        public WaitUntilTime(float time, ITimeSupplier timeSupplier)
        {
            this.Init(time, timeSupplier);
        }

        private void Init(float time, ITimeSupplier timeSupplier)
        {
            _finishTime = time;
            _timeSupplier = timeSupplier ?? SPTime.Normal;
        }

        #endregion

        #region Methods

        protected void Dispose()
        {
            _timeSupplier = null;
            _finishTime = 0f;
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        public bool IsComplete
        {
            get
            {
                return _timeSupplier.Total >= _finishTime;
            }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return !this.IsComplete;
        }

        #endregion

        #region IEnumerator Interface

        object IEnumerator.Current
        {
            get
            {
                return null;
            }
        }

        bool IEnumerator.MoveNext()
        {
            return !this.IsComplete;
        }

        void IEnumerator.Reset()
        {
            //this.Reset();
        }

        #endregion

        #region Static Factory

        private static com.spacepuppy.Collections.ObjectCachePool<PooledWaitForNextInterval> _pool = new com.spacepuppy.Collections.ObjectCachePool<PooledWaitForNextInterval>(-1, () => new PooledWaitForNextInterval());
        private class PooledWaitForNextInterval : WaitUntilTime, IPooledYieldInstruction
        {

            void System.IDisposable.Dispose()
            {
                this.Dispose();
                _pool.Release(this);
            }

        }

        public WaitUntilTime Time(float time, ITimeSupplier timeSupplier = null)
        {
            var obj = _pool.GetInstance();
            obj.Init(time, timeSupplier);
            return obj;
        }

        public WaitUntilTime NextInterval(float interval, ITimeSupplier timeSupplier = null)
        {
            var obj = _pool.GetInstance();
            var ts = timeSupplier ?? SPTime.Normal;
            float t = MathUtil.FloorToInterval(ts.Total, interval) + interval;
            obj.Init(t, ts);
            return obj;
        }

        public WaitUntilTime NextInterval(float interval, float offset, ITimeSupplier timeSupplier = null)
        {
            var obj = _pool.GetInstance();
            var ts = timeSupplier ?? SPTime.Normal;
            float t = MathUtil.FloorToInterval(ts.Total, interval, offset) + interval;
            obj.Init(t, ts);
            return obj;
        }

        public WaitUntilTime NextInterval(SPTimePeriod interval)
        {
            var obj = _pool.GetInstance();
            var ts = interval.TimeSupplier ?? SPTime.Normal;
            float t = MathUtil.FloorToInterval(ts.Total, interval.Seconds) + interval.Seconds;
            obj.Init(t, ts);
            return obj;
        }

        public WaitUntilTime NextInterval(SPTimePeriod interval, float offset)
        {
            var obj = _pool.GetInstance();
            var ts = interval.TimeSupplier ?? SPTime.Normal;
            float t = MathUtil.FloorToInterval(ts.Total, interval.Seconds, offset) + interval.Seconds;
            obj.Init(t, ts);
            return obj;
        }

        #endregion

    }

}
