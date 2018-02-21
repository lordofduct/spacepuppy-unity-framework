using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("PS2 Controller", TargetPlatform.Windows, Description = "PS2 Controller (Windows)")]
    [InputProfileJoystickName("Twin USB Joystick")]
    public class PS2WinProfile : XboxInputProfile
    {

        public PS2WinProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis4);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis3, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis5);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis6);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Button4);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Button5);


            this.RegisterButton(XboxInputId.A, SPInputId.Button2); //X
            this.RegisterButton(XboxInputId.B, SPInputId.Button1); //O
            this.RegisterButton(XboxInputId.X, SPInputId.Button3); //Sqr
            this.RegisterButton(XboxInputId.Y, SPInputId.Button0); //Tri
            this.RegisterButton(XboxInputId.LB, SPInputId.Button6); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button7); //R1
            this.RegisterButton(XboxInputId.Back, SPInputId.Button8); //Share
            this.RegisterButton(XboxInputId.Start, SPInputId.Button9); //Options
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button10);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button11);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis6, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis6, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis5, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis5, AxleValueConsideration.Negative);
        }

    }

}
