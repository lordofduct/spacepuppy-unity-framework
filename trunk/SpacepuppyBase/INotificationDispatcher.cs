using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public interface INotificationDispatcher
    {

        void RegisterObserver<T>(NotificationHandler<T> handler) where T : Notification;

        void RemoveObserver<T>(NotificationHandler<T> handler) where T : Notification;

        bool HasObserver<T>(bool bNotifyEntity) where T : Notification;

        bool PostNotification<T>(T n, bool bNotifyEntity) where T : Notification;


        void UnsafeRegisterObserver(System.Type tp, NotificationHandler handler);
        void UnsafeRemoveObserver(System.Type tp, NotificationHandler handler);
        bool HasObserver(System.Type tp, bool bNotifyEntity);
        bool UnsafePostNotification(Notification n, bool bNotifyEntity);

        void PurgeHandlers();
    }

    public class NotificationDispatcher : INotificationDispatcher
    {

        #region Fields

        private INotificationDispatcher _owner;
        private Dictionary<System.Type, System.Delegate> _handlers = new Dictionary<System.Type, System.Delegate>();
        private Dictionary<System.Type, NotificationHandler> _unsafeHandlers = new Dictionary<System.Type, NotificationHandler>();

        private GameObjectNotificationDispatcher _ownerGameObject;

        #endregion

        #region CONSTRUCTOR

        public NotificationDispatcher()
        {
            _owner = null;
            _ownerGameObject = null;
        }

        public NotificationDispatcher(INotificationDispatcher owner)
        {
            this.SetOwner(owner);
        }

        internal void SetOwner(INotificationDispatcher owner)
        {
            _owner = owner;
            if (GameObjectUtil.IsGameObjectSource(_owner))
            {
                _ownerGameObject = GameObjectUtil.GetGameObjectFromSource(_owner).AddOrGetComponent<GameObjectNotificationDispatcher>();
            }
            else
            {
                _ownerGameObject = null;
            }
        }

        #endregion

        public System.Type[] ListObservedNotifications()
        {
            return _handlers.Keys.Union(_unsafeHandlers.Keys).ToArray();
        }

        #region INotificationDispatcher Interface

        public void RegisterObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (handler == null) throw new System.ArgumentNullException("handler");
            var tp = typeof(T);
            if (_handlers.ContainsKey(tp))
            {
                _handlers[tp] = System.Delegate.Combine(_handlers[tp], handler);
            }
            else
            {
                _handlers[tp] = handler;
            }
        }

        public void RemoveObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (handler == null) throw new System.ArgumentNullException("handler");
            var tp = typeof(T);
            if (_handlers.ContainsKey(tp))
            {
                var d = _handlers[tp];
                d = System.Delegate.Remove(d, handler);
                if (d != null)
                {
                    _handlers[tp] = d;
                }
                else
                {
                    _handlers.Remove(tp);
                }
            }
        }

        public bool HasObserver<T>(bool bNotifyEntity) where T : Notification
        {
            return this.HasObserver_Imp(typeof(T), bNotifyEntity);
        }

        public bool PostNotification<T>(T n, bool bNotifyEntity) where T : Notification
        {
            if (n == null) throw new System.ArgumentNullException("n");

            return PostNotification_Imp(typeof(T), n, bNotifyEntity);
        }

        public void UnsafeRegisterObserver(System.Type notificationType, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (_unsafeHandlers.ContainsKey(notificationType))
            {
                _unsafeHandlers[notificationType] += handler;
            }
            else
            {
                _unsafeHandlers[notificationType] = handler;
            }
        }

        public void UnsafeRemoveObserver(System.Type notificationType, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");

            if (_handlers.ContainsKey(notificationType))
            {
                var d = _unsafeHandlers[notificationType];
                d -= handler;
                if (d != null)
                {
                    _unsafeHandlers[notificationType] = d;
                }
                else
                {
                    _unsafeHandlers.Remove(notificationType);
                }
            }
        }

        public bool HasObserver(System.Type notificationType, bool bNotifyEntity)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");

            return this.HasObserver_Imp(notificationType, bNotifyEntity);
        }

        public bool UnsafePostNotification(Notification n, bool bNotifyEntity)
        {
            if (n == null) throw new System.ArgumentNullException("n");

            return PostNotification_Imp(n.GetType(), n, bNotifyEntity);
        }

        public void PurgeHandlers()
        {
            _handlers.Clear();
            _unsafeHandlers.Clear();
        }

        #endregion

        #region Imp

        private bool HasObserver_Imp(System.Type tp, bool bNotifyEntity)
        {
            var tpForLoop = tp;
            var baseType = typeof(Notification);
            while (baseType.IsAssignableFrom(tpForLoop))
            {
                if (_handlers.ContainsKey(tpForLoop) || _unsafeHandlers.ContainsKey(tpForLoop)) return true;
                tpForLoop = tpForLoop.BaseType;
            }
            if (_ownerGameObject == null) return false;

            if (!object.ReferenceEquals(_ownerGameObject, _owner))
            {
                if (_ownerGameObject.HasObserver(tp, false)) return true;
            }

            if (bNotifyEntity)
            {
                //TODO - a bottleneck here, consider possibilities of speeding this up by removing the need to constantly call 'FindRoot'
                var root = _ownerGameObject.FindRoot();
                GameObjectNotificationDispatcher dispatcher;
                if (root != _ownerGameObject.gameObject && root.GetComponent<GameObjectNotificationDispatcher>(out dispatcher))
                {
                    if (dispatcher._dispatcher.HasObserver_Imp(tp, false)) return true;
                }
            }

            //return false;
            return Notification.HasGlobalObserver(tp);
        }

        private bool PostNotification_Imp(System.Type tp, Notification n, bool bNotifyEntity)
        {
            n.SetSender(_owner ?? this);
            bool handled = false;

            handled = this.PostNotification_JustSelf(tp, n);

            if (_ownerGameObject != null)
            {
                if (!Object.ReferenceEquals(_owner, _ownerGameObject))
                {
                    if (_ownerGameObject._dispatcher.PostNotification_JustSelf(tp, n)) handled = true;
                }

                if (bNotifyEntity)
                {
                    //TODO - a bottleneck here, consider possibilities of speeding this up by removing the need to constantly call 'FindRoot'
                    var root = _ownerGameObject.FindRoot();
                    GameObjectNotificationDispatcher dispatcher;
                    if (root != _ownerGameObject.gameObject && root.GetComponent<GameObjectNotificationDispatcher>(out dispatcher))
                    {
                        if (dispatcher._dispatcher.PostNotification_JustSelf(tp, n)) handled = true;
                    }
                    //root.BroadcastMessage(SPConstants.MSG_AUTONOTIFICATIONMESSAGEHANDLER, n, UnityEngine.SendMessageOptions.DontRequireReceiver);
                }
                //else
                //{
                //    _ownerGameObject.gameObject.SendMessage(SPConstants.MSG_AUTONOTIFICATIONMESSAGEHANDLER, n, UnityEngine.SendMessageOptions.DontRequireReceiver);
                //}
            }

            //let anyone registered with the global hear about it
            var globallyHandled = Notification.PostGlobalNotification(tp, n);

            //if not handled, check if we should throw an exception
            if (!(handled || globallyHandled) && System.Attribute.IsDefined(tp, typeof(RequireNotificationReceiverAttribute), false))
            {
                var requiredAttrib = tp.GetCustomAttributes(typeof(RequireNotificationReceiverAttribute), false).FirstOrDefault() as RequireNotificationReceiverAttribute;
                if (requiredAttrib != null)
                {
                    if (!handled)
                    {
                        if (!requiredAttrib.GlobalObserverConsidered || !globallyHandled)
                        {
                            if (n.GameObject != null)
                            {
                                var root = n.GameObject.FindRoot();
                                if (_owner is GameObjectNotificationDispatcher)
                                {
                                    throw new AbsentNotificationReceiverException(n, "A GameObject named '" + root.name + "' requires a receiver to notification of type '" + tp.Name + "'.");
                                }
                                else
                                {
                                    var senderTypeName = (_owner != null) ? _owner.GetType().Name : this.GetType().Name;
                                    throw new AbsentNotificationReceiverException(n, "An object of type '" + senderTypeName + "' on GameObject named '" + root.name + "' requires a receiver to notification of type '" + tp.Name + "'.");
                                }
                            }
                            else
                            {
                                var senderTypeName = (_owner != null) ? _owner.GetType().Name : this.GetType().Name;
                                throw new AbsentNotificationReceiverException(n, "An object of type '" + senderTypeName + "' requires a receiver to notification of type '" + tp.Name + "'.");
                            }
                        }
                    }
                }

                return false;
            }
            {
                return true;
            }
        }

        private bool PostNotification_JustSelf(System.Type tp, Notification n)
        {
            bool bHandled = false;

            var baseType = typeof(Notification);
            while (baseType.IsAssignableFrom(tp))
            {
                if (_handlers.ContainsKey(tp))
                {
                    var d = _handlers[tp];
                    if (d != null)
                    {
                        d.DynamicInvoke(n);
                        bHandled = true;
                    }
                }
                if (_unsafeHandlers.ContainsKey(tp))
                {
                    var d = _unsafeHandlers[tp];
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

    }

    public sealed class GameObjectNotificationDispatcher : MonoBehaviour, INotificationDispatcher
    {

        #region Fields

        internal NotificationDispatcher _dispatcher = new NotificationDispatcher();

        #endregion

        #region CONSTRUCTOR

        private void Awake()
        {
            _dispatcher.SetOwner(this);
        }

        private void OnDestroy()
        {
            _dispatcher.PurgeHandlers();
        }

        private void OnDespawn()
        {
            _dispatcher.PurgeHandlers();
        }

        #endregion

        public System.Type[] ListObserveredNotifications()
        {
            return _dispatcher.ListObservedNotifications();
        }

        #region INotificationDispatcher Interface

        public void RegisterObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            _dispatcher.RegisterObserver<T>(handler);
        }

        public void RemoveObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            _dispatcher.RemoveObserver<T>(handler);
        }

        public bool HasObserver<T>(bool bNotifyEntity) where T : Notification
        {
            return _dispatcher.HasObserver<T>(bNotifyEntity);
        }

        public bool PostNotification<T>(T n, bool bNotifyEntity) where T : Notification
        {
            return _dispatcher.PostNotification<T>(n, bNotifyEntity);
        }

        public void UnsafeRegisterObserver(System.Type tp, NotificationHandler handler)
        {
            _dispatcher.UnsafeRegisterObserver(tp, handler);
        }

        public void UnsafeRemoveObserver(System.Type tp, NotificationHandler handler)
        {
            _dispatcher.UnsafeRemoveObserver(tp, handler);
        }

        public bool HasObserver(System.Type tp, bool bNotifyEntity)
        {
            return _dispatcher.HasObserver(tp, bNotifyEntity);
        }

        public bool UnsafePostNotification(Notification n, bool bNotifyEntity)
        {
            return _dispatcher.UnsafePostNotification(n, bNotifyEntity);
        }

        public void PurgeHandlers()
        {
            _dispatcher.PurgeHandlers();
        }

        #endregion

    }

}
