using System;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.StateMachine
{

    /// <summary>
    /// Acts as a base state machine that only needs triggers wired up. The states are type driven and only one state of a given type can exist for that type.
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    public class TypedStateMachine<TBase> : ITypedStateMachine<TBase> where TBase : class
    {
        
        #region Fields

        private ComponentCollection<TBase> _states = new ComponentCollection<TBase>();
        private TBase _current;

        #endregion

        #region CONSTRUCTOR

        public TypedStateMachine()
        {

        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _states.Count; }
        }

        #endregion

        #region Methods

        public bool Add<T>(T state) where T : class, TBase
        {
            if (_states.Has<T>()) throw new ArgumentException("Can not add a state for a type that already exists.");

            _states.Add<T>(state);
            return true;
        }

        public bool Remove(TBase state)
        {
            if (_states.Remove(state))
            {
                if (_current == state)
                {
                    this.GotoNullState();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove<T>() where T : class, TBase
        {
            var state = _states.Get<T>();
            if (state == null) return false;
            return this.Remove(state);
        }

        public bool Contains(TBase state)
        {
            return _states.Contains(state);
        }

        public void GotoNullState()
        {
            if (_current == null) return;

            this.ChangeState((TBase)null);
        }

        #endregion

        #region ITypedStateMachine Interface

        public bool Contains<T>() where T : class, TBase
        {
            return _states.Has<T>();
        }

        public bool Contains(System.Type tp)
        {
            return _states.Has(tp);
        }

        public T GetState<T>() where T : class, TBase
        {
            return _states.Get<T>();
        }

        public TBase GetState(System.Type tp)
        {
            return _states.Get(tp);
        }

        public T ChangeState<T>() where T : class, TBase
        {
            var newState = this.GetState<T>();
            if (Object.Equals(newState, _current)) return _current as T;

            this.ChangeState(newState);

            return newState;
        }

        public TBase ChangeState(System.Type tp)
        {
            var newState = this.GetState(tp);
            if (Object.Equals(newState, _current)) return _current;

            this.ChangeState(newState);

            return newState;
        }

        #endregion

        #region IStateMachine Interface

        public event StateChangedEventHandler<TBase> StateChanged;
        protected void OnStateChanged(StateChangedEventArgs<TBase> e)
        {
            if (this.StateChanged != null) this.StateChanged(this, e);
        }

        public TBase Current
        {
            get { return _current; }
        }

        bool IStateMachine<TBase>.Contains(TBase state)
        {
            return this.Contains(state);
        }

        public TBase ChangeState(TBase newState)
        {
            if (!_states.Contains(newState)) throw new ArgumentException("State is not a member of this StateMachine.");
            var oldState = _current;
            _current = newState;

            this.OnStateChanged(new StateChangedEventArgs<TBase>(oldState, newState));

            return _current;
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<TBase> GetEnumerator()
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
