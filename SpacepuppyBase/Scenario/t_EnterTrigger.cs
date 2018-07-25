using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class t_EnterTrigger : TriggerComponent, ICompoundTriggerEnterResponder
    {

        #region Fields

        public ScenarioActivatorMask Mask = new ScenarioActivatorMask(-1);
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

                if (this.CooldownInterval > 0f)
                {
                    _coolingDown = true;
                    this.InvokeGuaranteed(() =>
                    {
                        _coolingDown = false;
                    }, this.CooldownInterval);
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (_coolingDown || this.HasComponent<CompoundTrigger>()) return;

            this.DoTestTriggerEnter(other);
        }

        void ICompoundTriggerEnterResponder.OnCompoundTriggerEnter(Collider other)
        {
            this.DoTestTriggerEnter(other);
        }
        
        #endregion
        
    }

}
