using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    /// <summary>
    /// InputName: "PS4 Controller"
    /// InputName (per InControl): "Sony Computer Entertainment Wireless Controller"
    /// </summary>
    [InputProfileDescription("PS4 Controller", TargetPlatform.Linux, Description = "PS4 Controller (Linux)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS4)]
    public class PS4LinuxProfile : XboxInputProfile
    {

        public PS4LinuxProfile()
        {
            this.RegisterAxis(XboxInputId.LStickX, SPInputId.Axis1);
            this.RegisterAxis(XboxInputId.LStickY, SPInputId.Axis2, true);
            this.RegisterAxis(XboxInputId.RStickX, SPInputId.Axis4);
            this.RegisterAxis(XboxInputId.RStickY, SPInputId.Axis5, true);
            this.RegisterAxis(XboxInputId.DPadX, SPInputId.Axis7);
            this.RegisterAxis(XboxInputId.DPadY, SPInputId.Axis8, true);
            this.RegisterAxis(XboxInputId.LTrigger, SPInputId.Axis3);
            this.RegisterAxis(XboxInputId.RTrigger, SPInputId.Axis6);

            this.RegisterButton(XboxInputId.A, SPInputId.Button0); //X
            this.RegisterButton(XboxInputId.B, SPInputId.Button1); //O
            this.RegisterButton(XboxInputId.X, SPInputId.Button2); //Sqr
            this.RegisterButton(XboxInputId.Y, SPInputId.Button3); //Tri
            this.RegisterButton(XboxInputId.LB, SPInputId.Button4); //L1
            this.RegisterButton(XboxInputId.RB, SPInputId.Button5); //R1
            this.RegisterButton(XboxInputId.Back, SPInputId.Button6); //Share
            this.RegisterButton(XboxInputId.Start, SPInputId.Button7); //Options
            this.RegisterButton(XboxInputId.LStickPress, SPInputId.Button9);
            this.RegisterButton(XboxInputId.RStickPress, SPInputId.Button10);
            this.RegisterAxleButton(XboxInputId.DPadUp, SPInputId.Axis8, AxleValueConsideration.Negative);
            this.RegisterAxleButton(XboxInputId.DPadDown, SPInputId.Axis8, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadRight, SPInputId.Axis7, AxleValueConsideration.Positive);
            this.RegisterAxleButton(XboxInputId.DPadLeft, SPInputId.Axis7, AxleValueConsideration.Negative);
        }

    }

}
