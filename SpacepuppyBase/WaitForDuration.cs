using UnityEngine;
using System.Collections;

namespace com.spacepuppy
{

    /// <summary>
    /// Represents a duration of time to wait for that can be paused, and can use various kinds of TimeSuppliers. 
    /// Reset must be called if reused.
    /// 
    /// NOTE - If you use the WaitForDuration objects that are spawned by the static factory methods, these objects are 
    /// pooled. The WaitForDuration object should be yielded immediately in a RadicalCoroutine and not reused.
    /// </summary>
    public class WaitForDuration : IRadicalEnumerator, IPausibleYieldInstruction, IProgressingYieldInstruction
    {

#if SP_LIB

        #region Events

        public event System.EventHandler OnComplete;

        #endregion

        #region Fields

        private ITimeSupplier _supplier;
        private double _t;
        private double _lastTotal;
        private float _dur;

        #endregion

        #region CONSTRUCTOR

        public WaitForDuration(float dur, ITimeSupplier supplier)
        {
            this.Reset(dur, supplier);
        }

        internal WaitForDuration()
        {
            //allow overriding
        }

        #endregion

        #region Properties

        public float Duration { get { return _dur; } }

        public double CurrentTime { get { return _t; } }

        #endregion

        #region Methods

        public void Reset()
        {
            _t = 0d;
            _lastTotal = double.NegativeInfinity;
        }

        public void Reset(float dur, ITimeSupplier supplier)
        {
            _supplier = supplier ?? SPTime.Normal;
            _dur = dur;
            _t = 0d;
            _lastTotal = double.NegativeInfinity;
        }

        public void Cancel()
        {
            _t = double.PositiveInfinity;
            _lastTotal = double.NegativeInfinity;
        }

        private bool Tick()
        {
            if (this.IsComplete) return false;

            if (double.IsNaN(_lastTotal))
            {
                //we're paused
            }
            else if (_lastTotal == double.NegativeInfinity)
            {
                //first time being called
                _lastTotal = _supplier.TotalPrecise;
                if (_dur <= 0f && _t <= 0d && this.OnComplete != null)
                {
                    _t = 0d;
                    this.OnComplete(this, System.EventArgs.Empty);
                    return false;
                }
            }
            else
            {
                _t += (_supplier.TotalPrecise - _lastTotal);
                _lastTotal = _supplier.TotalPrecise;
                if (this.OnComplete != null && this.IsComplete)
                {
                    this.OnComplete(this, System.EventArgs.Empty);
                    return false;
                }
            }

            return !this.IsComplete;
        }

