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
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis4);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis5, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis6);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis7);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Axis9);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Axis10);

            this.RegisterButton(XboxInputId.A, SPInputId.Button0);
            this.RegisterButton(XboxInputId.B, SPInputId.Button1);
            this.RegisterButton(XboxInputId.X, SPInputId.Button2);
            this.RegisterButton(XboxInputId.Y, SPInputId.Button3);
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4);
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5);
            this.RegisterButton(XboxInputId.Back, SPInputId.Button6);
            this.RegisterButton(XboxInputId.Start, SPInputId.Button7);
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button8);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button9);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis6, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis6, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis7, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis7, AxleValueConsideration.Negative);
        }

    }

}
