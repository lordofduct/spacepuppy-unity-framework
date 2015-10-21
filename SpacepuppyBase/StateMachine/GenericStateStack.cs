using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.StateMachine
{
    public class GenericStateStack<T> : IStateStack<T> where T : class
    {

        #region Fields

        private IStateMachine<T> _machine;
        private Stack<T> _stateStack = new Stack<T>();

        #endregion

        #region CONSTRUCTOR

        public GenericStateStack(IStateMachine<T> machine)
        {
            if (machine == null) throw new System.ArgumentNullException("machine");
            _machine = machine;
        }

        #endregion

        #region Properties

        public IStateMachine<T> Machine { get { return _machine; }}

        #endregion

        #region IStateStack Interface

        public IEnumerable<T> CurrentStack { get { return _stateStack; } }

        public T StackState(T state)
        {
            if (!object.ReferenceEquals(state, null) && !_machine.Contains(state)) throw new System.ArgumentException("MovementStyle is not a member of the state machine.", "style");

            _stateStack.Push(_machine.Current);
            return _machine.ChangeState(state);
        }

        public T PopState()
        {
            if (_stateStack.Count > 0)
            {
                return _machine.ChangeState(_stateStack.Pop());
            }
            else
            {
                return null;
            }
        }

        public T PopAllStates()
        {
            if (_stateStack.Count > 0)
            {
                var style = _stateStack.Last();
                _stateStack.Clear();
                return _machine.ChangeState(style);
            }
            else
            {
                return null;
            }
        }

        #endregion

    }

    public class GenericTypedStateStack<T> : GenericStateStack<T>, ITypedStateStack<T> where T : class
    {

        #region CONSTRUCTOR

        public GenericTypedStateStack(ITypedStateMachine<T> machine) : base(machine)
        {

        }

        #endregion

        #region Properties

        public new ITypedStateMachine<T> Machine { get { return base.Machine as ITypedStateMachine<T>; } }

        #endregion

        #region ITypedStateStack Interface

        public TSub StackState<TSub>() where TSub : class, T
        {
            var state = this.Machine.GetState<TSub>();
            if (state == null) throw new System.ArgumentException("Machine does not contain a state of type '" + typeof(TSub).Name + "'.", "TSub");

            return this.StackState(state) as TSub;
        }

        public T StackState(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            var state = this.Machine.GetState(tp);
            if (state == null) throw new System.ArgumentException("Machine does not contain a state of type '" + tp.Name + "'.", "tp");

            return this.StackState(state);
        }

        #endregion

    }
}
