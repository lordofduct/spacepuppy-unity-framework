using UnityEngine;
using System.Collections;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public sealed class RadicalCoroutine : IImmediatelyResumingYieldInstruction, IPausibleYieldInstruction
    {

        #region Events

        /// <summary>
        /// The coroutine completed successfully.
        /// </summary>
        public event System.EventHandler OnComplete;
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
            _derivative = null;
            _currentIEnumeratorYieldValue = null;
            _manualWaitingOnInstruction = null;
            if (_owner is MonoBehaviour)
            {
                try
                {
                    (_owner as MonoBehaviour).StopCoroutine(this); //NOTE - due to a bug in unity, a runtime warning appears if you pass in the Coroutine token while this routine is 'WaitForSeconds'
                }
                catch (System.Exception ex) { Debug.LogException(ex); }
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
        }

        #endregion

        #region Fields

        private object _owner; //this houses either the Coroutine or RadicalCoroutine that may already be operating this RadicalCoroutine
        private Coroutine _token;
        private RadicalCoroutineDisableMode _disableMode;

        private System.Collections.IEnumerator _routine;
        private System.Collections.IEnumerator _derivative;
        private object _currentIEnumeratorYieldValue;

        private RadicalCoroutineOperatingState _state;
        private ManualWaitForGeneric _manualWaitingOnInstruction;
        private bool _forcedTick = false;

        #endregion

        #region CONSTRUCTOR

        public RadicalCoroutine(System.Collections.IEnumerable routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");
            _routine = routine.GetEnumerator();
        }

        public RadicalCoroutine(System.Collections.IEnumerator routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");
            _routine = routine;
        }

        private RadicalCoroutine(System.Collections.IEnumerator routine, RadicalCoroutine owner)
        {
            _routine = routine;
            _owner = owner;
            _state = RadicalCoroutineOperatingState.Active;
        }

        #endregion

        #region Properties

        public RadicalCoroutineDisableMode DisableMode { get { return _disableMode; } }

        public bool Complete { get { return _state >= RadicalCoroutineOperatingState.Completing; } }

        public bool Cancelled { get { return _state <= RadicalCoroutineOperatingState.Cancelling; } }

        public bool Finished { get { return _state <= RadicalCoroutineOperatingState.Cancelling || _state >= RadicalCoroutineOperatingState.Completing; } }

        /// <summary>
        /// The current operating state of the coroutine. A manually ticked coroutine will not return as active.
        /// </summary>
        public RadicalCoroutineOperatingState OperatingState { get { return _state; } }

        /// <summary>
        /// The object operating this routine. Typically this is the MonoBehaviour that the routine was started with. 
        /// Though it could also be a RadicalCoroutine if its nested in another coroutine.
        /// </summary>
        public object Operator { get { return _owner; } }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the coroutine, one should always call this method or the StartRadicalCoroutine extension method. Never pass the coroutine into the 'StartCoroutine' method.
        /// </summary>
        /// <param name="behaviour">A reference to the MonoBehaviour that should be handling the coroutine.</param>
        /// <param name="disableMode">A disableMode other than Default is only supported if the behaviour is an SPComponent.</param>
        public void Start(MonoBehaviour behaviour, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (_state != RadicalCoroutineOperatingState.Inactive) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");

            _state = RadicalCoroutineOperatingState.Active;
            _owner = behaviour;
            _token = behaviour.StartCoroutine(this);

            _disableMode = disableMode;
            if(_disableMode > RadicalCoroutineDisableMode.Default)
            {
                var manager = behaviour.AddOrGetComponent<RadicalCoroutineManager>();
                manager.RegisterCoroutine(behaviour, this);
            }

            if (_derivative is IPausibleYieldInstruction) (_derivative as IPausibleYieldInstruction).OnResume();
        }

        internal void Resume(SPComponent behaviour)
        {
            if (_state != RadicalCoroutineOperatingState.Inactive) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");

            _state = RadicalCoroutineOperatingState.Active;
            _owner = behaviour;
            _token = behaviour.StartCoroutine(this);

            if (_derivative is IPausibleYieldInstruction) (_derivative as IPausibleYieldInstruction).OnResume();
        }

        /// <summary>
        /// Stops the coroutine, but preserves the state of it, so that it could be resumed again later by calling start.
        /// </summary>
        /// <param name="behaviour">A reference to the MonoBehaviour that is handling the coroutine.</param>
        public void Stop()
        {
            if (_state == RadicalCoroutineOperatingState.Inactive) return;

            if (_state == RadicalCoroutineOperatingState.Cancelling || _state == RadicalCoroutineOperatingState.Completing)
            {
                this.OnFinish(_state == RadicalCoroutineOperatingState.Cancelling);
            }
            else if(_state == RadicalCoroutineOperatingState.Active)
            {
                _state = RadicalCoroutineOperatingState.Inactive;
                if (_owner is MonoBehaviour)
                {
                    try
                    {
                        (_owner as MonoBehaviour).StopCoroutine(this); //NOTE - due to a bug in unity, a runtime warning appears if you pass in the Coroutine token while this routine is 'WaitForSeconds'
                    }
                    catch (System.Exception ex) { Debug.LogException(ex); }
                    _owner = null;
                    _token = null;
                }
                else if (_owner is RadicalCoroutine)
                {
                    //we stop the parent from operating us
                    var owner = _owner as RadicalCoroutine;
                    if (owner._derivative == this) owner._derivative = null;
                    _owner = null;
                }

                if (_derivative is IPausibleYieldInstruction) (_derivative as IPausibleYieldInstruction).OnPause();
            }
        }

        /// <summary>
        /// Stops the coroutine flagging it as complete. The coroutine still may not be used again as the operator may maintain a reference for resetting. Check the 'HasOwner' property.
        /// </summary>
        public void Cancel()
        {
            if (_routine is RadicalCoroutine)
            {
                (_routine as RadicalCoroutine).Cancel();
            }
            if (_derivative != null && _derivative is RadicalCoroutine)
            {
                (_derivative as RadicalCoroutine).Cancel();
            }

            //_derivative = null;
            //_currentIEnumeratorYieldValue = null;
            //_manualWaitingOnInstruction = null;
            //_cancelled = true;
            //this.OnFinish(true);
            _state = RadicalCoroutineOperatingState.Cancelling;
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

        public RadicalCoroutine Schedule(MonoBehaviour behaviour, CoroutineMethod routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine().GetEnumerator());
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

        #endregion

        #region Manual Coroutine Methods

        /// <summary>
        /// Manually step the coroutine. This is usually done from within a Update event.
        /// </summary>
        /// <param name="handle">A MonoBehaviour to use as a handle if a YieldInstruction is to be operated on.</param>
        /// <returns></returns>
        public bool ManualTick(MonoBehaviour handle)
        {
            if (_owner != null) throw new System.InvalidOperationException("Can not manually operate a RadicalCoroutine that is already being operated on.");
            if (handle == null) throw new System.ArgumentNullException("handle");

            if ((this as IEnumerator).MoveNext())
            {
                var current = _currentIEnumeratorYieldValue;
                _currentIEnumeratorYieldValue = null;

                if (current is YieldInstruction || current is WWW)
                {
                    var wait = new ManualWaitForGeneric(this, handle, current);
                    _manualWaitingOnInstruction = wait;
                    handle.StartCoroutine(wait);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ForceTick()
        {
            _forcedTick = false;
            if ((this as IEnumerator).MoveNext())
            {
                _forcedTick = true;
            }
        }

        #endregion

        #region IYieldInstruction/IEnumerator Interface

        object IEnumerator.Current
        {
            get { return _currentIEnumeratorYieldValue; }
        }

        bool IEnumerator.MoveNext()
        {
            if (this.Cancelled)
            {
                if(_state == RadicalCoroutineOperatingState.Cancelling)
                {
                    this.OnFinish(true);
                }
                return false;
            }
            else if(this.Complete)
            {
                if(_state == RadicalCoroutineOperatingState.Completing)
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

            if (_manualWaitingOnInstruction != null)
            {
                if (_manualWaitingOnInstruction.StillWait)
                {
                    return true;
                }
                else
                {
                    _manualWaitingOnInstruction = null;
                }
            }

            if (_derivative != null)
            {
                if (_derivative.MoveNext())
                {
                    if (this.Cancelled)
                    {
                        //routine cancelled itself
                        if (_state == RadicalCoroutineOperatingState.Cancelling)
                        {
                            this.OnFinish(true);
                        }
                        return false;
                    }
                    if (_derivative == null) return !this.Finished; //the derivative was cleared out by a manual tick, wait a frame or exit depending on if we completed in that time
                    _currentIEnumeratorYieldValue = _derivative.Current;
                    return true;
                }
                else
                {
                    _derivative = null;
                }
            }


            if (_routine.MoveNext())
            {
                if (this.Cancelled)
                {
                    //routine cancelled itself
                    if (_state == RadicalCoroutineOperatingState.Cancelling)
                    {
                        this.OnFinish(true);
                    }
                    return false;
                }

                var current = _routine.Current;
                if (current == null)
                {
                    //do nothing
                }
                else if (current is YieldInstruction)
                {
                    if(current is WaitForSeconds && _disableMode.HasFlag(RadicalCoroutineDisableMode.ResumeOnEnable))
                    {
                        _currentIEnumeratorYieldValue = null;
                        _derivative = WaitForDuration.FromWaitForSeconds(current as WaitForSeconds);
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
                    // //v1
                    //IEnumerator e = null;
                    //var rad = current as RadicalCoroutine;
                    //if (rad._owner == null)
                    //{
                    //    //honestly, this is weird, this means someone return a RadicalCoroutine that hasn't been started.
                    //    rad._owner = this;
                    //    e = rad as IEnumerator;
                    //}
                    //else
                    //{
                    //    e = WaitUntilDone_Routine(rad).GetEnumerator();
                    //}

                    //if (e.MoveNext())
                    //{
                    //    _currentIEnumeratorYieldValue = e.Current;
                    //    _derivative = e;
                    //}

                    // //v2
                    //var rad = current as RadicalCoroutine;
                    //_derivative = WaitUntilDone_Routine(rad);

                    // //v3
                    var rad = current as RadicalCoroutine;
                    if(!rad.Finished)
                    {
                        if (rad._token != null)
                        {
                            _currentIEnumeratorYieldValue = rad._token;
                        }
                        else
                        {
                            _derivative = WaitUntilDone_Routine(rad);
                        }
                    }
                    else
                    {
                        _currentIEnumeratorYieldValue = null;
                    }
                }
                else if (current is IRadicalYieldInstruction)
                {
                    var rad = current as IRadicalYieldInstruction;
                    if (rad is IImmediatelyResumingYieldInstruction) (rad as IImmediatelyResumingYieldInstruction).Signal += this.OnImmediatelyResumingYieldInstructionSignaled;
                    if (rad is IPausibleYieldInstruction) rad = (rad as IPausibleYieldInstruction).Validate();

                    var e = new RadicalCoroutine(rad, this) as IEnumerator;
                    if (e.MoveNext())
                    {
                        _currentIEnumeratorYieldValue = e.Current;
                        _derivative = e;
                    }
                }
                else if (current is IEnumerable)
                {
                    //yes we have to test for IEnumerable before IEnumerator. When a yield method is returned as an IEnumerator, it still needs 'GetEnumerator' called on it.
                    var e = new RadicalCoroutine((current as IEnumerable).GetEnumerator(), this) as IEnumerator;
                    if (e.MoveNext())
                    {
                        _currentIEnumeratorYieldValue = e.Current;
                        _derivative = e;
                    }
                }
                else if (current is IEnumerator)
                {
                    var e = new RadicalCoroutine(current as IEnumerator, this) as IEnumerator;
                    if (e.MoveNext())
                    {
                        _currentIEnumeratorYieldValue = e.Current;
                        _derivative = e;
                    }
                }
                else if(current is RadicalCoroutineEndCommand)
                {
                    var cmd = (RadicalCoroutineEndCommand)current;
                    
                    if(cmd.HasFlag(RadicalCoroutineEndCommand.Cancel))
                    {
                        this.Cancel();
                        if(cmd.HasFlag(RadicalCoroutineEndCommand.StallImmediateResume))
                        {
                            _currentIEnumeratorYieldValue = null;
                        }
                        else
                        {
                            this.OnFinish(true);
                            return false;
                        }
                    }
                    else
                    {
                        if (cmd.HasFlag(RadicalCoroutineEndCommand.StallImmediateResume))
                        {
                            _state = RadicalCoroutineOperatingState.Completing;
                            _currentIEnumeratorYieldValue = null;
                        }
                        else
                        {
                            this.OnFinish(false);
                            return false;
                        }
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

        private void OnImmediatelyResumingYieldInstructionSignaled(object sender, System.EventArgs e)
        {
            var instruction = sender as IImmediatelyResumingYieldInstruction;
            if (instruction != null) instruction.Signal -= this.OnImmediatelyResumingYieldInstructionSignaled;

            this.ForceTick();
        }

        #endregion

        #region IPausibleYieldInstruction Interface

        void IPausibleYieldInstruction.OnPause()
        {
            if (_routine is IPausibleYieldInstruction) (_routine as IPausibleYieldInstruction).OnPause();
            if (_derivative is IPausibleYieldInstruction) (_derivative as IPausibleYieldInstruction).OnPause();
        }

        void IPausibleYieldInstruction.OnResume()
        {
            if (_routine is IPausibleYieldInstruction) (_routine as IPausibleYieldInstruction).OnResume();
            if (_derivative is IPausibleYieldInstruction) (_derivative as IPausibleYieldInstruction).OnResume();
        }

        IPausibleYieldInstruction IPausibleYieldInstruction.Validate()
        {
            //this should never be called...
            return this;
        }

        #endregion

        #region Factory Methods

        public static System.Collections.IEnumerable Ticker(System.Func<object> f)
        {
            if (f == null) yield break;

            while (true)
            {
                var r = f();
                if (r is bool)
                {
                    if ((bool)r)
                        yield break;
                    else
                        yield return null;
                }
                else
                {
                    yield return r;
                }
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
            if(routine._owner is MonoBehaviour)
            {
                (routine._owner as MonoBehaviour).AddOrGetComponent<RadicalCoroutineManager>().RegisterCoroutine(routine._owner as MonoBehaviour, routine);
            }

            while (!routine.Finished)
            {
                yield return null;
            }
        }

        #endregion



        #region Special Types

        private class ManualWaitForGeneric : RadicalYieldInstruction
        {
            private RadicalCoroutine _owner;
            private MonoBehaviour _handle;
            private object _yieldObject;
            private bool _called;

            public ManualWaitForGeneric(RadicalCoroutine owner, MonoBehaviour handle, object yieldObject)
            {
                _owner = owner;
                _handle = handle;
                _yieldObject = yieldObject;
            }

            protected override object Tick()
            {
                if (!_called)
                {
                    _called = true;
                    return _yieldObject;
                }
                else if (_owner._manualWaitingOnInstruction == this)
                {
                    _owner._manualWaitingOnInstruction = null;
                    if (_yieldObject is WaitForEndOfFrame || _yieldObject is WaitForFixedUpdate)
                    {
                        _owner.ManualTick(_handle);
                    }
                }

                this.SetSignal();
                return null;
            }

            public bool StillWait
            {
                get { return _owner._manualWaitingOnInstruction == this; }
            }
        }

        #endregion

    }

}
