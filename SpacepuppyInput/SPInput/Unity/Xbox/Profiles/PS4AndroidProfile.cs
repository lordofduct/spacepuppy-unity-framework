using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("PS4 Controller", TargetPlatform.Android, Description = "PS4 Controller (Android)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS4)]
    [InputProfileJoystickName("Sony Computer Entertainment Wireless Controller")]
    public class PS4AndroidProfile : XboxInputProfile
    {

        public PS4AndroidProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis14);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis15, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis5);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis6, true);
            this.RegisterTrigger(XboxInputId.LTrigger, InputToken.CreateLongTrigger(SPInputId.Axis3));
            this.RegisterTrigger(XboxInputId.RTrigger, InputToken.CreateLongTrigger(SPInputId.Axis4));

            this.RegisterButton(XboxInputId.A, SPInputId.Button1); //X
            this.RegisterButton(XboxInputId.B, SPInputId.Button13); //O
            this.RegisterButton(XboxInputId.X, SPInputId.Button0); //Sqr
            this.RegisterButton(XboxInputId.Y, SPInputId.Button2); //Tri
            this.RegisterButton(XboxInputId.LB, SPInputId.Button3); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button14); //R1
            this.RegisterButton(XboxInputId.Back, SPInputId.Button7); //Share
            this.RegisterButton(XboxInputId.Start, SPInputId.Button6); //Options
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button11);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button10);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis6, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis6, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis5, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis5, AxleValueConsideration.Negative);
        }

    }

}
