#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Components
{

    public abstract class SimpleAIStateComponent : SPComponent, IAIState
    {

        #region Fields
        
        [SerializeField()]
        private Trigger _onEnterState;

        [System.NonSerialized()]
        private bool _isActive;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected abstract void OnStateEntered(IAIStateMachine machine, IAIState lastState);

        protected abstract void OnStateExited(IAIStateMachine machine, IAIState nextState);

        protected abstract void Tick(IAIController ai);

        #endregion

        #region IAIState Interface

        string IAINode.DisplayName
        {
            get
            {
                if (Application.isPlaying)
                    return string.Format("[State {0}] ({1})", this.name, (this.IsActive) ? "active" : "inactive");
                else
                    return string.Format("[State {0}]", this.name);
            }
        }

        int IAIActionGroup.ActionCount { get { return 0; } }

        public bool IsActive { get { return _isActive; } }

        void IAIState.OnStateEntered(IAIStateMachine machine, IAIState lastState)
        {
            _isActive = true;
            this.OnStateEntered(machine, lastState);
            _onEnterState.ActivateTrigger(this, null);
        }

        void IAIState.OnStateExited(IAIStateMachine machine, IAIState nextState)
        {
            this.OnStateExited(machine, nextState);
            _isActive = false;
        }




        ActionResult IAINode.Tick(IAIController ai)
        {
            if(_isActive)
            {
                this.Tick(ai);
                return ActionResult.Waiting;
            }
            else
            {
                return ActionResult.None;
            }
        }

        void IAINode.Reset()
        {

        }

        public IEnumerator<IAIAction> GetEnumerator()
        {
            return Enumerable.Empty<IAIAction>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }
}
