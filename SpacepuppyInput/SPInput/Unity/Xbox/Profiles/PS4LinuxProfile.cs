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
            this.Register(XboxAxis.LeftStickX, SPInputId.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputId.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputId.Axis4);
            this.Register(XboxAxis.RightStickY, SPInputId.Axis5, true);
            this.Register(XboxAxis.DPadX, SPInputId.Axis7);
            this.Register(XboxAxis.DPadY, SPInputId.Axis8, true);
            this.Register(XboxAxis.LeftTrigger, SPInputId.Axis3);
            this.Register(XboxAxis.RightTrigger, SPInputId.Axis6);

            this.Register(XboxButton.A, SPInputId.Button0); //X
            this.Register(XboxButton.B, SPInputId.Button1); //O
            this.Register(XboxButton.X, SPInputId.Button2); //Sqr
            this.Register(XboxButton.Y, SPInputId.Button3); //Tri
            this.Register(XboxButton.LB, SPInputId.Button4); //L1
            this.Register(XboxButton.RB, SPInputId.Button5); //R1
            this.Register(XboxButton.Back, SPInputId.Button6); //Share
            this.Register(XboxButton.Start, SPInputId.Button7); //Options
            this.Register(XboxButton.LeftStick, SPInputId.Button9);
            this.Register(XboxButton.RightStick, SPInputId.Button10);
            this.Register(XboxButton.DPadUp, SPInputId.Axis8, AxleValueConsideration.Negative);
            this.Register(XboxButton.DPadDown, SPInputId.Axis8, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadRight, SPInputId.Axis7, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadLeft, SPInputId.Axis7, AxleValueConsideration.Negative);
        }

    }

}
