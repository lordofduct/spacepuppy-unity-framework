using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;

namespace com.spacepuppy.UserInput
{
    public interface IInputDevice : IInputSignature
    {
        bool Active { get; set; }

        ButtonState GetButtonState(string id);
        ButtonState GetButtonState(int hash);

        bool GetButtonPressed(string id, float duration);
        bool GetButtonPressed(int hash, float duration);

        float GetAxleState(string id);
        float GetAxleState(int hash);

        Vector2 GetDualAxleState(string id);
        Vector2 GetDualAxleState(int hash);

        Vector2 GetCursorState(string id);
        Vector2 GetCursorState(int hash);

    }
}
