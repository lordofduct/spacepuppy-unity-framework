using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("MOGA Pro Controller", TargetPlatform.Linux, Description = "MOGA Pro Controller (Linux)")]
    [InputProfileJoystickName("Moga Pro HID")]
    public class MogaProLinuxProfile : XboxInputProfile
    {

        public MogaProLinuxProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis4, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis7);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis8, true);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Axis6);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Axis5);


            this.RegisterButton(XboxInputId.A, SPInputId.Button0);
            this.RegisterButton(XboxInputId.B, SPInputId.Button1);
            this.RegisterButton(XboxInputId.X, SPInputId.Button2);
            this.RegisterButton(XboxInputId.Y, SPInputId.Button3);
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4);
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5);
            //this.RegisterButton(XboxInputId.Back, SPInputId.Button6);
            this.RegisterButton(XboxInputId.Start, SPInputId.Button6);
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button7);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button8);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis7, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis7, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis8, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis8, AxleValueConsideration.Positive);
        }

    }

}
