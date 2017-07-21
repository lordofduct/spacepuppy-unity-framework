#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree.Components
{
    public class a_ChangeAIState : AIActionComponent
    {

        #region Fields

        [SerializeField()]
        [SelectableComponent(typeof(IAIStateMachine))]
        private Component _stateMachine;

        [SerializeField()]
        private AIStateComponent _state;

        [SerializeField()]
        [OneOrMany()]
        [SelectableComponent(typeof(IAIAction))]
        private Component[] _waitOn;

        #endregion

        #region Properties

        public IAIStateMachine StateMachine
        {
            get { return _stateMachine as IAIStateMachine; }
            set { _stateMachine = ComponentUtil.GetComponentFromSourceAsComponent<IAIStateMachine>(value) as Component; }
        }

        #endregion


        #region IAIAction Interface

        public override string DisplayName
        {
            get
            {
                return string.Format("{0} ({1})", base.DisplayName, (_state != null) ? _state.name : "null");
            }
        }

        protected override ActionResult OnTick(IAIController ai)
        {
            if (this.StateMachine == null) return ActionResult.Failed;

            if (_waitOn != null && _waitOn.Length > 0)
            {
                IAIAction a;
                for(int i = 0; i < _waitOn.Length; i++)
                {
                    a = _waitOn[i] as IAIAction;
                    if (a != null && a.ActionState == ActionResult.Waiting) return ActionResult.Waiting;
                }
            }

            if(_state == null)
            {
                this.StateMachine.ChangeState((IAIState)null);
                return ActionResult.Success;
            }
            else if (this.StateMachine.Contains(_state))
            {
                this.StateMachine.ChangeState(_state);
                return ActionResult.Success;
            }
            else
            {
                return ActionResult.Failed;
            }
        }

        #endregion

    }
}
