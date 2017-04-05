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

    /// <summary>
    /// When a state succeeds or fails, this will help determine if they state should repeat instead of moving on.
    /// </summary>
    public enum RepeatMode
    {
        Never = ActionResult.None,
        UntilSuccess = ActionResult.Success,
        UntilFailed = ActionResult.Failed,
        Forever = ActionResult.Waiting
    }

    public enum ActionGroupType
    {
        Sequential = 0, //performs each action in order until one fails
        Selector = 1, //performs each action in order until one succeeds
        Parrallel = 2, //performs all actions at the same time
        Random = 3, //performs a random action, can be modifiered with the IAIActionWeightSupplier
    }

    [System.Flags()]
    public enum ParallelPassOptions
    {
        FailOnAny = 1,
        SucceedOnAny = 2,
        SucceedOnTie = 4
    }

}
