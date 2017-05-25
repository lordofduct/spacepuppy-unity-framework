using System;
using System.Collections.Generic;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{
    /// <summary>
    /// Allows registering for specific 'Types' of notification denoted by a class inherited from this class. 
    /// You can register for a notification from a specific object, or register for a type of notification globally. 
    /// You can register to listen to notifications directly from the object, a gameObject if that object is a gameObject supplier, 
    /// or from a root gameObject if that gameObject has a root other than itself.
    /// 
    /// When using Unsafe access for adding and removing handlers. You must use the same for both. If you add a handler with Unsafe 
    /// you must remove it with Unsafe. If you add with the standard generic method, you must remove with the standard generic method.
    /// </summary>
    public abstract class Notification : System.EventArgs, ICloneable
    {

        #region Fields

        #endregion

        #region CONSTRUCTOR

        public Notification()
        {

        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Post(INotificationDispatcher dispatcher, bool bNotifyEntity = false)
        {

        }

        #endregion

        #region ICloneable Interface

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #region Static Interface

        private static Dictionary<System.Type, System.Delegate> _globalHandlers = new Dictionary<System.Type, System.Delegate>();
        private static Dictionary<System.Type, NotificationHandler> _unsafeGlobalHandlers = new Dictionary<Type, NotificationHandler>();

        #region Global Notification Interface

        public static void RegisterGlobalObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (handler == null) throw new System.ArgumentNullException("handler");

            var tp = typeof(T);
            System.Delegate d;
            if (_globalHandlers.TryGetValue(tp, out d))
            {
                _globalHandlers[tp] = System.Delegate.Combine(d, handler);
            }
            else
            {
                _globalHandlers[tp] = handler;
            }
        }

        public static void RemoveGlobalObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (handler == null) throw new System.ArgumentNullException("handler");

            var tp = typeof(T);
            System.Delegate d;
            if(_globalHandlers.TryGetValue(tp, out d))
            {
                d = System.Delegate.Remove(d, handler);
                if (d == null)
                {
                    _globalHandlers.Remove(tp);
                }
                else
                {
                    _globalHandlers[tp] = d;
                }
            }
        }

        public static void UnsafeRegisterGlobalObserver(System.Type notificationType, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");

            NotificationHandler d;
            if (_unsafeGlobalHandlers.TryGetValue(notificationType, out d))
            {
                _unsafeGlobalHandlers[notificationType] = d + handler;
            }
            else
            {
                _unsafeGlobalHandlers[notificationType] = handler;
            }
        }

        public static void UnsafeRemoveGlobalObserver(System.Type notificationType, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");

            NotificationHandler d;
            if (_unsafeGlobalHandlers.TryGetValue(notificationType, out d))
            {
                d -= handler;
                if (d != null)
                {
                    _unsafeGlobalHandlers[notificationType] = d;
                }
                else
                {
                    _unsafeGlobalHandlers.Remove(notificationType);
                }
            }
        }

        public static bool HasGlobalObserver<T>() where T : Notification
        {
            return HasGlobalObserver(typeof(T));
        }

        public static bool HasGlobalObserver(System.Type notificationType)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");

            var baseType = typeof(Notification);

            while (baseType.IsAssignableFrom(notificationType))
            {
                if (_globalHandlers.ContainsKey(notificationType) || _unsafeGlobalHandlers.ContainsKey(notificationType)) return true;
                notificationType = notificationType.BaseType;
            }

            return false;
        }

        internal static bool PostGlobalNotification(System.Type tp, object sender, Notification n)
        {
            bool bHandled = false;

            var baseType = typeof(Notification);
            while (baseType.IsAssignableFrom(tp))
            {
                if (_globalHandlers.ContainsKey(tp))
                {
                    var d = _globalHandlers[tp];
                    if (d != null)
                    {
                        d.DynamicInvoke(sender, n);
                        bHandled = true;
                    }
                }
                if (_unsafeGlobalHandlers.ContainsKey(tp))
                {
                    var d = _unsafeGlobalHandlers[tp];
                    if(d != null)
                    {
                        d(sender, n);
                        bHandled = true;
                    }
                }

                tp = tp.BaseType;
            }

            return bHandled;
        }

        #endregion

        #region Instance Notification Interface

        public static void RegisterObserver<T>(INotificationDispatcher sender, NotificationHandler<T> handler) where T : Notification
        {
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            sender.Observers.RegisterObserver<T>(handler);
        }

        public static void RegisterObserver<T>(object sender, NotificationHandler<T> handler) where T : Notification
        {
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (sender is INotificationDispatcher)
            {
                (sender as INotificationDispatcher).Observers.RegisterObserver<T>(handler);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                dispatcher.Observers.RegisterObserver<T>(handler);
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static void RemoveObserver<T>(INotificationDispatcher sender, NotificationHandler<T> handler) where T : Notification
        {
            if (object.ReferenceEquals(sender, null)) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            sender.Observers.RemoveObserver<T>(handler);
        }

        public static void RemoveObserver<T>(object sender, NotificationHandler<T> handler) where T : Notification
        {
            if (object.ReferenceEquals(sender, null)) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (sender is INotificationDispatcher)
            {
                (sender as INotificationDispatcher).Observers.RemoveObserver<T>(handler);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                if (dispatcher != null) dispatcher.Observers.RemoveObserver<T>(handler);
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static void UnsafeRegisterObserver(System.Type notificationType, INotificationDispatcher sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            sender.Observers.UnsafeRegisterObserver(notificationType, handler);
        }

        public static void UnsafeRegisterObserver(System.Type notificationType, object sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (sender is INotificationDispatcher)
            {
                (sender as INotificationDispatcher).Observers.UnsafeRegisterObserver(notificationType, handler);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                dispatcher.Observers.UnsafeRegisterObserver(notificationType, handler);
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static void UnsafeRemoveObserver(System.Type notificationType, INotificationDispatcher sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (object.ReferenceEquals(sender, null)) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            sender.Observers.UnsafeRemoveObserver(notificationType, handler);
        }

        public static void UnsafeRemoveObserver(System.Type notificationType, object sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (object.ReferenceEquals(sender, null)) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (sender is INotificationDispatcher)
            {
                (sender as INotificationDispatcher).Observers.UnsafeRemoveObserver(notificationType, handler);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                if (dispatcher != null) dispatcher.Observers.UnsafeRemoveObserver(notificationType, handler);
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static bool HasObserver<T>(INotificationDispatcher sender, bool bNotifyEntity = false) where T : Notification
        {
            if (object.ReferenceEquals(sender, null)) throw new ArgumentNullException("sender");

            return sender.Observers.HasObserver<T>(bNotifyEntity);
        }

        public static bool HasObserver<T>(object sender, bool bNotifyEntity = false) where T : Notification
        {
            if (object.ReferenceEquals(sender, null)) throw new ArgumentNullException("sender");

            if (sender is INotificationDispatcher)
            {
                return (sender as INotificationDispatcher).Observers.HasObserver<T>(bNotifyEntity);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                if (bNotifyEntity)
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher.Observers.HasObserver<T>(bNotifyEntity);
                }
                else
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher != null && dispatcher.Observers.HasObserver<T>(bNotifyEntity);
                }
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static bool HasObserver(System.Type notificationType, INotificationDispatcher sender, bool bNotifyEntity = false)
        {
            if (notificationType == null || !TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (object.ReferenceEquals(sender, null)) throw new ArgumentNullException("sender");

            return sender.Observers.HasObserver(notificationType, bNotifyEntity);
        }

        public static bool HasObserver(System.Type notificationType, object sender, bool bNotifyEntity = false)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (object.ReferenceEquals(sender, null)) throw new ArgumentNullException("sender");

            if (sender is INotificationDispatcher)
            {
                return (sender as INotificationDispatcher).Observers.HasObserver(notificationType, bNotifyEntity);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                if (bNotifyEntity)
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher.Observers.HasObserver(notificationType, bNotifyEntity);
                }
                else
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher != null && dispatcher.Observers.HasObserver(notificationType, bNotifyEntity);
                }
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }









        /// <summary>
        /// Posts a notification of type T. Returns true if an observer was found and received the notification.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        /// <param name="bNotifyEntity"></param>
        /// <returns></returns>
        public static bool PostNotification<T>(INotificationDispatcher sender, T notification, bool bNotifyEntity = false) where T : Notification
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (notification == null) throw new ArgumentNullException("notification");

            return sender.Observers.PostNotification<T>(notification, bNotifyEntity);
        }

        public static bool PostNotification<T>(UnityEngine.GameObject sender, T notification, bool bNotifyEntity = false) where T : Notification
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (notification == null) throw new ArgumentNullException("notification");

            if (bNotifyEntity)
            {
                var dispatcher = sender.AddOrGetComponent<GameObjectNotificationDispatcher>();
                return dispatcher.Observers.PostNotification<T>(notification, bNotifyEntity);
            }
            else
            {
                var dispatcher = sender.GetComponent<GameObjectNotificationDispatcher>();
                return dispatcher != null && dispatcher.Observers.PostNotification<T>(notification, bNotifyEntity);
            }
        }

        public static bool UnsafePostNotification(INotificationDispatcher sender, Notification notification, bool bNotifyEntity = false)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (notification == null) throw new ArgumentNullException("notification");

            return sender.Observers.UnsafePostNotification(notification, bNotifyEntity);
        }

        public static bool UnsafePostNotification(object sender, Notification notification, bool bNotifyEntity = false)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (notification == null) throw new ArgumentNullException("notification");

            if (sender is INotificationDispatcher)
            {
                return (sender as INotificationDispatcher).Observers.UnsafePostNotification(notification, bNotifyEntity);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                if (bNotifyEntity)
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher.Observers.UnsafePostNotification(notification, bNotifyEntity);
                }
                else
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher != null && dispatcher.Observers.UnsafePostNotification(notification, bNotifyEntity);
                }
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }





        /*
        public static bool PostNotification<T>(object sender, T notification, bool bNotifyEntity = false) where T : Notification
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (notification == null) throw new ArgumentNullException("notification");

            if (sender is INotificationDispatcher)
            {
                return (sender as INotificationDispatcher).PostNotification<T>(notification, bNotifyEntity);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                if (bNotifyEntity)
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher.PostNotification<T>(notification, bNotifyEntity);
                }
                else
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher != null && dispatcher.PostNotification<T>(notification, bNotifyEntity);
                }
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }
         */

        #endregion

        #region Purge/Clean Utilities

        public static void PurgeHandlers(UnityEngine.GameObject go)
        {
            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<INotificationDispatcher>())
            {
                go.GetComponents<INotificationDispatcher>(lst);
                var e = lst.GetEnumerator();
                while(e.MoveNext())
                {
                    e.Current.Observers.PurgeHandlers();
                }
            }
        }

        #endregion

        #endregion


        #region Temp Interface

        private static Dictionary<System.Type, TempState> _tempNotifTable;

        /// <summary>
        /// Attempts to get a cached version of the notification, or creates a new one. 
        /// Use this in tandem with Release to create temp notifications with low GC footprint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static bool TryGetCache<T>(out T obj) where T : Notification
        {
            if (_tempNotifTable == null)
                _tempNotifTable = new Dictionary<Type, TempState>();

            var tp = typeof(T);
            TempState state;
            if (_tempNotifTable.TryGetValue(tp, out state))
            {
                if(state != null && state.Notif != null)
                {
                    obj = state.Notif as T;
                    state.Notif = null;
                    return true;
                }
                else
                {
                    obj = null;
                    return false;
                }
            }
            else
            {
                _tempNotifTable[tp] = null; //add the key with no state, to represent this is the first potential of caching
                obj = null;
                return false;
            }
        }

        /// <summary>
        /// Release a temporary notification so it can be recycled for later use. The notification should have been created using 
        /// a static Create method on the notification type itself.
        /// </summary>
        /// <param name="notif"></param>
        public static void Release(Notification notif)
        {
            if (notif == null) throw new System.ArgumentNullException("notif");
            if (_tempNotifTable == null) return;

            var tp = notif.GetType();
            TempState state;
            if (!_tempNotifTable.TryGetValue(tp, out state))
                return; //if the key wasn't in there, then this notif type shouldn't be cached

            if(state != null)
            {
                if (state.Notif != null) return;

                state.Notif = notif;
            }
            else
            {
                state = new TempState();
                state.Notif = notif;
                state.Fields = tp.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _tempNotifTable[tp] = state;
            }

            foreach(var f in state.Fields)
            {
                try
                {
                    f.SetValue(notif, null);
                }
                catch { }
            }
        }
        
        private class TempState
        {
            public Notification Notif;
            public FieldInfo[] Fields;
        }

        #endregion

    }
    
}
