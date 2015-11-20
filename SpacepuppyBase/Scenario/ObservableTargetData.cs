using UnityEngine;

namespace com.spacepuppy.Scenario
{

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

}
