using UnityEngine;

namespace com.spacepuppy
{

    /// <summary>
    /// Just a name for contract purposes.
    /// </summary>
    public interface IRadicalYieldInstruction : System.Collections.IEnumerator
    {

    }

    public interface IProgressingYieldInstruction : IRadicalYieldInstruction
    {
        bool IsComplete { get; }
        float Progress { get; }
    }

    public interface IImmediatelyResumingYieldInstruction : IRadicalYieldInstruction
    {
        event System.EventHandler Signal;
    }

    public abstract class RadicalYieldInstruction : IRadicalYieldInstruction
    {

        #region Fields

        private object _current;
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

        protected abstract object Tick();

        #endregion

        #region IRadicalYieldInstruction Interface

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            _current = this.Tick();
            return !_complete;
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion



        #region Static Interface

        private static IProgressingYieldInstruction _null = new NullYieldInstruction();
        public static IProgressingYieldInstruction Null
        {
            get
            {
                return _null;
            }
        }

        private class NullYieldInstruction : IProgressingYieldInstruction
        {

            public bool IsComplete
            {
                get { return true; }
            }

            public float Progress
            {
                get { return 1f; }
            }

            public object Current
            {
                get { return null; }
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {

            }
        }

        #endregion

    }

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

        object System.Collections.IEnumerator.Current
        {
            get
            {
                for (int i = 0; i < _operations.Length; i++)
                {
                    if (!_operations[i].IsComplete) return _operations[i];
                }
                return null;
            }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return !this.IsComplete;
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion

    }

    public class WaitForRealSeconds : RadicalYieldInstruction
    {

        #region Fields

        private float _dur;
        private float _t;

        #endregion

        #region CONSTRUCTOR

        public WaitForRealSeconds(float duration)
        {
            _dur = duration;
            _t = 0f;
        }

        #endregion

        #region Methods

        protected override object Tick()
        {
            _t += GameTime.RealDeltaTime;
            if (_t >= _dur) this.SetSignal();
            return null;
        }

        #endregion

    }

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

        private void OnNotification(T n)
        {
            if (_ignoreCheck != null && _ignoreCheck(n)) return;

            _notification = n;
            _dispatcher.RemoveObserver<T>(this.OnNotification);
            this.SetSignal();
        }

        protected override object Tick()
        {
            return null;
        }

    }

}
