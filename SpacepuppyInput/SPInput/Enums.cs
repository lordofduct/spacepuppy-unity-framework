using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput
{
    
    public enum Joystick : sbyte
    {
        None = -1,
        All = 0,
        Joy1 = 1,
        Joy2 = 2,
        Joy3 = 3,
        Joy4 = 4,
        Joy5 = 5,
        Joy6 = 6,
        Joy7 = 7,
        Joy8 = 8,
        Joy9 = 9,
        Joy10 = 10,
        Joy11 = 11
    }

    public enum AxleValueConsideration : byte
    {
        Positive = 0,
        Negative = 1,
        Absolute = 2
    }

    public enum ButtonState : sbyte
    {
        None = 0,
        Down = 1,
        Held = 2,
        Released = -1
    }

    public enum DeadZoneCutoff
    {
        Scaled = 0,
        Shear = 1
    }

    public enum MergedAxlePrecedence
    {

        FirstActive = 0,
        LastActive = 1,
        Largest = 2,
        Smallest = 3

    }

}
