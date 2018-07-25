using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_ExitTrigger : TriggerComponent, ICompoundTriggerExitResponder
    {

        #region Fields

        public ScenarioActivatorMask Mask;
        public float CooldownInterval = 0f;
        public bool IncludeColliderAsTriggerArg = true;

        [System.NonSerialized()]
        private bool _coolingDown;

        #endregion

        #region CONSTRUCTOR

        protected override void OnDisable()
        {
            base.OnDisable();

            _coolingDown = false;
        }

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

                if(this.CooldownInterval > 0f)
                {
                    _coolingDown = true;
                    this.InvokeGuaranteed(() =>
                    {
                        _coolingDown = false;
                    }, this.CooldownInterval);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (_coolingDown || this.HasComponent<CompoundTrigger>()) return;

            this.DoTestTriggerExit(other);
        }

        
        void ICompoundTriggerExitResponder.OnCompoundTriggerExit(Collider other)
        {
            this.DoTestTriggerExit(other);
        }

        #endregion

    }
}
