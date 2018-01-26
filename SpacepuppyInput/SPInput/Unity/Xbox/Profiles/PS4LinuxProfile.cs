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
            this.Register(XboxAxis.LeftStickX, SPInputAxis.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputAxis.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputAxis.Axis4);
            this.Register(XboxAxis.RightStickY, SPInputAxis.Axis5, true);
            this.Register(XboxAxis.DPadX, SPInputAxis.Axis7);
            this.Register(XboxAxis.DPadY, SPInputAxis.Axis8, true);
            this.Register(XboxAxis.LeftTrigger, SPInputAxis.Axis3);
            this.Register(XboxAxis.RightTrigger, SPInputAxis.Axis6);

            this.Register(XboxButton.A, SPInputButton.Button0); //X
            this.Register(XboxButton.B, SPInputButton.Button1); //O
            this.Register(XboxButton.X, SPInputButton.Button2); //Sqr
            this.Register(XboxButton.Y, SPInputButton.Button3); //Tri
            this.Register(XboxButton.LB, SPInputButton.Button4); //L1
            this.Register(XboxButton.RB, SPInputButton.Button5); //R1
            this.Register(XboxButton.Back, SPInputButton.Button6); //Share
            this.Register(XboxButton.Start, SPInputButton.Button7); //Options
            this.Register(XboxButton.LeftStick, SPInputButton.Button9);
            this.Register(XboxButton.RightStick, SPInputButton.Button10);
            this.Register(XboxButton.DPadUp, SPInputAxis.Axis8, AxleValueConsideration.Negative);
            this.Register(XboxButton.DPadDown, SPInputAxis.Axis8, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadRight, SPInputAxis.Axis7, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadLeft, SPInputAxis.Axis7, AxleValueConsideration.Negative);
        }

    }

}
