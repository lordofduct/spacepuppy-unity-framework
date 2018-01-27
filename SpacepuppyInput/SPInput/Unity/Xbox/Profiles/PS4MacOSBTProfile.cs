using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    /// <summary>
    /// InputName: "Unknown Wireless Controller"
    /// </summary>
    [InputProfileDescription("PS4 Controller", TargetPlatform.MacOSX, Description = "PS4 Controller Bluetooth (MacOSX)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS4)]
    public class PS4MacOSBTProfile : XboxInputProfile
    {

        public PS4MacOSBTProfile()
        {
            this.Register(XboxAxis.LeftStickX, SPInputId.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputId.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputId.Axis3);
            this.Register(XboxAxis.RightStickY, SPInputId.Axis4, true);
            this.Register(XboxAxis.DPadX, SPInputId.Axis7);
            this.Register(XboxAxis.DPadY, SPInputId.Axis8, true);
            this.Register(XboxAxis.LeftTrigger, SPInputFactory.CreateAxisDelegateFactory_PS4TriggerLike(SPInputId.Axis5));
            this.Register(XboxAxis.RightTrigger, SPInputFactory.CreateAxisDelegateFactory_PS4TriggerLike(SPInputId.Axis6));
            //this.Register(XboxAxis.LeftTrigger, SPInputId.Button6, SPInputId.Unknown);
            //this.Register(XboxAxis.RightTrigger, SPInputId.Button7, SPInputId.Unknown);

            this.Register(XboxButton.A, SPInputId.Button1); //X
            this.Register(XboxButton.B, SPInputId.Button2); //O
            this.Register(XboxButton.X, SPInputId.Button0); //Sqr
            this.Register(XboxButton.Y, SPInputId.Button3); //Tri
            this.Register(XboxButton.LB, SPInputId.Button4); //L1
            this.Register(XboxButton.RB, SPInputId.Button5); //R1
            this.Register(XboxButton.Back, SPInputId.Button8); //Share
            this.Register(XboxButton.Start, SPInputId.Button9); //Options
            this.Register(XboxButton.LeftStick, SPInputId.Button10);
            this.Register(XboxButton.RightStick, SPInputId.Button11);
            this.Register(XboxButton.DPadUp, SPInputId.Axis8, AxleValueConsideration.Negative);
            this.Register(XboxButton.DPadDown, SPInputId.Axis8, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadRight, SPInputId.Axis7, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadLeft, SPInputId.Axis7, AxleValueConsideration.Negative);
        }

    }
}
