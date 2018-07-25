using System;

namespace com.spacepuppy
{

    /// <summary>
    /// Flagging enum to define how a Coroutine should deal with its operating MonoBehaviour is disabled or deactivated.
    /// </summary>
    [System.Flags()]
    public enum RadicalCoroutineDisableMode
    {
        PlayUntilDestroyed = 0,
        /// <summary>
        /// The default action from Unity. The routine cancels on deactivate, and plays through disable.
        /// </summary>
        Default = 1,
        /// <summary>
        /// The default action from Unity. The routine cancels on deactivate, and plays through disable.
        /// </summary>
        CancelOnDeactivate = 1,
        /// <summary>
        /// Cancels on disable, still cancels on Deactivate as well.
        /// </summary>
        CancelOnDisable = 2,
        StopOnDeactivate = 4,
        StopOnDisable = 8,
        Stops = 12,
        Resumes = 16,

        PausesOnDeactivate = 20,
        PausesOnDisable = 24,
        Pauses = 28
    }
    
    /// <summary>
    /// Represents the operating state of a RadicalCoroutine.
    /// </summary>
    public enum RadicalCoroutineOperatingState
    {
        Cancelled = -2,
        Cancelling = -1,
        Inactive = 0,
        Paused = 1,
        Active = 2,
        Completing = 3,
        Complete = 4
    }

}
