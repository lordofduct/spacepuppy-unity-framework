using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.UserInput
{

    public enum ButtonState : sbyte
    {
        None = 0,
        Down = 1,
        Held = 2,
        Released = -1
    }

}