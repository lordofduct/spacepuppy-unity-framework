using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.UserInput.UnityInput;


namespace com.spacepuppy.Scenario
{
    public sealed class t_OnSimpleButtonPress : TriggerComponent
    {

        #region Fields
        
        [SerializeField]
        [DisableOnPlay]
        private string _inputId;

        //[System.NonSerialized()]
        //private System.Action<string> _callback;

        #endregion

        #region CONSTRUCTOR

        /*
        protected override void OnEnable()
        {
            base.OnEnable();

            if(_callback == null)
            {
                _callback = (id) =>
                {
                    this.ActivateTrigger();
                };
            }

            if (_inputId != null)
            {
                var device = EventfulUnityInputDevice.GetDevice();
                device.RegisterButtonPress(_inputId, _callback);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if(_callback != null && _inputId != null)
            {
                var device = EventfulUnityInputDevice.GetDevice();
                if(device != null) device.UnregisterButtonPress(_inputId, _callback);
            }
        }
        */

        #endregion

        #region Properties

        public string InputId
        {
            get
            {
                return _inputId;
            }
            set
            {
                //if (_inputId == value) return;
                //if(this.enabled && _callback != null)
                //{
                //    var device = EventfulUnityInputDevice.GetDevice();
                //    device.UnregisterButtonPress(_inputId, _callback);
                //    if(value != null) device.RegisterButtonPress(value, _callback);
                //}

                _inputId = value;
            }
        }

        #endregion

        #region Methods

        private void Update()
        {
            var service = Services.Get<IGameInputManager>();
            var input = service != null ? service.Main : null;
            if(input != null)
            {
                if(input.GetCurrentButtonState(_inputId) == UserInput.ButtonState.Down)
                {
                    this.ActivateTrigger(null);
                }
            }
            else
            {
                if(Input.GetButtonDown(_inputId))
                {
                    this.ActivateTrigger(null);
                }
            }
        }

        #endregion

    }
}
