
namespace com.spacepuppy
{

    /// <summary>
    /// Blocks until the dispatcher fires some notification. See the notification system in com.spacepuppy.Notification.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WaitForNotification<T> : ImmediatelyResumingYieldInstruction where T : Notification
    {

        private INotificationDispatcher _dispatcher;
        private T _notification;
        private System.Func<T, bool> _ignoreCheck;

        public WaitForNotification(INotificationDispatcher dispatcher, System.Func<T, bool> ignoreCheck = null)
        {
            _dispatcher = dispatcher;
            _dispatcher.Observers.RegisterObserver<T>(this.OnNotification);
            _ignoreCheck = ignoreCheck;
        }

        public T Notification { get { return _notification; } }

        private void OnNotification(object sender, T n)
        {
            if (_ignoreCheck != null && _ignoreCheck(n)) return;

            _notification = n;
            _dispatcher.Observers.RemoveObserver<T>(this.OnNotification);
            this.SetSignal();
        }

    }

}
