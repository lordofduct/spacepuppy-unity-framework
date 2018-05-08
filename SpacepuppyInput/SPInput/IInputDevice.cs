using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;

namespace com.spacepuppy.SPInput
{

    public interface IInputDevice : IInputSignature
    {

        bool Active { get; set; }

        bool Contains(string id);

        IInputSignature GetSignature(string id);

        ButtonState GetButtonState(string id);
        
        float GetAxleState(string id);

        Vector2 GetDualAxleState(string id);

        Vector2 GetCursorState(string id);

    }

    public interface IInputSignatureCollection : ICollection<IInputSignature>
    {
        bool Contains(string id);
        IInputSignature GetSignature(string id);
        bool Remove(string id);
        void Sort();
    }

}
