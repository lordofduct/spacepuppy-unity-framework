using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Collections
{
    public class NotificationPool : ICollection<INotificationDispatcher>
    {

        #region Events

        public event System.EventHandler<Notification> OnNotification;

        #endregion

        #region Fields

        private List<INotificationDispatcher> _lst = new List<INotificationDispatcher>();
        private List<System.Type> _registeredNotifications = new List<System.Type>();

        #endregion

        #region Methods

        public void RegisterNotificationType<T>() where T : Notification
        {
            var notificationType = typeof(T);
            if (_registeredNotifications.Contains(notificationType)) return;
            _registeredNotifications.Add(notificationType);

            foreach (var d in _lst)
            {
                d.UnsafeRegisterObserver(notificationType, this.HandleGenericNotification);
            }
        }

        public void RegisterNotificationType(System.Type notificationType)
        {
            if (notificationType == null || !ObjUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (_registeredNotifications.Contains(notificationType)) return;
            _registeredNotifications.Add(notificationType);

            foreach (var d in _lst)
            {
                d.UnsafeRegisterObserver(notificationType, this.HandleGenericNotification);
            }
        }

        public void UnRegisterNotification<T>() where T : Notification
        {
            var notificationType = typeof(T);
            if (_registeredNotifications.Remove(notificationType))
            {
                foreach (var d in _lst)
                {
                    d.UnsafeRemoveObserver(notificationType, this.HandleGenericNotification);
                }
            }
        }

        public void UnRegisterNotification(System.Type notificationType)
        {
            if (notificationType == null || !ObjUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (_registeredNotifications.Remove(notificationType))
            {
                foreach (var d in _lst)
                {
                    d.UnsafeRemoveObserver(notificationType, this.HandleGenericNotification);
                }
            }
        }

        public bool ContainsNotificationType<T>() where T : Notification
        {
            return _registeredNotifications.Contains(typeof(T));
        }

        public bool ContainsNotificationType(System.Type notificationType)
        {
            if (notificationType == null || !ObjUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
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

            foreach (var d in go.GetLikeComponents<INotificationDispatcher>())
            {
                if (this.Contains(d)) return true;
            }
            return false;
        }

        public void Remove(GameObject go)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            foreach (var d in go.GetLikeComponents<INotificationDispatcher>())
            {
                if (_lst.Contains(d)) this.Remove(d);
            }
        }

        #endregion

        #region Notification Forwarding Handler

        private void HandleGenericNotification(Notification n)
        {
            if (this.OnNotification != null) this.OnNotification(this, n);
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _lst.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(INotificationDispatcher item)
        {
            if (item == null) throw new System.ArgumentNullException("item");
            if (_lst.Contains(item)) return;

            _lst.Add(item);
            foreach (var tp in _registeredNotifications)
            {
                item.UnsafeRegisterObserver(tp, this.HandleGenericNotification);
            }
        }

        public void Clear()
        {
            foreach (var d in _lst)
            {
                foreach (var tp in _registeredNotifications)
                {
                    d.UnsafeRemoveObserver(tp, this.HandleGenericNotification);
                }
            }

            _lst.Clear();
        }

        public bool Contains(INotificationDispatcher item)
        {
            return _lst.Contains(item);
        }

        public void CopyTo(INotificationDispatcher[] array, int arrayIndex)
        {
            _lst.CopyTo(array, arrayIndex);
        }

        public bool Remove(INotificationDispatcher item)
        {
            if (_lst.Remove(item))
            {
                foreach (var tp in _registeredNotifications)
                {
                    item.UnsafeRemoveObserver(tp, this.HandleGenericNotification);
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
            return _lst.GetEnumerator();
        }

        public IEnumerator<INotificationDispatcher> GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        #endregion

    }
}
