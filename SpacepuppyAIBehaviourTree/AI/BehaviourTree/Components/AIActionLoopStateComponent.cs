using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.StateMachine;

namespace com.spacepuppy.AI.Components
{
    public class AIActionLoopStateComponent : SPComponent, IAIState
    {

        #region Events

        public event System.EventHandler StateEntered;
        public event System.EventHandler StateExited;

        #endregion

        public enum LoopMode
        {
            WeightedRandom = 0,
            Sequential = 1,
        }

        #region Fields

        [SerializeField()]
        private LoopMode _loopMode;

        [System.NonSerialized()]
        private AIController _controller;

        #endregion

        #region CONSTRUCTOR



        #endregion

        #region Methods

        protected virtual void Init()
        {

        }

        protected virtual void OnStateEntered(IAIState lastState)
        {

        }

        protected virtual void OnStateExited(IAIState nextState)
        {

        }

        #endregion

        #region IAIActionLoopState Interface

        public AIController AI
        {
            get { return _controller; }
        }

        void IAIState.Init(AIController controller)
        {
            _controller = controller;
            this.Init();
        }

        void IAIState.OnStateEntered(IAIState lastState)
        {
            this.OnStateEntered(lastState);
            if (this.StateEntered != null) this.StateEntered(this, System.EventArgs.Empty);
        }

        void IAIState.OnStateExited(IAIState nextState)
        {
            this.OnStateExited(nextState);
            if (this.StateExited != null) this.StateExited(this, System.EventArgs.Empty);
        }

        #endregion

    }
}
