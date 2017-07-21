#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.AI.BehaviourTree.Components
{
    public class a_IfCurrentStateIs : AITrapActionComponent
    {

        #region Fields

        [SerializeField()]
        private AIStateMachineComponent _machine;
        [SerializeField()]
        [OneOrMany()]
        private AIStateComponent[] _component;

        #endregion

        #region IAIAction Interface

        protected override ActionResult EvaluateTrap(IAIController ai)
        {
            if (_machine == null) return ActionResult.Failed;

            return (System.Array.IndexOf(_component, _machine.Current) >= 0) ? ActionResult.Success : ActionResult.Failed;
        }

        #endregion

    }
}
