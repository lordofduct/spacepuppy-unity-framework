using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_EnterTrigger : TriggerComponent, ICompoundTriggerResponder
    {

        public ScenarioActivatorMask Mask = new ScenarioActivatorMask(-1);
        public float CooldownInterval = 1.0f;
        public bool IncludeColliderAsTriggerArg = true;

        [System.NonSerialized()]
        private bool _coolingDown;


        private void DoTestTriggerEnter(Collider other)
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

        void OnTriggerEnter(Collider other)
        {
            if (this.HasComponent<CompoundTrigger>()) return;

            this.DoTestTriggerEnter(other);
        }

        void ICompoundTriggerResponder.OnCompoundTriggerEnter(Collider other)
        {
            this.DoTestTriggerEnter(other);
        }

        void ICompoundTriggerResponder.OnCompoundTriggerExit(Collider other)
        {
            //do nothing
        }

    }

}
