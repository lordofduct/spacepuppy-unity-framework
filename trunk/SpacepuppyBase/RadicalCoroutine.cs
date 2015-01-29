using UnityEngine;
using System.Collections;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public sealed class RadicalCoroutine : IImmediatelyResumingYieldInstruction
    {

        private enum OperatingState
        {
            Inactive = 0,
            Active = 1,
            Completing = 2,
            Complete = 3,
            Cancelling = -1,
            Cancelled = -2
        }

        #region Events

        public event System.EventHandler OnComplete;
        public event System.EventHandler OnCancelled;
        private System.EventHandler _immediatelyResumingSignal;

        private void OnFinish(bool cancelled)
        {
            _derivative = null;
            _currentIEnumeratorYieldValue = null;
            _manualWaitingOnInstruction = null;
            if (_owner is Coroutine) _owner = null; //Coroutine stop owning as soon as false is returned

            if(cancelled)
            {
                _state = OperatingState.Cancelled;

                var ev = System.EventArgs.Empty;
                if (this.OnCancelled != null) this.OnCancelled(this, ev);
                if (_immediatelyResumingSignal != null) _immediatelyResumingSignal(this, ev);
            }
            else
            {
                _state = OperatingState.Complete;

                var ev = System.EventArgs.Empty;
                if (this.OnComplete != null) this.OnComplete(this, ev);
                if (_immediatelyResumingSignal != null) _immediatelyResumingSignal(this, ev);
            }
        }

        #endregion

        #region Fields

        private object _owner; //this houses either the Coroutine or RadicalCoroutine that may already be operating this RadicalCoroutine

        private System.Collections.IEnumerator _routine;
        private System.Collections.IEnumerator _derivative;
        private object _currentIEnumeratorYieldValue;

        //private bool _completed = false;
        //private bool _cancelled = false;
        private OperatingState _state;
        private IManualWait _manualWaitingOnInstruction;
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
        }

        #endregion

        #region Properties

        public bool Complete { get { return _state >= OperatingState.Completing; } }

        public bool Cancelled { get { return _state <= OperatingState.Cancelling; } }

        /// <summary>
        /// An operator is still operating this routine and it is not eligible to be started again.
        /// </summary>
        public bool HasOperator { get { return _owner != null; } }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the coroutine, one should always call this method or the StartRadicalCoroutine extension method. Never pass the coroutine into the 'StartCoroutine' method.
        /// </summary>
        /// <param name="behaviour">A reference to the MonoBehaviour that should be handling the coroutine.</param>
        public void Start(MonoBehaviour behaviour)
        {
            if (_state != OperatingState.Inactive) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");

            _state = OperatingState.Active;
            _owner = behaviour.StartCoroutine(this);
        }

        /// <summary>
        /// Stops the coroutine, but preserves the state of it, so that it could be resumed again later by calling start.
        /// </summary>
        /// <param name="behaviour">A reference to the MonoBehaviour that is handling the coroutine.</param>
        public void Stop(MonoBehaviour behaviour)
        {
            if (_owner is Coroutine)
            {
                _owner = null;
                behaviour.StopCoroutine(this);
            }
            else if (_owner is RadicalCoroutine)
            {
                (_owner as RadicalCoroutine).Stop(behaviour);
            }
            else
            {
                //assume that the coroutine was started with out calling 'start'...
                behaviour.StopCoroutine(this);
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
            _state = OperatingState.Cancelling;
        }

        #endregion

        #region Scheduling

        public RadicalCoroutine Schedule(MonoBehaviour behaviour, System.Collections.IEnumerator routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            this.OnComplete += (s, e) =>
            {
                if (co._owner == null && !co.Cancelled && !co.Complete) co.Start(behaviour);
            };
            return co;
        }

        public RadicalCoroutine Schedule(MonoBehaviour behaviour, System.Collections.IEnumerable routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            this.OnComplete += (s, e) =>
            {
                if (co._owner == null && !co.Cancelled && !co.Complete) co.Start(behaviour);
            };
            return co;
        }

        public RadicalCoroutine Schedule(MonoBehaviour behaviour, CoroutineMethod routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine().GetEnumerator());
            this.OnComplete += (s, e) =>
            {
                if (co._owner == null && !co.Cancelled && !co.Complete) co.Start(behaviour);
            };
            return co;
        }

        public void Schedule(MonoBehaviour behaviour, RadicalCoroutine routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            this.OnComplete += (s, e) =>
            {
                if (routine._owner == null && !routine.Cancelled && !routine.Complete) routine.Start(behaviour);
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
                    if (current is WaitForSeconds)
                    {
                        _manualWaitingOnInstruction = new ManualWaitForSeconds(Time.time, (float)com.spacepuppy.Utils.ObjUtil.GetValue(current, "m_Seconds"));
                    }
                    else
                    {
                        var wait = new ManualWaitForGeneric(this, handle, current as YieldInstruction);
                        _manualWaitingOnInstruction = wait;
                        handle.StartCoroutine(wait);
                    }
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
                if(_state == OperatingState.Cancelling)
                {
                    this.OnFinish(true);
                }
                return false;
            }
            else if(this.Complete)
            {
                if(_state == OperatingState.Completing)
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
                        if (_state == OperatingState.Cancelling)
                        {
                            this.OnFinish(true);
                        }
                        return false;
                    }
                    if (_derivative == null) return !this.Complete; //the derivative was cleared out by a manual tick, wait a frame or exit depending on if we completed in that time
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
                    if (_state == OperatingState.Cancelling)
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
                    _currentIEnumeratorYieldValue = current;
                }
                else if (current is WWW)
                {
                    _currentIEnumeratorYieldValue = current;
                }
                else if (current is RadicalCoroutine)
                {
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

                    var rad = current as RadicalCoroutine;
                    _derivative = WaitUntilDone_Routine(rad).GetEnumerator();

                }
                else if (current is IRadicalYieldInstruction)
                {
                    var rad = current as IRadicalYieldInstruction;
                    if (rad is IImmediatelyResumingYieldInstruction) (rad as IImmediatelyResumingYieldInstruction).Signal += this.OnImmediatelyResumingYieldInstructionSignaled;

                    var e = new RadicalCoroutine(current as IEnumerator, this) as IEnumerator;
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
                            _state = OperatingState.Completing;
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

        #region Factory Methods

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, System.Collections.IEnumerator routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            co.Start(behaviour);
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, System.Collections.IEnumerable routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine.GetEnumerator());
            co.Start(behaviour);
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, CoroutineMethod routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine().GetEnumerator());
            co.Start(behaviour);
            return co;
        }

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

            if (routine._owner != null && routine._owner is Coroutine) return routine._owner as Coroutine;

            return behaviour.StartCoroutine(WaitUntilDone_Routine(routine).GetEnumerator());
        }

        private static IEnumerable WaitUntilDone_Routine(RadicalCoroutine routine)
        {
            while (!routine.Complete)
            {
                yield return null;
            }
        }

        #endregion



        #region Special Types

        private interface IManualWait
        {

            bool StillWait { get; }

        }

        private class ManualWaitForSeconds : IManualWait
        {
            private float _startTime;
            private float _dur;

            public ManualWaitForSeconds(float startTime, float dur)
            {
                _startTime = startTime;
                _dur = dur;
            }

            public bool StillWait
            {
                get { return (Time.time - _startTime < _dur); }
            }
        }

        private class ManualWaitForGeneric : RadicalYieldInstruction, IManualWait
        {
            private RadicalCoroutine _owner;
            private MonoBehaviour _handle;
            private YieldInstruction _instruction;
            private bool _called;

            public ManualWaitForGeneric(RadicalCoroutine owner, MonoBehaviour handle, YieldInstruction instruction)
            {
                _owner = owner;
                _handle = handle;
                _instruction = instruction;
            }

            protected override bool ContinueBlocking(ref object yieldObject)
            {
                if (!_called)
                {
                    yieldObject = _instruction;
                    _called = true;
                    return true;
                }
                else if (_owner._manualWaitingOnInstruction == this)
                {
                    _owner._manualWaitingOnInstruction = null;
                    if (_instruction is WaitForEndOfFrame || _instruction is WaitForFixedUpdate)
                    {
                        _owner.ManualTick(_handle);
                    }
                }

                return false;
            }

            public bool StillWait
            {
                get { return _owner._manualWaitingOnInstruction == this; }
            }
        }

        #endregion


        
    }

}
