using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_ExitTrigger : TriggerComponent, ICompoundTriggerExitResponder
    {

        #region Fields

        public ScenarioActivatorMask Mask;
        public float CooldownInterval = 0.0f;
        public bool IncludeColliderAsTriggerArg = true;

        [System.NonSerialized()]
        private bool _coolingDown;

        #endregion

        #region Methods

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
                this.InvokeGuaranteed(() =>
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

        
        void ICompoundTriggerExitResponder.OnCompoundTriggerExit(Collider other)
        {
            this.DoTestTriggerExit(other);
        }

        #endregion

    }
}
