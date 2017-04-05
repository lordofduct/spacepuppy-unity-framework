using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.UserInput.Xbox
{
    public struct XboxGamePadState
    {

        public float LeftX;
        public float LeftY;
        public float RightX;
        public float RightY;
        public float LeftTrigger;
        public float RightTrigger;

        public ButtonState DPadUp;
        public ButtonState DPadRight;
        public ButtonState DPadDown;
        public ButtonState DPadLeft;

        public ButtonState A;
        public ButtonState B;
        public ButtonState X;
        public ButtonState Y;
        public ButtonState LB;
        public ButtonState RB;
        public ButtonState Start;
        public ButtonState Back;
        public ButtonState LeftStickClick;
        public ButtonState RightStickClick;


        public ButtonState GetButtonState(XboxButtons btn)
        {
            switch (btn)
            {
                case XboxButtons.DPadUp:
                    return DPadUp;
                case XboxButtons.DPadRight:
                    return DPadRight;
                case XboxButtons.DPadDown:
                    return DPadDown;
                case XboxButtons.DPadLeft:
                    return DPadLeft;
                case XboxButtons.A:
                    return A;
                case XboxButtons.B:
                    return B;
                case XboxButtons.X:
                    return X;
                case XboxButtons.Y:
                    return Y;
                case XboxButtons.LB:
                    return LB;
                case XboxButtons.RB:
                    return RB;
                case XboxButtons.Start:
                    return Start;
                case XboxButtons.Back:
                    return Back;
                case XboxButtons.LeftStick:
                    return LeftStickClick;
                case XboxButtons.RightStick:
                    return RightStickClick;
            }

            return ButtonState.None;
        }

        public float GetAxleState(XboxAxleInputs axis)
        {
            switch (axis)
            {
                case XboxAxleInputs.LeftStickX:
                    return LeftX;
                case XboxAxleInputs.LeftStickY:
                    return LeftY;
                case XboxAxleInputs.LeftTrigger:
                    return LeftTrigger;
                case XboxAxleInputs.RightStickX:
                    return RightX;
                case XboxAxleInputs.RightStickY:
                    return RightY;
                case XboxAxleInputs.RightTrigger:
                    return RightTrigger;
            }

            return 0f;
        }


    }
}
