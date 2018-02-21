using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("PS3 Controller", TargetPlatform.Linux, Description = "PS3 Controller (Linux)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS3)]
    [InputProfileJoystickName("Sony PLAYSTATION(R)3 Controller")]
    [InputProfileJoystickName("SHENGHIC 2009/0708ZXW-V1Inc. PLAYSTATION(R)3Conteroller")]
    public class PS3LinuxProfile : XboxInputProfile
    {

        public PS3LinuxProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis4, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Button5, SPInputId.Button7);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Button4, SPInputId.Button6);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Button8);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Button9);


            this.RegisterButton(XboxInputId.A, SPInputId.Button14); //X
            this.RegisterButton(XboxInputId.B, SPInputId.Button13); //O
            this.RegisterButton(XboxInputId.X, SPInputId.Button15); //Sqr
            this.RegisterButton(XboxInputId.Y, SPInputId.Button12); //Tri
            this.RegisterButton(XboxInputId.LB, SPInputId.Button10); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button11); //R1
            this.RegisterButton(XboxInputId.Back, SPInputId.Button0); //Share - no idea why, but sources say that select/start are the same button id
            this.RegisterButton(XboxInputId.Start, SPInputId.Button3); //Options
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button1);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button2);
            this.RegisterButton(XboxInputId.DPadUp, SPInputId.Button4);
            this.RegisterButton(XboxInputId.DPadDown, SPInputId.Button6);
            this.RegisterButton(XboxInputId.DPadRight, SPInputId.Button5);
            this.RegisterButton(XboxInputId.DPadLeft, SPInputId.Button7);
        }

    }

}
