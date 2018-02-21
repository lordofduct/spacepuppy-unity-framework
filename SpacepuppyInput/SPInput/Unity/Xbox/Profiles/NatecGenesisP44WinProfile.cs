using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("Natec Genesis P44 Controller", TargetPlatform.Windows, Description = "Natec Genesis P44 Controller (Windows)")]
    [InputProfileJoystickName("EX10 GAMEPAD")]
    public class NatecGenesisP44WinProfile : XboxInputProfile
    {

        public NatecGenesisP44WinProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis4, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis5);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis6);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Button6);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Button7);


            this.RegisterButton(XboxInputId.A, SPInputId.Button1); //X
            this.RegisterButton(XboxInputId.B, SPInputId.Button2); //O
            this.RegisterButton(XboxInputId.X, SPInputId.Button0); //Sqr
            this.RegisterButton(XboxInputId.Y, SPInputId.Button3); //Tri
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5); //R1
            this.RegisterButton(XboxInputId.Back, SPInputId.Button8); //select
            this.RegisterButton(XboxInputId.Start, SPInputId.Button9); //start
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button10);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button11);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis6, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis6, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis5, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis5, AxleValueConsideration.Negative);
        }

    }

}
