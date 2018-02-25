using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("Logitech WingMan Controller", TargetPlatform.Windows, Description = "Logitech WingMan Controller (Windows)")]
    [InputProfileJoystickName("WingMan Cordless Gamepad")]
    public class LogitechWingManWinProfile : XboxInputProfile
    {

        public LogitechWingManWinProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis4);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis5, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis6);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis7);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Button10);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Button11);


            this.RegisterButton(XboxInputId.A, SPInputId.Button2);
            this.RegisterButton(XboxInputId.B, SPInputId.Button3);
            this.RegisterButton(XboxInputId.X, SPInputId.Button4);
            this.RegisterButton(XboxInputId.Y, SPInputId.Button5);
            this.RegisterButton(XboxInputId.LB, SPInputId.Button7);
            this.RegisterButton(XboxInputId.RB, SPInputId.Button8);
            //this.RegisterButton(XboxInputId.Back, SPInputId.Button6);
            this.RegisterButton(XboxInputId.Start, SPInputId.Button9);
            //this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button8);
            //this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button9);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis6, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis6, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis7, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis7, AxleValueConsideration.Negative);
        }

    }

}
