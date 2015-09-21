using UnityEngine;
using System.Collections;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy
{

    /// <summary>
    /// Just a name for contract purposes. You should probably inherit from RadicalYieldInstruciton, or composite it, unless 
    /// you know what you're doing.
    /// </summary>
    public interface IRadicalYieldInstruction
    {
        bool IsComplete { get; }

        ///// <summary>
        ///// This method is called every tick of the RadicalCoroutine that is handling it.
        ///// </summary>
        ///// <returns>Return if the RadicalCoroutine should continue blocking (IsComplete should be false if this returns true)</returns>
        //bool ContinueBlocking();
        //object CurrentYieldObject { get; }

        bool Tick(out object yieldObject);

    }

    /// <summary>
    /// A contract joint of IEnumerator and IRadicalYieldInstruction. This can be implemented by an object that can be used in a dual way as either a IEnuemrator or a yield statement. 
    /// This is useful for objects that can be jsut directly tossed into 'MonoBehaviour.StartCoroutine' and do their job, or used as a yield instruction.
    /// </summary>
    public interface IRadicalEnumerator : System.Collections.IEnumerator, IRadicalYieldInstruction
    {
        //just a contract
    }

    /// <summary>
    /// Implement this if your RadicalYieldInstruction should have a progress property.
    /// </summary>
    public interface IProgressingYieldInstruction : IRadicalYieldInstruction
    {
        float Progress { get; }
    }

    /// <summary>
    /// Implement this if the coroutine should immediately resume on complete instead of waiting to the next frame. This is used 
    /// in situations that are similar to WaitForFixedUpdate, where we want to wait for some section of the update pipeline and 
    /// need to operate immediately.
    /// </summary>
    public interface IImmediatelyResumingYieldInstruction : IRadicalYieldInstruction
    {
        event System.EventHandler Signal;
    }

    /// <summary>
    /// Receive a signal that the yielding RadicalCoroutine was paused, so that the instruction can deal with that state accordingly 
    /// if it must do something special. For example WaitForDuration wants to store the time so that when the coroutine resumes 
    /// it starts counting from the appropriate time instead of appearing finished.
    /// </summary>
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

    /// <summary>
    /// By implementing this interface, the RadicalCoroutine knows to call Reset any time it receives the instruction 
    /// from the routine being operated. This way an instruction can clean itself up for reuse if yielded more than once.
    /// </summary>
    public interface IResettingYieldInstruction : IRadicalYieldInstruction
    {

        void Reset();

    }

    public interface IRadicalWaitHandle : IRadicalYieldInstruction
    {

        void OnComplete(System.Action<IRadicalWaitHandle> callback);

    }

    /// <summary>
    /// Base abstract class that implements IRadicalYieldInstruction. It implements IRadicalYieldInstruction in the most 
    /// commonly used setup. You should only ever implement IRadicalYieldInstruction directly if you can't inherit from this 
    /// in your inheritance chain, or you want none standard behaviour.
    /// </summary>
    public abstract class RadicalYieldInstruction : IRadicalYieldInstruction
    {

        #region Fields

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

        protected void ResetSignal()
        {
            _complete = false;
        }

        protected virtual bool Tick(out object yieldObject)
        {
            yieldObject = null;
            return !_complete;
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if(_complete)
            {
                yieldObject = null;
                return false;
            }

            return this.Tick(out yieldObject);
        }

        #endregion



        #region Static Interface

        public static IProgressingYieldInstruction Null
        {
            get
            {
                return NullYieldInstruction.Null;
            }
        }

        #endregion

    }

    public class NullYieldInstruction : IProgressingYieldInstruction, IRadicalWaitHandle
    {

        #region COSNTRUCTOR

        private NullYieldInstruction()
        {

        }

        #endregion

        #region IRadicalYieldInstruction Interface

        bool IRadicalYieldInstruction.IsComplete
        {
            get { return true; }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return false;
        }

        float IProgressingYieldInstruction.Progress
        {
            get { return 1f; }
        }

        void IRadicalWaitHandle.OnComplete(System.Action<IRadicalWaitHandle> callback)
        {
            //do nothing
        }

        #endregion

        #region Static Interface

        private static NullYieldInstruction _null = new NullYieldInstruction();
        public static NullYieldInstruction Null
        {
            get
            {
                return _null;
            }
        }

        #endregion

    }

    /// <summary>
    /// Base abstract class that implement IImmediatelyResumingYieldInstruction. Inherit from this instead of directly implementing 
    /// IImmediatelyResumingYieldInstruction, unless you can't otherwise.
    /// </summary>
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

    /// <summary>
    /// Base abstract class that implements IProgressingYieldInstruction. Inherit from this instead of directly implementing 
    /// IProgressingYieldInstruction, unless you can't otherwise.
    /// </summary>
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

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            for (int i = 0; i < _operations.Length; i++)
            {
                if (!_operations[i].IsComplete)
                {
                    return _operations[i].Tick(out yieldObject);
                }
            }
            yieldObject = null;
            return false;
        }

        #endregion

    }

    public class RadicalWaitHandle : IRadicalWaitHandle, IPooledYieldInstruction
    {

        #region Fields

        private bool _complete;
        private System.Action<IRadicalWaitHandle> _callback;

        #endregion

        #region CONSTRUCTOR

        protected RadicalWaitHandle()
        {

        }

        #endregion

        #region Methods

        public void SignalComplete()
        {
            if (_complete) return;

            _complete = true;
            if (_callback != null) _callback(this);
        }

        protected virtual bool Tick(out object yieldObject)
        {
            yieldObject = null;
            return !_complete;
        }

        #endregion

        #region IRadicalWaitHandle Interface

        public bool IsComplete
        {
            get { return _complete; }
        }

        public void OnComplete(System.Action<IRadicalWaitHandle> callback)
        {
            if (callback == null) throw new System.ArgumentNullException("callback");
            if (_complete) throw new System.InvalidOperationException("Can not wait for complete on an already completed IRadicalWaitHandle.");
            _callback += callback;
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if(_complete)
            {
                yieldObject = null;
                return false;
            }

            return this.Tick(out yieldObject);
        }

        #endregion

        #region IPooledYieldInstruction Interface

        void System.IDisposable.Dispose()
        {
            if(this.GetType() == typeof(RadicalWaitHandle))
            {
                _complete = false;
                _callback = null;
                _pool.Release(this);
            }
        }

        #endregion

        #region Static Interface

        private static com.spacepuppy.Collections.ObjectCachePool<RadicalWaitHandle> _pool = new com.spacepuppy.Collections.ObjectCachePool<RadicalWaitHandle>(-1, () => new RadicalWaitHandle());

        public static IRadicalWaitHandle Null
        {
            get
            {
                return NullYieldInstruction.Null;
            }
        }

        public static RadicalWaitHandle Create()
        {
            return _pool.GetInstance();
        }

        #endregion

    }

    /// <summary>
    /// Represents a duration of time to wait for that can be paused, and can use various kinds of TimeSuppliers. 
    /// Reset must be called if reused.
    /// 
    /// NOTE - If you use the WaitForDuration objects that are spawned by the static factory methods, these objects are 
    /// pooled. The WaitForDuration object should be yielded immediately in a RadicalCoroutine and not reused.
    /// </summary>
    public class WaitForDuration : IRadicalEnumerator, IPausibleYieldInstruction, IProgressingYieldInstruction
    {

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
                if(_dur <= 0f && _t <= 0d && this.OnComplete != null)
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
            if(this.Tick())
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

            void IDisposable.Dispose()
            {
                this.Dispose();
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
        public static WaitForDuration FromWaitForSeconds(WaitForSeconds wait, bool returnNullIfZero = true)
        {
            var dur = ConvertUtil.ToSingle(DynamicUtil.GetValue(wait, "m_Seconds"));
            if (returnNullIfZero && dur <= 0f)
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

    }



    /// <summary>
    /// Blocks until the dispatcher fires some notification. See the notification system in com.spacepuppy.Notification.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

    }

    /// <summary>
    /// A composite yield instruction that waits for multiple instructions to all complete before continuing.
    /// </summary>
    public class WaitForAllComplete : RadicalYieldInstruction
    {

        private MonoBehaviour _handle;
        private System.Collections.Generic.List<object> _instructions;
        private int _waitCount;

        public WaitForAllComplete(MonoBehaviour handle, params object[] instructions)
        {
            _handle = handle;
            _instructions = new System.Collections.Generic.List<object>(instructions);
        }

        protected override bool Tick(out object yieldObject)
        {
            if(this.IsComplete)
            {
                yieldObject = null;
                return false;
            }

            object current;
            for (int i = 0; i < _instructions.Count; i++)
            {
                current = _instructions[i];
                if (current == null)
                {
                    _instructions.RemoveAt(i);
                    i--;
                }
                else if (current is YieldInstruction || current is WWW)
                {
                    _instructions.RemoveAt(i);
                    i--;
                    _handle.StartCoroutine(this.WaitForStandard(current));
                }
                else if (current is RadicalCoroutine)
                {
                    if ((current as RadicalCoroutine).Complete)
                    {
                        _instructions.RemoveAt(i);
                        i--;
                    }
                }
                else if (current is IRadicalYieldInstruction)
                {
                    object sub;
                    if ((current as IRadicalYieldInstruction).Tick(out sub))
                    {
                        if (sub != null)
                        {
                            _instructions[i] = _handle.StartRadicalCoroutine(this.WaitForRadical(current as IRadicalYieldInstruction));
                        }
                    }
                    else
                    {
                        _instructions.RemoveAt(i);
                        i--;
                    }
                }
                else if (current is IEnumerable)
                {
                    var e = (current as IEnumerable).GetEnumerator();
                    _instructions[i] = e;
                    if (e.MoveNext())
                    {
                        if (e.Current != null) _instructions.Add(e.Current);
                    }
                    else
                    {
                        _instructions.RemoveAt(i);
                        i--;
                    }
                }
                else if (current is IEnumerator)
                {
                    var e = current as IEnumerator;
                    if (e.MoveNext())
                    {
                        if (e.Current != null) _instructions.Add(e.Current);
                    }
                    else
                    {
                        _instructions.RemoveAt(i);
                        i--;
                    }
                }
                else
                {
                    _instructions[i] = null;
                }
            }

            if (_instructions.Count == 0 && _waitCount <= 0)
            {
                this.SetSignal();
                yieldObject = null;
                return false;
            }
            else
            {
                yieldObject = null;
                return true;
            }
        }

        private IEnumerator WaitForStandard(object inst)
        {
            _waitCount++;
            yield return inst;
            _waitCount--;
        }

        private IEnumerator WaitForRadical(IRadicalYieldInstruction inst)
        {
            object yieldObject;
            while (inst.Tick(out yieldObject))
            {
                yield return yieldObject;
            }
        }

    }

    /// <summary>
    /// A composite yield instruction that waits for any one of multiple instruction to complete before continuing.
    /// </summary>
    public class WaitForAnyComplete : RadicalYieldInstruction
    {

        private MonoBehaviour _handle;
        private System.Collections.Generic.List<object> _instructions;
        private System.Collections.Generic.List<object> _waitingRoutines = new System.Collections.Generic.List<object>();
        private bool _signalNextTime;

        public WaitForAnyComplete(MonoBehaviour handle, params object[] instructions)
        {
            _handle = handle;
            _instructions = new System.Collections.Generic.List<object>(instructions);
        }

        protected override void SetSignal()
        {
            object obj;
            for (int i = 0; i < _waitingRoutines.Count; i++)
            {
                obj = _waitingRoutines[i];
                if (obj is Coroutine)
                    _handle.StopCoroutine(obj as Coroutine);
                else if (obj is RadicalCoroutine)
                    (obj as RadicalCoroutine).Cancel();
            }
            _waitingRoutines.Clear();
            base.SetSignal();
        }

        protected override bool Tick(out object yieldObject)
        {
            yieldObject = null;
            if (this.IsComplete) return false;
            if (_signalNextTime)
            {
                this.SetSignal();
                return false;
            }

            object current;
            for (int i = 0; i < _instructions.Count; i++)
            {
                current = _instructions[i];
                if (current == null)
                {
                    _signalNextTime = true;
                }
                else if (current is YieldInstruction || current is WWW)
                {
                    _instructions.RemoveAt(i);
                    i--;
                    _waitingRoutines.Add(_handle.StartCoroutine(this.WaitForStandard(current)));
                }
                else if (current is RadicalCoroutine)
                {
                    if ((current as RadicalCoroutine).Complete)
                    {
                        this.SetSignal();
                        return false;
                    }
                }
                else if (current is IRadicalYieldInstruction)
                {
                    object sub;
                    if ((current as IRadicalYieldInstruction).Tick(out sub))
                    {
                        if (sub != null)
                        {
                            _instructions.RemoveAt(i);
                            i--;
                            _waitingRoutines.Add(_handle.StartRadicalCoroutine(this.WaitForRadical(current)));
                        }
                    }
                    else
                    {
                        this.SetSignal();
                        return false;
                    }
                }
                else if (current is IEnumerator || current is IEnumerable)
                {
                    _instructions.RemoveAt(i);
                    i--;
                    _waitingRoutines.Add(_handle.StartRadicalCoroutine(this.WaitForRadical(current)));
                }
                else
                {
                    _signalNextTime = true;
                }
            }

            return true;
        }

        private IEnumerator WaitForStandard(object inst)
        {
            yield return inst;
            this.SetSignal();
        }

        private IEnumerator WaitForRadicalYield(IRadicalYieldInstruction inst)
        {
            object yieldObject;
            while (inst.Tick(out yieldObject))
            {
                yield return yieldObject;
            }
        }

        private IEnumerator WaitForRadical(object inst)
        {
            yield return inst;
            this.SetSignal();
        }

    }

}
