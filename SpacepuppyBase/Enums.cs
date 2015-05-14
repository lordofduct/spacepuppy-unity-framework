using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        CancelOnDeactivate = 0,
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
    [System.Flags()]
    public enum RadicalCoroutineEndCommand
    {
        None = 0,
        Cancel = 1,
        StallImmediateResume = 2
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

    /// <summary>
    /// An enum for describing the different parts of the update pipeline.
    /// </summary>
    public enum UpdateSequence
    {
        None = -1,
        Update = 0,
        FixedUpdate = 1,
        LateUpdate = 2
    }

    /// <summary>
    /// An enum for representing the different TimeSuppliers out there. Useful for 
    /// components that need to serialize which TimeSupplier to use.
    /// </summary>
    public enum DeltaTimeType
    {
        Normal = 0,
        Real = 1,
        Smooth = 2,
        Custom = 3
    }

    /// <summary>
    /// Used with the TypeReference class to define how the inspector should show the types in the drop down.
    /// </summary>
    public enum TypeDropDownListingStyle
    {
        Namespace = 0,
        Flat = 1,
        ComponentMenu = 2
    }

    /// <summary>
    /// Describe an axis in cartesian coordinates, Useful for components that need to serialize which axis to use in some fashion.
    /// </summary>
    public enum CartesianAxis
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    /// <summary>
    /// Enum used by InfoboxAttribute to define which message box type to display as.
    /// </summary>
    public enum InfoBoxMessageType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

}
