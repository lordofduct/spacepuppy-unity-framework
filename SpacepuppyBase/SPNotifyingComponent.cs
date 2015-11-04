namespace com.spacepuppy
{

    public abstract class SPNotifyingComponent : SPComponent, INotificationDispatcher
    {

        #region Fields

        [System.NonSerialized]
        private NotificationDispatcher _dispatcher;

        #endregion

        #region CONSTRUCTOR

        protected override void OnDespawn()
        {
            this.PurgeHandlers();
        }

        #endregion

        #region INotificationDispatcher Interface

        public void RegisterObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (_dispatcher == null) _dispatcher = new NotificationDispatcher(this);
            _dispatcher.RegisterObserver<T>(handler);
        }

        public void RemoveObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (_dispatcher == null) return;
            _dispatcher.RemoveObserver<T>(handler);
        }

        public bool HasObserver<T>(bool bNotifyEntity) where T : Notification
        {
            //if(bNotifyEntity)
            //{
            //    if (_dispatcher == null) _dispatcher = new NotificationDispatcher(this);
            //    return _dispatcher.HasObserver<T>(bNotifyEntity);
            //}
            //else
            //{
            //    if (_dispatcher == null) return false;
            //    return _dispatcher.HasObserver<T>(bNotifyEntity);
            //}
            if (_dispatcher == null) _dispatcher = new NotificationDispatcher(this);
            return _dispatcher.HasObserver<T>(bNotifyEntity);
        }

        public bool PostNotification<T>(T n, bool bNotifyEntity) where T : Notification
        {
            if (_dispatcher == null) _dispatcher = new NotificationDispatcher(this);
            return _dispatcher.PostNotification<T>(n, bNotifyEntity);
        }

        public void UnsafeRegisterObserver(System.Type tp, NotificationHandler handler)
        {
            if (_dispatcher == null) _dispatcher = new NotificationDispatcher(this);
            _dispatcher.UnsafeRegisterObserver(tp, handler);
        }

        public void UnsafeRemoveObserver(System.Type tp, NotificationHandler handler)
        {
            if (_dispatcher == null) return;
            _dispatcher.UnsafeRemoveObserver(tp, handler);
        }

        public bool HasObserver(System.Type tp, bool bNotifyEntity)
        {
            //if (bNotifyEntity)
            //{
            //    if (_dispatcher == null) _dispatcher = new NotificationDispatcher(this);
            //    return _dispatcher.HasObserver(tp, bNotifyEntity);
            //}
            //else
            //{
            //    if (_dispatcher == null) return false;
            //    return _dispatcher.HasObserver(tp, bNotifyEntity);
            //}
            if (_dispatcher == null) _dispatcher = new NotificationDispatcher(this);
            return _dispatcher.HasObserver(tp, bNotifyEntity);
        }

        public bool UnsafePostNotification(Notification n, bool bNotifyEntity)
        {
            if (_dispatcher == null) _dispatcher = new NotificationDispatcher(this);
            return _dispatcher.UnsafePostNotification(n, bNotifyEntity);
        }

        public void PurgeHandlers()
        {
            if (_dispatcher == null) return;
            _dispatcher.PurgeHandlers();
        }

        #endregion

    }

}
