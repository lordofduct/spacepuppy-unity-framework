using UnityEngine;

namespace com.spacepuppy.Scenario
{
    [System.Serializable()]
    public class ObservableTargetData
    {

        public System.EventHandler<TempEventArgs> TriggerActivated;

        #region Fields

        [SerializeField]
        [TypeRestriction(typeof(IObservableTrigger))]
        private UnityEngine.Object _target;

        [SerializeField]
        private int _triggerIndex;


        [System.NonSerialized]
        private bool _initialized;
        [System.NonSerialized()]
        private BaseSPEvent _targetEvent;
        
        [System.NonSerialized]
        private TriggerTarget[] _hijackCache;

        #endregion

        #region Properties

        /// <summary>
        /// The object being observed.
        /// 
        /// If you set this after hijacking/adding a handler. You may need to call Init and recall 'BeginHijack'.
        /// </summary>
        public IObservableTrigger Target
        {
            get { return _target as IObservableTrigger; }
            set
            {
                var targ = value as UnityEngine.Object;
                if (targ == _target) return;

                this.DeInit();
                _target = targ;
                _initialized = false;
                _targetEvent = null;
            }
        }

        public int TriggerIndex
        {
            get { return _triggerIndex; }
            set { _triggerIndex = value; }
        }

        public BaseSPEvent TargetEvent
        {
            get
            {
                if (!_initialized) this.Init();
                return _targetEvent;
            }
        }

        #endregion

        #region Methods

        public void Init()
        {
            if (_initialized) return;

            _initialized = true;
            if(_triggerIndex >= 0 && _target is IObservableTrigger)
            {
                var arr = (_target as IObservableTrigger).GetTriggers();
                if (arr != null && _triggerIndex < arr.Length)
                {
                    _targetEvent = arr[_triggerIndex];
                    if (_targetEvent != null) _targetEvent.TriggerActivated += this.OnTriggerActivated;
                }
            }
        }

        public void DeInit()
        {
            if(_targetEvent != null)
            {
                if (_hijackCache != null) this.EndHijack();
                _targetEvent.TriggerActivated -= this.OnTriggerActivated;
                _targetEvent = null;
            }
        }
        
        public bool BeginHijack()
        {
            if (!_initialized) this.Init();

            if (_targetEvent == null) return false;
            
            if(_hijackCache == null)
            {
                _hijackCache = _targetEvent.Targets.ToArray();
                _targetEvent.Targets.Clear();
            }
            return true;
        }

        public void EndHijack()
        {
            if(_hijackCache != null)
            {
                _targetEvent.Targets.AddRange(_hijackCache);
                _hijackCache = null;
            }
        }

        private void OnTriggerActivated(object sender, TempEventArgs e)
        {
            var d = this.TriggerActivated;
            if (d != null) d(this, e);
        }

        #endregion

    }




    /*
        * Old version
        * 
    [System.Serializable()]
    public class ObservableTargetData
    {

        #region Fields

        [SerializeField()]
        private Component _trigger;
        [SerializeField()]
        private string _triggerId;


        [System.NonSerialized()]
        private Trigger _targetTrigger;
        [System.NonSerialized()]
        private System.Action<ObservableTargetData> _callback;

        #endregion

        #region Properties

        public IObservableTrigger Trigger
        {
            get { return _trigger as IObservableTrigger; }
            set
            {
                if (_trigger == value.component) return;

                _trigger = value.component;

                if(_callback != null)
                {
                    this.RegisterTargetTriggerEventHandler();
                }
            }
        }

        public string TriggerId
        {
            get { return _triggerId; }
            set
            {
                if (_triggerId == value) return;
                _triggerId = value;

                if (_callback != null)
                {
                    this.RegisterTargetTriggerEventHandler();
                }
            }
        }

        #endregion

        #region Methods

        public void AddHandler(System.Action<ObservableTargetData> handler)
        {
            if (handler == null) return;

            if (_callback == null) this.RegisterTargetTriggerEventHandler();
            _callback += handler;
        }

        public void RemoveHandler(System.Action<ObservableTargetData> handler)
        {
            if (handler == null) return;

            _callback -= handler;
            if (_callback == null) this.UnregisterTargetTriggerEventHandler();
        }


        private void RegisterTargetTriggerEventHandler()
        {
            this.UnregisterTargetTriggerEventHandler();

            var t = this.Trigger;
            if (t == null) return;

            _targetTrigger = null;
            var arr = t.GetTriggers();
            if (arr.Length == 1)
            {
                _targetTrigger = arr[0];
            }
            else
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].ObservableTriggerId == _triggerId)
                    {
                        _targetTrigger = arr[i];
                        break;
                    }
                }
            }

            if(_targetTrigger != null)
            {
                _targetTrigger.TriggerActivated += this.OnTriggerActivated;
            }
        }

        private void UnregisterTargetTriggerEventHandler()
        {
            if (_targetTrigger != null)
            {
                _targetTrigger.TriggerActivated -= this.OnTriggerActivated;
                _targetTrigger = null;
            }
        }

        private void OnTriggerActivated(object sender, System.EventArgs e)
        {
            if (_callback != null) _callback(this);
        }

        #endregion

    }
        */

}
