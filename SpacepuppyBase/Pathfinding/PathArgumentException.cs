using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Pathfinding
{
    public class PathArgumentException : ArgumentException
    {

        public PathArgumentException()
            : base("IPath is not supported by this PathSeeker.")
        {

        }

    }
}
