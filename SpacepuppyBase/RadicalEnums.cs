using System;

namespace com.spacepuppy
{

    /// <summary>
    /// Flagging enum to define how a Coroutine should deal with its operating MonoBehaviour is disabled or deactivated.
    /// </summary>
    [System.Flags()]
    public enum RadicalCoroutineDisableMode
    {
        /// <summary>
        /// The default action from Unity. The routine cancels on deactivate, and plays through disable.
        /// </summary>
        Default = 0,
        /// <summary>
        /// The default action from Unity. The routine cancels on deactivate, and plays through disable.
        /// </summary>
        CancelOnDeactivate = 0,
        /// <summary>
        /// Cancels on disable, still cancels on Deactivate as well.
        /// </summary>
        CancelOnDisable = 1,
        StopOnDeactivate = 2,
        StopOnDisable = 4,
        ResumeOnEnable = 8,

        PausesOnDisable = 9,
        PausesOnDeactivate = 10,
        Pauses = 14
    }

    /// <summary>
    /// When ending a coroutine you can yield one of these to tell the RadicalCoroutine how to clean up the routine.
    /// </summary>
    public enum RadicalCoroutineEndCommand
    {
        Stop = 0,
        Cancel = 2
    }

    /// <summary>
    /// Represents the operating state of a RadicalCoroutine.
    /// </summary>
    public enum RadicalCoroutineOperatingState
    {
        Cancelled = -2,
        Cancelling = -1,
        Inactive = 0,
        Active = 1,
        Completing = 2,
        Complete = 3
    }

}
