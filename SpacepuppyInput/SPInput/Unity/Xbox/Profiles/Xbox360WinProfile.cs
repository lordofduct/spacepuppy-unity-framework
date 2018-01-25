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
            this.Register(XboxAxis.LeftStickX, SPInputAxis.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputAxis.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputAxis.Axis4);
            this.Register(XboxAxis.RightStickY, SPInputAxis.Axis5);
            this.Register(XboxAxis.DPadX, SPInputAxis.Axis6);
            this.Register(XboxAxis.DPadY, SPInputAxis.Axis7);
            this.Register(XboxAxis.LeftTrigger, SPInputAxis.Axis9);
            this.Register(XboxAxis.RightTrigger, SPInputAxis.Axis10);

            this.Register(XboxButton.A, SPInputButton.Button0);
            this.Register(XboxButton.B, SPInputButton.Button1);
            this.Register(XboxButton.X, SPInputButton.Button2);
            this.Register(XboxButton.Y, SPInputButton.Button3);
            this.Register(XboxButton.LB, SPInputButton.Button4);
            this.Register(XboxButton.RB, SPInputButton.Button5);
            this.Register(XboxButton.Back, SPInputButton.Button6);
            this.Register(XboxButton.Start, SPInputButton.Button7);
            this.Register(XboxButton.LeftStick, SPInputButton.Button8);
            this.Register(XboxButton.RightStick, SPInputButton.Button9);
            this.Register(XboxButton.DPadUp, SPInputAxis.Axis7, AxleValueConsideration.Negative);
            this.Register(XboxButton.DPadDown, SPInputAxis.Axis7, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadRight, SPInputAxis.Axis6, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadLeft, SPInputAxis.Axis6, AxleValueConsideration.Negative);
        }

    }

}
