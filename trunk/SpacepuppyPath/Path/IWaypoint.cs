using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Path
{

    public interface IWaypoint
    {

        Vector3 Position { get; set; }
        Vector3 Heading { get; set; }
        float Strength { get; set; }

    }

}