        protected void Dispose()
        {
            this.OnComplete = null;
            _supplier = null;
            _t = 0d;
            _lastTotal = float.NegativeInfinity;
            _dur = 0f;
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        public bool IsComplete
        {
            get { return _t >= _dur; }
        }

        float IProgressingYieldInstruction.Progress
        {
            get { return Mathf.Clamp01((float)(_t / _dur)); }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return this.Tick();
        }

        void IPausibleYieldInstruction.OnPause()
        {
            _lastTotal = double.NaN;
        }

        void IPausibleYieldInstruction.OnResume()
        {
            _lastTotal = _supplier.TotalPrecise;
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
            if (this.Tick())
            {
                return true;
            }
            else
            {
                //in case a PooledYieldInstruction was used in a regular Unity Coroutine
                if (this is IPooledYieldInstruction)
                    (this as IPooledYieldInstruction).Dispose();
                return false;
            }
        }

        void IEnumerator.Reset()
        {
            this.Reset();
        }

        #endregion
        
        #region Static Interface

        private static com.spacepuppy.Collections.ObjectCachePool<PooledWaitForDuration> _pool = new com.spacepuppy.Collections.ObjectCachePool<PooledWaitForDuration>(-1, () => new PooledWaitForDuration());
        private class PooledWaitForDuration : WaitForDuration, IPooledYieldInstruction
        {

            void System.IDisposable.Dispose()
            {
                this.Dispose();
                _pool.Release(this);
            }

        }

        /// <summary>
        /// Create a WaitForDuration in seconds as a pooled object.
        /// 
        /// NOTE - This retrieves a pooled WaitForDuration that should be used only once. It should be immediately yielded and not used again.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static WaitForDuration Seconds(float seconds, ITimeSupplier supplier = null)
        {
            var w = _pool.GetInstance();
            w.Reset(seconds, supplier);
            return w;
        }

        /// <summary>
        /// Create a WaitForDuration from a WaitForSeconds object as a pooled object.
        /// 
        /// NOTE - This retrieves a pooled WaitForDuration that should be used only once. It should be immediately yielded and not used again.
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="returnNullIfZero"></param>
        /// <returns></returns>
        public static WaitForDuration FromWaitForSeconds(WaitForSeconds wait, bool returnObjEvenIfZero = false)
        {
            //var dur = ConvertUtil.ToSingle(DynamicUtil.GetValue(wait, "m_Seconds"));
            var field = typeof(WaitForSeconds).GetField("m_Seconds", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            float dur = (float)field.GetValue(wait);
            if (dur <= 0f && !returnObjEvenIfZero)
            {
                return null;
            }
            else
            {
                var w = _pool.GetInstance();
                w.Reset(dur, SPTime.Normal);
                return w;
            }
        }

        /// <summary>
        /// Create a WaitForDuration from a SPTimePeriod as a pooled object.
        /// 
        /// NOTE - This retrieves a pooled WaitForDuration that should be used only once. It should be immediately yielded and not used again.
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static WaitForDuration Period(SPTimePeriod period)
        {
            var w = _pool.GetInstance();
            w.Reset((float)period.Seconds, period.TimeSupplier);
            return w;
        }

        #endregion

#else

        #region Events

        public event System.EventHandler OnComplete;

        #endregion

        #region Fields
        
        private double _t;
        private double _lastTotal;
        private float _dur;

        #endregion

        #region CONSTRUCTOR

        public WaitForDuration(float dur)
        {
            this.Reset(dur);
        }

        internal WaitForDuration()
        {
            //allow overriding
        }

        #endregion

        #region Properties

        public float Duration { get { return _dur; } }

        public double CurrentTime { get { return _t; } }

        #endregion

        #region Methods

        public void Reset()
        {
            _t = 0d;
            _lastTotal = double.NegativeInfinity;
        }

        public void Reset(float dur)
        {
            _dur = dur;
            _t = 0d;
            _lastTotal = double.NegativeInfinity;
        }

        public void Cancel()
        {
            _t = double.PositiveInfinity;
            _lastTotal = double.NegativeInfinity;
        }

        private bool Tick()
        {
            if (this.IsComplete) return false;

            if (double.IsNaN(_lastTotal))
            {
                //we're paused
            }
            else if (_lastTotal == double.NegativeInfinity)
            {
                //first time being called
                _lastTotal = Time.time;
                if (_dur <= 0f && _t <= 0d && this.OnComplete != null)
                {
                    _t = 0d;
                    this.OnComplete(this, System.EventArgs.Empty);
                    return false;
                }
            }
            else
            {
                _t += (Time.time - _lastTotal);
                _lastTotal = Time.time;
                if (this.OnComplete != null && this.IsComplete)
                {
                    this.OnComplete(this, System.EventArgs.Empty);
                    return false;
                }
            }

            return !this.IsComplete;
        }

        protected void Dispose()
        {
            this.OnComplete = null;
            _t = 0d;
            _lastTotal = float.NegativeInfinity;
            _dur = 0f;
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        public bool IsComplete
        {
            get { return _t >= _dur; }
        }

        float IProgressingYieldInstruction.Progress
        {
            get { return Mathf.Clamp01((float)(_t / _dur)); }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return this.Tick();
        }

        void IPausibleYieldInstruction.OnPause()
        {
            _lastTotal = double.NaN;
        }

        void IPausibleYieldInstruction.OnResume()
        {
            _lastTotal = Time.time;
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
            if (this.Tick())
            {
                return true;
            }
            else
            {
                //in case a PooledYieldInstruction was used in a regular Unity Coroutine
                if (this is IPooledYieldInstruction)
                    (this as IPooledYieldInstruction).Dispose();
                return false;
            }
        }

        void IEnumerator.Reset()
        {
            this.Reset();
        }

        #endregion

        #region Static Interface

        private static com.spacepuppy.Collections.ObjectCachePool<PooledWaitForDuration> _pool = new com.spacepuppy.Collections.ObjectCachePool<PooledWaitForDuration>(-1, () => new PooledWaitForDuration());
        private class PooledWaitForDuration : WaitForDuration, IPooledYieldInstruction
        {

            void System.IDisposable.Dispose()
            {
                this.Dispose();
                _pool.Release(this);
            }

        }

        /// <summary>
        /// Create a WaitForDuration in seconds as a pooled object.
        /// 
        /// NOTE - This retrieves a pooled WaitForDuration that should be used only once. It should be immediately yielded and not used again.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static WaitForDuration Seconds(float seconds)
        {
            var w = _pool.GetInstance();
            w.Reset(seconds);
            return w;
        }

        /// <summary>
        /// Create a WaitForDuration from a WaitForSeconds object as a pooled object.
        /// 
        /// NOTE - This retrieves a pooled WaitForDuration that should be used only once. It should be immediately yielded and not used again.
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="returnNullIfZero"></param>
        /// <returns></returns>
        public static WaitForDuration FromWaitForSeconds(WaitForSeconds wait, bool returnObjEvenIfZero = false)
        {
            //var dur = ConvertUtil.ToSingle(DynamicUtil.GetValue(wait, "m_Seconds"));
            var field = typeof(WaitForSeconds).GetField("m_Seconds", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            float dur = (float)field.GetValue(wait);
            if (dur <= 0f && !returnObjEvenIfZero)
            {
                return null;
            }
            else
            {
                var w = _pool.GetInstance();
                w.Reset(dur);
                return w;
            }
        }

        #endregion

#endif

    }

}
