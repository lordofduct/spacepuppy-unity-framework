using UnityEngine;

namespace com.spacepuppy.Scenario
{
    public class t_OnNotificationTrigger : TriggerComponent
    {

        #region Fields
        
        [SerializeField()]
        [TypeReference.Config(typeof(com.spacepuppy.Notification))]
        private TypeReference _notificationType = new TypeReference();

        [SerializeField()]
        private bool _useGlobal = true;
        
        [SerializeField()]
        [DefaultFromSelf()]
        [Tooltip("The target to whom to listen to for the notification.")]
        private GameObject _targetGameObject;

        [System.NonSerialized()]
        private GameObject _targCache; //cache who we registered the listerner with on enable, so we can remove it on exit
        [System.NonSerialized()]
        private System.Type _typeCache;

        #endregion

        #region CONSTRUCTOR
        
        protected override void OnEnable()
        {
            base.OnEnable();

            this.AddHandler();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.RemoveHandler();
        }

        #endregion

        #region Properties

        public TypeReference NotificationType { get { return _notificationType; } }

        public bool UseGlobal
        {
            get { return _useGlobal; }
            set
            {
                _useGlobal = value;
                if (_useGlobal) _targetGameObject = null;
            }
        }

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
            this.RemoveHandler();

            if (!this.enabled) return;
            
            this.AddHandler();
        }

        private void RemoveHandler()
        {
            if (_typeCache != null)
            {
                if (!object.ReferenceEquals(_targCache, null))
                {
                    Notification.UnsafeRemoveObserver(_typeCache, _targCache, this.OnNotification);
                }
                else
                {
                    Notification.UnsafeRemoveGlobalObserver(_typeCache, this.OnNotification);
                }
                _typeCache = null;
                _targCache = null;
            }
        }

        private void AddHandler()
        {
            _typeCache = null;
            _targCache = null;
            if(_notificationType.Type != null)
            {
                if (_useGlobal)
                {
                    _typeCache = _notificationType.Type;
                    _targCache = null;
                    Notification.UnsafeRegisterGlobalObserver(_typeCache, this.OnNotification);
                }
                else if (_targetGameObject != null)
                {
                    _typeCache = _notificationType.Type;
                    _targCache = _targetGameObject;
                    Notification.UnsafeRegisterObserver(_typeCache, _targCache, this.OnNotification);
                }
            }
            
        }


        private void OnNotification(object sender, Notification n)
        {
            this.ActivateTrigger(n);
        }

        #endregion

    }
}
