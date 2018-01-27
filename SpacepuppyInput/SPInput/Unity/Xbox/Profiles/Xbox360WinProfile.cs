using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    /// <summary>
    /// InputName: "Controller (Xbox 360 For Windows)"
    /// InputName (per InControl): "XBOX 360 For Windows (Controller)", "Controller (Xbox 360 Wireless Receiver for Windows)", "Controller (Gamepad for Xbox 360)"
    /// InputName (similar, per InControl): "Controller (Afterglow Gamepad for Xbox 360)", "Controller (Batarang wired controller (XBOX))", "Controller (Infinity Controller 360)", "Controller (Mad Catz FPS Pro GamePad)", 
    ///           "Controller (MadCatz Call of Duty GamePad)", "Controller (MadCatz GamePad)", "Controller (MLG GamePad for Xbox 360)", "Controller (Razer Sabertooth Elite)", "Controller (Rock Candy Gamepad for Xbox 360)", 
    ///           "Controller (XEOX Gamepad)"
    /// </summary>
    [InputProfileDescription("Xbox 360 Controller", TargetPlatform.Windows, Description = "Xbox 360 Controller (Windows)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_XBOX360)]
    [InputProfileJoystickName("Controller (Xbox 360 For Windows)")]
    [InputProfileJoystickName("XBOX 360 For Windows (Controller)")]
    [InputProfileJoystickName("Controller (Xbox 360 Wireless Receiver for Windows)")]
    [InputProfileJoystickName("Controller (Gamepad for Xbox 360)")]
    [InputProfileJoystickName("Controller (Afterglow Gamepad for Xbox 360)")]
    [InputProfileJoystickName("Controller (Batarang wired controller (XBOX))")]
    [InputProfileJoystickName("Controller (Infinity Controller 360)")]
    [InputProfileJoystickName("Controller (Mad Catz FPS Pro GamePad)")]
    [InputProfileJoystickName("Controller (MadCatz Call of Duty GamePad)")]
    [InputProfileJoystickName("Controller (MadCatz GamePad)")]
    [InputProfileJoystickName("Controller (MLG GamePad for Xbox 360)")]
    [InputProfileJoystickName("Controller (Razer Sabertooth Elite)")]
    [InputProfileJoystickName("Controller (Rock Candy Gamepad for Xbox 360)")]
    [InputProfileJoystickName("Controller (XEOX Gamepad)")]
    public class Xbox360WinProfile : XboxInputProfile
    {

        public Xbox360WinProfile()
        {
            this.Register(XboxAxis.LeftStickX, SPInputId.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputId.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputId.Axis4);
            this.Register(XboxAxis.RightStickY, SPInputId.Axis5);
            this.Register(XboxAxis.DPadX, SPInputId.Axis6);
            this.Register(XboxAxis.DPadY, SPInputId.Axis7);
            this.Register(XboxAxis.LeftTrigger, SPInputId.Axis9);
            this.Register(XboxAxis.RightTrigger, SPInputId.Axis10);

            this.Register(XboxButton.A, SPInputId.Button0);
            this.Register(XboxButton.B, SPInputId.Button1);
            this.Register(XboxButton.X, SPInputId.Button2);
            this.Register(XboxButton.Y, SPInputId.Button3);
            this.Register(XboxButton.LB, SPInputId.Button4);
            this.Register(XboxButton.RB, SPInputId.Button5);
            this.Register(XboxButton.Back, SPInputId.Button6);
            this.Register(XboxButton.Start, SPInputId.Button7);
            this.Register(XboxButton.LeftStick, SPInputId.Button8);
            this.Register(XboxButton.RightStick, SPInputId.Button9);
            this.Register(XboxButton.DPadUp, SPInputId.Axis7, AxleValueConsideration.Negative);
            this.Register(XboxButton.DPadDown, SPInputId.Axis7, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadRight, SPInputId.Axis6, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadLeft, SPInputId.Axis6, AxleValueConsideration.Negative);
        }

    }

}
