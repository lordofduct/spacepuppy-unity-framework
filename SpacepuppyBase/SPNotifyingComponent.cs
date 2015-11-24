namespace com.spacepuppy
{

    public abstract class SPNotifyingComponent : SPComponent, INotificationDispatcher
    {

        #region Fields

        [System.NonSerialized]
        private NotificationDispatcher _observers;

        #endregion

        #region CONSTRUCTOR

        protected override void OnDespawn()
        {
            if (_observers != null) _observers.PurgeHandlers();
        }

        #endregion

        public NotificationDispatcher Observers
        {
            get
            {
                if (_observers == null) _observers = new NotificationDispatcher(this);
                return _observers;
            }
        }
        
    }

}
