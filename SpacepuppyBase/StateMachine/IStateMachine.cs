using System.Collections.Generic;
using com.spacepuppy.Collections;

namespace com.spacepuppy.StateMachine
{

    public interface IStateMachine<T> : IEnumerable<T>, IForeachEnumerator<T>
    {

        event StateChangedEventHandler<T> StateChanged;

        T Current { get; }
        //int Count { get; }

        bool Contains(T state);
        T ChangeState(T state);

        void GetStates(ICollection<T> coll);

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
