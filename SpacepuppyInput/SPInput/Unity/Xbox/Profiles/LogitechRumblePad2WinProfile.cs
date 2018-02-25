using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{
    
    [InputProfileDescription("Logitech RumblePad 2 Controller", TargetPlatform.Windows, Description = "Logitech RumblePad 2 Controller (Windows)")]
    [InputProfileJoystickName("Logitech Rumblepad 2 USB")]
    public class LogitechRumblePad2WinProfile : XboxInputProfile
    {

        public LogitechRumblePad2WinProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis4, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis5);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis6, true);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Button6);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Button7);


            this.RegisterButton(XboxInputId.A, SPInputId.Button1); //2
            this.RegisterButton(XboxInputId.B, SPInputId.Button2); //3
            this.RegisterButton(XboxInputId.X, SPInputId.Button0); //1
            this.RegisterButton(XboxInputId.Y, SPInputId.Button3); //4
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4);
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5);
            this.RegisterButton(XboxInputId.Back, SPInputId.Button8);
            this.RegisterButton(XboxInputId.Start, SPInputId.Button9);
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button11);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button12);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis5, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis5, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis6, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis6, AxleValueConsideration.Positive);
        }

    }

}
