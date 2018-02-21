using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("Xbox One Controller", TargetPlatform.MacOSX, Description = "Xbox One Controller (OSX)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_XBOXONE)]
    [InputProfileJoystickName("Microsoft Xbox One Wired Controller")]
    public class XboxOneMacProfile : XboxInputProfile
    {

        public XboxOneMacProfile()
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
