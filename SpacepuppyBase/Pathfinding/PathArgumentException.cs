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

        public PathArgumentException(string message)
            : base(message)
        {

        }

        public PathArgumentException(string message, string paramName)
            : base(message, paramName)
        {

        }

        public PathArgumentException(string message, string paramName, System.Exception innerException)
            : base(message, paramName, innerException)
        {

        }

    }
}
