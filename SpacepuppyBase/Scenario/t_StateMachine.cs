#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.StateMachine;
using System.Collections;

namespace com.spacepuppy.Scenario
{
    public class t_StateMachine : SPComponent, IStateMachine<string>, IEnumerable<t_StateMachine.State>
    {

        #region Fields

        [SerializeField]
        [ReorderableArray(DrawElementAtBottom = true, ChildPropertyToDrawAsElementLabel = "_name")]
        private State[] _states;

        [SerializeField]
        private int _initialState;

        [SerializeField]
        [Tooltip("When starting should the initial states 'OnEnterState' event be fired?")]
        private bool _notifyFirstStateOnStart;

        [System.NonSerialized]
        private State _currentState;

        #endregion

        #region CONSTRUCTOR

        protected override void Start()
        {
            base.Start();

            _currentState = (_initialState >= 0 && _initialState < _states.Length) ? _states[_initialState] : null;
            if(_notifyFirstStateOnStart && _currentState != null)
            {
                _currentState.NotifyStateEntered();
                if (this.StateChanged != null) this.StateChanged(this, new StateChangedEventArgs<string>(null, _currentState.Name));
            }
        }

        #endregion

        #region Properties

        public int NumOfStates
        {
            get { return _states.Length; }
        }

        public State Current
        {
            get { return _currentState; }
        }

        public string CurrentStateName
        {
            get { return _currentState != null ? _currentState.Name : null; }
        }

        public int CurrentStateIndex
        {
            get { return System.Array.IndexOf(_states, _currentState); }
        }

        public State this[int index]
        {
            get
            {
                if (index < 0 || index >= _states.Length) throw new System.IndexOutOfRangeException();
                return _states[index];
            }
        }

        #endregion

        #region Methods

        public int IndexOf(State state)
        {
            return System.Array.IndexOf(_states, state);
        }

        public bool Contains(State state)
        {
            return System.Array.IndexOf(_states, state) >= 0;
        }
        
        public State ChangeState(int index)
        {
            if (index < 0 || index >= _states.Length) throw new System.IndexOutOfRangeException();

            var s = _states[index];
            if (_currentState == s) return s;

            var lastState = _currentState;
            _currentState = s;
            if (lastState != null) lastState.NotifyStateExited();
            if (_currentState != null) _currentState.NotifyStateEntered();

            if (this.StateChanged != null) this.StateChanged(this, new StateChangedEventArgs<string>(lastState != null ? lastState.Name : null, _currentState != null ? _currentState.Name : null));
            return _currentState;
        }

        #endregion

        #region StateMachine Interface

        public event StateChangedEventHandler<string> StateChanged;

        string IStateMachine<string>.Current
        {
            get
            {
                return _currentState != null ? _currentState.Name : null;
            }
        }


        public bool Contains(string state)
        {
            foreach(var s in _states)
            {
                if (string.Equals(s.Name, state)) return true;
            }

            return false;
        }

        public string ChangeState(string state)
        {
            if (_currentState != null && _currentState.Name == state) return state;

            foreach (var s in _states)
            {
                if (string.Equals(s.Name, state))
                {
                    var lastState = _currentState;
                    _currentState = s;
                    if (lastState != null) lastState.NotifyStateExited();
                    _currentState.NotifyStateEntered();

                    if (this.StateChanged != null) this.StateChanged(this, new StateChangedEventArgs<string>(lastState != null ? lastState.Name : null, state));

                    return state;
                }
            }

            return null;
        }
        
        void IStateMachine<string>.GetStates(ICollection<string> coll)
        {
            for (int i = 0; i < _states.Length; i++)
            {
                coll.Add(_states[i].Name);
            }
        }

        void com.spacepuppy.Collections.IForeachEnumerator<string>.Foreach(System.Action<string> callback)
        {
            for (int i = 0; i < _states.Length; i++)
            {
                callback(_states[i].Name);
            }
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            foreach(var s in _states)
            {
                yield return s.Name;
            }
        }

        #endregion

        #region IEnumerable Interface
        
        public IEnumerator<State> GetEnumerator()
        {
            return (_states as IList<State>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Special Types

        [System.Serializable]
        public class State
        {
            [SerializeField]
            private string _name;
            [SerializeField]
            private Trigger _onEnterState;
            [SerializeField]
            private Trigger _onExitState;

            public string Name
            {
                get { return _name; }
            }

            public void NotifyStateEntered()
            {
                _onEnterState.ActivateTrigger(this, null);
            }

            public void NotifyStateExited()
            {
                _onExitState.ActivateTrigger(this, null);
            }
        }

        #endregion

    }
}
