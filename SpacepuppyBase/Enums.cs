using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    [System.Flags()]
    public enum RadicalCoroutineEndCommand
    {
        None = 0,
        Cancel = 1,
        StallImmediateResume = 2
    }

    public enum UpdateSequence
    {
        None = 0,
        Update = 1,
        FixedUpdate = 2
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

}
