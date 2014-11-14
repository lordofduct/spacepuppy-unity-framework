using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
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
    public abstract class Notification
    {

        #region Fields

        private object _sender;
        private UnityEngine.GameObject _go;

        #endregion

        #region CONSTRUCTOR

        public Notification()
        {

        }

        #endregion

        #region Properties

        public object Sender { get { return _sender; } }
        public UnityEngine.GameObject GameObject { get { return _go; } }

        internal bool AutoReceived { get; set; }

        #endregion

        #region Methods

        internal void SetSender(object sender)
        {
            _sender = sender;
            _go = com.spacepuppy.Utils.GameObjectUtil.GetGameObjectFromSource(sender);
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
            if (_globalHandlers.ContainsKey(tp))
            {
                _globalHandlers[tp] = System.Delegate.Combine(_globalHandlers[tp], handler);
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
            if (_globalHandlers.ContainsKey(tp))
            {
                var d = System.Delegate.Remove(_globalHandlers[tp], handler);
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
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (_unsafeGlobalHandlers.ContainsKey(notificationType))
            {
                _unsafeGlobalHandlers[notificationType] += handler;
            }
            else
            {
                _unsafeGlobalHandlers[notificationType] = handler;
            }
        }

        public static void UnsafeRemoveGlobalObserver(System.Type notificationType, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (_unsafeGlobalHandlers.ContainsKey(notificationType))
            {
                var d = _unsafeGlobalHandlers[notificationType];
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
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");

            var baseType = typeof(Notification);

            while (baseType.IsAssignableFrom(notificationType))
            {
                if (_globalHandlers.ContainsKey(notificationType) || _unsafeGlobalHandlers.ContainsKey(notificationType)) return true;
                notificationType = notificationType.BaseType;
            }

            return false;
        }

        internal static bool PostGlobalNotification(System.Type tp, Notification n)
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
                        d.DynamicInvoke(n);
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

            sender.RegisterObserver<T>(handler);
        }

        public static void RegisterObserver<T>(object sender, NotificationHandler<T> handler) where T : Notification
        {
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (sender is INotificationDispatcher)
            {
                (sender as INotificationDispatcher).RegisterObserver<T>(handler);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                dispatcher.RegisterObserver<T>(handler);
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static void RemoveObserver<T>(INotificationDispatcher sender, NotificationHandler<T> handler) where T : Notification
        {
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            sender.RemoveObserver<T>(handler);
        }

        public static void RemoveObserver<T>(object sender, NotificationHandler<T> handler) where T : Notification
        {
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (sender is INotificationDispatcher)
            {
                (sender as INotificationDispatcher).RemoveObserver<T>(handler);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                if (dispatcher != null) dispatcher.RemoveObserver<T>(handler);
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static void UnsafeRegisterObserver(System.Type notificationType, INotificationDispatcher sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            sender.UnsafeRegisterObserver(notificationType, handler);
        }

        public static void UnsafeRegisterObserver(System.Type notificationType, object sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (sender is INotificationDispatcher)
            {
                (sender as INotificationDispatcher).UnsafeRegisterObserver(notificationType, handler);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                dispatcher.UnsafeRegisterObserver(notificationType, handler);
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static void UnsafeRemoveObserver(System.Type notificationType, INotificationDispatcher sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            sender.UnsafeRemoveObserver(notificationType, handler);
        }

        public static void UnsafeRemoveObserver(System.Type notificationType, object sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (sender is INotificationDispatcher)
            {
                (sender as INotificationDispatcher).UnsafeRemoveObserver(notificationType, handler);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                if (dispatcher != null) dispatcher.UnsafeRemoveObserver(notificationType, handler);
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static bool HasObserver<T>(INotificationDispatcher sender, bool bNotifyEntity = false) where T : Notification
        {
            if (sender == null) throw new ArgumentNullException("sender");

            return sender.HasObserver<T>(bNotifyEntity);
        }

        public static bool HasObserver<T>(object sender, bool bNotifyEntity = false) where T : Notification
        {
            if (sender == null) throw new ArgumentNullException("sender");

            if (sender is INotificationDispatcher)
            {
                return (sender as INotificationDispatcher).HasObserver<T>(bNotifyEntity);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                if (bNotifyEntity)
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher.HasObserver<T>(bNotifyEntity);
                }
                else
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher != null && dispatcher.HasObserver<T>(bNotifyEntity);
                }
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        public static bool HasObserver(System.Type notificationType, INotificationDispatcher sender, bool bNotifyEntity = false)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (sender == null) throw new ArgumentNullException("sender");

            return sender.HasObserver(notificationType, bNotifyEntity);
        }

        public static bool HasObserver(System.Type notificationType, object sender, bool bNotifyEntity = false)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (sender == null) throw new ArgumentNullException("sender");

            if (sender is INotificationDispatcher)
            {
                return (sender as INotificationDispatcher).HasObserver(notificationType, bNotifyEntity);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                if (bNotifyEntity)
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher.HasObserver(notificationType, bNotifyEntity);
                }
                else
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher != null && dispatcher.HasObserver(notificationType, bNotifyEntity);
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

            return sender.PostNotification<T>(notification, bNotifyEntity);
        }

        public static bool PostNotification<T>(UnityEngine.GameObject sender, T notification, bool bNotifyEntity = false) where T : Notification
        {
            return false;
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

        public static bool UnsafePostNotification(INotificationDispatcher sender, Notification notification, bool bNotifyEntity = false)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (notification == null) throw new ArgumentNullException("notification");

            return sender.PostNotification(notification, bNotifyEntity);
        }

        public static bool UnsafePostNotification(object sender, Notification notification, bool bNotifyEntity = false)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (notification == null) throw new ArgumentNullException("notification");

            if (sender is INotificationDispatcher)
            {
                return (sender as INotificationDispatcher).UnsafePostNotification(notification, bNotifyEntity);
            }
            else if (GameObjectUtil.IsGameObjectSource(sender))
            {
                if (bNotifyEntity)
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).AddOrGetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher.UnsafePostNotification(notification, bNotifyEntity);
                }
                else
                {
                    var dispatcher = GameObjectUtil.GetGameObjectFromSource(sender).GetComponent<GameObjectNotificationDispatcher>();
                    return dispatcher != null && dispatcher.UnsafePostNotification(notification, bNotifyEntity);
                }
            }
            else
            {
                throw new System.ArgumentException("Sender is not a NotificationDispatcher.", "sender");
            }
        }

        #endregion

        #region Purge/Clean Utilities

        public static void PurgeHandlers(UnityEngine.GameObject go)
        {
            foreach (var c in go.GetLikeComponents<INotificationDispatcher>())
            {
                c.PurgeHandlers();
            }
        }

        #endregion

        #endregion

    }
}
