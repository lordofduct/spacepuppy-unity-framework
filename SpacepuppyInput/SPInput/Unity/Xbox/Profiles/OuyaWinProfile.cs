using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("Ouya Controller", TargetPlatform.Windows, Description = "Ouya Controller (Windows)")]
    [InputProfileJoystickName("OUYA Game Controller")]
    public class OuyaWinProfile : XboxInputProfile
    {

        public OuyaWinProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis4);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis5, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Button11, SPInputId.Button10);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Button8, SPInputId.Button9);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Axis3);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Axis6);


            this.RegisterButton(XboxInputId.A, SPInputId.Button0); //O
            this.RegisterButton(XboxInputId.B, SPInputId.Button3); //A
            this.RegisterButton(XboxInputId.X, SPInputId.Button1); //U
            this.RegisterButton(XboxInputId.Y, SPInputId.Button2); //Y
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5); //R1
            //this.RegisterButton(XboxInputId.Back, SPInputId.Button8); //none
            this.RegisterButton(XboxInputId.Start, SPInputId.MouseButton0); //touchpad tap
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button6);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button7);
            this.RegisterButton(XboxInputId.DPadUp, SPInputId.Button8);
            this.RegisterButton(XboxInputId.DPadDown, SPInputId.Button9);
            this.RegisterButton(XboxInputId.DPadRight, SPInputId.Button11);
            this.RegisterButton(XboxInputId.DPadLeft, SPInputId.Button10);
        }

    }

}
