using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput
{

    public interface IMappedInputDevice<T> : IInputDevice where T : struct, System.IConvertible
    {

        ButtonState GetButtonState(T btn);
        bool GetButtonPressed(T btn, float duration);
        float GetAxleState(T axis);
        Vector2 GetDualAxleState(T axis);
        Vector2 GetCursorState(T mapping);

    }

}
