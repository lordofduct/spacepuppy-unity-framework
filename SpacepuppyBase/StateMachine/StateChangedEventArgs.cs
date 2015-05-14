using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.StateMachine
{
    public delegate void StateChangedEventHandler<T>(object sender, StateChangedEventArgs<T> e);

    public class StateChangedEventArgs<T> : EventArgs
    {

        private T _fromState;
        private T _toState;

        public StateChangedEventArgs(T fromState, T toState)
        {
            _fromState = fromState;
            _toState = toState;
        }

        public T FromState { get { return _fromState; } }

        public T ToState { get { return _toState; } }

    }
}
