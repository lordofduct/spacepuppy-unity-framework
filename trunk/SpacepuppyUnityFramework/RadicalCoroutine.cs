using UnityEngine;
using System.Collections;
using System.Linq;

namespace com.spacepuppy
{

    public class RadicalCoroutine : RadicalYieldInstruction
    {

        #region Fields

        //the coroutine operating this radical coroutine, a reference is kept in case we want to yield this to a standard coroutine
        private Coroutine _coroutine;

        private System.Collections.IEnumerator _routine;
        private System.Collections.IEnumerator _derivative;

        private bool _completed = false;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Operating level constructor.
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="coroutine"></param>
        private RadicalCoroutine(System.Collections.IEnumerator routine, Coroutine coroutine)
        {
            _routine = routine;
            _coroutine = coroutine;
        }

        /// <summary>
        /// Factory level constructor. SetOwner is required to be called after this.
        /// </summary>
        /// <param name="routine"></param>
        private RadicalCoroutine(System.Collections.IEnumerator routine)
        {
            _routine = routine;
        }

        private void SetOwner(Coroutine routine)
        {
            _coroutine = routine;
        }

        #endregion

        #region Properties

        public bool Complete { get { return _completed; } }

        #endregion

        #region Methods

        public void Cancel()
        {
            _completed = true;
        }

        public override void Reset()
        {
            base.Reset();

            _completed = false;
            _routine.Reset();
        }

        /// <summary>
        /// Get a YieldInstruction for yielding to another Coroutine that isn't a radical coroutine.
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public YieldInstruction Yield()
        {
            return _coroutine;
        }


        protected override bool ContinueBlocking(ref object yieldObject)
        {
            if (_completed) return false;

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
                    //yes we have to test for IEnumerable before IEnumerator. When a yield method is returned as an IEnumerator, it still needs 'GetEnumerator' called on it.
                    var e = new RadicalCoroutine((current as IEnumerable).GetEnumerator(), _coroutine) as IEnumerator;
                    if (e.MoveNext())
                    {
                        yieldObject = e.Current;
                        _derivative = e;
                    }
                }
                else if (current is IEnumerator)
                {
                    var e = new RadicalCoroutine(current as IEnumerator, _coroutine) as IEnumerator;
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
