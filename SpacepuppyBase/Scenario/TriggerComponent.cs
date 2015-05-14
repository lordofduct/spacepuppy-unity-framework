using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public abstract class TriggerComponent : SPNotifyingComponent, IObservableTrigger
    {

        #region Fields

        [SerializeField()]
        private Trigger _trigger = new Trigger();

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _trigger.ObservableTriggerOwner = this;
            _trigger.ObservableTriggerId = this.GetType().Name;
        }

        #endregion

        #region Properties

        public Trigger Trigger
        {
            get
            {
                return _trigger;
            }
        }

        #endregion

        #region Methods

        public void ActivateTrigger()
        {
            _trigger.ActivateTrigger();
        }

        public void ActivateTrigger(object arg)
        {
            _trigger.ActivateTrigger(arg);
        }

        #endregion

        #region IObservableTrigger Interface

        bool IObservableTrigger.IsComplex
        {
            get { return false; }
        }

        string[] IObservableTrigger.GetComplexIds()
        {
            return new string[] { this.GetType().Name };
        }

        #endregion

    }

}
