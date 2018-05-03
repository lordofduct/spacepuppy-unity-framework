#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;

using com.spacepuppy.StateMachine;

namespace com.spacepuppy.Scenario
{
    public class i_ChangeStateOfSimpleStateMachine : TriggerableMechanism
    {

        [SerializeField]
        private string _state;
        [SerializeField]
        [TriggerableTargetObject.Config(typeof(IStateMachine<string>))]
        private TriggerableTargetObject _stateMachine;

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            if (_stateMachine == null) return false;

            //var machine = _stateMachine as IStateMachine<string>;
            var machine = _stateMachine.GetTarget<IStateMachine<string>>(arg);
            if (machine == null) return false;

            return machine.ChangeState(_state) != null;
        }
    }
}
