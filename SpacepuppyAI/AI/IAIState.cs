using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.AI
{

    public interface IAIState : IAIActionGroup
    {

        bool IsActive { get; }


        void OnStateEntered(IAIStateMachine machine, IAIState lastState);

        void OnStateExited(IAIStateMachine machine, IAIState nextState);

    }

}
