using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_EnterTrigger : TriggerComponent
    {

        public ScenarioActivatorMask Mask = new ScenarioActivatorMask(-1);
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
