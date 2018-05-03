#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenario
{

    public class HijackTriggerEvents : SPComponent, IObservableTrigger
    {

        #region Fields

        [SerializeField]
        [ReorderableArray]
        private ObservableTargetData[] _targets;

        [SerializeField]
        private Trigger _onHijacked;

        [SerializeField]
        [Tooltip("If true the target won't be purged of its listeners when hijacked.")]
        private bool _dontOverrideTargets;

        #endregion

        #region CONSTRUCTOR

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            foreach (var t in _targets)
            {
                t.Init();
                t.TriggerActivated += this.OnHijackedEventActivated;
                if(!_dontOverrideTargets) t.BeginHijack();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var t in _targets)
            {
                t.TriggerActivated -= this.OnHijackedEventActivated;
                t.EndHijack();
                t.DeInit();
            }
        }

        #endregion

        #region Properties

        public Trigger OnHijacked
        {
            get { return _onHijacked; }
        }

        public bool DontOverrideTargets
        {
            get { return _dontOverrideTargets; }
            set { _dontOverrideTargets = value; }
        }

        #endregion

        #region Methods

        private void OnHijackedEventActivated(object sender, TempEventArgs e)
        {
            _onHijacked.ActivateTrigger(this, e.Value);
        }

        #endregion

        #region IObservableTrigger Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onHijacked };
        }

        #endregion

    }

}
