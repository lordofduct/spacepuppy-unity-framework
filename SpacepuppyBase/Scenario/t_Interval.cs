#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    /// <summary>
    /// Perform an action on some interval.
    /// 
    /// TODO - Need to remove the old float for interval that is now replaced with TimePeriod. It's kept for now 
    /// until all the scripts are updated. Remove ISerializationCallbackReceiver when that is done.
    /// </summary>
    public class t_Interval : AutoTriggerComponent
    {

        #region Fields
        
        [SerializeField()]
        private SPTimePeriod _interval = new SPTimePeriod(1f);

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

        public SPTimePeriod Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public float Duration
        {
            get { return _interval.Seconds; }
            set { _interval.Seconds = value; }
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
            if (_delay > 0f) yield return WaitForDuration.Seconds(_delay, _interval.TimeSupplier);

            int cnt = 0;

            while(true)
            {
                yield return WaitForDuration.Period(_interval);

                this.ActivateTrigger();

                cnt++;
                if(cnt > _repeatCount)
                {
                    yield break;
                }
            }

        }

        #endregion
        
    }

    [System.Obsolete("Use t_Interval instead.")]
    public class IntervalTrigger : t_Interval
    {

        //public float Interval = 1.0f;

        //private float _currentCoolDown;


        //// Update is called once per frame
        //void Update()
        //{
        //    if (_currentCoolDown > Interval)
        //    {
        //        _currentCoolDown = 0;
        //        this.ActivateTrigger();
        //    }

        //    _currentCoolDown += GameTime.DeltaTime;
        //}
    }

}
