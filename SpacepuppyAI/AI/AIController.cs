#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Sensors;
using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI
{

    /// <summary>
    /// Generic state machine AIController.
    /// </summary>
    public class AIController : SPNotifyingComponent, IAIController, IAIStateMachine
    {

        public event System.EventHandler OnTick;

        #region Fields

        [SerializeField()]
        private AIStateMachineSourceMode _stateSource;

        [SerializeField()]
        private Component _defaultState;

        [SerializeField()]
        [MinRange(0.02f)]
        private float _interval = 0.1f;

        [SerializeField()]
        private AIVariableCollection _variables;

        [System.NonSerialized()]
        private ITypedStateMachine<IAIState> _stateMachine;

        [System.NonSerialized()]
        private float _t;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.InitStateMachine();
        }

        protected override void Start()
        {
            base.Start();

            if (_stateMachine.Current == null)
            {
                var state = _defaultState as IAIState;
                if (state.IsNullOrDestroyed() || !_stateMachine.Contains(state))
                {
                    state = _stateMachine.FirstOrDefault();
                    _defaultState = state as Component;
                }
                _stateMachine.ChangeState(state);
            }
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

                if(value != null && _stateMachine.Contains(value))
                {
                    _defaultState = value as Component;
                }
                else
                {
                    _defaultState = null;
                }
            }
        }

        public AIVariableCollection Variables { get { return _variables; } }

        public float Interval
        {
            get { return _interval; }
            set
            {
                _interval = Mathf.Max(0f, value);
            }
        }

        public AIStateMachineSourceMode StateSource
        {
            get { return _stateSource; }
            set
            {
                if (_stateSource == value) return;

                _stateSource = value;
                if(this.started && _stateMachine != null)
                {
                    this.InitStateMachine();
                }
            }
        }

        #endregion

        #region Methods

        public void OffsetTicker(float t)
        {
            _t = Time.time - t;
        }

        private void InitStateMachine()
        {
            IAIState state = null;
            if(_stateMachine != null)
            {
                state = _stateMachine.Current;
                _stateMachine.StateChanged -= this.OnStateChanged;
                _stateMachine = null;
            }

            switch(_stateSource)
            {
                case AIStateMachineSourceMode.SelfSourced:
                    _stateMachine = TypedStateMachine<IAIState>.CreateFromComponentSource(this.gameObject);
                    break;
                case AIStateMachineSourceMode.ChildSourced:
                    _stateMachine = TypedStateMachine<IAIState>.CreateFromParentComponentSource(this.gameObject, false, true);
                    break;
            }
            _stateMachine.StateChanged += this.OnStateChanged;
            foreach(var st in _stateMachine)
            {
                st.Init(this);
            }

            if(this.started)
            {
                if(!state.IsNullOrDestroyed() && _stateMachine.Contains(state))
                {
                    _stateMachine.ChangeState(state);
                }
                else
                {
                    _stateMachine.ChangeState((IAIState)null);
                }
            }
        }

        protected void Update()
        {
            if (Time.time - _t > _interval)
            {
                _t = Time.time;
                if(_stateMachine.Current != null)
                {
                    _stateMachine.Current.Tick(this);
                }

                this.OnTick?.Invoke(this, System.EventArgs.Empty);
            }
        }

        #endregion

        #region Event Handlers

        protected virtual void OnStateChanged(object sender, StateChangedEventArgs<IAIState> e)
        {
            if (e.FromState != null) e.FromState.OnStateExited(this, e.ToState);
            if (e.ToState != null) e.ToState.OnStateEntered(this, e.FromState);

            //Notification.PostNotification<AIStateChanged>(this, new AIStateChanged(e.FromState, e.ToState), true);
        }

        #endregion
        
        #region Notifications

        /*
        public class AIStateChanged : Notification
        {

            private IAIState _fromState;
            private IAIState _toState;

            public AIStateChanged(IAIState fromState, IAIState toState)
            {
                _fromState = fromState;
                _toState = toState;
            }

            public IAIState FromState { get { return _fromState; } }

            public IAIState ToState { get { return _toState; } }

        }
        */

        #endregion






        #region AIStateMachine Interface

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

    }
}
