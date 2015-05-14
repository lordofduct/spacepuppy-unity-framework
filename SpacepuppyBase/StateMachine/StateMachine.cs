using System;
using System.Collections.Generic;

using com.spacepuppy.Collections;

namespace com.spacepuppy.StateMachine
{

    /// <summary>
    /// Acts as a basic base state machine that manages a single state at a time. All one must do is wire up the triggers.
    /// </summary>
    public class StateMachine<T> : IStateMachine<T>
    {

        #region Fields

        private UniqueList<T> _states = new UniqueList<T>();
        private T _current;

        #endregion

        #region CONSTRUCTOR

        public StateMachine()
        {

        }

        #endregion

        #region Properties

        public int Count { get { return _states.Count; } }

        public T Current { get { return _current; } }

        #endregion

        #region Methods

        public void Add(T state)
        {
            _states.Add(state);
        }

        public bool Remove(T state)
        {
            return _states.Remove(state);
        }

        public bool Contains(T state)
        {
            return _states.Contains(state);
        }

        public T ChangeState(T newState)
        {
            if (object.Equals(newState, _current)) return _current;

            var oldState = _current;
            _current = newState;

            this.OnStateChanged(new StateChangedEventArgs<T>(oldState, newState));

            return _current;
        }

        #endregion

        #region IStateMachine Interface

        public event StateChangedEventHandler<T> StateChanged;
        protected void OnStateChanged(StateChangedEventArgs<T> e)
        {
            if (this.StateChanged != null) this.StateChanged(this, e);
        }

        T IStateMachine<T>.Current
        {
            get { return this.Current; }
        }

        bool IStateMachine<T>.Contains(T state)
        {
            return this.Contains(state);
        }

        T IStateMachine<T>.ChangeState(T state)
        {
            return this.ChangeState(state);
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<T> GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        #endregion

    }
}
