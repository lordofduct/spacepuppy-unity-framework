using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Scenario
{

    [System.Obsolete("This should not be used.")]
    public class TriggerActivatedNotification : Notification
    {

        #region Fields

        private IObservableTrigger _trigger;
        private string _triggeredId;

        #endregion

        #region CONSTRUCTOR

        public TriggerActivatedNotification(IObservableTrigger trigger, string triggeredId)
        {
            _trigger = trigger;
            _triggeredId = triggeredId;
        }

        #endregion

        #region Properties

        public IObservableTrigger Trigger { get { return _trigger; } }

        public string TriggerId { get { return _triggeredId; } }

        #endregion

    }

}
