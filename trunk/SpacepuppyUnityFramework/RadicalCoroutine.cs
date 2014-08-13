using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public class RadicalCoroutine : IRadicalYieldInstruction, System.Collections.IEnumerator
    {

        #region Fields

        private Coroutine _coroutine;

        private System.Collections.IEnumerator _routine;
        private object _current;
        private DerivativeOperation _derivative;

        private bool _cancelled = false;

        #endregion

        #region CONSTRUCTOR

        private RadicalCoroutine(System.Collections.IEnumerator routine)
        {
            _routine = routine;
        }

        private void SetOwner(Coroutine co)
        {
            _coroutine = co;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A refernence to the routine that is operating this RadicalCoroutine. 
        /// This can be used for yielding this RadicalCoroutine in the midst of 
        /// a standard coroutine.
        /// </summary>
        public Coroutine Coroutine { get { return _coroutine; } }

        public bool Cancelled { get { return _cancelled; } }

        #endregion

        #region Methods

        public void Cancel()
        {
            _cancelled = true;
        }

        public void Reset()
        {
            _current = null;
            _derivative = null;
            _cancelled = false;
            _routine.Reset();
        }

        private bool MoveNext()
        {
            _current = null;

            if (_cancelled)
            {
                _derivative = null;
                return false;
            }

            int derivativeResult = -1;

            derivativeResult = this.OperateDerivative();
            if (derivativeResult >= 0)
            {
                return (derivativeResult > 0);
            }

            while (_routine.MoveNext())
            {
                var obj = _routine.Current;

                if (obj is IRadicalYieldInstruction)
                {
                    _derivative = new DerivativeOperation(obj as IRadicalYieldInstruction);
                    derivativeResult = this.OperateDerivative();
                    if (derivativeResult >= 0)
                    {
                        return (derivativeResult > 0);
                    }
                }
                else if (obj is System.Collections.IEnumerable)
                {
                    _derivative = new DerivativeOperation(new RadicalCoroutine((obj as System.Collections.IEnumerable).GetEnumerator()));
                    derivativeResult = this.OperateDerivative();
                    if (derivativeResult >= 0)
                    {
                        return (derivativeResult > 0);
                    }
                }
                else if (obj is System.Collections.IEnumerator)
                {
                    _derivative = new DerivativeOperation(new RadicalCoroutine(obj as System.Collections.IEnumerator));
                    derivativeResult = this.OperateDerivative();
                    if (derivativeResult >= 0)
                    {
                        return (derivativeResult > 0);
                    }
                }
                else
                {
                    _current = obj;
                    return true;
                }
            }

            _current = null;
            _derivative = null;
            return false;
        }

        /// <summary>
        /// 1 - continue blocking
        /// 0 - stop blocking
        /// -1 - loop past
        /// </summary>
        /// <returns></returns>
        private int OperateDerivative()
        {
            if (_derivative == null) return -1;

            if (_derivative.ContinueBlocking(this))
            {
                if (_cancelled)
                {
                    _current = null;
                    return 0;
                }
                _current = _derivative.CurrentYieldObject;
                return 1;
            }
            else
            {
                _derivative = null;
                if (_cancelled)
                {
                    _current = null;
                    return 0;
                }
                _current = null;
                return -1;
            }
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        object IRadicalYieldInstruction.CurrentYieldObject
        {
            get { return _current; }
        }

        bool IRadicalYieldInstruction.ContinueBlocking(RadicalCoroutine routine)
        {
            return this.MoveNext();
        }

        #endregion

        #region IEnumerator Interface

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return this.MoveNext();
        }

        void System.Collections.IEnumerator.Reset()
        {
            this.Reset();
        }

        #endregion

        #region Factory Methods

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, System.Collections.IEnumerator routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            co.SetOwner(behaviour.StartCoroutine(co));
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, System.Collections.IEnumerable routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine.GetEnumerator());
            co.SetOwner(behaviour.StartCoroutine(co));
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, System.Delegate method, params object[] args)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            System.Collections.IEnumerator e;
            if (com.spacepuppy.Utils.ObjUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerable)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerable).GetEnumerator();
            }
            else if (com.spacepuppy.Utils.ObjUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerator)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerator);
            }
            else
            {
                throw new System.ArgumentException("Delegate must have a return type of IEnumerable or IEnumerator.", "method");
            }

            var co = new RadicalCoroutine(e);
            co.SetOwner(behaviour.StartCoroutine(co));
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
                    if((bool)r)
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

        #endregion

        #region Special Types

        private class DerivativeOperation : IRadicalYieldInstruction
        {
            private IRadicalYieldInstruction _instruction;
            private DerivativeOperation _derivative;

            public DerivativeOperation(IRadicalYieldInstruction instruction)
            {
                if (instruction == null) throw new System.ArgumentNullException("instruction");
                _instruction = instruction;
            }

            public object CurrentYieldObject
            {
                get { return (_derivative != null) ? _derivative.CurrentYieldObject : _instruction.CurrentYieldObject; }
            }

            public bool ContinueBlocking(RadicalCoroutine routine)
            {
                if (_derivative != null)
                {
                    if (_derivative.ContinueBlocking(routine))
                    {
                        return true;
                    }
                    else
                    {
                        _derivative = null;
                    }
                }

                while (_instruction.ContinueBlocking(routine))
                {
                    if (_instruction.CurrentYieldObject is IRadicalYieldInstruction)
                    {
                        _derivative = new DerivativeOperation(_instruction.CurrentYieldObject as IRadicalYieldInstruction);
                        if (!_derivative.ContinueBlocking(routine))
                        {
                            _derivative = null;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (_instruction.CurrentYieldObject is System.Collections.IEnumerable)
                    {
                        _derivative = new DerivativeOperation(new RadicalCoroutine((_instruction.CurrentYieldObject as System.Collections.IEnumerable).GetEnumerator()));
                        if (!_derivative.ContinueBlocking(routine))
                        {
                            _derivative = null;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (_instruction.CurrentYieldObject is System.Collections.IEnumerator)
                    {
                        _derivative = new DerivativeOperation(new RadicalCoroutine(_instruction.CurrentYieldObject as System.Collections.IEnumerator));
                        if (!_derivative.ContinueBlocking(routine))
                        {
                            _derivative = null;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

    }

}
