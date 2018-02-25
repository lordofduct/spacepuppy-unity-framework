using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("Speedlink Strike Controller", TargetPlatform.MacOSX, Description = "Speedlink Strike Controller (MacOSX)")]
    [InputProfileJoystickName("DragonRise Inc.   Generic   USB  Joystick  ")]
    class SpeedlinkStrikeMacProfile : XboxInputProfile
    {

        public SpeedlinkStrikeMacProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis4);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis5, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis6);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis7, true);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Button6);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Button7);

            this.RegisterButton(XboxInputId.A, SPInputId.Button2);
            this.RegisterButton(XboxInputId.B, SPInputId.Button1);
            this.RegisterButton(XboxInputId.X, SPInputId.Button3);
            this.RegisterButton(XboxInputId.Y, SPInputId.Button0);
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4);
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5);
            this.RegisterButton(XboxInputId.Back, SPInputId.Button8);
            this.RegisterButton(XboxInputId.Start, SPInputId.Button9);
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button10);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button11);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis5, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis5, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis6, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis6, AxleValueConsideration.Positive);
        }

    }
}
