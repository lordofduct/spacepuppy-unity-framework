using System;
using System.Collections.Generic;

namespace com.spacepuppy.UserInput.SPInput
{

    public static class SPInputFactory
    {

        public static ButtonInputSignature CreateButtonSignature(string id, SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            return new ButtonInputSignature(id, SPInputDirect.GetButtonInputId(button, joystick));
        }
        
        public static AxleButtonInputSignature CreateAxleButtonSignature(string id, SPInputAxis axis, AxleButtonInputSignature.AxleValueConsideration consideration, SPJoystick joystick = SPJoystick.All)
        {
            return new AxleButtonInputSignature(id, SPInputDirect.GetAxisInputId(axis, joystick))
            {
                Consideration = consideration
            };
        }
        
        public static KeyboardButtonInputSignature CreateKeyCodeButtonSignature(string id, UnityEngine.KeyCode key)
        {
            return new KeyboardButtonInputSignature(id, key);
        }
        
        public static AxleInputSignature CreateAxisSignature(string id, SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            return new AxleInputSignature(id, SPInputDirect.GetAxisInputId(axis, joystick));
        }
        
        public static KeyboardAxleInputSignature CreateKeyCodeAxisSignature(string id, UnityEngine.KeyCode positiveKey, UnityEngine.KeyCode negativeKey)
        {
            return new KeyboardAxleInputSignature(id, positiveKey, negativeKey);
        }
        
        public static DualAxleInputSignature CreateDualAxisSignature(string id, SPInputAxis axisX, SPInputAxis axisY, SPJoystick joystick = SPJoystick.All)
        {
            return new DualAxleInputSignature(id, SPInputDirect.GetAxisInputId(axisX, joystick), SPInputDirect.GetAxisInputId(axisY, joystick));
        }

        public static DualAxleInputSignature CreateDualAxisSignature(string id, int hash, SPInputAxis axisX, SPInputAxis axisY, SPJoystick joystick = SPJoystick.All)
        {
            return new DualAxleInputSignature(id, SPInputDirect.GetAxisInputId(axisX, joystick), SPInputDirect.GetAxisInputId(axisY, joystick));
        }

        public static KeyboardDualAxleInputSignature CreateKeyCodeDualAxisSignature(string id, UnityEngine.KeyCode horizontalPositiveKey, UnityEngine.KeyCode horizontalNegativeKey, UnityEngine.KeyCode verticalPositiveKey, UnityEngine.KeyCode verticalNegativeKey)
        {
            return new KeyboardDualAxleInputSignature(id, horizontalPositiveKey, horizontalNegativeKey, verticalPositiveKey, verticalNegativeKey);
        }
        
    }

}
