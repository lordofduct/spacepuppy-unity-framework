using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.StateMachine
{
    public interface IStateStack<T>
    {

        IEnumerable<T> CurrentStack { get; }

        T StackState(T state);

        T PopState();
        T PopAllStates();

    }

    public interface ITypedStateStack<T> : IStateStack<T> where T : class
    {

        TSub StackState<TSub>() where TSub : class, T;
        T StackState(System.Type tp);

    }
}
