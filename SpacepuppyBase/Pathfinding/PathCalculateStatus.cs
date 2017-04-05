using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Pathfinding
{
    public enum PathCalculateStatus
    {
        Invalid = -1,
        Uncalculated = 0,
        Partial = 1,
        Success = 2,
    }
}
