using System;
using System.Collections.Generic;

namespace com.spacepuppy.StateMachine
{

    public interface IStateMachine<T> : IEnumerable<T>
    {

        event StateChangedEventHandler<T> StateChanged;

        T Current { get; }

        bool Contains(T state);
        T ChangeState(T state);

    }

    public interface ITypedStateMachine<T> : IStateMachine<T>
    {

        bool Contains<TSub>() where TSub : class, T;
        bool Contains(System.Type tp);

        TSub GetState<TSub>() where TSub : class, T;
        T GetState(System.Type tp);

        TSub ChangeState<TSub>() where TSub : class, T;
        T ChangeState(System.Type tp);

    }

}
