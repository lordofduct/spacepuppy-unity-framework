using UnityEngine;

namespace com.spacepuppy
{

    /// <summary>
    /// Just a name for contract purposes.
    /// </summary>
    public interface IRadicalYieldInstruction : System.Collections.IEnumerator
    {
        void Init(RadicalCoroutine routine);
    }

    public abstract class RadicalYieldInstruction : IRadicalYieldInstruction
    {

        #region Fields

        private object _current;
        private RadicalCoroutine _routine;

        #endregion

        #region Properties

        protected RadicalCoroutine Routine { get { return _routine; } }

        #endregion

        #region Methods

        protected virtual void Init()
        {

        }

        protected virtual void DeInit()
        {

        }

        protected abstract bool ContinueBlocking(ref object yieldObject);

        #endregion

        #region IRadicalYieldInstruction Interface

        void IRadicalYieldInstruction.Init(RadicalCoroutine routine)
        {
            _routine = routine;
            this.Init();
        }

        #endregion

        #region IEnumerator Interface

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            _current = null;
            if (this.ContinueBlocking(ref _current))
            {
                return true;
            }
            else
            {
                this.DeInit();
                _routine = null;
                return false;
            }
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

        protected override void Init()
        {
            _t = 0f;
        }

        protected override bool ContinueBlocking(ref object yieldObject)
        {
            _t += GameTime.RealDeltaTime;

            yieldObject = null;
            return _t < _dur;
        }

        #endregion

    }

    public class WaitForNotification<T> : RadicalYieldInstruction where T : Notification
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
        }

        protected override bool ContinueBlocking(ref object yieldObject)
        {
            yieldObject = null;
            return _notification == null;
        }

    }

}
