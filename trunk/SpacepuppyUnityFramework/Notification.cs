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

        private void SetSender(object sender)
        {
            _sender = sender;
            _go = com.spacepuppy.Utils.GameObjectUtil.GetGameObjectFromSource(sender);
        }

        #endregion

        #region Static Interface

        private static WeakKeyDictionary<object, NotificationHandlerCollection> _senderSpecificNotificationHandlers = new WeakKeyDictionary<object, NotificationHandlerCollection>();
        private static NotificationHandlerCollection _globalNotificationHandlers = new NotificationHandlerCollection();
        private static com.spacepuppy.Timers.Timer _timer;

        static Notification()
        {
            _timer = new com.spacepuppy.Timers.Timer(10.0f, 0);
            _timer.TimerCount += delegate(com.spacepuppy.Timers.Timer t)
            {
                Notification.CleanDestroyedObjects();
            };
            com.spacepuppy.Timers.SystemTimers.Start(_timer);
        }

        #region Weak Reference Clean Interface

        /// <summary>
        /// The interval at which the Notification weak list cleans itself.
        /// </summary>
        public static float AutoCleanInterval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        /// <summary>
        /// Clears out any notification handlers for objects that have been destroyed.
        /// </summary>
        public static void CleanDestroyedObjects()
        {
            var toRemove = (from o in _senderSpecificNotificationHandlers.Keys where o is UnityEngine.Object && o == null select o).ToArray();
            foreach (var o in toRemove)
            {
                _senderSpecificNotificationHandlers.Remove(o);
            }
        }

        public static void PurgeNotificationsFor(object obj)
        {
            if (_senderSpecificNotificationHandlers.ContainsKey(obj)) _senderSpecificNotificationHandlers.Remove(obj);
        }

        #endregion

        #region Register/Post Interface

        public static void RegisterGlobalObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (handler == null) throw new System.ArgumentNullException("handler");
            _globalNotificationHandlers.RegisterObserver<T>(handler);
        }

        public static void RemoveGlobalObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (handler == null) throw new System.ArgumentNullException("handler");

            _globalNotificationHandlers.RemoveObserver<T>(handler);
        }

        public static void RegisterObserver<T>(object sender, NotificationHandler<T> handler) where T : Notification
        {
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");
            _senderSpecificNotificationHandlers.Clean();

            NotificationHandlerCollection coll;
            if (!_senderSpecificNotificationHandlers.ContainsKey(sender))
            {
                coll = new NotificationHandlerCollection();
                _senderSpecificNotificationHandlers[sender] = coll;
            }
            else
            {
                coll = _senderSpecificNotificationHandlers[sender];
            }
            coll.RegisterObserver<T>(handler);
        }

        public static void RemoveObserver<T>(object sender, NotificationHandler<T> handler) where T : Notification
        {
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");
            _senderSpecificNotificationHandlers.Clean();

            if (!_senderSpecificNotificationHandlers.ContainsKey(sender)) return;

            var coll = _senderSpecificNotificationHandlers[sender];
            coll.RemoveObserver<T>(handler);
            if (coll.IsEmpty)
            {
                _senderSpecificNotificationHandlers.Remove(sender);
            }
        }

        /// <summary>
        /// Posts a notification of type T. Returns true if an observer was found and received the notification.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        /// <param name="bNotifyRoot"></param>
        /// <returns></returns>
        public static bool PostNotification<T>(object sender, T notification, bool bNotifyRoot = false) where T : Notification
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (notification == null) throw new ArgumentNullException("notification");
            notification.SetSender(sender);

            NotificationHandlerCollection coll;

            bool handled = false;
            ////we first notify those registered directly with the sender
            if (_senderSpecificNotificationHandlers.TryGetValue(sender, out coll))
            {
                if (coll.PostNotification<T>(notification)) handled = true;
            }

            //if the sender was a gameObject source, let anyone registered with the gameObject hear about it
            var go = notification.GameObject;
            if (go != null)
            {
                if (go != sender)
                {
                    if (_senderSpecificNotificationHandlers.TryGetValue(go, out coll))
                    {
                        if (coll.PostNotification<T>(notification)) handled = true;
                    }
                }

                if (bNotifyRoot)
                {
                    var root = com.spacepuppy.Utils.GameObjectUtil.FindRoot(go);
                    if (root != go)
                    {
                        if (_senderSpecificNotificationHandlers.TryGetValue(root, out coll))
                        {
                            if (coll.PostNotification<T>(notification)) handled = true;
                        }
                    }
                    //AUTO NOTIFICATION
                    root.BroadcastMessage(SPConstants.MSG_AUTONOTIFICATIONMESSAGEHANDLER, notification, UnityEngine.SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    //AUTO NOTIFICATION
                    go.SendMessage(SPConstants.MSG_AUTONOTIFICATIONMESSAGEHANDLER, notification, UnityEngine.SendMessageOptions.DontRequireReceiver);
                }
                if (notification.AutoReceived) handled = true;
            }

            //finally let anyone registered with the global hear about it
            bool globallyHandled = _globalNotificationHandlers.PostNotification<T>(notification);

            //if not handled, check if we should throw an exception
            if (!(handled || globallyHandled) && Attribute.IsDefined(typeof(T), typeof(RequireReceiverAttribute), false))
            {
                var requiredAttrib = typeof(T).GetCustomAttributes(typeof(RequireReceiverAttribute), false).FirstOrDefault() as RequireReceiverAttribute;
                if (requiredAttrib != null)
                {
                    if (!handled)
                    {
                        if (!requiredAttrib.GlobalObserverConsidered || !globallyHandled)
                        {
                            if (notification.GameObject != null)
                            {
                                var root = notification.GameObject.FindRoot();
                                if (sender == go)
                                {
                                    throw new AbsentNotificationReceiverException(notification, "A GameObject named '" + root.name + "' requires a receiver to notification of type '" + typeof(T).Name + "'.");
                                }
                                else
                                {
                                    throw new AbsentNotificationReceiverException(notification, "An object of type '" + sender.GetType().Name + "' on GameObject named '" + root.name + "' requires a receiver to notification of type '" + typeof(T).Name + "'.");
                                }
                            }
                            else
                            {
                                throw new AbsentNotificationReceiverException(notification, "An object of type '" + sender.GetType().Name + "' requires a receiver to notification of type '" + typeof(T).Name + "'.");
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

        #endregion

        #region Unsafe Register Interface

        public static void UnsafeRegisterGlobalObserver(System.Type notificationType, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");
            _globalNotificationHandlers.UnsafeRegisterObserver(notificationType, handler);
        }

        public static void UnsafeRemoveGlobalObserver(System.Type notificationType, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (handler == null) throw new System.ArgumentNullException("handler");

            _globalNotificationHandlers.UnsafeRemoveObserver(notificationType, handler);
        }

        public static void UnsafeRegisterObserver(System.Type notificationType, object sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");
            _senderSpecificNotificationHandlers.Clean();

            NotificationHandlerCollection coll;
            if (!_senderSpecificNotificationHandlers.ContainsKey(sender))
            {
                coll = new NotificationHandlerCollection();
                _senderSpecificNotificationHandlers[sender] = coll;
            }
            else
            {
                coll = _senderSpecificNotificationHandlers[sender];
            }
            coll.UnsafeRegisterObserver(notificationType, handler);
        }

        public static void UnsafeRemoveObserver(System.Type notificationType, object sender, NotificationHandler handler)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (sender == null) throw new System.ArgumentNullException("sender");
            if (handler == null) throw new System.ArgumentNullException("handler");
            _senderSpecificNotificationHandlers.Clean();

            if (!_senderSpecificNotificationHandlers.ContainsKey(sender)) return;

            var coll = _senderSpecificNotificationHandlers[sender];
            coll.UnsafeRemoveObserver(notificationType, handler);
            if (coll.IsEmpty)
            {
                _senderSpecificNotificationHandlers.Remove(sender);
            }
        }

        #endregion

        #region Has/Contains

        public static bool HasObserver<T>(object sender, bool bNotifyRoot = false) where T : Notification
        {
            if (sender == null) throw new ArgumentNullException("sender");

            if (_senderSpecificNotificationHandlers.ContainsKey(sender) && _senderSpecificNotificationHandlers[sender].HasObserverOf<T>()) return true;

            var go = GameObjectUtil.GetGameObjectFromSource(sender);
            if (go != null)
            {
                if (go != sender)
                {
                    if (_senderSpecificNotificationHandlers.ContainsKey(go) && _senderSpecificNotificationHandlers[go].HasObserverOf<T>()) return true;
                }

                if (bNotifyRoot)
                {
                    //if not bubbles, only test the root
                    var root = com.spacepuppy.Utils.GameObjectUtil.FindRoot(go);
                    if (root != null && root != go)
                    {
                        if (_senderSpecificNotificationHandlers.ContainsKey(root) && _senderSpecificNotificationHandlers[root].HasObserverOf<T>()) return true;
                    }
                }
            }

            return _globalNotificationHandlers.HasObserverOf<T>();
        }

        public static bool HasGlobalObserver<T>() where T : Notification
        {
            return _globalNotificationHandlers.HasObserverOf<T>();
        }

        public static bool HasObserver(System.Type notificationType, object sender, bool bNotifyRoot = false)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");
            if (sender == null) throw new ArgumentNullException("sender");

            if (_senderSpecificNotificationHandlers.ContainsKey(sender) && _senderSpecificNotificationHandlers[sender].HasObserverOf(notificationType)) return true;

            var go = GameObjectUtil.GetGameObjectFromSource(sender);
            if (go != null)
            {
                if (go != sender)
                {
                    if (_senderSpecificNotificationHandlers.ContainsKey(go) && _senderSpecificNotificationHandlers[go].HasObserverOf(notificationType)) return true;
                }

                if (bNotifyRoot)
                {
                    //if not bubbles, only test the root
                    var root = com.spacepuppy.Utils.GameObjectUtil.FindRoot(go);
                    if (root != null && root != go)
                    {
                        if (_senderSpecificNotificationHandlers.ContainsKey(root) && _senderSpecificNotificationHandlers[root].HasObserverOf(notificationType)) return true;
                    }
                }
            }

            return _globalNotificationHandlers.HasObserverOf(notificationType);
        }

        public static bool HasGlobalObserver(System.Type notificationType)
        {
            if (notificationType == null) throw new System.ArgumentNullException("notificationType");
            if (!ObjUtil.IsType(notificationType, typeof(Notification))) throw new System.ArgumentException("Notification Type must be a type that inherits from Notification.", "notificationType");

            return _globalNotificationHandlers.HasObserverOf(notificationType);
        }

        #endregion

        #endregion

        #region Special Types

        public delegate void NotificationHandler(Notification n);
        public delegate void NotificationHandler<T>(T notification) where T : Notification;

        [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public class RequireReceiverAttribute : System.Attribute
        {

            public bool GlobalObserverConsidered;

        }

        public class AbsentNotificationReceiverException : System.Exception
        {

            private Notification _n;

            public AbsentNotificationReceiverException(Notification n, string msg)
                : base()
            {
                _n = n;
            }

            public Notification Notification { get { return _n; } }

        }

        private class NotificationHandlerCollection
        {

            #region Fields

            private Dictionary<Type, Delegate> _table = new Dictionary<Type, Delegate>();
            private Dictionary<Type, NotificationHandler> _unsafeTable = new Dictionary<Type, NotificationHandler>();

            #endregion

            #region CONSTRUCTOR

            public NotificationHandlerCollection()
            {

            }

            #endregion

            #region Properties

            public bool IsEmpty { get { return _table.Count == 0 && _unsafeTable.Count == 0; } }

            #endregion

            #region Methods

            #region Safe

            public void RegisterObserver<T>(NotificationHandler<T> handler) where T : Notification
            {
                var tp = typeof(T);
                System.Delegate d = (_table.ContainsKey(tp)) ? _table[tp] : null;
                //this would only fail if someone modified this code to allow adding mismatched delegates with notification types
                d = Delegate.Combine(d, handler);
                _table[tp] = d;
            }

            public void RemoveObserver<T>(NotificationHandler<T> handler) where T : Notification
            {
                var notificationType = typeof(T);
                if (_table.ContainsKey(notificationType))
                {
                    var d = _table[notificationType];
                    if (d != null)
                    {
                        d = Delegate.Remove(d, handler);
                        if (d != null)
                        {
                            _table[notificationType] = d;
                        }
                        else
                        {
                            _table.Remove(notificationType);
                        }
                    }
                }
            }

            #endregion

            #region Unsafe

            public void UnsafeRegisterObserver(System.Type notificationType, NotificationHandler handler)
            {
                NotificationHandler d = (_unsafeTable.ContainsKey(notificationType)) ? _unsafeTable[notificationType] : null;
                d += handler;
                _unsafeTable[notificationType] = d;
            }

            public void UnsafeRemoveObserver(System.Type notificationType, NotificationHandler handler)
            {
                if (_unsafeTable.ContainsKey(notificationType))
                {
                    var d = _unsafeTable[notificationType];
                    if (d != null)
                    {
                        d -= handler;
                        if (d != null)
                        {
                            _unsafeTable[notificationType] = d;
                        }
                        else
                        {
                            _unsafeTable.Remove(notificationType);
                        }
                    }
                }
            }

            #endregion

            #region Post

            public bool PostNotification<T>(T notification) where T : Notification
            {
                bool bSuccess = false;

                //post safe receivers
                var notificationType = typeof(T);
                var baseType = typeof(Notification);
                while (baseType.IsAssignableFrom(notificationType))
                {
                    Delegate d = null;
                    if (_table.TryGetValue(notificationType, out d) && d != null && d is NotificationHandler<T>)
                    {
                        (d as NotificationHandler<T>)(notification);
                        bSuccess = true;
                    }

                    //post unsafe receivers
                    if (_unsafeTable.ContainsKey(notificationType))
                    {
                        var ud = _unsafeTable[notificationType];
                        if (ud != null)
                        {
                            ud(notification);
                            bSuccess = true;
                        }
                    }

                    notificationType = notificationType.BaseType;
                }

                return bSuccess;
            }

            #endregion

            public bool HasObserverOf<T>() where T : Notification
            {
                return this.HasObserverOf(typeof(T));
            }
            public bool HasObserverOf(System.Type notificationType)
            {
                var baseType = typeof(Notification);
                while (baseType.IsAssignableFrom(notificationType))
                {
                    if (_table.ContainsKey(notificationType))
                    {
                        if (_table[notificationType] != null) return true;
                    }

                    if (_unsafeTable.ContainsKey(notificationType))
                    {
                        if (_unsafeTable[notificationType] != null) return true;
                    }

                    notificationType = notificationType.BaseType;
                }

                return false;
            }

            #endregion

        }

        #endregion

    }

}
