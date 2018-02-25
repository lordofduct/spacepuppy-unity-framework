using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    /// <remarks>
    /// TODO - need to test this on a proper PS4 using my InputMapTester program.
    /// </remarks>
    [InputProfileDescription("PS4 Controller", TargetPlatform.PS4, Description = "PS4 Controller (PS4)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS4)]
    [InputProfileJoystickName("controller")]
    public class PS4ConsoleProfile : XboxInputProfile
    {

        public PS4ConsoleProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis4);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis5, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis6);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis7);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Axis8); //TODO - need to test
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Axis3, AxleValueConsideration.Negative); //TODO - need to test

            this.RegisterButton(XboxInputId.A, SPInputId.Button0); //X
            this.RegisterButton(XboxInputId.B, SPInputId.Button1); //O
            this.RegisterButton(XboxInputId.X, SPInputId.Button2); //Sqr
            this.RegisterButton(XboxInputId.Y, SPInputId.Button3); //Tri
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5); //R1
            //this.RegisterButton(XboxInputId.Back, SPInputId.Button6); //Share - Share is used by the PS4 itself and is not accessible
            this.RegisterButton(XboxInputId.Start, SPInputId.Button7); //Options
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button8);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button9);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis6, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis6, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis7, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis7, AxleValueConsideration.Negative);
        }

    }
}
