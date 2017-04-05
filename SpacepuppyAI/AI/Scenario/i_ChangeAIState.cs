#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Components;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Scenario
{
    public class i_ChangeAIState : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TypeRestriction(typeof(IAIStateMachine))]
        private UnityEngine.Object _stateMachine;

        [SerializeField()]
        [TypeRestriction(typeof(IAIState))]
        private UnityEngine.Object _state;

        #endregion

        #region Properties

        public IAIStateMachine StateMachine
        {
            get { return _stateMachine as IAIStateMachine; }
        }

        public IAIState State
        {
            get { return _state as IAIState; }
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _stateMachine != null;
            }
        }

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            this.StateMachine.ChangeState(this.State);
            return true;
        }

        #endregion

    }
}
