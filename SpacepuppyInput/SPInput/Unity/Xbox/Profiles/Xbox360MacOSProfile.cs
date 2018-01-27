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
            this.Register(XboxAxis.LeftStickX, SPInputId.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputId.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputId.Axis3);
            this.Register(XboxAxis.RightStickY, SPInputId.Axis4);
            this.Register(XboxAxis.DPadX, SPInputId.Button5, SPInputId.Button6);
            this.Register(XboxAxis.DPadY, SPInputId.Button7, SPInputId.Button8);
            this.Register(XboxAxis.LeftTrigger, SPInputId.Axis5);
            this.Register(XboxAxis.RightTrigger, SPInputId.Axis6);

            this.Register(XboxButton.A, SPInputId.Button16);
            this.Register(XboxButton.B, SPInputId.Button17);
            this.Register(XboxButton.X, SPInputId.Button18);
            this.Register(XboxButton.Y, SPInputId.Button19);
            this.Register(XboxButton.LB, SPInputId.Button13);
            this.Register(XboxButton.RB, SPInputId.Button14);
            this.Register(XboxButton.Back, SPInputId.Button10);
            this.Register(XboxButton.Start, SPInputId.Button9);
            this.Register(XboxButton.LeftStick, SPInputId.Button11);
            this.Register(XboxButton.RightStick, SPInputId.Button12);
            this.Register(XboxButton.DPadUp, SPInputId.Button5);
            this.Register(XboxButton.DPadDown, SPInputId.Button6);
            this.Register(XboxButton.DPadRight, SPInputId.Button7);
            this.Register(XboxButton.DPadLeft, SPInputId.Button8);
        }

    }

}
