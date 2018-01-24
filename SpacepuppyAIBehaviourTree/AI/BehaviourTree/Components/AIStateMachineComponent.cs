using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree.Components
{
    public class AIStateMachineComponent : SPComponent, IAITreeStateMachine, IAIEditorSyncActionsCallbackReceiver
    {

        #region Fields

        [SerializeField()]
        private RepeatMode _repeat;
        [SerializeField()]
        private bool _alwaysSucceed;

        [SerializeField()]
        [ListAIStates()]
        private Component _defaultState;

        [System.NonSerialized()]
        private AITreeStateMachine _stateMachine;

        #endregion

        #region CONSTRUCTOR

        public AIStateMachineComponent()
        {
            _stateMachine = new AITreeStateMachine(this);
        }

        protected override void Awake()
        {
            base.Awake();
            
            this.SyncStates();
        }

        protected override void Start()
        {
            base.Start();

            _stateMachine.Repeat = _repeat;
            _stateMachine.AlwaysSucceed = _alwaysSucceed;

            if(_stateMachine.Current == null)
            {
                var state = _defaultState as IAIState;
                if (state == null || !_stateMachine.Contains(state))
                {
                    state = _stateMachine.FirstOrDefault();
                    _defaultState = state as Component;
                }
                _stateMachine.ChangeState(state);
            }
        }

        #endregion

        #region Methods

        public void SyncStates()
        {
            var state = _stateMachine.Current;
            _stateMachine.ChangeState((IAIState)null);
            _stateMachine.SetStateMachine(TypedStateMachine<IAIState>.CreateFromParentComponentSource(this.gameObject, false, true));
            if (state != null && _stateMachine.Contains(state)) _stateMachine.ChangeState(state);
        }

        void IAIEditorSyncActionsCallbackReceiver.SyncActions()
        {
            this.SyncStates();
        }

        #endregion

        #region IAIAction Interface

        public virtual string DisplayName { get { return string.Format("{0} : {1}", _stateMachine.DisplayName, this.name); } }

        bool IAIAction.Enabled { get { return this.isActiveAndEnabled; } }

        public RepeatMode Repeat
        {
            get { return _repeat; }
            set
            {
                _repeat = value;
                _stateMachine.Repeat = value;
            }
        }

        public bool AlwaysSucceed
        {
            get { return _alwaysSucceed; }
            set
            {
                _alwaysSucceed = value;
                _stateMachine.AlwaysSucceed = value;
            }
        }

        public ActionResult ActionState
        {
            get { return _stateMachine.ActionState; }
        }

        ActionResult IAINode.Tick(IAIController ai)
        {
            return _stateMachine.Tick(ai);
        }

        void IAINode.Reset()
        {
            _stateMachine.Reset();
        }

        #endregion

        #region IAIStateMachine Interface

        public event StateChangedEventHandler<IAIState> StateChanged
        {
            add { _stateMachine.StateChanged += value; }
            remove { _stateMachine.StateChanged -= value; }
        }
        
        public IAIState Current
        {
            get { return _stateMachine.Current; }
        }

        public bool Contains(IAIState state)
        {
            return _stateMachine.Contains(state);
        }

        public IAIState ChangeState(IAIState state)
        {
            return _stateMachine.ChangeState(state);
        }

        public IEnumerator<IAIState> GetEnumerator()
        {
            return _stateMachine.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _stateMachine.GetEnumerator();
        }

        public void GetStates(ICollection<IAIState> coll)
        {
            _stateMachine.GetStates(coll);
        }

        public void Foreach(System.Action<IAIState> callback)
        {
            _stateMachine.Foreach(callback);
        }

        #endregion

    }
}
