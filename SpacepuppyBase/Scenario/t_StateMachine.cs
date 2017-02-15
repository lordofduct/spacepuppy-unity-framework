using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.StateMachine;
using System;
using System.Collections;

namespace com.spacepuppy.Scenario
{
    public class t_StateMachine : SPComponent, IStateMachine<string>
    {

        #region Fields

        [SerializeField]
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

        #region StateMachine Interface

        public event StateChangedEventHandler<string> StateChanged;

        public string Current
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

        public IEnumerator<string> GetEnumerator()
        {
            foreach(var s in _states)
            {
                yield return s.Name;
            }
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
                _onEnterState.ActivateTrigger();
            }

            public void NotifyStateExited()
            {
                _onExitState.ActivateTrigger();
            }
        }

        #endregion


    }
}
