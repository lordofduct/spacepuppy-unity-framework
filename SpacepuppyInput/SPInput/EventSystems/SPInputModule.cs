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

            this.m_InputOverride = this.AddComponent<SPBaseInput>();
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
            get { return _horizontal; }
        }

        public static AxisDelegate DefaultVerticalCallback
        {
            get { return _vertical; }
        }

        public static ButtonDelegate DefaultSubmitCallback
        {
            get { return _submit; }
        }

        public static ButtonDelegate DefaultCancelCallback
        {
            get { return _cancel; }
        }



        public class SPBaseInput : BaseInput
        {

            public override float GetAxisRaw(string axisName)
            {
                var service = Services.Get<IInputManager>();
                if (service != null && service.Main != null)
                {
                    var device = service.Main;
                    if (device.Contains(axisName))
                        return device.GetAxleState(axisName);
                }

                switch (axisName)
                {
                    case "Horizontal":
                        if (_horizontal != null) return _horizontal();
                        break;
                    case "Vertical":
                        if (_vertical != null) return _vertical();
                        break;
                    case "Submit":
                        if (_submit != null) return _submit() ? 1f : 0f;
                        break;
                    case "Cancel":
                        if (_cancel != null) return _cancel() ? 1f : 0f;
                        break;
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

                switch (buttonName)
                {
                    case "Horizontal":
                        if (_horizontal != null) return false;
                        break;
                    case "Vertical":
                        if (_vertical != null) return false;
                        break;
                    case "Submit":
                        if (_submit != null) return _submit();
                        break;
                    case "Cancel":
                        if (_cancel != null) return _cancel();
                        break;
                }

                return base.GetButtonDown(buttonName);
            }

        }
    }

}
