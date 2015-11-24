using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public interface INotificationDispatcher
    {

        NotificationDispatcher Observers { get; }

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

        #region Methods

        public void RegisterObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (handler == null) throw new System.ArgumentNullException("handler");
            var tp = typeof(T);
            System.Delegate d;
            if (_handlers.TryGetValue(tp, out d))
            {
                _handlers[tp] = System.Delegate.Combine(d, handler);
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
            System.Delegate d;
            if (_handlers.TryGetValue(tp, out d))
            {
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
            if (notificationType == null || !TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");

            NotificationHandler d;
            if (_unsafeHandlers.TryGetValue(notificationType, out d))
            {
                _unsafeHandlers[notificationType] = d + handler;
            }
            else
            {
                _unsafeHandlers[notificationType] = handler;
            }
        }

        public void UnsafeRemoveObserver(System.Type notificationType, NotificationHandler handler)
        {
            if (notificationType == null || !TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");

            NotificationHandler d;
            if (_unsafeHandlers.TryGetValue(notificationType, out d))
            {
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
            if (notificationType == null || !TypeUtil.IsType(notificationType, typeof(Notification))) throw new TypeArgumentMismatchException(notificationType, typeof(Notification), "notificationType");

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
                if (_ownerGameObject.Observers.HasObserver(tp, false)) return true;
            }

            if (bNotifyEntity)
            {
                //TODO - a bottleneck here, consider possibilities of speeding this up by removing the need to constantly call 'FindRoot'
                var root = _ownerGameObject.FindRoot();
                GameObjectNotificationDispatcher dispatcher;
                if (root != _ownerGameObject.gameObject && root.GetComponent<GameObjectNotificationDispatcher>(out dispatcher))
                {
                    if (dispatcher.Observers.HasObserver_Imp(tp, false)) return true;
                }
            }

            //return false;
            return Notification.HasGlobalObserver(tp);
        }

        private bool PostNotification_Imp(System.Type tp, Notification n, bool bNotifyEntity)
        {
            bool handled = false;

            handled = this.PostNotificationToJustSelf(tp, _owner, n);

            if (_ownerGameObject != null)
            {
                if (!Object.ReferenceEquals(_owner, _ownerGameObject))
                {
                    if (_ownerGameObject.Observers.PostNotificationToJustSelf(tp, _owner, n)) handled = true;
                }

                if (bNotifyEntity)
                {
                    //TODO - a bottleneck here, consider possibilities of speeding this up by removing the need to constantly call 'FindRoot'
                    var root = _ownerGameObject.FindRoot();
                    GameObjectNotificationDispatcher dispatcher;
                    if (root != _ownerGameObject.gameObject && root.GetComponent<GameObjectNotificationDispatcher>(out dispatcher))
                    {
                        if (dispatcher.Observers.PostNotificationToJustSelf(tp, _owner, n)) handled = true;
                    }
                    //root.BroadcastMessage(SPConstants.MSG_AUTONOTIFICATIONMESSAGEHANDLER, n, UnityEngine.SendMessageOptions.DontRequireReceiver);
                }
                //else
                //{
                //    _ownerGameObject.gameObject.SendMessage(SPConstants.MSG_AUTONOTIFICATIONMESSAGEHANDLER, n, UnityEngine.SendMessageOptions.DontRequireReceiver);
                //}
            }

            //let anyone registered with the global hear about it
            var globallyHandled = Notification.PostGlobalNotification(tp, _owner, n);

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
                            if (_ownerGameObject != null)
                            {
                                var root = _ownerGameObject.FindRoot();
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

        
        private bool PostNotificationToJustSelf(System.Type tp, object sender, Notification n)
        {
            bool bHandled = false;

            var baseType = typeof(Notification);
            System.Delegate d;
            NotificationHandler ud;
            while (baseType.IsAssignableFrom(tp))
            {
                if (_handlers.TryGetValue(tp, out d))
                {
                    if (d != null)
                    {
                        d.DynamicInvoke(sender, n);
                        bHandled = true;
                    }
                }
                if (_unsafeHandlers.TryGetValue(tp, out ud))
                {
                    if (ud != null)
                    {
                        //ud.DynamicInvoke(sender, n);
                        ud(sender, n);
                        bHandled = true;
                    }
                }

                tp = tp.BaseType;
            }

            return bHandled;
        }

        /// <summary>
        /// Limited posting of a notification. Only handlers registered directly with this NotificationDispatcher will be notified. 
        /// Not even the global notification dispatcher will be signaled. This is for use if you want some special object to forward 
        /// a notification along as if it were in its hierarchy chain.
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool ForwardNotificationToJustSelf(object sender, Notification n)
        {
            if (object.ReferenceEquals(sender, null)) throw new System.ArgumentNullException("sender");
            if (n == null) throw new System.ArgumentNullException("n");

            return PostNotificationToJustSelf(n.GetType(), sender, n);
        }

        #endregion

        #region INotificationDispatcher Interface

        NotificationDispatcher INotificationDispatcher.Observers
        {
            get { return this; }
        }

        #endregion

    }

    [DisallowMultipleComponent()]
    public sealed class GameObjectNotificationDispatcher : MonoBehaviour, INotificationDispatcher
    {

        #region Fields

        private NotificationDispatcher _observers = new NotificationDispatcher();

        #endregion

        #region CONSTRUCTOR

        private void Awake()
        {
            _observers.SetOwner(this);
        }

        private void OnDestroy()
        {
            _observers.PurgeHandlers();
        }

        private void OnDespawn()
        {
            _observers.PurgeHandlers();
        }

        #endregion

        public System.Type[] ListObserveredNotifications()
        {
            return _observers.ListObservedNotifications();
        }

        public NotificationDispatcher Observers
        {
            get { return _observers; }
        }

    }

}
