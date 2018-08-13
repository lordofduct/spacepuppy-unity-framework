using UnityEngine;
using System.Collections;

using com.spacepuppy.Dynamic;

namespace com.spacepuppy
{
    public class TandemCoroutine : RadicalYieldInstruction
    {

        #region Fields

        private CancellableCoroutine _updateRoutine;
        private CancellableCoroutine _fixedUpdateRoutine;

        private FiberCollection _fibers = new FiberCollection();
        private Fiber[] iteratorFibers = new Fiber[0];

        private WaitForEndOfFrame _cachedWaitForEndOfFrame = new WaitForEndOfFrame();
        private WaitForFixedUpdate _cachedWaitForFixedUpdate = new WaitForFixedUpdate();

        #endregion

        #region CONSTRUCTOR

        public TandemCoroutine()
        {
            
        }

        public TandemCoroutine(params IEnumerator[] routines)
        {
            foreach(var r in routines)
            {
                _fibers.Add(r);
            }
        }

        #endregion

        #region Properties

        public FiberCollection Fibers { get { return _fibers; } }

        #endregion

        #region Methods

        public void Start(MonoBehaviour behaviour)
        {
            if (_updateRoutine != null) throw new System.InvalidOperationException("Failed to start RadicalCoroutine. The Coroutine is already being processed.");
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");

            _updateRoutine = new CancellableCoroutine(this.Update());
            _fixedUpdateRoutine = new CancellableCoroutine(this.FixedUpdate().GetEnumerator());

            iteratorFibers = new Fiber[_fibers.Count];

            behaviour.StartCoroutine(_updateRoutine);
            behaviour.StartCoroutine(_fixedUpdateRoutine);
        }

        public void Stop()
        {
            if(_updateRoutine != null)
            {
                _updateRoutine.Cancel();
                _fixedUpdateRoutine.Cancel();
            }

            _updateRoutine = null;
            _fixedUpdateRoutine = null;
        }

        #endregion

        #region Update Methods

        private IEnumerator Update()
        {
            int len;
            while (true)
            {
                if (iteratorFibers.Length < _fibers.Count)
                {
                    System.Array.Resize(ref iteratorFibers, _fibers.Count);
                }
                _fibers.CopyTo(iteratorFibers, 0);
                len = _fibers.Count;
                for (int i = 0; i < len; i++)
                {
                    if (!iteratorFibers[i].Tick(UpdateState.Update)) _fibers.Remove(iteratorFibers[i]);
                    iteratorFibers[i] = null;
                }
                if (iteratorFibers.Length < _fibers.Count)
                {
                    System.Array.Resize(ref iteratorFibers, _fibers.Count);
                }

                yield return _cachedWaitForEndOfFrame;

                if (iteratorFibers.Length < _fibers.Count)
                {
                    System.Array.Resize(ref iteratorFibers, _fibers.Count);
                }
                _fibers.CopyTo(iteratorFibers, 0);
                len = _fibers.Count;
                for (int i = 0; i < len; i++)
                {
                    if (!iteratorFibers[i].Tick(UpdateState.EndOfFrame)) _fibers.Remove(iteratorFibers[i]);
                    iteratorFibers[i] = null;
                }
                if (iteratorFibers.Length < _fibers.Count)
                {
                    System.Array.Resize(ref iteratorFibers, _fibers.Count);
                }

                if (_fibers.Count == 0) this.SetSignal();
                yield return null;
            }
        }

        private IEnumerable FixedUpdate()
        {
            int len;
            while (true)
            {
                _fibers.CopyTo(iteratorFibers, 0);
                len = _fibers.Count;
                for (int i = 0; i < len; i++)
                {
                    if (!iteratorFibers[i].Tick(UpdateState.FixedUpdate)) _fibers.Remove(iteratorFibers[i]);
                    iteratorFibers[i] = null;
                }
                if (iteratorFibers.Length < _fibers.Count)
                {
                    System.Array.Resize(ref iteratorFibers, _fibers.Count);
                }

                yield return _cachedWaitForFixedUpdate;
            }
        }

        #endregion

        #region Special Types

        private class CancellableCoroutine : IEnumerator
        {
            private IEnumerator _enum;

            public CancellableCoroutine(IEnumerator e)
            {
                _enum = e;
            }

            public void Cancel()
            {
                _enum = null;
            }

            object IEnumerator.Current
            {
                get { return _enum.Current; }
            }

            bool IEnumerator.MoveNext()
            {
                if (_enum == null) return false;
                return _enum.MoveNext();
            }

            void IEnumerator.Reset()
            {
                throw new System.NotSupportedException();
            }
        }

        internal enum UpdateState
        {
            Update = 0,
            EndOfFrame = 1,
            FixedUpdate = 2
        }

