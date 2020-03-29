using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

using com.spacepuppy;
using com.spacepuppy.Utils;
using com.spacepuppy.SPInput.Unity;

namespace com.spacepuppy.SPInput.EventSystems
{

    public class SPInputModule : StandaloneInputModule
    {

        protected override void Awake()
        {
            base.Awake();

            this.m_InputOverride = this.AddOrGetComponent<SPBaseInput>().Init(this);
        }


        private static AxisDelegate _defaultHorizontal = () => SPInputDirect.GetAxisRaw(SPInputId.Axis1);
        private static AxisDelegate _defaultVertical = () => SPInputDirect.GetAxisRaw(SPInputId.Axis2);
        private static ButtonDelegate _defaultSubmit = () => SPInputDirect.GetButtonDown(SPInputId.Button0);
        private static ButtonDelegate _defaultCancel = () => SPInputDirect.GetButtonDown(SPInputId.Button1);
        private static AxisDelegate _horizontal = _defaultHorizontal;
        private static AxisDelegate _vertical = _defaultVertical;
        private static ButtonDelegate _submit = _defaultSubmit;
        private static ButtonDelegate _cancel = _defaultCancel;

        public static AxisDelegate HorizontalCallback
        {
            get { return _horizontal; }
            set { _horizontal = value; }
        }

        public static AxisDelegate VerticalCallback
        {
            get { return _vertical; }
            set { _vertical = value; }
        }

        public static ButtonDelegate SubmitCallback
        {
            get { return _submit; }
            set { _submit = value; }
        }

        public static ButtonDelegate CancelCallback
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        public static AxisDelegate DefaultHorizontalCallback
        {
            get { return _defaultHorizontal; }
        }

        public static AxisDelegate DefaultVerticalCallback
        {
            get { return _defaultVertical; }
        }

        public static ButtonDelegate DefaultSubmitCallback
        {
            get { return _defaultSubmit; }
        }

        public static ButtonDelegate DefaultCancelCallback
        {
            get { return _defaultCancel; }
        }



        public class SPBaseInput : BaseInput
        {

            private StandaloneInputModule _module;

            internal SPBaseInput Init(StandaloneInputModule module)
            {
                _module = module;
                return this;
            }

            public override float GetAxisRaw(string axisName)
            {
                var service = Services.Get<IInputManager>();
                if (service != null && service.Main != null)
                {
                    var device = service.Main;
                    if (device.Contains(axisName))
                        return device.GetAxleState(axisName);
                }

                if (_module != null)
                {
                    if (axisName == _module.horizontalAxis)
                    {
                        if (_horizontal != null) return _horizontal();
                    }
                    else if (axisName == _module.verticalAxis)
                    {
                        if (_vertical != null) return _vertical();
                    }
                    else if (axisName == _module.submitButton)
                    {
                        if (_submit != null) return _submit() ? 1f : 0f;
                    }
                    else if (axisName == _module.cancelButton)
                    {
                        if (_cancel != null) return _cancel() ? 1f : 0f;
                    }
                }

                return base.GetAxisRaw(axisName);
            }

            public override bool GetButtonDown(string buttonName)
            {
                var service = Services.Get<IInputManager>();
                if (service != null && service.Main != null)
                {
                    var device = service.Main;
                    if (device.Contains(buttonName))
                        return device.GetButtonState(buttonName) == SPInput.ButtonState.Down;
                }

                if (_module != null)
                {
                    if (buttonName == _module.horizontalAxis)
                    {
                        if (_horizontal != null) return false;
                    }
                    else if (buttonName == _module.verticalAxis)
                    {
                        if (_vertical != null) return false;
                    }
                    else if (buttonName == _module.submitButton)
                    {
                        if (_submit != null) return _submit();
                    }
                    else if (buttonName == _module.cancelButton)
                    {
                        if (_cancel != null) return _cancel();
                    }
                }

                return base.GetButtonDown(buttonName);
            }

        }
    }

}
