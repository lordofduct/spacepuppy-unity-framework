using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Collections
{
    public class NotificationPool : ICollection<INotificationDispatcher>
    {

        #region Events

        public event NotificationHandler OnNotification;

        #endregion

        #region Fields

        private HashSet<INotificationDispatcher> _coll = new HashSet<INotificationDispatcher>();
        private List<System.Type> _registeredNotifications = new List<System.Type>();
        private NotificationHandler _genericHandler;

        #endregion

        #region CONSTRUCTOR

        public NotificationPool()
        {
            _genericHandler = new NotificationHandler(this.HandleGenericNotification);
        }

        #endregion

        #region Methods

        public void RegisterNotificationType<T>() where T : Notification
        {
            var notificationType = typeof(T);
            if (_registeredNotifications.Contains(notificationType)) return;
            _registeredNotifications.Add(notificationType);

            var e = _coll.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Observers.UnsafeRegisterObserver(notificationType, _genericHandler);
            }
        }

        public void RegisterNotificationType(System.Type notificationType)
        {
            if (notificationType == null || !TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (_registeredNotifications.Contains(notificationType)) return;
            _registeredNotifications.Add(notificationType);

            var e = _coll.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Observers.UnsafeRegisterObserver(notificationType, _genericHandler);
            }
        }

        public void UnRegisterNotification<T>() where T : Notification
        {
            var notificationType = typeof(T);
            if (_registeredNotifications.Remove(notificationType))
            {
                var e = _coll.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Observers.UnsafeRemoveObserver(notificationType, _genericHandler);
                }
            }
        }

        public void UnRegisterNotification(System.Type notificationType)
        {
            if (notificationType == null || !TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (_registeredNotifications.Remove(notificationType))
            {
                var e = _coll.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Observers.UnsafeRemoveObserver(notificationType, _genericHandler);
                }
            }
        }

        public bool ContainsNotificationType<T>() where T : Notification
        {
            return _registeredNotifications.Contains(typeof(T));
        }

        public bool ContainsNotificationType(System.Type notificationType)
        {
            if (notificationType == null || !TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            return _registeredNotifications.Contains(notificationType);
        }

        public void Add(GameObject go)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            var d = go.AddOrGetComponent<GameObjectNotificationDispatcher>();
            this.Add(d);
        }

        public bool Contains(GameObject go)
        {
            if (go == null) throw new System.ArgumentNullException("go");
            
            using (var lst = TempCollection.GetList<INotificationDispatcher>())
            {
                go.GetComponents<INotificationDispatcher>(lst);
                var e = lst.GetEnumerator();
                while(e.MoveNext())
                {
                    if (this.Contains(e.Current)) return true;
                }
            }

            return false;
        }

        public void Remove(GameObject go)
        {
            if (go == null) throw new System.ArgumentNullException("go");
            
            using (var lst = TempCollection.GetList<INotificationDispatcher>())
            {
                go.GetComponents<INotificationDispatcher>(lst);
                var e = lst.GetEnumerator();
                while (e.MoveNext())
                {
                    if (_coll.Contains(e.Current)) this.Remove(e.Current);
                }
            }
        }

        #endregion

        #region Notification Forwarding Handler

        private void HandleGenericNotification(object sender, Notification n)
        {
            if (this.OnNotification != null) this.OnNotification(sender, n);
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _coll.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(INotificationDispatcher item)
        {
            if (item == null) throw new System.ArgumentNullException("item");
            if (_coll.Contains(item)) return;

            _coll.Add(item);
            var e = _registeredNotifications.GetEnumerator();
            while(e.MoveNext())
            {
                item.Observers.UnsafeRegisterObserver(e.Current, _genericHandler);
            }
        }

        public void Clear()
        {
            var e = _coll.GetEnumerator();
            while(e.MoveNext())
            {
                var e2 = _registeredNotifications.GetEnumerator();
                while(e2.MoveNext())
                {
                    e.Current.Observers.UnsafeRemoveObserver(e2.Current, _genericHandler);
                }
            }

            _coll.Clear();
        }

        public bool Contains(INotificationDispatcher item)
        {
            return _coll.Contains(item);
        }

        public void CopyTo(INotificationDispatcher[] array, int arrayIndex)
        {
            _coll.CopyTo(array, arrayIndex);
        }

        public bool Remove(INotificationDispatcher item)
        {
            if (_coll.Remove(item))
            {
                var e = _registeredNotifications.GetEnumerator();
                while(e.MoveNext())
                {
                    item.Observers.UnsafeRemoveObserver(e.Current, _genericHandler);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _coll.GetEnumerator();
        }

        public IEnumerator<INotificationDispatcher> GetEnumerator()
        {
            return _coll.GetEnumerator();
        }

        #endregion

    }
}
