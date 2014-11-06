using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class EnterTrigger : TriggerComponent
    {

        public ScenarioActivatorMask Mask;
        public float CooldownInterval = 1.0f;
        public bool IncludeColliderAsTriggerArg = true;

        [System.NonSerialized()]
        private bool _coolingDown;

        void OnTriggerEnter(Collider other)
        {
            if (_coolingDown) return;

            if (Mask == null || Mask.Intersects(other))
            {
                if (this.IncludeColliderAsTriggerArg)
                {
                    this.ActivateTrigger(other);
                }
                else
                {
                    this.ActivateTrigger();
                }

                _coolingDown = true;
                this.Invoke(() =>
                {
                    _coolingDown = false;
                }, this.CooldownInterval);
            }
        }

    }
}
