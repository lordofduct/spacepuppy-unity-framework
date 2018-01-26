using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox.Profiles
{

    /// <summary>
    /// InputName: "Wireless Controller"
    /// </summary>
    [InputProfileDescription("PS4 Controller", TargetPlatform.Windows, Description = "PS4 Controller (Windows)")]
    [InputProfileJoystickName(XboxInputProfile.GENERIC_PS4)]
    public class PS4WinProfile : XboxInputProfile
    {

        public PS4WinProfile()
        {
            this.Register(XboxAxis.LeftStickX, SPInputAxis.Axis1);
            this.Register(XboxAxis.LeftStickY, SPInputAxis.Axis2, true);
            this.Register(XboxAxis.RightStickX, SPInputAxis.Axis3);
            this.Register(XboxAxis.RightStickY, SPInputAxis.Axis6);
            this.Register(XboxAxis.DPadX, SPInputAxis.Axis7);
            this.Register(XboxAxis.DPadY, SPInputAxis.Axis8);
            this.Register(XboxAxis.LeftTrigger, SPInputFactory.CreateAxisDelegateFactory_PS4TriggerLike(SPInputAxis.Axis4));
            this.Register(XboxAxis.RightTrigger, SPInputFactory.CreateAxisDelegateFactory_PS4TriggerLike(SPInputAxis.Axis5));
            //this.Register(XboxAxis.LeftTrigger, SPInputButton.Button6, SPInputButton.Unknown);
            //this.Register(XboxAxis.RightTrigger, SPInputButton.Button7, SPInputButton.Unknown);


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
            this.Register(XboxButton.DPadUp, SPInputAxis.Axis8, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadDown, SPInputAxis.Axis8, AxleValueConsideration.Negative);
            this.Register(XboxButton.DPadRight, SPInputAxis.Axis7, AxleValueConsideration.Positive);
            this.Register(XboxButton.DPadLeft, SPInputAxis.Axis7, AxleValueConsideration.Negative);
        }

    }

}
