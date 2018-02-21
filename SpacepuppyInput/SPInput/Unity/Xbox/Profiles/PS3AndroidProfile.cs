using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("PS3 Controller", TargetPlatform.Android, Description = "PS3 Controller (Android)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS3)]
    [InputProfileJoystickName("PLAYSTATION(R)3 Controller")]
    [InputProfileJoystickName("SHENGHIC 2009/0708ZXW-V1Inc. PLAYSTATION(R)3Conteroller")]
    [InputProfileJoystickName("Sony PLAYSTATION(R)3 Controller")]
    public class PS3AndroidProfile : XboxInputProfile
    {

        public PS3AndroidProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis4, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis5);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis6, true);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Button7);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Button8);


            this.RegisterButton(XboxInputId.A, SPInputId.Button2); //X
            this.RegisterButton(XboxInputId.B, SPInputId.Button3); //O
            this.RegisterButton(XboxInputId.X, SPInputId.Button0); //Sqr
            this.RegisterButton(XboxInputId.Y, SPInputId.Button1); //Tri
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5); //R1
            //this.RegisterButton(XboxInputId.Back, SPInputId.Button8); //Share
            this.RegisterButton(XboxInputId.Start, SPInputId.Button10); //Options
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button8);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button9);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis10, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis10, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis9, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis9, AxleValueConsideration.Negative);
        }

    }

}
