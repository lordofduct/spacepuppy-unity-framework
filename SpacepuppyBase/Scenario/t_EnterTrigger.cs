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

    [Infobox("If you're looking for the old 'mask in the inspector' approach, use t_EnterTrigger.")]
    public class t_OnEnterTrigger : TriggerComponent, ICompoundTriggerEnterResponder
    {

        #region Fields

        [SerializeField]
        private EventActivatorMaskRef _mask = new EventActivatorMaskRef();
        [SerializeField]
        private float _cooldownInterval = 0f;
        [SerializeField]
        private bool _includeColliderAsTriggerArg = true;

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

        #region Properties

        public IEventActivatorMask Mask
        {
            get { return _mask.Value; }
            set { _mask.Value = value; }
        }

        public float CooldownInterval
        {
            get { return _cooldownInterval; }
            set { _cooldownInterval = value; }
        }

        public bool IncludeCollidersAsTriggerArg
        {
            get { return _includeColliderAsTriggerArg; }
            set { _includeColliderAsTriggerArg = value; }
        }

        #endregion

        #region Methods

        private void DoTestTriggerEnter(Collider other)
        {
            if (_coolingDown) return;

            if (_mask.Value == null || _mask.Value.Intersects(other))
            {
                if (_includeColliderAsTriggerArg)
                {
                    this.ActivateTrigger(other);
                }
                else
                {
                    this.ActivateTrigger();
                }

                if (_cooldownInterval > 0f)
                {
                    _coolingDown = true;
                    this.InvokeGuaranteed(() =>
                    {
                        _coolingDown = false;
                    }, _cooldownInterval);
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
