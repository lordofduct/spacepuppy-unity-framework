using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.UserInput.Xbox
{
    public interface IXboxGamePad : IPlayerInputDevice
    {

        ButtonState GetButtonState(XboxButtons btn);

        float GetAxleState(XboxAxleInputs axis);

    }
}
