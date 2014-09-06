using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenario
{
    public class EnterTrigger : Trigger
    {

        public ScenarioActivatorMask Mask;
        public float CooldownInterval = 1.0f;

        [System.NonSerialized()]
        private float _currentCooldown;

        void OnTriggerEnter(Collider other)
        {
            if (_currentCooldown > 0f) return;

            if (Mask == null || Mask.Intersects(other))
            {
                _currentCooldown = this.CooldownInterval;
                this.ActivateTrigger();
            }
        }

        void Update()
        {
            if (_currentCooldown > 0f)
            {
                _currentCooldown -= GameTime.DeltaTime;
                if (_currentCooldown <= 0.0f) _currentCooldown = 0f;
            }
        }

    }
}
