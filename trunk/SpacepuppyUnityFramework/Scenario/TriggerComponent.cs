using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class TriggerComponent : SPComponent
    {

        #region Fields

        [SerializeField()]
        private Trigger _trigger;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Properties

        public Trigger Targets
        {
            get
            {
                if (_trigger == null) _trigger = new Trigger();
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

    }

}
