using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenario
{
    public class t_Interval : TriggerComponent
    {
        public float Interval = 1.0f;

        private float _currentCoolDown;


        // Update is called once per frame
        void Update()
        {
            if (_currentCoolDown > Interval)
            {
                _currentCoolDown = 0;
                this.ActivateTrigger();
            }

            _currentCoolDown += Time.deltaTime;
        }
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
