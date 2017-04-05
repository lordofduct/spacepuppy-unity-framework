using UnityEngine;
using System.Collections;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Acts just like Coroutine, but with an expanded feature set.
    /// 
    /// *Custom Yield Instructions
    /// IYieldInstruction - ability to define reusable yield instructions as objects.
    /// IProgessingYieldInstruction - a customizable yield instruction that has a progress property
    /// IImmediatelyResumingYieldInstruction - a customizable yield instruction that will return to operation of the coroutine 
    ///     immediately on complete (i.e. return during FixedUpdate)
    /// IPausibleYieldInstruction - a yield instruction should implement this if it needs to deal with a coroutine pausing in 
    ///     any special way
    /// IPooledYieldInstruction - a yield instruction should implement this if when complete it needs to be returned to a pool
    /// IResettingYieldInstruction - a yield instruction should implement this if when complete it should be signaled so its 
    ///     state can be reset
    /// 
    /// *Events
    /// Register for events when the state of a coroutine changes.
    /// OnComplete
    /// OnCancelled
    /// OnFinished
    /// 
    /// *State
    /// Poll the state of the coroutine, if it's finished/active/cancelled, what MonoBehaviour is operating the coroutine, etc.
    /// 
    /// *Start/Pause/Resume/Cancel
    /// RadicalCoroutines can be paused and restarted later (they can not be reset). When starting a RadicalCoroutine you can 
    /// include an enum value that determines what should be done with the coroutine when the operating MonoBehaviour is disabled, 
    /// automatically pausing or cancelling the coroutine as desired, and automatically resuming it when re-enabled.
    /// 
    /// *Scheduling
    /// Schedule a coroutine to run when a current coroutine is complete.
    /// 
    /// *RadicalTask
    /// You can quickly and easily jump from a pooled thread to a main unity thread with RadicalTask.
    /// yield 'RadicalTask.JumpToAsync' to enter a pooled thread, and yield 'RadicalTask.JumpToUnityThread' to return.
    /// 
    /// *Manual Override
    /// Tick the operation of a coroutine manually. Won't be needed really ever, but that one time you want it, it's a life saver.
    /// 
    /// *Coroutine Manager
    /// A GameObject that is operating coroutines has a RadicalCoroutineManager attached. This is what tracks and maintains 
    /// automatic pausing/cancelling/resuming of coroutines. As well as gives an inspector view of what coroutines are operating 
    /// on what components and the name of the function being operated. Great for debugging.
    /// 
    /// *WARNING
    /// When using RadicalCoroutine, NEVER call StopCoroutine, and instead use the 'Cancel' method on the routine. 
    /// StopAllCoroutines works correctly on the SPComponent though, but not on MonoBehaviour.
    /// </summary>
    /// <notes>
    /// 
    /// TODO - #100 - We should set up a RadicalCoroutine pool to reduce garbage collection
    /// 
    /// </notes>
    public sealed class RadicalCoroutine : IRadicalEnumerator, IImmediatelyResumingYieldInstruction, IRadicalWaitHandle, IEnumerator, System.IDisposable
    {

        #region Events

        /// <summary>
        /// The coroutine completed successfully.
        /// </summary>
        public event System.EventHandler OnComplete;
        /// <summary>
        /// Called when the coroutine is flagged to be cancelled.
        /// OnCancelled may take a frame or more to actually occur 
        /// depending the state of the coroutine, where as this is 
        /// immediate.
        /// </summary>
        public event System.EventHandler OnCancelling;
        /// <summary>
        /// The coroutine was cancelled.
        /// </summary>
        public event System.EventHandler OnCancelled;
        /// <summary>
        /// The coroutine completed or was cancelled.
        /// </summary>
        public event System.EventHandler OnFinished;
        private System.EventHandler _immediatelyResumingSignal;

        private void OnFinish(bool cancelled)
        {
            _stack.Clear();
            _currentIEnumeratorYieldValue = null;
            _forcedTick = false;
            try
            {
                if (_owner != null) _owner.StopCoroutine(this); //NOTE - due to a bug in unity, a runtime warning appears if you pass in the Coroutine token while this routine is 'WaitForSeconds'
            }
            catch (System.Exception ex) { Debug.LogException(ex); }

            if (_manager != null)
            {
                _manager.UnregisterCoroutine(this);
            }

            var ev = System.EventArgs.Empty;
            try
            {
                if (cancelled)
                {
                    _state = RadicalCoroutineOperatingState.Cancelled;
                    if (this.OnCancelled != null) this.OnCancelled(this, ev);
                }
                else
                {
                    _state = RadicalCoroutineOperatingState.Complete;
                    if (this.OnComplete != null) this.OnComplete(this, ev);
                }
            }
            catch (System.Exception ex) { Debug.LogException(ex); }

            if (_immediatelyResumingSignal != null)
            {
                try { _immediatelyResumingSignal(this, ev); }
                catch (System.Exception ex) { Debug.LogException(ex); }
            }
            if (this.OnFinished != null)
            {
                try { this.OnFinished(this, System.EventArgs.Empty); }
                catch (System.Exception ex) { Debug.LogException(ex); }
            }

            _owner = null;
            _token = null;
            _manager = null;
        }

        #endregion

        #region Fields

        private MonoBehaviour _owner;
        private Coroutine _token;
        private RadicalCoroutineManager _manager;
        private RadicalCoroutineDisableMode _disableMode;

        private RadicalOperationStack _stack;
        private object _currentIEnumeratorYieldValue;

        private RadicalCoroutineOperatingState _state;
        private bool _forcedTick = false;

        #endregion

        #region CONSTRUCTOR

        public RadicalCoroutine(System.Collections.IEnumerable routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");
            _stack = new RadicalOperationStack(this);

            var e = routine.GetEnumerator();
            if (e is IRadicalYieldInstruction)
                _stack.Push(e as IRadicalYieldInstruction);
            else
                _stack.Push(EnumWrapper.Create(e));
        }

        public RadicalCoroutine(System.Collections.IEnumerator routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");
            _stack = new RadicalOperationStack(this);

            if (routine is IRadicalYieldInstruction)
                _stack.Push(routine as IRadicalYieldInstruction);
            else
                _stack.Push(EnumWrapper.Create(routine));
        }

        private RadicalCoroutine()
        {
            _stack = new RadicalOperationStack(this);
            //was created for recycling
        }

        #endregion

        #region Properties

        public RadicalCoroutineDisableMode DisableMode { get { return _disableMode; } }

        public bool Active { get { return _state == RadicalCoroutineOperatingState.Active; } }

        public bool Complete { get { return _state >= RadicalCoroutineOperatingState.Completing; } }

        public bool Cancelled { get { return _state <= RadicalCoroutineOperatingState.Cancelling; } }

        public bool Finished { get { return _state <= RadicalCoroutineOperatingState.Cancelling || _state >= RadicalCoroutineOperatingState.Completing; } }

        /// <summary>
        /// The current operating state of the coroutine. A manually ticked coroutine will not return as active.
        /// </summary>
        public RadicalCoroutineOperatingState OperatingState { get { return _state; } }

        /// <summary>
        /// The MonoBehaviour operating this routine. This may be null if it hasn't been started, or it's being manually ticked.
        /// </summary>
        public MonoBehaviour Operator { get { return _owner; } }

        internal RadicalOperationStack OperationStack
        {
            get { return _stack; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the coroutine, one should always call this method or the StartRadicalCoroutine extension method. Never pass 
        /// the RadicalCoroutine into the 'StartCoroutine' method.
        /// </summary>
        /// <param name="behaviour">A reference to the MonoBehaviour that should be handling the coroutine.</param>
        /// <param name="disableMode">A disableMode other than Default is only supported if the behaviour is an SPComponent.</param>
        /// <remarks>
        /// Disable modes allow you to decide how the coroutine is dealt with when the component/gameobject are disabled. Note that 
        /// 'CancelOnDeactivate' is a specical case flag, it only takes effect if NO OTHER flag is set (it's a 0 flag actually). 
        /// What this results in is that Deactivate and Disable are pausible... but a routine can only play through Disable, not 
        /// Deactivate. This is due to the default behaviour of coroutine in unity. In default mode, coroutines continue playing 
        /// when a component gets disabled, but when deactivated the coroutine gets cancelled. This means we can not play through 
        /// a deactivation.
        /// </remarks>
        public void Start(MonoBehaviour behaviour, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (_state != RadicalCoroutineOperatingState.Inactive) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");

            _state = RadicalCoroutineOperatingState.Active;
            _owner = behaviour;
            _disableMode = disableMode;

            if (_stack.CurrentOperation is IPausibleYieldInstruction) (_stack.CurrentOperation as IPausibleYieldInstruction).OnResume();

#if SP_LIB
            var manager = behaviour.AddOrGetComponent<RadicalCoroutineManager>();
#else
            var manager = behaviour.GetComponent<RadicalCoroutineManager>();
            if (manager == null) manager = behaviour.gameObject.AddComponent<RadicalCoroutineManager>();
#endif
            _manager = manager;
            _manager.RegisterCoroutine(this);
            _token = behaviour.StartCoroutine(this);

        }

        public void StartAsync(MonoBehaviour behaviour, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (_state != RadicalCoroutineOperatingState.Inactive) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");

            _state = RadicalCoroutineOperatingState.Active;
            _owner = behaviour;
            _disableMode = disableMode;

            if (_stack.CurrentOperation is IPausibleYieldInstruction) (_stack.CurrentOperation as IPausibleYieldInstruction).OnResume();
            _stack.Push(com.spacepuppy.Async.RadicalTask.Create(this)); //we start the task as an async operation

#if SP_LIB
            var manager = behaviour.AddOrGetComponent<RadicalCoroutineManager>();
#else
            var manager = behaviour.GetComponent<RadicalCoroutineManager>();
            if (manager == null) manager = behaviour.gameObject.AddComponent<RadicalCoroutineManager>();
#endif
            _manager = manager;
            _manager.RegisterCoroutine(this);
            _token = behaviour.StartCoroutine(this);

        }

        internal void Resume(MonoBehaviour behaviour)
        {
            if (_state != RadicalCoroutineOperatingState.Inactive && _state != RadicalCoroutineOperatingState.Paused) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");

            _state = RadicalCoroutineOperatingState.Active;
            _owner = behaviour;

            if (_stack.CurrentOperation is IPausibleYieldInstruction) (_stack.CurrentOperation as IPausibleYieldInstruction).OnResume();

#if SP_LIB
            var manager = behaviour.AddOrGetComponent<RadicalCoroutineManager>();
#else
            var manager = behaviour.GetComponent<RadicalCoroutineManager>();
            if (manager == null) manager = behaviour.gameObject.AddComponent<RadicalCoroutineManager>();
#endif
            _manager = manager;
            _manager.RegisterCoroutine(this);
            _token = behaviour.StartCoroutine(this);
        }

        /// <summary>
        /// Stops the coroutine, but preserves the state of it, so that it could be resumed again later by calling start.
        /// </summary>
        /// <param name="behaviour">A reference to the MonoBehaviour that is handling the coroutine.</param>
        public void Stop()
        {
            this.Stop(false);
        }

        /// <summary>
        /// Should only be ever called from RadicalCoroutineManager.
        /// </summary>
        /// <param name="stalledByManager"></param>
        internal void Stop(bool stalledByManager)
        {
            if (_state == RadicalCoroutineOperatingState.Inactive) return;

            if (_state == RadicalCoroutineOperatingState.Cancelling || _state == RadicalCoroutineOperatingState.Completing)
            {
                this.OnFinish(_state == RadicalCoroutineOperatingState.Cancelling);
            }
            else if (_state == RadicalCoroutineOperatingState.Active)
            {
                if (_owner != null)
                {
                    try
                    {
                        if (_owner != null) _owner.StopCoroutine(this);//NOTE - due to a bug in unity, a runtime warning appears if you pass in the Coroutine token while this routine is 'WaitForSeconds'
                    }
                    catch (System.Exception ex) { Debug.LogException(ex, _owner); }
                }

                if (!stalledByManager && _manager != null)
                {
                    _manager.UnregisterCoroutine(this);

                    _state = RadicalCoroutineOperatingState.Inactive;
                    _owner = null;
                    _token = null;
                }
                else
                {
                    _state = RadicalCoroutineOperatingState.Paused;
                    _token = null;

                }

                if (_stack.CurrentOperation is IPausibleYieldInstruction) (_stack.CurrentOperation as IPausibleYieldInstruction).OnPause();
            }
        }

        /// <summary>
        /// Stops the coroutine flagging it as finished.
        /// </summary>
        public void Cancel()
        {
            if (this.Finished) return;
            _state = RadicalCoroutineOperatingState.Cancelling;
            if (this.OnCancelling != null) this.OnCancelling(this, System.EventArgs.Empty);
        }

        /// <summary>
        /// Should only be ever called from RadicalCoroutineManager.
        /// </summary>
        /// <param name="cancelledByManager"></param>
        internal void Cancel(bool cancelledByManager)
        {
            if (this.Finished) return;
            _state = RadicalCoroutineOperatingState.Cancelling;
            if (cancelledByManager)
            {
                _manager = null;
            }
            if (this.OnCancelling != null) this.OnCancelling(this, System.EventArgs.Empty);
        }

        #endregion

        #region Scheduling

        public RadicalCoroutine Schedule(MonoBehaviour behaviour, System.Collections.IEnumerator routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            this.OnComplete += (s, e) =>
            {
                if (co._state == RadicalCoroutineOperatingState.Inactive) co.Start(behaviour, disableMode);
            };
            return co;
        }

        public RadicalCoroutine Schedule(MonoBehaviour behaviour, System.Collections.IEnumerable routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            this.OnComplete += (s, e) =>
            {
                if (co._state == RadicalCoroutineOperatingState.Inactive) co.Start(behaviour, disableMode);
            };
            return co;
        }

        public RadicalCoroutine Schedule(MonoBehaviour behaviour, System.Func<IEnumerator> routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine());
            this.OnComplete += (s, e) =>
            {
                if (co._state == RadicalCoroutineOperatingState.Inactive) co.Start(behaviour, disableMode);
            };
            return co;
        }

        public void Schedule(MonoBehaviour behaviour, RadicalCoroutine routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            this.OnComplete += (s, e) =>
            {
                if (routine._state == RadicalCoroutineOperatingState.Inactive) routine.Start(behaviour, disableMode);
            };
        }

        public void Schedule(System.Action callback)
        {
            this.OnComplete += (s, e) =>
            {
                callback();
            };
        }

        #endregion

        #region Manual Coroutine Methods

        /// <summary>
        /// Manually step the coroutine. This is usually done from within a Update event.
        /// </summary>
        /// <param name="handle">A MonoBehaviour to use as a handle if a YieldInstruction is to be operated on.</param>
        /// <returns></returns>
        public bool ManualTick(MonoBehaviour handle)
        {
            if (_owner != null || _state != RadicalCoroutineOperatingState.Inactive) throw new System.InvalidOperationException("Can not manually operate a RadicalCoroutine that is already being operated.");
            if (handle == null) throw new System.ArgumentNullException("handle");

            _state = RadicalCoroutineOperatingState.Active;
            bool result = false;
            try
            {
                result = (this as IEnumerator).MoveNext();
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex, handle);
            }
            if (_state == RadicalCoroutineOperatingState.Active) _state = RadicalCoroutineOperatingState.Inactive;

            if (result)
            {
                var current = _currentIEnumeratorYieldValue;
                _currentIEnumeratorYieldValue = null;

                if (current is YieldInstruction || current is WWW)
                {
                    var wait = ManualWaitForGeneric.Create(this, handle, current);
                    _stack.Push(wait);
                    wait.Start();
                }
            }

            return result;
        }

        private void ForceTick()
        {
            _forcedTick = false;
            if ((this as IEnumerator).MoveNext())
            {
                _forcedTick = true;
            }
        }

        #endregion

        #region IYieldInstruction/IEnumerator Interface

        bool IRadicalYieldInstruction.IsComplete { get { return this.Finished; } }

        object IEnumerator.Current
        {
            get { return _currentIEnumeratorYieldValue; }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if (this.Finished)
            {
                yieldObject = null;
                return false;
            }
            else if ((this as IEnumerator).MoveNext())
            {
                yieldObject = _currentIEnumeratorYieldValue;
                return true;
            }
            else
            {
                yieldObject = null;
                return false;
            }
        }

        bool IEnumerator.MoveNext()
        {
            if (_state == RadicalCoroutineOperatingState.Inactive) return false;

            if (this.Cancelled)
            {
                if (_state == RadicalCoroutineOperatingState.Cancelling)
                {
                    this.OnFinish(true);
                }
                return false;
            }
            else if (this.Complete)
            {
                if (_state == RadicalCoroutineOperatingState.Completing)
                {
                    this.OnFinish(false);
                }
                return false;
            }

            if (_forcedTick)
            {
                _forcedTick = false;
                return true;
            }

            _currentIEnumeratorYieldValue = null;

            //clear completed entries
            while (_stack.CurrentOperation != null)
            {
                try
                {
                    if (_stack.CurrentOperation.IsComplete)
                    {
                        _stack.Pop();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                    _stack.Pop();
                }
            }

            if (_stack.CurrentOperation != null)
            {
                //operate
                object current;
                var r = _stack.CurrentOperation;
                if (!r.Tick(out current))
                {
                    //the tick may have forced a tick, which could have popped this yieldinstruction already, this usually means it was an IImmediatelyResumingYieldInstruction
                    //deal with accordingly
                    if (_stack.CurrentOperation == r) _stack.Pop();

                    if (this.Cancelled)
                    {
                        if (_state == RadicalCoroutineOperatingState.Cancelling)
                            this.OnFinish(true);

                        _forcedTick = false; //make sure this is reset
                        return false;
                    }
                    else if (this.Complete)
                    {
                        if (_state == RadicalCoroutineOperatingState.Completing)
                            this.OnFinish(false);

                        _forcedTick = false; //make sure this is reset
                        return false;
                    }
                    else if (_stack.Count == 0)
                    {
                        this.OnFinish(false);
                        return false;
                    }
                    else if (_forcedTick)
                    {
                        _forcedTick = false;
                        return true;
                    }
                    else
                    {
                        current = null;
                    }
                }
                else if (this.Cancelled)
                {
                    //routine cancelled itself
                    if (_state == RadicalCoroutineOperatingState.Cancelling)
                    {
                        this.OnFinish(true);
                    }
                    return false;
                }


                //deal with the current yieldObject
                if (current == null)
                {
                    //do nothing
                }
                else if (current is YieldInstruction)
                {
                    if (current is WaitForSeconds && _disableMode.HasFlag(RadicalCoroutineDisableMode.Resumes))
                    {
                        _currentIEnumeratorYieldValue = null;
                        _stack.Push(WaitForDuration.FromWaitForSeconds(current as WaitForSeconds));
                    }
                    else
                    {
                        _currentIEnumeratorYieldValue = current;
                    }
                }
                else if (current is WWW)
                {
                    _currentIEnumeratorYieldValue = current;
                }
                else if (current is RadicalCoroutine)
                {
                    // //v3
                    var rad = current as RadicalCoroutine;
                    if (!rad.Finished)
                    {
                        if (rad._token != null)
                        {
                            _currentIEnumeratorYieldValue = rad._token;
                        }
                        else
                        {
                            _stack.Push(EnumWrapper.Create(WaitUntilDone_Routine(rad)));
                        }
                    }
                    else
                    {
                        _currentIEnumeratorYieldValue = null;
                    }
                }
                else if (current is IRadicalYieldInstruction)
                {
                    var instruction = current as IRadicalYieldInstruction;
                    if (instruction is IResettingYieldInstruction) (instruction as IResettingYieldInstruction).Reset();
                    if (instruction is IImmediatelyResumingYieldInstruction) (instruction as IImmediatelyResumingYieldInstruction).Signal += this.OnImmediatelyResumingYieldInstructionSignaled;

                    //we push first, incase the instruction checks if the 'currentoperation' is itself on first tick
                    _stack.Push(instruction);
                    object yieldObject;
                    if (instruction.Tick(out yieldObject))
                    {
                        _currentIEnumeratorYieldValue = yieldObject;
                    }
                    else
                    {
                        if (_stack.CurrentOperation == instruction) _stack.Pop();
                    }
                }
                else if (current is IEnumerable)
                {
                    //yes we have to test for IEnumerable before IEnumerator. When a yield method is returned as an IEnumerator, it still needs 'GetEnumerator' called on it.
                    var e = (current as IEnumerable).GetEnumerator();
                    if (e.MoveNext())
                    {
                        _currentIEnumeratorYieldValue = e.Current;
                        _stack.Push(EnumWrapper.Create(e));
                    }
                }
                else if (current is IEnumerator)
                {
                    var e = current as IEnumerator;
                    if (e.MoveNext())
                    {
                        _currentIEnumeratorYieldValue = e.Current;
                        _stack.Push(EnumWrapper.Create(e));
                    }
                }
                else if (current is RadicalCoroutineEndCommand)
                {
                    switch ((RadicalCoroutineEndCommand)current)
                    {
                        case RadicalCoroutineEndCommand.Stop:
                            this.Stop();
                            return false;
                        case RadicalCoroutineEndCommand.Cancel:
                            this.Cancel();
                            return false;
                    }
                }
                else if (current == com.spacepuppy.Async.RadicalTask.JumpToAsync)
                {
                    //auto async flag
                    var instruction = com.spacepuppy.Async.RadicalTask.Create(this) as IRadicalYieldInstruction;

                    //we push first, incase the instruction checks if the 'currentoperation' is itself on first tick
                    _stack.Push(instruction);
                    object yieldObject;
                    if (instruction.Tick(out yieldObject))
                    {
                        _currentIEnumeratorYieldValue = yieldObject;
                    }
                    else
                    {
                        if (_stack.CurrentOperation == instruction) _stack.Pop();
                    }
                }
                else
                {
                    _currentIEnumeratorYieldValue = current;
                }

                return true;
            }
            else
            {
                this.OnFinish(false);
                return false;
            }
        }

        void IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion

        #region IImmediatelyResumingYieldInstruction Interface

        event System.EventHandler IImmediatelyResumingYieldInstruction.Signal
        {
            add { _immediatelyResumingSignal += value; }
            remove { _immediatelyResumingSignal -= value; }
        }

        #endregion

        #region IImmediatelyResumingYieldInstruction Handler

        private System.EventHandler _onImmediatelyResumingYieldInstructionSignaled;
        private System.EventHandler OnImmediatelyResumingYieldInstructionSignaled
        {
            get
            {
                if (_onImmediatelyResumingYieldInstructionSignaled == null)
                    _onImmediatelyResumingYieldInstructionSignaled = (sender, e) =>
                    {
                        var instruction = sender as IImmediatelyResumingYieldInstruction;
                        if (instruction == null) return;
                        if (_stack.CurrentOperation == instruction)
                        {
                            _stack.Pop();
                            this.ForceTick();
                        }
                        else
                        {
                            instruction.Signal -= this.OnImmediatelyResumingYieldInstructionSignaled;
                        }
                    };
                return _onImmediatelyResumingYieldInstructionSignaled;
            }
        }
        //private void OnImmediatelyResumingYieldInstructionSignaled(object sender, System.EventArgs e)
        //{
        //    var instruction = sender as IImmediatelyResumingYieldInstruction;
        //    if (instruction == null) return;
        //    if (_stack.CurrentOperation == instruction)
        //    {
        //        _stack.Pop();
        //        this.ForceTick();
        //    }
        //    else
        //    {
        //        instruction.Signal -= this.OnImmediatelyResumingYieldInstructionSignaled;
        //    }
        //}

        #endregion

        #region IRadicalWaitHandle Interface

        void IRadicalWaitHandle.OnComplete(System.Action<IRadicalWaitHandle> callback)
        {
            this.OnFinished += (s, e) => { callback(this); };
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            if (this.Active) this.Stop(false);

            _owner = null;
            _token = null;
            _manager = null;
            _disableMode = RadicalCoroutineDisableMode.Default;
            _stack.Clear();
            _currentIEnumeratorYieldValue = null;
            _state = RadicalCoroutineOperatingState.Inactive;
            _forcedTick = false;
            this.OnComplete = null;
            this.OnCancelling = null;
            this.OnCancelled = null;
            this.OnFinished = null;
            _immediatelyResumingSignal = null;

            //TODO - #100 - allow releasing when we've fully implemented coroutine object caching 
            //_pool.Release(this);
        }

        #endregion


        #region Static Utils

        /// <summary>
        /// A radical coroutine that when running will repeadtly call an action and yield null. Simulating the Update function.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static RadicalCoroutine UpdateTicker(System.Action a)
        {
            var e = UpdateTickerIterator(a);
            e.MoveNext(); //we want to get the ticker up to the first yield statement, this way the action doesn't get called until RadicalCoroutine.Start is called.
            return new RadicalCoroutine(e);
        }

        /// <summary>
        /// Calls the action and yields null over and over. Simulates an 'Update'.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static System.Collections.IEnumerator UpdateTickerIterator(System.Action a)
        {
            if (a == null) throw new System.ArgumentNullException("a");

            yield return null;
            while (true)
            {
                a();
                yield return null;
            }
        }

        public static RadicalCoroutine FixedUpdateTicker(System.Action a)
        {
            return new RadicalCoroutine(FixedUpdateTickerIterator(a));
        }

        public static System.Collections.IEnumerator FixedUpdateTickerIterator(System.Action a)
        {
            if (a == null) throw new System.ArgumentNullException("a");

            var wait = new WaitForFixedUpdate();
            yield return wait;
            while (true)
            {
                a();
                yield return wait;
            }
        }

        public static RadicalCoroutine LateUpdateTicker(System.Action a)
        {
            return new RadicalCoroutine(LateUpdateTickerIterator(a));
        }

        public static System.Collections.IEnumerator LateUpdateTickerIterator(System.Action a)
        {
            if (a == null) throw new System.ArgumentNullException("a");

            var wait = new WaitForEndOfFrame();
            yield return wait;
            while (true)
            {
                a();
                yield return wait;
            }
        }


        /// <summary>
        /// A radical coroutine that when running will repeatedly call a function and yield the object returned by it. 
        /// Return a RadicalCoroutineEndCommand to stop the RadicalCoroutine from within the function.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static RadicalCoroutine Ticker(System.Func<object> f)
        {
            return new RadicalCoroutine(TickerIterator(f));
        }

        /// <summary>
        /// Calls the function, and yields the object returned. Return a RadicalCoroutineEndCommand to have 
        /// the running RadicalCoroutine cancel the action.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static System.Collections.IEnumerator TickerIterator(System.Func<object> f)
        {
            if (f == null) throw new System.ArgumentNullException("f");

            while (true)
            {
                yield return f();
            }
        }


        /// <summary>
        /// Returns a YieldInstruction that can be used to have a standard Unity Coroutine wait for a RadicalCoroutine to complete.
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="routine"></param>
        /// <returns></returns>
        public static YieldInstruction WaitUntilDone(MonoBehaviour behaviour, RadicalCoroutine routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            if (routine._token != null) return routine._token;

            if (routine.Finished) return null;
            return behaviour.StartCoroutine(WaitUntilDone_Routine(routine));
        }

        private static IEnumerator WaitUntilDone_Routine(RadicalCoroutine routine)
        {
            //if (routine._owner != null && routine._owner is SPComponent)
            //{
            //    routine._owner.AddOrGetComponent<RadicalCoroutineManager>().RegisterCoroutine(routine._owner as SPComponent, routine);
            //}

            while (!routine.Finished)
            {
                yield return null;
            }
        }

        #endregion

        #region Static Operators/Conversion

        public static implicit operator bool(RadicalCoroutine routine)
        {
            if (routine == null) return false;

            return routine._state == RadicalCoroutineOperatingState.Active || routine._state == RadicalCoroutineOperatingState.Paused;
        }


        #endregion



        #region Static Pool

        /*
        private static com.spacepuppy.Collections.ObjectCachePool<RadicalCoroutine> _pool = new com.spacepuppy.Collections.ObjectCachePool<RadicalCoroutine>(1000,
                                                                                                                                                            () =>
                                                                                                                                                            {
                                                                                                                                                                return new RadicalCoroutine();
                                                                                                                                                            },
                                                                                                                                                            (r) =>
                                                                                                                                                            {
                                                                                                                                                                r._owner = null;
                                                                                                                                                                r._token = null;
                                                                                                                                                                r._manager = null;
                                                                                                                                                                r._disableMode = RadicalCoroutineDisableMode.Default;
                                                                                                                                                                r._stack.Clear();
                                                                                                                                                                r._currentIEnumeratorYieldValue = null;
                                                                                                                                                                r._state = RadicalCoroutineOperatingState.Inactive;
                                                                                                                                                                r._forcedTick = false;
                                                                                                                                                                r.OnComplete = null;
                                                                                                                                                                r.OnCancelling = null;
                                                                                                                                                                r.OnCancelled = null;
                                                                                                                                                                r.OnFinished = null;
                                                                                                                                                                r._immediatelyResumingSignal = null;
                                                                                                                                                            },
                                                                                                                                                            true);

        internal RadicalCoroutine CreatePooledRoutine(IEnumerator e)
        {
            if (e == null) throw new System.ArgumentNullException("routine");
            var routine = _pool.GetInstance();
            if (e is IRadicalYieldInstruction)
                routine._stack.Push(e as IRadicalYieldInstruction);
            else
                routine._stack.Push(EnumWrapper.Create(e));
            return routine;
        }
        */

        #endregion

        #region Special Types

        private class ManualWaitForGeneric : IPooledYieldInstruction, System.Collections.IEnumerator
        {

#region Fields

            private RadicalCoroutine _owner;
            private MonoBehaviour _handle;
            private object _yieldObject;
            private object _enumCurrentValue;

#endregion

#region CONSTRUCTOR

            public ManualWaitForGeneric()
            {

            }

#endregion

#region Methods

            public void Start()
            {
                _handle.StartCoroutine(this);
            }

#endregion

#region IResettingYieldInstruction Interface

            public bool IsComplete
            {
                get { return _yieldObject != null; }
            }

            public bool Tick(out object yieldObject)
            {
                if (_enumCurrentValue == null)
                {
                    yieldObject = _yieldObject;
                    return true;
                }
                else
                {
                    yieldObject = null;
                    if (_owner.OperationStack.CurrentOperation == this)
                    {
                        if (_yieldObject is WaitForEndOfFrame || _yieldObject is WaitForFixedUpdate)
                        {
                            _owner.ManualTick(_handle);
                        }
                    }
                    return false;
                }
            }

#endregion

#region IEnumerator Interface

            public object Current
            {
                get { return _enumCurrentValue; }
            }

            public bool MoveNext()
            {
                return this.Tick(out _enumCurrentValue);
            }

            void System.Collections.IEnumerator.Reset()
            {
                _enumCurrentValue = null;
            }

#endregion

#region IDisposable Interface

            private static com.spacepuppy.Collections.ObjectCachePool<ManualWaitForGeneric> _pool = new com.spacepuppy.Collections.ObjectCachePool<ManualWaitForGeneric>(-1, () => new ManualWaitForGeneric());
            public static ManualWaitForGeneric Create(RadicalCoroutine owner, MonoBehaviour handle, object yieldObj)
            {
                var w = _pool.GetInstance();
                w._owner = owner;
                w._handle = handle;
                w._yieldObject = yieldObj;
                return w;
            }

            void System.IDisposable.Dispose()
            {
                _owner = null;
                _handle = null;
                _yieldObject = null;
                _enumCurrentValue = null;
            }

#endregion

        }

        internal class EnumWrapper : IRadicalYieldInstruction, IRadicalEnumerator, IPooledYieldInstruction
        {

            private static com.spacepuppy.Collections.ObjectCachePool<EnumWrapper> _pool = new com.spacepuppy.Collections.ObjectCachePool<EnumWrapper>(-1, () => new EnumWrapper());
            public static EnumWrapper Create(IEnumerator e)
            {
                var w = _pool.GetInstance();
                w._e = e;
                return w;
            }


            internal IEnumerator _e;
            private bool _complete;

            private EnumWrapper()
            {

            }

            public bool Tick(out object yieldObject)
            {
                if (_e == null)
                {
                    yieldObject = null;
                    return false;
                }

                if (_e.MoveNext())
                {
                    yieldObject = _e.Current;
                    return true;
                }
                else
                {
                    yieldObject = null;
                    _e = null;
                    return false;
                }
            }

            public bool IsComplete
            {
                get
                {
                    return _e == null;
                }
            }

            public object Current
            {
                get
                {
                    return _e != null ? _e.Current : null;
                }
            }

            public bool MoveNext()
            {
                return _e != null ? _e.MoveNext() : false;
            }

            public void Reset()
            {
                if (_e != null) _e.Reset();
            }

            void System.IDisposable.Dispose()
            {
                _e = null;
                _pool.Release(this);
            }

        }


        internal class RadicalOperationStack
        {

#region Fields

            private RadicalCoroutine _owner;
            private IRadicalYieldInstruction _currentOperation;
            private System.Collections.Generic.Stack<IRadicalYieldInstruction> _stack;

#endregion

#region CONSTRUCTOR

            internal RadicalOperationStack(RadicalCoroutine owner)
            {
                _owner = owner;
            }

#endregion

#region Properties

            public IRadicalYieldInstruction CurrentOperation
            {
                get { return _currentOperation; }
            }

            public int Count
            {
                get
                {
                    if (_currentOperation == null) return 0;
                    return (_stack != null) ? _stack.Count + 1 : 1;
                }
            }

#endregion

#region Methods

            public void Push(IRadicalYieldInstruction op)
            {
                if (_currentOperation != null)
                {
                    if (_stack == null) _stack = new System.Collections.Generic.Stack<IRadicalYieldInstruction>();
                    _stack.Push(_currentOperation);
                }

                _currentOperation = op;
            }

            public IRadicalYieldInstruction Pop()
            {
                var old = _currentOperation;
                if (_stack != null && _stack.Count > 0)
                {
                    _currentOperation = _stack.Pop();
                }
                else
                {
                    _currentOperation = null;
                }

                if (old != null)
                {
                    if (old is IPooledYieldInstruction) (old as IPooledYieldInstruction).Dispose();
                    if (old is IImmediatelyResumingYieldInstruction) (old as IImmediatelyResumingYieldInstruction).Signal -= _owner.OnImmediatelyResumingYieldInstructionSignaled;
                }

                return _currentOperation;
            }

            public void Clear()
            {
                if (_currentOperation != null)
                {
                    if (_currentOperation is IPooledYieldInstruction) (_currentOperation as IPooledYieldInstruction).Dispose();
                    if (_currentOperation is IImmediatelyResumingYieldInstruction) (_currentOperation as IImmediatelyResumingYieldInstruction).Signal -= _owner.OnImmediatelyResumingYieldInstructionSignaled;
                }

                if(_stack != null && _stack.Count > 0)
                {
                    var e = _stack.GetEnumerator();
                    while(e.MoveNext())
                    {
                        if (e.Current is IPooledYieldInstruction) (e.Current as IPooledYieldInstruction).Dispose();
                        if (e.Current is IImmediatelyResumingYieldInstruction) (e.Current as IImmediatelyResumingYieldInstruction).Signal -= _owner.OnImmediatelyResumingYieldInstructionSignaled;
                    }
                    _stack.Clear();
                }
                _currentOperation = null;
            }


            public IRadicalYieldInstruction PeekSubOperation()
            {
                if (_stack != null && _stack.Count > 0)
                    return _stack.Peek();
                else
                    return null;
            }

            public void PushSubOperation(IRadicalYieldInstruction op)
            {
                if (op == null) throw new System.ArgumentNullException("op");

                if (_stack == null) _stack = new System.Collections.Generic.Stack<IRadicalYieldInstruction>();
                _stack.Push(op);
            }

            public IRadicalYieldInstruction PopSubOperation()
            {
                if (_stack == null || _stack.Count == 0) return null;

                var old = _stack.Pop();
                if (old != null)
                {
                    if (old is IPooledYieldInstruction) (old as IPooledYieldInstruction).Dispose();
                    if (old is IImmediatelyResumingYieldInstruction) (old as IImmediatelyResumingYieldInstruction).Signal -= _owner.OnImmediatelyResumingYieldInstructionSignaled;
                }
                return old;
            }

            public void ClearSubOperations()
            {
                if (_stack != null && _stack.Count > 0)
                {
                    var e = _stack.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current is IPooledYieldInstruction) (e.Current as IPooledYieldInstruction).Dispose();
                        if (e.Current is IImmediatelyResumingYieldInstruction) (e.Current as IImmediatelyResumingYieldInstruction).Signal -= _owner.OnImmediatelyResumingYieldInstructionSignaled;
                    }
                    _stack.Clear();
                }
            }

            public IRadicalYieldInstruction Last()
            {
                if (_stack == null || _stack.Count == 0)
                    return _currentOperation;
                else
                    return _stack.Last();
            }

#endregion

        }

        #endregion

