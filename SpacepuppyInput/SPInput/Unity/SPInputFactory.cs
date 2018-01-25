using System;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput.Unity
{

    public static class SPInputFactory
    {

        public static ButtonInputSignature CreateButtonSignature(string id, SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            return new ButtonInputSignature(id, SPInputDirect.GetButtonInputId(button, joystick));
        }
        
        public static AxleButtonInputSignature CreateAxleButtonSignature(string id, SPInputAxis axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, SPJoystick joystick = SPJoystick.All, float axleButtonDeadZone = AxleButtonInputSignature.DEFAULT_BTNDEADZONE)
        {
            return new AxleButtonInputSignature(id, SPInputDirect.GetAxisInputId(axis, joystick), consideration, axleButtonDeadZone);
        }
        
        public static KeyboardButtonInputSignature CreateKeyCodeButtonSignature(string id, UnityEngine.KeyCode key)
        {
            return new KeyboardButtonInputSignature(id, key);
        }
        
        public static AxleInputSignature CreateAxisSignature(string id, SPInputAxis axis, SPJoystick joystick = SPJoystick.All, bool invert = false)
        {
            return new AxleInputSignature(id, SPInputDirect.GetAxisInputId(axis, joystick))
            {
                Invert = invert
            };
        }
        
        public static KeyboardAxleInputSignature CreateKeyCodeAxisSignature(string id, UnityEngine.KeyCode positiveKey, UnityEngine.KeyCode negativeKey)
        {
            return new KeyboardAxleInputSignature(id, positiveKey, negativeKey);
        }
        
        public static DualAxleInputSignature CreateDualAxisSignature(string id, SPInputAxis axisX, SPInputAxis axisY, SPJoystick joystick = SPJoystick.All, bool invertX = false, bool invertY = false)
        {
            return new DualAxleInputSignature(id, SPInputDirect.GetAxisInputId(axisX, joystick), SPInputDirect.GetAxisInputId(axisY, joystick))
            {
                InvertX = invertX,
                InvertY = invertY
            };
        }
        
        public static KeyboardDualAxleInputSignature CreateKeyCodeDualAxisSignature(string id, UnityEngine.KeyCode horizontalPositiveKey, UnityEngine.KeyCode horizontalNegativeKey, UnityEngine.KeyCode verticalPositiveKey, UnityEngine.KeyCode verticalNegativeKey)
        {
            return new KeyboardDualAxleInputSignature(id, horizontalPositiveKey, horizontalNegativeKey, verticalPositiveKey, verticalNegativeKey);
        }
        
        public static EmulatedAxleInputSignature CreateEmulatedAxixSignature(string id, SPInputButton positive, SPInputButton negative, SPJoystick joystick = SPJoystick.All)
        {
            return new EmulatedAxleInputSignature(id, SPInputDirect.GetButtonInputId(positive, joystick), SPInputDirect.GetButtonInputId(negative, joystick));
        }

        public static EmulatedDualAxleInputSignature CreateEmulatedDualAxixSignature(string id, SPInputButton positiveX, SPInputButton negativeX, SPInputButton positiveY, SPInputButton negativeY, SPJoystick joystick = SPJoystick.All)
        {
            return new EmulatedDualAxleInputSignature(id, SPInputDirect.GetButtonInputId(positiveX, joystick), SPInputDirect.GetButtonInputId(negativeX, joystick), SPInputDirect.GetButtonInputId(positiveY, joystick), SPInputDirect.GetButtonInputId(negativeY, joystick));
        }

    }

}
