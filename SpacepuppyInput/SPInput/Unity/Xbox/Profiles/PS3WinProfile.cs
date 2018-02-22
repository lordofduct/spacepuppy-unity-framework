using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("PS3 Controller", TargetPlatform.Windows, Description = "PS3 Controller (Windows)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS3)]
    [InputProfileJoystickName("MotioninJoy Virtual Game Controller")]
    public class PS3WinProfile : XboxInputProfile
    {

        public PS3WinProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis6, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis9);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis10);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Button6);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Button7);


            this.RegisterButton(XboxInputId.A, SPInputId.Button2); //X
            this.RegisterButton(XboxInputId.B, SPInputId.Button1); //O
            this.RegisterButton(XboxInputId.X, SPInputId.Button3); //Sqr
            this.RegisterButton(XboxInputId.Y, SPInputId.Button0); //Tri
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5); //R1
            this.RegisterButton(XboxInputId.Back, SPInputId.Button8); //Share
            this.RegisterButton(XboxInputId.Start, SPInputId.Button11); //Options
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button9);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button10);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis9, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis9, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis10, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis10, AxleValueConsideration.Negative);
        }

    }

}
