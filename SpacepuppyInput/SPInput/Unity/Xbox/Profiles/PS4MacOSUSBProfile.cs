using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    /// <summary>
    /// InputName: "Sony Computer Entertainment Wireless Controller"
    /// </summary>
    [InputProfileDescription("PS4 Controller", TargetPlatform.MacOSX, Description = "PS4 Controller USB (MacOSX)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS4)]
    public class PS4MacOSUSBProfile : XboxInputProfile
    {

        public PS4MacOSUSBProfile()
        {

            this.Register(XboxAxis.LeftStickX, SPInputAxis.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputAxis.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputAxis.Axis3);
            this.Register(XboxAxis.RightStickY, SPInputAxis.Axis4, true);
            this.Register(XboxAxis.DPadX, SPInputAxis.Axis7);
            this.Register(XboxAxis.DPadY, SPInputAxis.Axis8, true);
            /*
             * PS4 controllers on mac osx for some weird ass reason go from -1 to 1 (depressed to pressed)
             * Need to work out a way to deal with that.
             * In the mean time emulate the axis with the digital versions of these inputs
             */
            //this.Register(XboxAxis.LeftTrigger, SPInputAxis.Axis5);
            //this.Register(XboxAxis.RightTrigger, SPInputAxis.Axis6);
            this.Register(XboxAxis.LeftTrigger, SPInputButton.Button6, SPInputButton.Unknown);
            this.Register(XboxAxis.RightTrigger, SPInputButton.Button7, SPInputButton.Unknown);

            this.Register(XboxButton.A, SPInputButton.Button1); //X
            this.Register(XboxButton.B, SPInputButton.Button2); //O
            this.Register(XboxButton.X, SPInputButton.Button0); //Sqr
            this.Register(XboxButton.Y, SPInputButton.Button3); //Tri
            this.Register(XboxButton.LB, SPInputButton.Button4); //L1
            this.Register(XboxButton.RB, SPInputButton.Button5); //R1
            this.Register(XboxButton.Back, SPInputButton.Button8); //Share
            this.Register(XboxButton.Start, SPInputButton.Button9); //Options
            this.Register(XboxButton.LeftStick, SPInputButton.Button10);
            this.Register(XboxButton.RightStick, SPInputButton.Button11);
            this.Register(XboxButton.DPadUp, SPInputAxis.Axis8, AxleValueConsideration.Negative);
            this.Register(XboxButton.DPadDown, SPInputAxis.Axis8, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadRight, SPInputAxis.Axis7, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadLeft, SPInputAxis.Axis7, AxleValueConsideration.Negative);

        }

    }

}
