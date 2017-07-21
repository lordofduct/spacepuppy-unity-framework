using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.AI
{
    public enum ActionResult
    {
        None = 0,
        Waiting = 1,
        Success = 2,
        Failed = 3
    }
    
    public enum AIStateMachineSourceMode
    {
        SelfSourced = 0,
        ChildSourced = 1
    }


}
