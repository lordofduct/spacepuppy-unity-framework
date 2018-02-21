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
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis4, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Button8, SPInputId.Button7);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Button5, SPInputId.Button6);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Axis5);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Axis6);

            this.RegisterButton(XboxInputId.A, SPInputId.Button16);
            this.RegisterButton(XboxInputId.B, SPInputId.Button17);
            this.RegisterButton(XboxInputId.X, SPInputId.Button18);
            this.RegisterButton(XboxInputId.Y, SPInputId.Button19);
            this.RegisterButton(XboxInputId.LB, SPInputId.Button13);
            this.RegisterButton(XboxInputId.RB, SPInputId.Button14);
            this.RegisterButton(XboxInputId.Back, SPInputId.Button10);
            this.RegisterButton(XboxInputId.Start, SPInputId.Button9);
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button11);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button12);
            this.RegisterButton(XboxInputId.DPadUp, SPInputId.Button5);
            this.RegisterButton(XboxInputId.DPadDown, SPInputId.Button6);
            this.RegisterButton(XboxInputId.DPadRight, SPInputId.Button8);
            this.RegisterButton(XboxInputId.DPadLeft, SPInputId.Button7);
        }

    }

}
