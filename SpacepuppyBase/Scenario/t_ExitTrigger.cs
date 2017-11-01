using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_ExitTrigger : TriggerComponent, ICompoundTriggerResponder
    {

        public ScenarioActivatorMask Mask;
        public float CooldownInterval = 0.0f;
        public bool IncludeColliderAsTriggerArg = true;

        [System.NonSerialized()]
        private bool _coolingDown;

        private void DoTestTriggerExit(Collider other)
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
                //use global incase this gets disable
                GameLoopEntry.Hook.Invoke(() =>
                {
                    _coolingDown = false;
                }, this.CooldownInterval);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (this.HasComponent<CompoundTrigger>()) return;

            this.DoTestTriggerExit(other);
        }


        void ICompoundTriggerResponder.OnCompoundTriggerEnter(Collider other)
        {
            //do nothing
        }

        void ICompoundTriggerResponder.OnCompoundTriggerExit(Collider other)
        {
            this.DoTestTriggerExit(other);
        }


    }
}
