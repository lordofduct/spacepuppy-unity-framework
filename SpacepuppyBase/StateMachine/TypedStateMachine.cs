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
    public class TypedStateMachine<T> : ITypedStateMachine<T> where T : class
    {
        
        #region Fields

        private ITypedStateSupplier<T> _states;
        private T _current;

        #endregion

        #region CONSTRUCTOR

        public TypedStateMachine(ITypedStateSupplier<T> supplier)
        {
            if (supplier == null) throw new System.ArgumentNullException("supplier");
            _states = supplier;
        }

        #endregion

        #region Properties

        public ITypedStateSupplier<T> StateSupplier { get { return _states; } }

        #endregion

        #region Methods

        public void GotoNullState()
        {
            if (_current == null) return;

            this.ChangeState((T)null);
        }

        private T ChangeState_Imp(T newState)
        {
            if (object.Equals(newState, _current)) return _current;

            var oldState = _current;
            _current = newState;

            this.OnStateChanged(new StateChangedEventArgs<T>(oldState, newState));

            return _current;
        }

        #endregion

        #region ITypedStateMachine Interface

        public bool Contains<TSub>() where TSub : class, T
        {
            return _states.Contains<TSub>();
        }

        public bool Contains(System.Type tp)
        {
            return _states.Contains(tp);
        }

        public TSub GetState<TSub>() where TSub : class, T
        {
            return _states.GetState<TSub>();
        }

        public T GetState(System.Type tp)
        {
            return _states.GetState(tp);
        }

        public TSub ChangeState<TSub>() where TSub : class, T
        {
            var newState = this.GetState<TSub>();
            return this.ChangeState_Imp(newState) as TSub;
        }

        public T ChangeState(System.Type tp)
        {
            var newState = this.GetState(tp);
            return this.ChangeState_Imp(newState);
        }

        #endregion

        #region IStateMachine Interface

        public event StateChangedEventHandler<T> StateChanged;
        protected void OnStateChanged(StateChangedEventArgs<T> e)
        {
            if (this.StateChanged != null) this.StateChanged(this, e);
        }

        public T Current
        {
            get { return _current; }
        }

        public bool Contains(T state)
        {
            return _states.Contains(state);
        }

        public T ChangeState(T newState)
        {
            if (!_states.Contains(newState)) throw new ArgumentException("State is not a member of this StateMachine.");
            return this.ChangeState_Imp(newState);
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


        #region Static Factory

        public static TypedStateMachine<T> CreateFromComponentSource(UnityEngine.GameObject source)
        {
            return new TypedStateMachine<T>(new ComponentStateSupplier<T>(source));
        }

        public static TypedStateMachine<T> CreateFromParentComponentSource(UnityEngine.GameObject source, bool includeStatesOnContainer)
        {
            return new TypedStateMachine<T>(new ParentComponentStateSupplier<T>(source, includeStatesOnContainer));
        }

        #endregion

    }
}
