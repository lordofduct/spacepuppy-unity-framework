using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    /// <summary>
    /// InputName: "Unknown Wireless Controller"
    /// </summary>
    [InputProfileDescription("PS4 Controller", TargetPlatform.MacOSX, Description = "PS4 Controller Bluetooth (MacOSX)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS4)]
    [InputProfileJoystickName("Unknown Wireless Controller")]
    public class PS4MacOSBTProfile : XboxInputProfile
    {

        public PS4MacOSBTProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis4, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis7);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis8, true);
            this.RegisterTrigger(XboxInputId.LTrigger, InputToken.CreateLongTrigger(SPInputId.Axis5));
            this.RegisterTrigger(XboxInputId.RTrigger, InputToken.CreateLongTrigger(SPInputId.Axis6));
            //this.RegisterAxis(XboxInputId.LTrigger, SPInputId.Button6, SPInputId.Unknown);
            //this.RegisterAxis(XboxInputId.RTrigger, SPInputId.Button7, SPInputId.Unknown);

            this.RegisterButton(XboxInputId.A, SPInputId.Button1); //X
            this.RegisterButton(XboxInputId.B, SPInputId.Button2); //O
            this.RegisterButton(XboxInputId.X, SPInputId.Button0); //Sqr
            this.RegisterButton(XboxInputId.Y, SPInputId.Button3); //Tri
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5); //R1
            this.RegisterButton(XboxInputId.Back, SPInputId.Button8); //Share
            this.RegisterButton(XboxInputId.Start, SPInputId.Button9); //Options
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button10);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button11);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis7, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis7, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis8, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis8, AxleValueConsideration.Positive);
        }

    }
}
