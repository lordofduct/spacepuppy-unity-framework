#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.AI.BehaviourTree.Components
{
    public class a_SetVariable : AIActionComponent
    {

        #region Fields

        [SerializeField()]
        [AIVariableName()]
        private string _variable;
        [SerializeField()]
        private VariantReference _value;

        #endregion


        #region IAIAction Interface

        protected override ActionResult OnTick(IAIController ai)
        {
            if (string.IsNullOrEmpty(_variable)) return ActionResult.Failed;

            ai.Variables[_variable] = _value.Value;
            return ActionResult.Success;
        }

        #endregion

    }
}
