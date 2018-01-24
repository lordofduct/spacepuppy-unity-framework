using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree
{

    public interface IAITreeStateMachine : IAIStateMachine, IAIAction
    {

    }


    public class AITreeStateMachine : AIAction, IAITreeStateMachine
    {

        #region Fields

        [System.NonSerialized()]
        private IStateMachine<IAIState> _stateMachine;
        [System.NonSerialized()]
        private IAIStateMachine _owner;

        #endregion

        #region CONSTRUCTOR

        public AITreeStateMachine()
        {
            _owner = this;
        }

        public AITreeStateMachine(IAIStateMachine owner)
        {
            _owner = owner ?? this;
        }

        #endregion

        #region Properties
        
        #endregion

        #region Methods

        public void SetStateMachine(IStateMachine<IAIState> machine)
        {
            if (_stateMachine != null)
            {
                _stateMachine.StateChanged -= this.OnStateChanged;
                _stateMachine = null;
            }

            _stateMachine = machine;
            if (_stateMachine != null)
            {
                _stateMachine.StateChanged += this.OnStateChanged;
                foreach (var st in _stateMachine)
                {
                    st.Init(_owner);
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OnStateChanged(object sender, StateChangedEventArgs<IAIState> e)
        {
            if (e.FromState != null) e.FromState.OnStateExited(_owner, e.ToState);
            if (e.ToState != null) e.ToState.OnStateEntered(_owner, e.FromState);

            if (this.StateChanged != null) this.StateChanged(this, e);
        }

        #endregion

        #region IAIAction Interface

        public override string DisplayName
        {
            get
            {
                return "[State Machine]";
            }
        }

        protected override ActionResult OnTick(IAIController ai)
        {
            if (_stateMachine != null && _stateMachine.Current != null)
            {
                return _stateMachine.Current.Tick(ai);
            }
            else
            {
                return ActionResult.Success;
            }
        }

        protected override void OnReset()
        {
            foreach (var state in _stateMachine)
            {
                state.Reset();
            }
        }

        #endregion

        #region IStateMachine Interface

        public event StateChangedEventHandler<IAIState> StateChanged;

        public IAIState Current
        {
            get { return (_stateMachine != null) ? _stateMachine.Current : null; }
        }

        public bool Contains(IAIState state)
        {
            if (_stateMachine == null) return false;
            return _stateMachine.Contains(state);
        }

        public IAIState ChangeState(IAIState state)
        {
            if (_stateMachine == null) return null;
            if (_stateMachine.Current == state) return state;
            return _stateMachine.ChangeState(state);
        }

        public IEnumerator<IAIState> GetEnumerator()
        {
            if (_stateMachine == null) return Enumerable.Empty<IAIState>().GetEnumerator();
            return _stateMachine.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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
