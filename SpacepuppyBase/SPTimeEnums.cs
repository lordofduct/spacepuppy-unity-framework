using System;

namespace com.spacepuppy
{

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

    public enum TimeUnits
    {
        Seconds = 0,
        Minutes = 1,
        Hours = 2,
        Days = 3,
        Years = 4
    }

}
