using UnityEngine;

namespace com.spacepuppy
{

    /// <summary>
    /// Just a name for contract purposes.
    /// </summary>
    public interface IRadicalYieldInstruction : System.Collections.IEnumerator
    {

    }

    public interface IImmediatelyResumingYieldInstruction : IRadicalYieldInstruction
    {
        event System.EventHandler Signal;
    }

    public abstract class RadicalYieldInstruction : IRadicalYieldInstruction
    {

        #region Fields

        private object _current;

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected abstract bool ContinueBlocking(ref object yieldObject);

        #endregion

        #region IRadicalYieldInstruction Interface

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            _current = null;
            return this.ContinueBlocking(ref _current);
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion

    }

    public abstract class ImmediatelyResumingYieldInstruction : RadicalYieldInstruction, IImmediatelyResumingYieldInstruction
    {

        private System.EventHandler _handler;

        protected void SetSignal()
        {
            if (_handler != null) _handler(this, System.EventArgs.Empty);
        }

        event System.EventHandler IImmediatelyResumingYieldInstruction.Signal
        {
            add { _handler += value; }
            remove { _handler -= value; }
        }

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

        protected override bool ContinueBlocking(ref object yieldObject)
        {
            _t += GameTime.RealDeltaTime;

            yieldObject = null;
            return _t < _dur;
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

        protected override bool ContinueBlocking(ref object yieldObject)
        {
            yieldObject = null;
            return _notification == null;
        }

    }

}
