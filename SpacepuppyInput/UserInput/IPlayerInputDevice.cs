using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;

namespace com.spacepuppy.UserInput
{
    public interface IPlayerInputDevice : IInputSignature
    {
        bool Active { get; set; }

        ButtonState GetCurrentButtonState(string id);
        ButtonState GetCurrentButtonState(int hash);

        float GetCurrentAxleState(string id);
        float GetCurrentAxleState(int hash);

        Vector2 GetCurrentDualAxleState(string id);
        Vector2 GetCurrentDualAxleState(int hash);

        Vector2 GetCurrentCursorState(string id);
        Vector2 GetCurrentCursorState(int hash);

    }
}
