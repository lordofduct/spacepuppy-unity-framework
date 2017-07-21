using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.AI
{

    public interface IAIState : IAINode
    {

        bool IsActive { get; }
        IAIStateMachine StateMachine { get; }

        /// <summary>
        /// Called anytime a state machine syncs and locates its states. This can be called multiple times if the machine is re-synced. Use it only to get a reference to the state machine.
        /// </summary>
        /// <param name="machine"></param>
        void Init(IAIStateMachine machine);

        void OnStateEntered(IAIStateMachine machine, IAIState lastState);

        void OnStateExited(IAIStateMachine machine, IAIState nextState);

    }

}
