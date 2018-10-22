using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.AI;
using com.spacepuppy.Collections;
using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI
{
    public class AISubController : SimpleAIStateComponent, IAIStateMachine
    {
        
        #region Fields

        [SerializeField()]
        private AIStateMachineSourceMode _stateSource;

        [SerializeField()]
        private Component _defaultState;

        [System.NonSerialized()]
        private ITypedStateMachine<IAIState> _stateMachine;
        
        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
            
            this.InitStateMachine();
        }

        #endregion

        #region Properties

        public ITypedStateMachine<IAIState> States { get { return _stateMachine; } }

        public IAIState DefaultState
        {
            get { return _defaultState as IAIState; }
            set
            {
                if (object.ReferenceEquals(_defaultState, value)) return;

                if (value != null && _stateMachine.Contains(value))
                {
                    _defaultState = value as Component;
                }
                else
                {
                    _defaultState = null;
                }
            }
        }
        
        public AIStateMachineSourceMode StateSource
        {
            get { return _stateSource; }
            set
            {
                if (_stateSource == value) return;

                _stateSource = value;
            }
        }

        #endregion

        #region Methods

        private void InitStateMachine()
        {
            IAIState state = null;
            if (_stateMachine != null)
            {
                state = _stateMachine.Current;
                _stateMachine.StateChanged -= this.OnStateChanged;
                _stateMachine = null;
            }

            switch (_stateSource)
            {
                case AIStateMachineSourceMode.SelfSourced:
                    _stateMachine = TypedStateMachine<IAIState>.CreateFromComponentSource(this.gameObject);
                    break;
                case AIStateMachineSourceMode.ChildSourced:
                    _stateMachine = TypedStateMachine<IAIState>.CreateFromParentComponentSource(this.gameObject, false, true);
                    break;
            }
            _stateMachine.StateChanged += this.OnStateChanged;
            foreach (var st in _stateMachine)
            {
                st.Init(this);
            }
        }

        #endregion

        #region Event Handlers

        protected virtual void OnStateChanged(object sender, StateChangedEventArgs<IAIState> e)
        {
            if (e.FromState != null) e.FromState.OnStateExited(this, e.ToState);
            if (e.ToState != null) e.ToState.OnStateEntered(this, e.FromState);
        }

        #endregion

        #region IAIStateMachine Interface

        event StateChangedEventHandler<IAIState> IStateMachine<IAIState>.StateChanged
        {
            add { _stateMachine.StateChanged += value; }
            remove { _stateMachine.StateChanged -= value; }
        }
        
        IAIState IStateMachine<IAIState>.Current
        {
            get { return _stateMachine.Current; }
        }

        bool IStateMachine<IAIState>.Contains(IAIState state)
        {
            return _stateMachine.Contains(state);
        }

        IAIState IStateMachine<IAIState>.ChangeState(IAIState state)
        {
            return _stateMachine.ChangeState(state);
        }

        IEnumerator<IAIState> IEnumerable<IAIState>.GetEnumerator()
        {
            return _stateMachine.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _stateMachine.GetEnumerator();
        }

        void IStateMachine<IAIState>.GetStates(ICollection<IAIState> coll)
        {
            _stateMachine.GetStates(coll);
        }

        void com.spacepuppy.Collections.IForeachEnumerator<IAIState>.Foreach(System.Action<IAIState> callback)
        {
            _stateMachine.Foreach(callback);
        }

        #endregion

        #region IAIState Interface

        protected override void OnStateEntered(IAIStateMachine machine, IAIState lastState)
        {
            if (_defaultState is IAIState)
            {
                _stateMachine.ChangeState(_defaultState as IAIState);
            }
        }

        protected override void OnStateExited(IAIStateMachine machine, IAIState nextState)
        {
            if (_stateMachine.Current != null)
            {
                _stateMachine.ChangeState((IAIState)null);
            }
        }

        protected override void Tick(IAIController ai)
        {
            if (_stateMachine.Current != null)
            {
                _stateMachine.Current.Tick(ai);
            }
        }

        #endregion
        
    }
}
