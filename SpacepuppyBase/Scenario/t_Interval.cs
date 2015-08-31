using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenario
{

    /// <summary>
    /// Perform an action on some interval.
    /// 
    /// Need to remove the old float for interval that is now replaced with TimePeriod. It's kept for now 
    /// until all the scripts are updated. Remove ISerializationCallbackReceiver when that is done.
    /// </summary>
    public class t_Interval : TriggerComponent, ISerializationCallbackReceiver
    {

        [HideInInspector()]
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Interval")]
        [System.Obsolete("This needs to be removed.")]
        private float _interval_old = 1.0f;

        [SerializeField()]
        private TimePeriod _interval;

        [System.NonSerialized()]
        private RadicalCoroutine _ticker;


        #region CONSTRUCTOR

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            if(_ticker == null || _ticker.Finished || _ticker.Cancelled)
            {
                _ticker = RadicalCoroutine.Ticker(this.TickerCallback);
            }
            _ticker.Start(this, RadicalCoroutineDisableMode.Pauses);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _ticker.Stop();
        }

        #endregion

        #region Properties

        public TimePeriod Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public float Duration
        {
            get { return _interval.Seconds; }
            set { _interval.Seconds = value; }
        }

        #endregion

        #region Methods


        private object TickerCallback()
        {
            return WaitForDuration.Period(_interval);
        }

        #endregion


        #region Temp ISerializationCallbackReceiver Interface

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if(_interval_old != 0f)
                _interval.Seconds = _interval_old;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_interval_old != 0f)
                _interval_old = 0f;
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
