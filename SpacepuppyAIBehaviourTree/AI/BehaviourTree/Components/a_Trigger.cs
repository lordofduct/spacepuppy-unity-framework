using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;

namespace com.spacepuppy.AI.BehaviourTree.Components
{
    public class a_Trigger : AIActionComponent
    {

        #region Fields

        [SerializeField()]
        private Trigger _trigger = new Trigger(true);

        #endregion

        #region IAIAction Interface

        protected override ActionResult OnTick(IAIController ai)
        {
            _trigger.ActivateTrigger(this, null);
            return ActionResult.Success;
        }

        #endregion

    }
}
