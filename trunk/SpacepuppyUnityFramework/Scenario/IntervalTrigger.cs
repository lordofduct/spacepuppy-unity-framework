using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenario
{
    public class IntervalTrigger : TriggerComponent
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

            _currentCoolDown += GameTime.DeltaTime;
        }
    }
}
