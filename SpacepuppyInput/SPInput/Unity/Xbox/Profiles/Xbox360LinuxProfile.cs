using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    /// <summary>
    /// InputName: "XInput Controller"
    /// InputName (per InControl): "Microsoft X-Box 360 pad", "Generic X-Box pad"
    /// </summary>
    [InputProfileDescription("Xbox 360 Controller", TargetPlatform.Linux, Description = "Xbox 360 Controller (Linux)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_XBOX360)]
    public class Xbox360LinuxProfile : XboxInputProfile
    {

        public Xbox360LinuxProfile()
        {
            this.Register(XboxAxis.LeftStickX, SPInputId.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputId.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputId.Axis4);
            this.Register(XboxAxis.RightStickY, SPInputId.Axis5);
            this.Register(XboxAxis.DPadX, SPInputId.Axis7);
            this.Register(XboxAxis.DPadY, SPInputId.Axis8);
            this.Register(XboxAxis.LeftTrigger, SPInputId.Axis3);
            this.Register(XboxAxis.RightTrigger, SPInputId.Axis6);

            this.Register(XboxButton.A, SPInputId.Button0);
            this.Register(XboxButton.B, SPInputId.Button1);
            this.Register(XboxButton.X, SPInputId.Button2);
            this.Register(XboxButton.Y, SPInputId.Button3);
            this.Register(XboxButton.LB, SPInputId.Button4);
            this.Register(XboxButton.RB, SPInputId.Button5);
            this.Register(XboxButton.Back, SPInputId.Button6);
            this.Register(XboxButton.Start, SPInputId.Button7);
            this.Register(XboxButton.LeftStick, SPInputId.Button9);
            this.Register(XboxButton.RightStick, SPInputId.Button10);
            this.Register(XboxButton.DPadUp, SPInputId.Button13);
            this.Register(XboxButton.DPadDown, SPInputId.Button14);
            this.Register(XboxButton.DPadRight, SPInputId.Button15);
            this.Register(XboxButton.DPadLeft, SPInputId.Button16);
        }

    }

}
