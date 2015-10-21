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

        #endregion

        #region Properties

        public IObservableTrigger Trigger
        {
            get { return _trigger as IObservableTrigger; }
            set { _trigger = value.component; }
        }

        public string TriggerId
        {
            get { return _triggerId; }
            set { _triggerId = value; }
        }

        #endregion

    }

}
