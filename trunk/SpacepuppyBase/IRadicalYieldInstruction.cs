using UnityEngine;
using System.Collections;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Just a name for contract purposes.
    /// </summary>
    public interface IRadicalYieldInstruction : System.Collections.IEnumerator
    {
    }

    public interface IProgressingYieldInstruction : IRadicalYieldInstruction
    {
        bool IsComplete { get; }
        float Progress { get; }
    }

    public interface IImmediatelyResumingYieldInstruction : IRadicalYieldInstruction
    {
        event System.EventHandler Signal;
    }

    public interface IPausibleYieldInstruction : IRadicalYieldInstruction
    {

        void OnPause();
        void OnResume();

    }

    /// <summary>
    /// By implementing this interface, the RadicalCoroutine knows to call Dispose on the instruction when it is 
    /// done yielding for its duration. The instruction can than return itself to a object cache pool during Dispose 
    /// for reuse later.
    /// </summary>
    public interface IPooledYieldInstruction : IRadicalYieldInstruction, System.IDisposable
    {

    }

    public abstract class RadicalYieldInstruction : IRadicalYieldInstruction
    {

        #region Fields

        private object _current;
        private bool _complete;

        #endregion

        #region Properties

        public bool IsComplete { get { return _complete; } }

        #endregion

        #region Methods

        protected virtual void SetSignal()
        {
            _complete = true;
        }

        protected abstract object Tick();

        protected virtual void OnPause()
        {

        }

        protected virtual void OnResume()
        {

        }

        #endregion

        #region IRadicalYieldInstruction Interface

        object IEnumerator.Current
        {
            get { return _current; }
        }

        bool IEnumerator.MoveNext()
        {
            _current = this.Tick();
            return !_complete;
        }

        void IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion



        #region Static Interface

        private static IProgressingYieldInstruction _null = new NullYieldInstruction();
        public static IProgressingYieldInstruction Null
        {
            get
            {
                return _null;
            }
        }

        private class NullYieldInstruction : IProgressingYieldInstruction
        {

            public bool IsComplete
            {
                get { return true; }
            }

            public float Progress
            {
                get { return 1f; }
            }

            public object Current
            {
                get { return null; }
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {

            }

            public void OnPause()
            {
            }

            public void OnResume()
            {
            }
        }

        #endregion

    }

    public abstract class ImmediatelyResumingYieldInstruction : RadicalYieldInstruction, IImmediatelyResumingYieldInstruction
    {

        private System.EventHandler _handler;

        protected override void SetSignal()
        {
            base.SetSignal();
            if (_handler != null) _handler(this, System.EventArgs.Empty);
        }

        event System.EventHandler IImmediatelyResumingYieldInstruction.Signal
        {
            add { _handler += value; }
            remove { _handler -= value; }
        }

    }

    public class ProgressingYieldInstructionQueue : IProgressingYieldInstruction
    {
        
        #region Fields

        private IProgressingYieldInstruction[] _operations;
        private bool _complete;

        #endregion

        #region CONSTRUCTOR

        public ProgressingYieldInstructionQueue(params IProgressingYieldInstruction[] operations)
        {
            if (operations == null) throw new System.ArgumentNullException("operations");
            _operations = operations;
        }

        #endregion

        #region IProgressingAsyncOperation Interface

        public float Progress
        {
            get
            {
                if (_operations.Length == 0) return 1f;
                else if (_operations.Length == 1) return _operations[0].Progress;
                else
                {
                    float p = 0f;
                    for (int i = 0; i < _operations.Length; i++)
                    {
                        p += _operations[i].Progress;
                    }
                    return p / _operations.Length;
                }
            }
        }

        public bool IsComplete
        {
            get
            {
                if (_complete) return true;
                else
                {
                    for (int i = 0; i < _operations.Length; i++)
                    {
                        if (!_operations[i].IsComplete) return false;
                    }
                    _complete = true;
                    return true;
                }
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                for (int i = 0; i < _operations.Length; i++)
                {
                    if (!_operations[i].IsComplete) return _operations[i];
                }
                return null;
            }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return !this.IsComplete;
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion

    }

    /// <summary>
    /// Represents a duration of time to wait for that can be paused, and can use various kinds of TimeSuppliers. 
    /// NOTE - this yield instruction is pooled, NEVER store one for reuse. The pool takes care of that for you. Instead 
    /// use the static factory methods.
    /// </summary>
    public class WaitForDuration : IPausibleYieldInstruction, IPooledYieldInstruction
    {

        #region Fields

        private ITimeSupplier _supplier;
        private float _t;
        private float _cache;
        private float _dur;

        #endregion

        #region CONSTRUCTOR

        private WaitForDuration()
        {

        }

        private void Init(float dur, ITimeSupplier supplier)
        {
            _supplier = supplier ?? SPTime.Normal;
            _dur = dur;
            (this as System.Collections.IEnumerator).Reset();
        }

        #endregion

        #region Properties

        public float Duration { get { return _dur; } }

        public float CurrentTime { get { return (float.IsNaN(_t)) ? _cache : (_supplier.Total - _t) + _cache; } }

        #endregion

        #region IEnumerator Interface

        object IEnumerator.Current
        {
            get { return null; }
        }

        bool IEnumerator.MoveNext()
        {
            return this.CurrentTime < _dur;
        }

        void IEnumerator.Reset()
        {
            _t = _supplier.Total;
            _cache = 0f;
        }

        void IPausibleYieldInstruction.OnPause()
        {
            _cache = (_supplier.Total - _t) + _cache;
            _t = float.NaN;
        }

        void IPausibleYieldInstruction.OnResume()
        {
            _t = Time.time;
        }

        #endregion

        #region IPooledYieldInstruction Interface

        void System.IDisposable.Dispose()
        {
            _pool.Release(this);
        }

        #endregion

        #region Static Factory

        private static com.spacepuppy.Collections.ObjectCachePool<WaitForDuration> _pool = new com.spacepuppy.Collections.ObjectCachePool<WaitForDuration>(1000, () => new WaitForDuration());

        public static WaitForDuration Seconds(float seconds, ITimeSupplier supplier = null)
        {
            var w = _pool.GetInstance();
            w.Init(seconds, supplier);
            return w;
        }

        public static WaitForDuration FromWaitForSeconds(WaitForSeconds wait, bool returnNullIfZero = true)
        {
            var dur = ConvertUtil.ToSingle(ObjUtil.GetValue(wait, "m_Seconds"));
            if (returnNullIfZero && dur <= 0f)
            {
                return null;
            }
            else
            {
                var w = _pool.GetInstance();
                w.Init(dur, SPTime.Normal);
                return w;
            }
        }

        #endregion

    }

    public class WaitForNotification<T> : ImmediatelyResumingYieldInstruction where T : Notification
    {

        private INotificationDispatcher _dispatcher;
        private T _notification;
        private System.Func<T, bool> _ignoreCheck;

        public WaitForNotification(INotificationDispatcher dispatcher, System.Func<T, bool> ignoreCheck = null)
        {
            _dispatcher = dispatcher;
            _dispatcher.RegisterObserver<T>(this.OnNotification);
            _ignoreCheck = ignoreCheck;
        }

        public T Notification { get { return _notification; } }

        private void OnNotification(object sender, T n)
        {
            if (_ignoreCheck != null && _ignoreCheck(n)) return;

            _notification = n;
            _dispatcher.RemoveObserver<T>(this.OnNotification);
            this.SetSignal();
        }

        protected override object Tick()
        {
            return null;
        }

    }

}