        public sealed class Fiber
        {

            private IEnumerator _routine;

            private bool _waitForEndOfFrame;
            private bool _waitForFixedUpdate;
            private float _waitForSeconds;
            private object _waitForObject;

            internal Fiber(IEnumerator e)
            {
                _routine = new RadicalCoroutine(e);
            }

            internal bool Tick(UpdateState state)
            {
                //tick derivative first
                if(_waitForObject != null)
                {
                    if (_waitForObject is AsyncOperation)
                    {
                        if ((_waitForObject as AsyncOperation).isDone)
                        {
                            _waitForObject = null;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (_waitForObject is WWW)
                    {
                        if ((_waitForObject as WWW).isDone)
                        {
                            _waitForObject = null;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        _waitForObject = null;
                    }
                }

                //check state
                switch(state)
                {
                    case UpdateState.Update:
                        if(_waitForSeconds > 0f)
                        {
                            _waitForSeconds -= Time.deltaTime;
                            return true;
                        }
                        if (_waitForFixedUpdate) return true; //waiting for a fixedupdate, don't tick
                        break;
                    case UpdateState.EndOfFrame:
                        if (!_waitForEndOfFrame) return true; //not waiting for end of frame
                         _waitForEndOfFrame = false;
                        break;
                    case UpdateState.FixedUpdate:
                        if (!_waitForFixedUpdate) return true; //not waiting for fixed update
                        _waitForFixedUpdate = false;
                        break;
                }

                //get current
                object c;
                if(_routine != null && _routine.MoveNext())
                {
                    c = _routine.Current;
                }
                else
                {
                    return false;
                }

                if (c == null)
                {
                    //do nothing, no need to test all others if null
                }
                else if (c is WaitForSeconds)
                {
                    _waitForSeconds = (float)DynamicUtil.GetValue(c, "m_Seconds");
                }
                else if (c is WaitForEndOfFrame)
                {
                    _waitForEndOfFrame = true;
                }
                else if (c is WaitForFixedUpdate)
                {
                    _waitForFixedUpdate = true;
                }
                else if (c is AsyncOperation)
                {
                    _waitForObject = c;
                }
                else if (c is Coroutine)
                {
                    throw new System.NotSupportedException("TandemCoroutine does not support its routines to yield a Coroutine. Try yielding a RadicalCoroutine instead.");
                }
                else if (c is WWW)
                {
                    _waitForObject = c;
                }

                return true;
            }

        }

        public sealed class FiberCollection : System.Collections.Generic.IList<Fiber>
        {

            private System.Collections.Generic.List<Fiber> _lst = new System.Collections.Generic.List<Fiber>();

            #region Other Methods

            public Fiber Add(IEnumerator routine)
            {
                var f = new Fiber(routine);
                _lst.Add(f);
                return f;
            }

            public Fiber Insert(int index, IEnumerator routine)
            {
                var f = new Fiber(routine);
                _lst.Insert(index, f);
                return f;
            }

            public void Swap(Fiber a, Fiber b)
            {
                var i = _lst.IndexOf(a);
                var j = _lst.IndexOf(b);
                if (i < 0 || j < 0) return;

                _lst[j] = a;
                _lst[i] = b;
            }

            public void SwapAt(int i, int j)
            {
                if (i == j) return;
                if (i < 0 || j < 0 || i >= _lst.Count || j >= _lst.Count) return;

                var t = _lst[j];
                _lst[j] = _lst[i];
                _lst[i] = t;
            }

            #endregion

            #region IList Interface

            public Fiber this[int index]
            {
                get
                {
                    return _lst[index];
                }
                set
                {
                    _lst[index] = value;
                }
            }

            public int Count
            {
                get { return _lst.Count; }
            }

            bool System.Collections.Generic.ICollection<Fiber>.IsReadOnly
            {
                get { return false; }
            }

            void System.Collections.Generic.ICollection<Fiber>.Add(Fiber item)
            {
                _lst.Add(item);
            }

            public void Clear()
            {
                _lst.Clear();
            }

            public bool Contains(Fiber item)
            {
                return _lst.Contains(item);
            }

            public void CopyTo(Fiber[] array, int arrayIndex)
            {
                _lst.CopyTo(array, arrayIndex);
            }

            public int IndexOf(Fiber item)
            {
                return _lst.IndexOf(item);
            }

            void System.Collections.Generic.IList<Fiber>.Insert(int index, Fiber item)
            {
                _lst.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                _lst.RemoveAt(index);
            }

            public bool Remove(Fiber item)
            {
                return _lst.Remove(item);
            }

            public IEnumerator GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            System.Collections.Generic.IEnumerator<Fiber> System.Collections.Generic.IEnumerable<Fiber>.GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            #endregion

        }

        #endregion

    }
}
