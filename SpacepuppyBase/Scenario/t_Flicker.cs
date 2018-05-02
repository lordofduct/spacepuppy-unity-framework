#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Geom;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class t_Flicker : SPComponent, IObservableTrigger
    {

        #region Fields

        [SerializeField]
        private Interval _initialDelay = new Interval(0f);
        [SerializeField]
        private Interval _delayAfterA = new Interval(1f);
        [SerializeField]
        private Interval _delayAfterB = new Interval(1f);

        [SerializeField]
        private Trigger _triggerA;

        [SerializeField]
        private Trigger _triggerB;


        [System.NonSerialized]
        private bool _state;
        [System.NonSerialized]
        private float _t;

        #endregion

        #region CONSTRUCTOR

        protected override void OnEnable()
        {
            base.OnEnable();

            _state = false;
            _t = RandomUtil.Standard.Range(_initialDelay.Max, _initialDelay.Min);
        }

        #endregion

        #region Methods

        private void Update()
        {
            if (_t <= 0f)
            {
                if (_state)
                {
                    _triggerB.ActivateTrigger(this, null);
                    _t = RandomUtil.Standard.Range(_delayAfterB.Max, _delayAfterB.Min);
                }
                else
                {
                    _triggerA.ActivateTrigger(this, null);
                    _t = RandomUtil.Standard.Range(_delayAfterA.Max, _delayAfterA.Min);
                }

                _state = !_state;
            }

            _t -= Time.deltaTime;
        }

        #endregion

        #region IObserverableTrigger Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _triggerA, _triggerB };
        }

        #endregion

    }

}