#region Editor Special Types

        public static class EditorHelper
        {

            public static string GetInternalRoutineID(RadicalCoroutine routine)
            {
                if (routine.OperationStack.Count == 0) return "";
                var r = routine.OperationStack.Last(); //the bottom of the stack is the actual routine
                if (r is IRadicalYieldInstruction)
                {
                    return GetIterableID(r as IRadicalYieldInstruction);
                }
                else
                {
                    return r.GetType().FullName.Split('.').Last();
                }
            }

            public static string GetYieldID(RadicalCoroutine routine)
            {
                if (routine._currentIEnumeratorYieldValue is WaitForSeconds)
                {
                    //float dur = ConvertUtil.ToSingle(DynamicUtil.GetValue(routine._currentIEnumeratorYieldValue, "m_Seconds"));
                    var field = typeof(WaitForSeconds).GetField("m_Seconds", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    float dur = (float)field.GetValue(routine._currentIEnumeratorYieldValue);
                    return string.Format("WaitForSeconds[{0:0.00}]", dur);
                }
                else if (routine._currentIEnumeratorYieldValue is YieldInstruction)
                {
                    return routine._currentIEnumeratorYieldValue.GetType().Name;
                }
                else if (routine._currentIEnumeratorYieldValue is WWW)
                {
                    return string.Format("WWW[{0}%]", (routine._currentIEnumeratorYieldValue as WWW).progress * 100);
                }
                else
                {
                    return "WaitOneFrame";
                }
            }

            public static string GetDerivativeID(RadicalCoroutine routine)
            {
                if (routine.OperationStack.Count <= 1) return "";

                return GetIterableID(routine.OperationStack.CurrentOperation);
            }


            private static string GetIterableID(IRadicalYieldInstruction e)
            {
                if (e == null) return string.Empty;

                if (e is WaitForDuration)
                {
                    var wait = e as WaitForDuration;
                    return string.Format("WaitForDuration[{0:0.00}, {1:0.00}]", wait.CurrentTime, wait.Duration);
                }
                else if (e is EnumWrapper)
                {
                    var w = e as EnumWrapper;
                    if (w._e == null) return string.Empty;

                    return w._e.GetType().FullName.Split('.').Last();
                }
                else
                {
                    return e.GetType().Name;
                }
            }

        }

#endregion
        
    }

}
