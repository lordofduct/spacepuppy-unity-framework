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
            _timer = new com.spacepuppy.Timers.Timer(1.0f, 0);
            _timer.TimerCount += delegate(com.spacepuppy.Timers.Timer t)
            {
                Notification.CleanWeakReferences();
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

        public static void CleanWeakReferences()
        {
            foreach (var h in _senderSpecificNotificationHandlers.Values)
            {
                h.CleanWeakReferences();
            }

            _globalNotificationHandlers.CleanWeakReferences();
        }

        #endregion

        #region Register/Post Interface

        public static void RegisterGlobalObserver<T>(NotificationHandler<T> handler, bool useWeakReference = false) where T : Notification
        {
            if (handler == null) throw new System.ArgumentNullException("handler");
            _globalNotificationHandlers.RegisterObserver<T>(handler, useWeakReference);
        }

        public static void RemoveGlobalObserver<T>(NotificationHandler<T> handler) where T : Notification
        {
            if (handler == null) throw new System.ArgumentNullException("handler");
            _globalNotificationHandlers.RemoveObserver<T>(handler);
        }

        public static void RegisterObserver<T>(object sender, NotificationHandler<T> handler, bool useWeakReference = false) where T : Notification
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
            coll.RegisterObserver<T>(handler, useWeakReference);
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
                    if (root != null && root != go)
                    {
                        if (_senderSpecificNotificationHandlers.TryGetValue(root, out coll))
                        {
                            if (coll.PostNotification<T>(notification)) handled = true;
                        }
                    }
                }
            }

            //finally let anyone registered with the global hear about it
            bool globallyHandled = _globalNotificationHandlers.PostNotification<T>(notification);

            //if not handled, check if we should throw an exception
            if (!(handled || globallyHandled))
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

        public static bool HasObserver<T>(object sender, bool bNotifyRoot = false)
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

        public static bool HasGlobalObserver<T>()
        {
            return _globalNotificationHandlers.HasObserverOf<T>();
        }

        #endregion

        #endregion

        #region Special Types

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
            private ListDictionary<Type, Delegate> _weakTable = new ListDictionary<Type, Delegate>(() => new WeakList<Delegate>());

            #endregion

            #region CONSTRUCTOR

            public NotificationHandlerCollection()
            {

            }

            #endregion

            #region Properties

            public bool IsEmpty { get { return _table.Count == 0 && _weakTable.Count == 0; } }

            #endregion

            #region Methods

            public void RegisterObserver<T>(NotificationHandler<T> handler, bool useWeakReference = false) where T : Notification
            {
                var tp = typeof(T);
                if (useWeakReference)
                {
                    _weakTable.Add(tp, handler);
                    (_weakTable.Lists[tp] as WeakList<Delegate>).Clean();
                }
                else
                {
                    System.Delegate d = (_table.ContainsKey(tp)) ? _table[tp] : null;
                    //this would only fail if someone modified this code to allow adding mismatched delegates with notification types
                    d = Delegate.Combine(d, handler);
                    _table[tp] = d;
                }
            }

            public void RemoveObserver<T>(NotificationHandler<T> handler) where T : Notification
            {
                var tp = typeof(T);
                if (_weakTable.ContainsKey(tp) && _weakTable.Lists[tp].Contains(handler))
                {
                    _weakTable.Lists[tp].Remove(handler);
                    (_weakTable.Lists[tp] as WeakList<Delegate>).Clean();
                    _weakTable.Purge();
                }
                else if (_table.ContainsKey(tp))
                {
                    var d = _table[tp];
                    if (d != null)
                    {
                        d = Delegate.Remove(d, handler);
                        if (d != null)
                        {
                            _table[tp] = d;
                        }
                        else
                        {
                            _table.Remove(tp);
                        }
                    }
                }
            }

            public bool PostNotification<T>(T notification) where T : Notification
            {
                var tp = typeof(T);
                Delegate d = null;
                _table.TryGetValue(tp, out d);

                if (_weakTable.ContainsKey(tp))
                {
                    var lst = _weakTable.Lists[tp];
                    if (lst != null && lst.Count > 0)
                    {
                        //try
                        //{
                        //    var wd = Delegate.Combine(lst.ToArray());
                        //    d = Delegate.Combine(d, wd);
                        //}
                        //catch
                        //{

                        //}
                        foreach (var wd in lst)
                        {
                            if (wd != null) d = Delegate.Combine(d, wd);
                        }
                    }
                }

                if (d != null && d is NotificationHandler<T>)
                {
                    (d as NotificationHandler<T>)(notification);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool HasObserverOf<T>()
            {
                var tp = typeof(T);

                if (_table.ContainsKey(tp))
                {
                    return _table[tp] != null;
                }

                if (_weakTable.ContainsKey(tp))
                {
                    return _weakTable.Lists[tp].Count != 0;
                }

                return false;
            }


            public void CleanWeakReferences()
            {
                var arr = _weakTable.ToArray();
                foreach (var pair in arr)
                {
                    var lst = pair.Value as WeakList<Delegate>;
                    if (lst != null)
                    {
                        lst.Clean();
                        if (lst.Count == 0) _weakTable.Remove(pair.Key);
                    }
                }
            }

            #endregion

        }

        #endregion

    }

}
