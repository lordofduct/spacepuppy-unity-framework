using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public class RadicalCoroutine : RadicalYieldInstruction
    {

        #region Fields

        private Coroutine _coroutine;

        private System.Collections.IEnumerator _routine;
        private System.Collections.IEnumerator _derivative;

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

        public override void Reset()
        {
            base.Reset();

            _cancelled = false;
            _routine.Reset();
        }

        protected override bool ContinueBlocking(ref object yieldObject)
        {
            if (_cancelled) return false;

            if (_derivative != null)
            {
                if (_derivative.MoveNext())
                {
                    yieldObject = _derivative.Current;
                    return true;
                }
                else
                {
                    _derivative = null;
                }
            }


            if (_routine.MoveNext())
            {
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
                    var e = current as IEnumerator;
                    if (e.MoveNext())
                    {
                        yieldObject = e.Current;
                        _derivative = e;
                    }
                }
                else if (current is IEnumerable)
                {
                    var e = new RadicalCoroutine((current as IEnumerable).GetEnumerator()) as IEnumerator;
                    if (e.MoveNext())
                    {
                        yieldObject = e.Current;
                        _derivative = e;
                    }
                }
                else if (current is IEnumerator)
                {
                    var e = new RadicalCoroutine(current as IEnumerator) as IEnumerator;
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
                return false;
            }
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

        public static RadicalCoroutine StartRadicalCoroutine(MonoBehaviour behaviour, CoroutineMethod routine)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine().GetEnumerator());
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

        #endregion

    }

}
