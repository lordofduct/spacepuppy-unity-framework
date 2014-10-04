using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Base contract for any interface contract that should be considered a Component
    /// </summary>
    public interface IComponent : IGameObjectSource
    {
        event System.EventHandler ComponentDestroyed;

        bool enabled { get; set; }
        Component component { get; }

    }

    public abstract class SPComponent : MonoBehaviour, IComponent, INotificationDispatcher
    {

        #region Fields

        [System.NonSerialized]
        private GameObject _entityRoot;
        private bool _started = false;

        private NotificationDispatcher _dispatcher;

        #endregion

        #region CONSTRUCTOR

        protected virtual void Awake()
        {
            if (com.spacepuppy.Utils.Assertions.AssertRequireLikeComponentAttrib(this))
            {
                Object.Destroy(this);
            }
            if (com.spacepuppy.Utils.Assertions.AssertUniqueToEntityAttrib(this))
            {
                Object.Destroy(this);
            }

            this.SyncEntityRoot();
        }

        protected virtual void Start()
        {
            _started = true;
            this.SyncEntityRoot();
            this.OnStartOrEnable();
        }

        protected virtual void OnDestroy()
        {
            //InvokeUtil.CancelInvoke(this);
            if (this.ComponentDestroyed != null)
            {
                this.ComponentDestroyed(this, System.EventArgs.Empty);
            }
        }

        protected virtual void OnEnable()
        {
            this.SendMessage(SPConstants.MSG_ONSPCOMPONENTENABLED, this, SendMessageOptions.DontRequireReceiver);

            if (_started) this.OnStartOrEnable();
        }

        /// <summary>
        /// On start or on enable if and only if start already occurred. This adjusts the order of 'OnEnable' so that it can be used in conjunction with 'OnDisable' to wire up handlers cleanly. 
        /// OnEnable occurs BEFORE Start sometimes, and other components aren't ready yet. This remedies that.
        /// </summary>
        protected virtual void OnStartOrEnable()
        {

        }

        protected virtual void OnDisable()
        {
            this.SendMessage(SPConstants.MSG_ONSPCOMPONENTDISABLED, this, SendMessageOptions.DontRequireReceiver);
        }

        protected virtual void OnDespawn()
        {
            this.PurgeHandlers();
        }

        #endregion

        #region Properties

        public GameObject entityRoot { get { return _entityRoot; } }

        /// <summary>
        /// Start has been called on this component.
        /// </summary>
        public bool started { get { return _started; } }

        #endregion

        #region Methods

        /// <summary>
        /// Call this to resync the 'root' property incase the hierarchy of this object has changed. This needs to be performed since 
        /// unity doesn't have an event/message to signal a change in hierarchy.
        /// </summary>
        public virtual void SyncEntityRoot()
        {
            _entityRoot = this.FindRoot();
        }

        #endregion

        #region IComponent Interface

        public event System.EventHandler ComponentDestroyed;

        bool IComponent.enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        Component IComponent.component
        {
            get { return this; }
        }

        //implemented implicitly
        /*
        GameObject IComponent.gameObject { get { return this.gameObject; } }
        Transform IComponent.transform { get { return this.transform; } }
        */

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
            return _dispatcher.PostNotification(n, bNotifyEntity);
        }

        public void PurgeHandlers()
        {
            if (_dispatcher == null) return;
            _dispatcher.PurgeHandlers();
        }

        #endregion

    }

}
