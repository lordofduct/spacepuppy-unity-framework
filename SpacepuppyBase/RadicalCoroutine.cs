using UnityEngine;
using System.Collections;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public sealed class RadicalCoroutine : IImmediatelyResumingYieldInstruction
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
            _stack.Clear();
            _currentIEnumeratorYieldValue = null;
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

        private MonoBehaviour _owner;
        private Coroutine _token;
        private RadicalCoroutineDisableMode _disableMode;

        private System.Collections.Generic.Stack<IEnumerator> _stack = new System.Collections.Generic.Stack<IEnumerator>();
        private object _currentIEnumeratorYieldValue;

        private RadicalCoroutineOperatingState _state;
        private bool _forcedTick = false;

        #endregion
        
        #region CONSTRUCTOR

        public RadicalCoroutine(System.Collections.IEnumerable routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");
            _stack.Push(routine.GetEnumerator());
        }

        public RadicalCoroutine(System.Collections.IEnumerator routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");
            _stack.Push(routine);
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

        #endregion

        #region Methods

        /// <summary>
        /// Starts the coroutine, one should always call this method or the StartRadicalCoroutine extension method. Never pass the RadicalCoroutine into the 'StartCoroutine' method.
        /// </summary>
        /// <param name="behaviour">A reference to the MonoBehaviour that should be handling the coroutine.</param>
        /// <param name="disableMode">A disableMode other than Default is only supported if the behaviour is an SPComponent.</param>
        /// <remarks>
        /// Disable modes allow you to decide how the coroutine is dealt with when the component/gameobject are disabled. Note that 'CancelOnDeactivate' is a specical case flag, 
        /// it only takes effect if NO OTHER flag is set (it's a 0 flag actually). What this results in is that Deactivate and Disable are pausible... but a routine can only play 
        /// through Disable, not Deactivate. This is due to the default behaviour of coroutine in unity. In default mode, coroutines continue playing when a component gets disabled, 
        /// but when deactivated the coroutine gets cancelled. This means we can not play through a deactivation.
        /// </remarks>
        public void Start(MonoBehaviour behaviour, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (_state != RadicalCoroutineOperatingState.Inactive) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");

            _state = RadicalCoroutineOperatingState.Active;
            _owner = behaviour;
            _token = behaviour.StartCoroutine(this);

            _disableMode = disableMode;
            if (_disableMode > RadicalCoroutineDisableMode.Default && _disableMode != RadicalCoroutineDisableMode.ResumeOnEnable)
            {
                //no point in managing the routine if it acts in default mode... a flag of 'ResumeOnEnable' is a redundant mode to default
                var manager = behaviour.AddOrGetComponent<RadicalCoroutineManager>();
                manager.RegisterCoroutine(behaviour, this);
            }

            if (_stack.Peek() is IPausibleYieldInstruction) (_stack.Peek() as IPausibleYieldInstruction).OnResume();
        }

        internal void Resume(SPComponent behaviour)
        {
            if (_state != RadicalCoroutineOperatingState.Inactive) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");

            _state = RadicalCoroutineOperatingState.Active;
            _owner = behaviour;
            _token = behaviour.StartCoroutine(this);

            if (_stack.Peek() is IPausibleYieldInstruction) (_stack.Peek() as IPausibleYieldInstruction).OnResume();
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
            else if (_state == RadicalCoroutineOperatingState.Active)
            {
                _state = RadicalCoroutineOperatingState.Inactive;
                try
                {
                    (_owner as MonoBehaviour).StopCoroutine(this); //NOTE - due to a bug in unity, a runtime warning appears if you pass in the Coroutine token while this routine is 'WaitForSeconds'
                }
                catch (System.Exception ex) { Debug.LogException(ex); }
                _owner = null;
                _token = null;

                if (_stack.Peek() is IPausibleYieldInstruction) (_stack.Peek() as IPausibleYieldInstruction).OnPause();
            }
        }

        /// <summary>
        /// Stops the coroutine flagging it as finished.
        /// </summary>
        public void Cancel()
        {
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
                    _stack.Push(wait);
                    wait.Start();
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

            //actually operate
            while (_stack.Count > 0 && !_stack.Peek().MoveNext())
            {
                if (_stack.Peek() is IPooledYieldInstruction)
                    (_stack.Pop() as IPooledYieldInstruction).Dispose();
                else
                    _stack.Pop();
            }

            if (_stack.Count > 0)
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

                var current = _stack.Peek().Current;
                if (current == null)
                {
                    //do nothing
                }
                else if (current is YieldInstruction)
                {
                    if (current is WaitForSeconds && _disableMode.HasFlag(RadicalCoroutineDisableMode.ResumeOnEnable))
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
                            _stack.Push(WaitUntilDone_Routine(rad));
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
                    if (instruction is IImmediatelyResumingYieldInstruction) (instruction as IImmediatelyResumingYieldInstruction).Signal += this.OnImmediatelyResumingYieldInstructionSignaled;

                    if (instruction.MoveNext())
                    {
                        _currentIEnumeratorYieldValue = instruction.Current;
                        _stack.Push(instruction);
                    }
                }
                else if (current is IEnumerable)
                {
                    //yes we have to test for IEnumerable before IEnumerator. When a yield method is returned as an IEnumerator, it still needs 'GetEnumerator' called on it.
                    var e = (current as IEnumerable).GetEnumerator();
                    if (e.MoveNext())
                    {
                        _currentIEnumeratorYieldValue = e.Current;
                        _stack.Push(e);
                    }
                }
                else if (current is IEnumerator)
                {
                    var e = current as IEnumerator;
                    if (e.MoveNext())
                    {
                        _currentIEnumeratorYieldValue = e.Current;
                        _stack.Push(e);
                    }
                }
                else if (current is RadicalCoroutineEndCommand)
                {
                    var cmd = (RadicalCoroutineEndCommand)current;

                    if (cmd.HasFlag(RadicalCoroutineEndCommand.Cancel))
                    {
                        this.Cancel();
                        if (cmd.HasFlag(RadicalCoroutineEndCommand.StallImmediateResume))
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



        #region Static Utils

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
            if (routine._owner is MonoBehaviour)
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

            #region Fields

            private RadicalCoroutine _owner;
            private MonoBehaviour _handle;
            private object _yieldObject;

            #endregion

            #region CONSTRUCTOR

            public ManualWaitForGeneric(RadicalCoroutine owner, MonoBehaviour handle, object yieldObj)
            {
                _owner = owner;
                _handle = handle;
                _yieldObject = yieldObj;
            }

            #endregion

            #region Methods

            public void Start()
            {
                _handle.StartCoroutine(this.WaitRoutine());
            }

            private System.Collections.IEnumerator WaitRoutine()
            {
                yield return _yieldObject;
                this.SetSignal();
                if (_owner._stack.Peek() == this)
                {
                    if (_yieldObject is WaitForEndOfFrame || _yieldObject is WaitForFixedUpdate)
                    {
                        _owner.ManualTick(_handle);
                    }
                }
            }

            protected override object Tick()
            {
                return null;
            }

            #endregion

        }

        #endregion

        #region Editor Special Types

        public static class EditorHelper
        {

            public static string GetInternalRoutineID(RadicalCoroutine routine)
            {
                if (routine._stack.Count == 0) return "";
                var r = routine._stack.Last(); //the bottom of the stack is the actual routine
                if (r is IRadicalYieldInstruction)
                {
                    if (r is WaitForDuration)
                    {
                        var wait = r as WaitForDuration;
                        return string.Format("WaitForDuration[{0:0.00}, {1:0.00}]", wait.CurrentTime, wait.Duration);
                    }
                    else
                    {
                        return r.GetType().Name;
                    }
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
                    float dur = ConvertUtil.ToSingle(ObjUtil.GetValue(routine._currentIEnumeratorYieldValue, "m_Seconds"));
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
                if (routine._stack.Count <= 1) return "";

                return GetIterableID(routine._stack.Peek());
            }


            private static string GetIterableID(IEnumerator e)
            {
                if (e is IRadicalYieldInstruction)
                {
                    if (e is WaitForDuration)
                    {
                        var wait = e as WaitForDuration;
                        return string.Format("WaitForDuration[{0:0.00}, {1:0.00}]", wait.CurrentTime, wait.Duration);
                    }
                    else
                    {
                        return e.GetType().Name;
                    }
                }
                else if (e != null)
                {
                    return e.GetType().FullName.Split('.').Last();
                }
                else
                {
                    return "";
                }
            }

        }

        #endregion

    }

}
