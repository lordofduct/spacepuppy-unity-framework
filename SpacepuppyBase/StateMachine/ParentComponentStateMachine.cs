using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.StateMachine
{

    /// <summary>
    /// Acts as a base state machine that only needs triggers wired up. This state machine wraps around a GameObject and the states are handled as 
    /// components attached to that GameObject or children of that GameObject.
    /// </summary>
    public class ParentComponentStateMachine<TBase> : ITypedStateMachine<TBase> where TBase : class
    {
        
        #region Fields

        private GameObject _container;
        private bool _includeStatesOnContainer;
        private TBase _current;

        #endregion

        #region CONSTRUCTOR

        public ParentComponentStateMachine(GameObject container, bool includeStatesOnContainer)
        {
            if (container == null) throw new System.ArgumentNullException("container");
            _container = container;
            _includeStatesOnContainer = includeStatesOnContainer;
        }

        #endregion

        #region Properties

        public GameObject Container { get { return _container; } }

        public int Count
        {
            get { return this.GetAll().Count(); }
        }

        #endregion

        #region Methods

        public void GotoNullState()
        {
            this.ChangeState_Imp(default(TBase));
        }

        public IEnumerable<TBase> GetAll()
        {
            //var gos = (_includeStatesOnContainer) ? _container.GetAllChildrenAndSelf() : _container.GetAllChildren();
            //return (from go in gos
            //        from s in go.GetLikeComponents<TBase>()
            //        //where !(s is Behaviour) || (s as Behaviour).enabled
            //        select s);
            return ParentComponentStateMachine<TBase>.GetComponentsOnTarg(_container, _includeStatesOnContainer);
        }

        private TBase ChangeState_Imp(TBase newState)
        {
            if (object.Equals(newState, _current)) return _current;

            var oldState = _current;
            _current = newState;

            this.OnStateChanged(new StateChangedEventArgs<TBase>(oldState, newState));

            return _current;
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

        public bool Contains(TBase state)
        {
            return this.GetAll().Contains(state);
        }

        public TBase ChangeState(TBase state)
        {
            if (!state.IsNullOrDestroyed() && !this.Contains(state)) throw new System.InvalidOperationException("State must be a member of the state machine or null.");

            this.ChangeState_Imp(state);
            return state;
        }

        #endregion

        #region ITypedStateMachine Interface

        public bool Contains<T>() where T : class, TBase
        {
            var gos = (_includeStatesOnContainer) ? _container.GetAllChildrenAndSelf() : _container.GetAllChildren();
            T comp = (from go in gos
                      from s in go.GetLikeComponents<T>()
                      //where !(s is Behaviour) || (s as Behaviour).enabled
                      select s).FirstOrDefault();
            return !comp.IsNullOrDestroyed();
        }

        public bool Contains(System.Type tp)
        {
            if (!TypeUtil.IsType(tp, typeof(TBase))) throw new TypeArgumentMismatchException(tp, typeof(TBase), "tp");

            var gos = (_includeStatesOnContainer) ? _container.GetAllChildrenAndSelf() : _container.GetAllChildren();
            var comp = (from go in gos
                        from s in go.GetLikeComponents(tp)
                        //where s.IsEnabled()
                        select s).FirstOrDefault();
            return (!comp.IsNullOrDestroyed() && comp.IsEnabled());
        }

        public T GetState<T>() where T : class, TBase
        {
            var gos = (_includeStatesOnContainer) ? _container.GetAllChildrenAndSelf() : _container.GetAllChildren();
            T comp = (from go in gos
                      from s in go.GetLikeComponents<T>()
                      //where !(s is Behaviour) || (s as Behaviour).enabled
                      select s).FirstOrDefault();
            if (!comp.IsNullOrDestroyed())
                return comp;
            else
                return null;
        }

        public TBase GetState(System.Type tp)
        {
            if (!TypeUtil.IsType(tp, typeof(TBase))) throw new TypeArgumentMismatchException(tp, typeof(TBase), "tp");

            var gos = (_includeStatesOnContainer) ? _container.GetAllChildrenAndSelf() : _container.GetAllChildren();
            var comp = (from go in gos
                        from s in go.GetLikeComponents(tp)
                        //where s.IsEnabled()
                        select s).FirstOrDefault() as TBase;
            if (!comp.IsNullOrDestroyed())
                return comp;
            else
                return null;
        }

        public T ChangeState<T>() where T : class, TBase
        {
            var newState = this.GetState<T>();
            return this.ChangeState_Imp(newState) as T;
        }

        public TBase ChangeState(System.Type tp)
        {
            if (!TypeUtil.IsType(tp, typeof(TBase))) throw new TypeArgumentMismatchException(tp, typeof(TBase), "tp");
            var newState = this.GetState(tp);
            return this.ChangeState_Imp(newState);
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<TBase> GetEnumerator()
        {
            return this.GetAll().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Static Interface

        public static IEnumerable<TBase> GetComponentsOnTarg(GameObject container, bool includeComponentsOnContainer)
        {
            var gos = (includeComponentsOnContainer) ? container.GetAllChildrenAndSelf() : container.GetAllChildren();
            return (from go in gos
                    from s in go.GetLikeComponents<TBase>()
                    //where !(s is Behaviour) || (s as Behaviour).enabled
                    select s);
        }

        #endregion

    }

}
