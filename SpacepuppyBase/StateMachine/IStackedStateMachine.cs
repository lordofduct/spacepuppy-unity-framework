using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.StateMachine
{

    public interface IStackedStateMachine<T> : IStateMachine<T>, IStateStack<T>
    {
    }

    public interface ITypedStackedStateMachine<T> : IStackedStateMachine<T>, ITypedStateMachine<T>, ITypedStateStack<T> where T : class
    {

    }


    public class StackedStateMachine<T> : StateMachine<T>, IStackedStateMachine<T> where T : class
    {

        #region Fields

        private GenericStateStack<T> _stack;

        #endregion

        #region CONSTRUCTOR

        public StackedStateMachine(IStateSupplier<T> supplier)
            : base(supplier)
        {
            _stack = new GenericStateStack<T>(this);
        }

        #endregion

        #region IStackedStateMachine Interface

        public IEnumerable<T> CurrentStack
        {
            get
            {
                return _stack.CurrentStack;
            }
        }

        public T PopAllStates()
        {
            return _stack.PopAllStates();
        }

        public T PopState()
        {
            return _stack.PopState();
        }

        public T StackState(T state)
        {
            return _stack.StackState(state);
        }

        #endregion

    }

    public class TypedStackedStateMachine<T> : TypedStateMachine<T>, ITypedStackedStateMachine<T> where T : class
    {

        #region Fields

        private GenericTypedStateStack<T> _stack;

        #endregion

        #region CONSTRUCTOR

        public TypedStackedStateMachine(ITypedStateSupplier<T> supplier)
            : base(supplier)
        {
            _stack = new GenericTypedStateStack<T>(this);
        }

        #endregion

        #region ITypedStackedStateMachine Interface

        public IEnumerable<T> CurrentStack
        {
            get
            {
                return _stack.CurrentStack;
            }
        }

        public T PopAllStates()
        {
            return _stack.PopAllStates();
        }

        public T PopState()
        {
            return _stack.PopState();
        }

        public T StackState(T state)
        {
            return _stack.StackState(state);
        }

        public T StackState(Type tp)
        {
            return _stack.StackState(tp);
        }

        public TSub StackState<TSub>() where TSub : class, T
        {
            return this.StackState<TSub>();
        }

        #endregion

        #region Static Factory

        public new static TypedStackedStateMachine<T> CreateFromComponentSource(UnityEngine.GameObject source)
        {
            return new TypedStackedStateMachine<T>(new ComponentStateSupplier<T>(source));
        }

        public new static TypedStackedStateMachine<T> CreateFromParentComponentSource(UnityEngine.GameObject source, bool includeStatesOnContainer, bool isStatic)
        {
            return new TypedStackedStateMachine<T>(new ParentComponentStateSupplier<T>(source, includeStatesOnContainer, isStatic));
        }

        #endregion

    }

}
