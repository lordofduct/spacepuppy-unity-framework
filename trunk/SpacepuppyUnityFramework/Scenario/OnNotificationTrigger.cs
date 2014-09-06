using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class OnNotificationTrigger : Trigger
    {

        #region Fields

        [ModifierChain()]
        [DisableOnPlay()]
        [TypeReferenceRestriction(typeof(com.spacepuppy.Notification))]
        [SerializeField()]
        private TypeReference _notificationType = new TypeReference();

        [ModifierChain()]
        [DisableOnPlay()]
        [DefaultFromSelf()]
        [Tooltip("The target to whom to listen to for the notification.")]
        [SerializeField()]
        private GameObject _targetGameObject;

        [System.NonSerialized()]
        private GameObject _targCache; //cache who we registered the listerner with on enable, so we can remove it on exit
        [System.NonSerialized()]
        private System.Type _typeCache;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if (_targetGameObject == null) _targetGameObject = this.gameObject;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_targetGameObject != null && _notificationType.Type != null)
            {
                _typeCache = _notificationType.Type;
                _targCache = _targetGameObject;
                Notification.UnsafeRegisterObserver(_typeCache, _targCache, this.OnNotification);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if(_targCache != null)
            {
                Notification.UnsafeRemoveObserver(_typeCache, _targCache, this.OnNotification);
                _typeCache = null;
                _targCache = null;
            }
        }

        #endregion

        #region Properties

        public TypeReference NotificationType { get { return _notificationType; } }

        public GameObject TargetGameObject
        {
            get { return _targetGameObject; }
            set
            {
                _targetGameObject = value;
            }
        }

        #endregion

        #region Methods

        public void ResetHandler()
        {
            if (!this.enabled) return;
            if (_targCache != null)
            {
                Notification.UnsafeRemoveObserver(_typeCache, _targCache, this.OnNotification);
                _typeCache = null;
                _targCache = null;
            }

            if (_targetGameObject != null && _notificationType.Type != null)
            {
                _typeCache = _notificationType.Type;
                _targCache = _targetGameObject;
                Notification.UnsafeRegisterObserver(_typeCache, _targCache, this.OnNotification);
            }
        }

        private void OnNotification(Notification n)
        {
            this.ActivateTrigger();
        }

        #endregion

    }
}
