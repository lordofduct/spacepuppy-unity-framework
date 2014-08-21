using UnityEngine;
using System.Collections;
using System.Linq;

namespace com.spacepuppy
{

    public class RadicalCoroutine : RadicalYieldInstruction
    {

        #region Fields

        private object _owner; //this houses either the Coroutine or RadicalCoroutine that may already be operating this RadicalCoroutine

        private System.Collections.IEnumerator _routine;
        private System.Collections.IEnumerator _derivative;

        private bool _completed = false;
        private ManualWaitRedirectYieldInstruction _manualWaitingOnInstruction;

        #endregion

        #region CONSTRUCTOR

        public RadicalCoroutine()
        {

        }

        private RadicalCoroutine(System.Collections.IEnumerator routine, RadicalCoroutine owner)
        {
            _routine = routine;
            _owner = owner;
        }

        protected void SetOwner(object owner)
        {
            if (_owner != null) throw new System.InvalidOperationException("Can not set the owner of a routine that is already owned.");

            _owner = owner;
        }

        #endregion

        #region Properties

        public bool Complete { get { return _completed; } }

        /// <summary>
        /// An operator is still operating this routine and it is not eligible to be started again.
        /// </summary>
        public bool HasOperator { get { return _owner != null; } }

        #endregion

        #region Methods

        public void Start(MonoBehaviour behaviour, IEnumerator routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");
            if (_owner != null) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");

            this.Clear();

            _routine = routine;
            _owner = behaviour.StartCoroutine(this);
        }

        /// <summary>
        /// Resets coroutine so it can start over from the beginning.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (_routine != null)
            {
                _routine.Reset();
            }
            if (_derivative != null && _derivative is RadicalCoroutine)
            {
                (_derivative as RadicalCoroutine).Cancel();
            }

            _derivative = null;
            _manualWaitingOnInstruction = null;
            _completed = false;
        }

        /// <summary>
        /// Stops the coroutine flagging it as complete. The coroutine still may not be used again as the operator may maintain a reference for resetting. Check the 'HasOwner' property.
        /// </summary>
        public void Cancel()
        {
            if (_routine != null && _routine is RadicalCoroutine)
            {
                (_routine as RadicalCoroutine).Cancel();
            }
            if (_derivative != null && _derivative is RadicalCoroutine)
            {
                (_derivative as RadicalCoroutine).Cancel();
            }

            _derivative = null;
            _manualWaitingOnInstruction = null;
            _completed = true;
        }

        /// <summary>
        /// Stops the coroutine completely and purges the underlying routines. The coroutine still may not be used again until the operator has cleaned itself. Can take about one frame. Check the 'HasOwner' property.
        /// </summary>
        public void Clear()
        {
            if (_routine != null && _routine is RadicalCoroutine)
            {
                (_routine as RadicalCoroutine).Clear();
            }
            if (_derivative != null && _derivative is RadicalCoroutine)
            {
                (_derivative as RadicalCoroutine).Clear();
            }
            _routine = null;
            _derivative = null;
            _completed = false;
            _manualWaitingOnInstruction = null;

            if (_owner != null && _owner is RadicalCoroutine)
            {
                var rad = _owner as RadicalCoroutine;
                if (rad._routine == this)
                {
                    rad.Clear();
                }
                else if (rad._derivative == this)
                {
                    rad._derivative = null;
                }
                _owner = null;
            }
        }




        protected override bool ContinueBlocking(ref object yieldObject)
        {
            if (_completed || _routine == null)
            {
                if (_owner is Coroutine) _owner = null; //Coroutine stop owning as soon as false is returned
                return false;
            }

            if (_manualWaitingOnInstruction != null) return true;

            if (_derivative != null)
            {
                if (_derivative.MoveNext())
                {
                    if (_derivative == null) return !_completed; //our routine cancelled/cleared/reset itself
                    yieldObject = _derivative.Current;
                    return true;
                }
                else
                {
                    if (_derivative is RadicalCoroutine) (_derivative as RadicalCoroutine)._owner = null;
                    _derivative = null;
                }
            }


            if (_routine.MoveNext())
            {
                if (_completed || _routine == null) return false; //our routine cancelled/cleared/reset itself

                var current = _routine.Current;
                if (current == null)
                {
                    //do nothing
                }
                else if (current is YieldInstruction)
                {
                    yieldObject = current;
                }
                else if (current is RadicalCoroutine)
                {
                    IEnumerator e = null;
                    var rad = current as RadicalCoroutine;
                    if (rad._owner == null)
                    {
                        rad._owner = this;
                        e = rad as IEnumerator;
                    }
                    else
                    {
                        e = WaitUntilDone_Routine(rad).GetEnumerator();
                    }

                    if (e.MoveNext())
                    {
                        yieldObject = e.Current;
                        _derivative = e;
                    }
                }
                else if (current is IEnumerable)
                {
                    //yes we have to test for IEnumerable before IEnumerator. When a yield method is returned as an IEnumerator, it still needs 'GetEnumerator' called on it.
                    var e = new RadicalCoroutine((current as IEnumerable).GetEnumerator(), this) as IEnumerator;
                    if (e.MoveNext())
                    {
                        yieldObject = e.Current;
                        _derivative = e;
                    }
                }
                else if (current is IEnumerator)
                {
                    var e = new RadicalCoroutine(current as IEnumerator, this) as IEnumerator;
                    if (e.MoveNext())
                    {
                        yieldObject = e.Current;
                        _derivative = e;
                    }
                }
                else
                {
                    yieldObject = current;
                }

                return true;
            }
            else
            {
                _completed = true;
                if (_owner is Coroutine) _owner = null; //Coroutine stop owning as soon as false is returned
                return false;
            }
        }

        #endregion

        #region Manual Coroutine Methods

        public void StartManual(IEnumerator routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");
            if (_owner != null) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");

            this.Clear();

            _routine = routine;
        }

        public void StartManual(IEnumerable routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");
            if (_owner != null) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");

            this.Clear();

            _routine = routine.GetEnumerator();
        }

        public bool ManualTick()
        {
            if (_owner == null) throw new System.InvalidOperationException("Can not manually operate a RadicalCoroutine that is already being operated on.");

            object current = null;
            if (this.ContinueBlocking(ref current))
            {
                if(current is YieldInstruction)
                {
                    _manualWaitingOnInstruction = new ManualWaitRedirectYieldInstruction(this, current as YieldInstruction);
                    GameLoopEntry.StartCoroutine(_manualWaitingOnInstruction);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region Factory Methods

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, System.Collections.IEnumerator routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine();
            co.Start(behaviour, routine);
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, System.Collections.IEnumerable routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine();
            co.Start(behaviour, routine.GetEnumerator());
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, CoroutineMethod routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine();
            co.Start(behaviour, routine().GetEnumerator());
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

        private class ManualWaitRedirectYieldInstruction : RadicalYieldInstruction
        {
            private RadicalCoroutine _owner;
            private YieldInstruction _instruction;
            private bool _called;

            public ManualWaitRedirectYieldInstruction(RadicalCoroutine owner, YieldInstruction instruction)
            {
                _owner = owner;
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
                        _owner.ManualTick();
                    }
                }

                return false;
            }
        }

        #endregion

    }

}
