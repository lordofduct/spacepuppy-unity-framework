using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Graphs
{
    public class NonMemberNodeException : ArgumentException
    {

        public NonMemberNodeException()
            : this("Node must be a member of the graph.")
        {

        }

        public NonMemberNodeException(string message)
            : base(message)
        {

        }

    }
}
