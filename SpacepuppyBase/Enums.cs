using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    [System.Flags()]
    public enum RadicalCoroutineDisableMode
    {
        /// <summary>
        /// The default action from Unity.
        /// </summary>
        Default = 0,
        CancelOnDeactivate = 0,
        CancelOnDisable = 1,
        StopOnDeactivate = 2,
        StopOnDisable = 4,
        ResumeOnEnable = 8,

        Pauses = 14
    }

    [System.Flags()]
    public enum RadicalCoroutineEndCommand
    {
        None = 0,
        Cancel = 1,
        StallImmediateResume = 2
    }

    public enum RadicalCoroutineOperatingState
    {
        Inactive = 0,
        Active = 1,
        Completing = 2,
        Complete = 3,
        Cancelling = -1,
        Cancelled = -2
    }

    public enum UpdateSequence
    {
        None = -1,
        Update = 0,
        FixedUpdate = 1,
        LateUpdate = 2
    }

    public enum DeltaTimeType
    {
        Normal = 0,
        Real = 1,
        Smooth = 2,
        Custom = 3
    }

    public enum TypeDropDownListingStyle
    {
        Namespace = 0,
        Flat = 1,
        ComponentMenu = 2
    }

    public enum CartesianAxis
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    public enum InfoBoxMessageType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

}
