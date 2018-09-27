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
            _initialized = false;
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
    
}
