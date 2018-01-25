using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    /// <summary>
    /// InputName: "UNIDENTIFIED CONTROLLER"
    /// InputName (per InControl): "Microsoft Wireless 360 Controller", "\u00A9Microsoft Corporation Controller", "\u00A9Microsoft Corporation Xbox Original Wired Controller"
    /// InputName (similar, per InControl): "Mad Catz, Inc. Mad Catz FPS Pro GamePad"
    /// </summary>
    [InputProfileDescription("Xbox 360 Controller", TargetPlatform.MacOSX, Description = "Xbox 360 Controller (MacOSX)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_XBOX360)]
    public class Xbox360MacOSProfile : XboxInputProfile
    {

        public Xbox360MacOSProfile()
        {
            this.Register(XboxAxis.LeftStickX, SPInputAxis.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputAxis.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputAxis.Axis3);
            this.Register(XboxAxis.RightStickY, SPInputAxis.Axis4);
            this.Register(XboxAxis.DPadX, SPInputButton.Button5, SPInputButton.Button6);
            this.Register(XboxAxis.DPadY, SPInputButton.Button7, SPInputButton.Button8);
            this.Register(XboxAxis.LeftTrigger, SPInputAxis.Axis5);
            this.Register(XboxAxis.RightTrigger, SPInputAxis.Axis6);

            this.Register(XboxButton.A, SPInputButton.Button16);
            this.Register(XboxButton.B, SPInputButton.Button17);
            this.Register(XboxButton.X, SPInputButton.Button18);
            this.Register(XboxButton.Y, SPInputButton.Button19);
            this.Register(XboxButton.LB, SPInputButton.Button13);
            this.Register(XboxButton.RB, SPInputButton.Button14);
            this.Register(XboxButton.Back, SPInputButton.Button10);
            this.Register(XboxButton.Start, SPInputButton.Button9);
            this.Register(XboxButton.LeftStick, SPInputButton.Button11);
            this.Register(XboxButton.RightStick, SPInputButton.Button12);
            this.Register(XboxButton.DPadUp, SPInputButton.Button5);
            this.Register(XboxButton.DPadDown, SPInputButton.Button6);
            this.Register(XboxButton.DPadRight, SPInputButton.Button7);
            this.Register(XboxButton.DPadLeft, SPInputButton.Button8);
        }

    }

}
