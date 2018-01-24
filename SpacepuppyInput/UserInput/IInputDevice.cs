using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;

namespace com.spacepuppy.UserInput
{
    public interface IInputDevice : IInputSignature
    {
        bool Active { get; set; }

        ButtonState GetButtonState(string id);

        bool GetButtonPressed(string id, float duration);

        float GetAxleState(string id);

        Vector2 GetDualAxleState(string id);

        Vector2 GetCursorState(string id);

    }
}
