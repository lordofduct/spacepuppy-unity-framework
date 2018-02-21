using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    [InputProfileDescription("Steel Series Free", TargetPlatform.Linux, Description = "Steel Series Free (Linux)")]
    [InputProfileJoystickName("Zeemote: SteelSeries FREE")]
    public class SteelSeriesFreeLinuxProfile : XboxInputProfile
    {

        public SteelSeriesFreeLinuxProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis4, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis5);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis6);
            this.RegisterTrigger(XboxInputId.LTrigger, SPInputId.Axis10, AxleValueConsideration.Positive); //doesn't necessarily work on linux
            this.RegisterTrigger(XboxInputId.RTrigger, SPInputId.Axis10, AxleValueConsideration.Negative); //doesn't necessarily work on linux

            this.RegisterButton(XboxInputId.A, SPInputId.Button0);
            this.RegisterButton(XboxInputId.B, SPInputId.Button1);
            this.RegisterButton(XboxInputId.X, SPInputId.Button3);
            this.RegisterButton(XboxInputId.Y, SPInputId.Button4);
            this.RegisterButton(XboxInputId.LB, SPInputId.Button6);
            this.RegisterButton(XboxInputId.RB, SPInputId.Button7);
            this.RegisterButton(XboxInputId.Back, SPInputId.Button12);
            this.RegisterButton(XboxInputId.Start, SPInputId.Button11);
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button8); //doesn't necessarily work on linux
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button9); //doesn't necessarily work on linux
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis6, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis6, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis5, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis5, AxleValueConsideration.Negative);
        }

    }
}
