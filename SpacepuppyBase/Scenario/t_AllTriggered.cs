using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class t_AllTriggered : TriggerComponent
    {

        #region Fields

        [ReorderableArray()]
        [SerializeField()]
        private ObservableTargetData[] _observedTargets;
        [OnChangedInEditor("RegisterListeners", OnlyAtRuntime = true)]
        [SerializeField()]
        private bool _resetOnTriggered;

        private UniqueList<ObservableTargetData> _activatedTriggers = new UniqueList<ObservableTargetData>();
        [System.NonSerialized()]
        private bool _triggered;

        #endregion

        #region CONSTRUCTOR

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            _activatedTriggers.Clear();
            this.RegisterListeners();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.UnRegisterListeners();
            _activatedTriggers.Clear();
        }

        #endregion

        #region Properties

        public bool ResetOnTriggered
        {
            get { return _resetOnTriggered; }
            set
            {
                _resetOnTriggered = value;
                if(_resetOnTriggered && _triggered && this.enabled)
                {
                    _triggered = false;
                    this.RegisterListeners();
                }
            }
        }

        #endregion

        #region Methods

        private void RegisterListeners()
        {
            if (_triggered) return;

            ObservableTargetData targ;
            for (int i = 0; i < _observedTargets.Length; i++)
            {
                targ = _observedTargets[i];
                if (targ.Trigger != null)
                {
                    Notification.RemoveObserver<TriggerActivatedNotification>(targ.Trigger, this.OnTriggerActivated);
                    Notification.RegisterObserver<TriggerActivatedNotification>(targ.Trigger, this.OnTriggerActivated);
                }
            }
        }

        private void UnRegisterListeners()
        {
            ObservableTargetData targ;
            for (int i = 0; i < _observedTargets.Length; i++)
            {
                targ = _observedTargets[i];
                if (targ.Trigger != null)
                {
                    Notification.RemoveObserver<TriggerActivatedNotification>(targ.Trigger, this.OnTriggerActivated);
                }
            }
        }

        private void OnTriggerActivated(object sender, TriggerActivatedNotification n)
        {
            if (_triggered) return;

            foreach (var targ in _observedTargets.Except(_activatedTriggers))
            {
                if (object.Equals(targ.Trigger, n.Trigger) && (!n.Trigger.IsComplex || n.Trigger.GetComplexIds().Contains(targ.TriggerId)))
                {
                    _activatedTriggers.Add(targ);
                    break;
                }
            }

            if(_activatedTriggers.SimilarTo(_observedTargets))
            {
                _activatedTriggers.Clear();
                if (this._resetOnTriggered)
                {
                    _triggered = false;
                }
                else
                {
                    _triggered = true;
                    this.UnRegisterListeners();
                }
                this.ActivateTrigger();
            }
        }

        #endregion

    }

}
