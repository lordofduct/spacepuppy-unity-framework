using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput
{

    public interface IMappedInputDevice<T> : IInputDevice where T : struct, System.IConvertible
    {

        IInputSignature GetSignature(T id);

        ButtonState GetButtonState(T btn);
        float GetAxleState(T axis);
        Vector2 GetDualAxleState(T axis);
        Vector2 GetCursorState(T mapping);

    }
    
}
