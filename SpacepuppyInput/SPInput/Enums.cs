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

    /// <summary>
    /// Represents the current state of a button.
    /// </summary>
    public enum ButtonState : sbyte
    {
        None = 0,
        Down = 1,
        Held = 2,
        Released = -1
    }
    
    /// <summary>
    /// Represents a button press over some duration of time. Returned by 'GetButtonPress'.
    /// </summary>
    public enum ButtonPress
    {
        /// <summary>
        /// Button was pressed and released after the defined time.
        /// </summary>
        Released = -2,
        /// <summary>
        /// Button was pressed and released before the defined time.
        /// </summary>
        Tapped = -1,
        /// <summary>
        /// Button is not active.
        /// </summary>
        None = 0,
        /// <summary>
        /// Button just became active.
        /// </summary>
        Down = 1,
        /// <summary>
        /// Button is down but the duration has yet to pass.
        /// </summary>
        Holding = 2,
        /// <summary>
        /// Button is down and the duration has passed.
        /// </summary>
        Held = 3
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
