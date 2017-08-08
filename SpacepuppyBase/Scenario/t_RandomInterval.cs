#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_RandomInterval : AutoTriggerComponent
    {

        #region Fields

        [SerializeField]
        private SPTime _timeSupplier;
        [SerializeField()]
        private Interval _interval = com.spacepuppy.Geom.Interval.MinMax(0f, 1f);

        [SerializeField()]
        [Tooltip("Negative values will repeat forever like infinity.")]
        [DiscreteFloat.NonNegative()]
        private DiscreteFloat _repeatCount = DiscreteFloat.PositiveInfinity;

        [SerializeField()]
        [TimeUnitsSelector()]
        [Tooltip("Wait a duration before starting interval, waits using the same timesupplier as interval.")]
        private float _delay;


        [System.NonSerialized()]
        private RadicalCoroutine _routine;

        #endregion

        #region CONSTRUCTOR

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_routine != null)
            {
                _routine.Cancel();
                _routine = null;
            }
        }

        #endregion

        #region Properties

        public SPTime TimeSupplier
        {
            get { return _timeSupplier; }
            set { _timeSupplier = value; }
        }

        public Interval Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }
        
        public float Delay
        {
            get { return _delay; }
        }

        public DiscreteFloat RepeatCount
        {
            get { return _repeatCount; }
            set { _repeatCount = (value < 0f) ? DiscreteFloat.Zero : value; }
        }

        #endregion

        #region Methods

        protected override void OnTriggerActivate()
        {
            if (_routine != null)
            {
                _routine.Cancel();
                _routine = null;
            }

            this.StartRadicalCoroutine(this.TickerCallback(), RadicalCoroutineDisableMode.CancelOnDisable);
        }


        private System.Collections.IEnumerator TickerCallback()
        {
            if (_delay > 0f) yield return WaitForDuration.Seconds(_delay, _timeSupplier);

            int cnt = 0;

            while (true)
            {
                yield return WaitForDuration.Seconds(RandomUtil.Standard.Range(_interval.Max, _interval.Min), _timeSupplier.TimeSupplier);

                this.ActivateTrigger();

                cnt++;
                if (cnt > _repeatCount)
                {
                    yield break;
                }
            }

        }

        #endregion

    }
}
