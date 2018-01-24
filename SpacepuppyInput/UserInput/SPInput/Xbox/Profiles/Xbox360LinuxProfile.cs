using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.UserInput.SPInput.Xbox.Profiles
{

    public class Xbox360LinuxProfile : XboxInputProfile
    {

        public Xbox360LinuxProfile()
        {
            this.Register(XboxAxis.LeftStickX, SPInputAxis.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputAxis.Axis2);
            this.Register(XboxAxis.RightStickX, SPInputAxis.Axis4);
            this.Register(XboxAxis.RightStickY, SPInputAxis.Axis5);
            this.Register(XboxAxis.DPadX, SPInputAxis.Axis7);
            this.Register(XboxAxis.DPadY, SPInputAxis.Axis8);
            this.Register(XboxAxis.LeftTrigger, SPInputAxis.Axis3);
            this.Register(XboxAxis.RightTrigger, SPInputAxis.Axis6);

            this.Register(XboxButton.A, SPInputButton.Button0);
            this.Register(XboxButton.B, SPInputButton.Button1);
            this.Register(XboxButton.X, SPInputButton.Button2);
            this.Register(XboxButton.Y, SPInputButton.Button3);
            this.Register(XboxButton.LB, SPInputButton.Button4);
            this.Register(XboxButton.RB, SPInputButton.Button5);
            this.Register(XboxButton.Back, SPInputButton.Button6);
            this.Register(XboxButton.Start, SPInputButton.Button7);
            this.Register(XboxButton.LeftStick, SPInputButton.Button9);
            this.Register(XboxButton.RightStick, SPInputButton.Button10);
            this.Register(XboxButton.DPadUp, SPInputButton.Button13);
            this.Register(XboxButton.DPadDown, SPInputButton.Button14);
            this.Register(XboxButton.DPadRight, SPInputButton.Button15);
            this.Register(XboxButton.DPadLeft, SPInputButton.Button16);
        }

    }

}
