using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("Nvidia Shield Controller", TargetPlatform.Windows, Description = "Nvidia Shield Controller (Windows)")]
    [InputProfileJoystickName("NVIDIA Controller")]
    public class NvidiaShieldWinProfile : XboxInputProfile
    {

        public NvidiaShieldWinProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis4, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis5);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis6);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Axis7);
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Axis8);

            this.RegisterButton(XboxInputId.A, SPInputId.Button9);
            this.RegisterButton(XboxInputId.B, SPInputId.Button8);
            this.RegisterButton(XboxInputId.X, SPInputId.Button7);
            this.RegisterButton(XboxInputId.Y, SPInputId.Button6);
            this.RegisterButton(XboxInputId.LB, SPInputId.Button5);
            this.RegisterButton(XboxInputId.RB, SPInputId.Button4);
            this.RegisterButton(XboxInputId.Back, SPInputId.Button1);
            this.RegisterButton(XboxInputId.Start, SPInputId.Button0);
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button3);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button2);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis5, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis5, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis6, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis6, AxleValueConsideration.Negative);
        }

    }

}
