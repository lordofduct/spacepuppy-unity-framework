using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Movement
{

    [UniqueToEntity()]
    [RequireComponentInEntity(typeof(MovementController))]
    public class MovementMotor : SPComponent
    {

        #region Events

        public event System.EventHandler BeforeUpdateMovement;
        public event System.EventHandler UpdateMovementComplete;
        public event StyleChangedEventHandler StyleChanged;

        #endregion

        #region Fields

        [DefaultFromSelf(UseEntity=true)]
        [SerializeField()]
        [Tooltip("The controller that this motor handles.")]
        private MovementController _controller;
        [System.NonSerialized()]
        private MovementStyleStateMachine _stateMachine;

        [SerializeField()]
        private Component _defaultMovementStyle;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _stateMachine = new MovementStyleStateMachine(this);
        }

        protected override void Start()
        {
            base.Start();

            if (_controller == null) _controller = this.FindComponent<MovementController>();
            if (_controller == null) throw new System.InvalidOperationException("MovementMotor requires an attach MovementController.");

            if (_defaultMovementStyle != null && _stateMachine.Contains(_defaultMovementStyle))
            {
                _stateMachine.ChangeState(_defaultMovementStyle as IMovementStyle);
            }
        }

        #endregion

        #region Properties

        public virtual bool UseFixedUpdate { get { return (_controller.Mover != null) ? _controller.Mover.PrefersFixedUpdate : false; } }

        public MovementController Controller { get { return _controller; } }

        public MovementStyleStateMachine States { get { return _stateMachine; } }

        public IMovementStyle DefaultMovementStyle
        {
            get { return _defaultMovementStyle as IMovementStyle; }
            set
            {
                if(value != null)
                {
                    if (_stateMachine != null)
                    {
                        if (!_stateMachine.Contains(value)) throw new System.ArgumentException("Default Movement Style must be a style already attached to the motor's GameObject.");
                    }
                    else
                    {
                        if (this.gameObject != value.gameObject) throw new System.ArgumentException("Default Movement Style must be a style already attached to the motor's GameObject.");
                    }
                }
                _defaultMovementStyle = value as Component;
            }
        }

        public bool InUpdateSequence
        {
            get
            {
                return _controller != null && _controller.InUpdateSequence;
            }
        }

        #endregion

        #region Game Messages

        protected void Update()
        {
            if (this.UseFixedUpdate) return;

            this.ProcessMovement();
        }

        protected void FixedUpdate()
        {
            if (!this.UseFixedUpdate) return;

            this.ProcessMovement();
        }

        #endregion

        #region Movement Processing

        protected void ProcessMovement()
        {
            if (_controller == null) return;

            try
            {
                _controller.OnBeforeUpdate();

                OnBeforeUpdateMovement();

                _stateMachine.DoChange(); //if BeforeUpdate called for a change, or something before this update loop, lets do that change

                this.OnUpdateMovement();

                //once movements are done, call complete
                this.OnUpdateMovementComplete();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
            finally
            {
                _stateMachine.DoChange(); //a final check for a change

                _controller.OnUpdateComplete();
            }
        }

        protected virtual void OnBeforeUpdateMovement()
        {
            if (this.BeforeUpdateMovement != null) this.BeforeUpdateMovement(this, System.EventArgs.Empty);
        }

        protected virtual void OnUpdateMovement()
        {
            if (_stateMachine.Current != null)
            {
                _stateMachine.Current.UpdateMovement();
            }
        }

        protected virtual void OnUpdateMovementComplete()
        {
            if (_stateMachine.Current != null)
            {
                _stateMachine.Current.OnUpdateMovementComplete();
            }

            if (this.UpdateMovementComplete != null) this.UpdateMovementComplete(this, System.EventArgs.Empty);
        }

        #endregion

        #region EditorOnly

        public void SetMovementControllerInEditor(MovementController controller)
        {
            if(Application.isEditor)
            {
                _controller = controller;
            }
        }

        #endregion


        #region Special Types

        public enum ReleaseMode
        {
            Pop = 1,
            PopAll = 2,
            Default = 3,
            Null = 1 << 31,
            PopOrDefault = 4,
            PopAllOrDefault = 5,
            PopOrNull = Pop | Null,
            PopAllOrNull = PopAll | Null
        }

        public class MovementStyleStateMachine : ITypedStateMachine<IMovementStyle>, ITypedStateStack<IMovementStyle>
        {

            #region Fields

            private MovementMotor _owner;
            private ComponentStateSupplier<IMovementStyle> _stateSupplier;
            private IMovementStyle _current;

            private System.Action _changeStyleDelayed;
            private float _changeStyleDelayedPrecedence;
            private OrderedDelegate<System.Action> _stackStyleDelayed = new OrderedDelegate<System.Action>();

            private Deque<IMovementStyle> _styleStack;
            private bool _stackingState = false;

            #endregion

            #region CONSTRUCTOR

            internal MovementStyleStateMachine(MovementMotor owner)
            {
                _owner = owner;
                _stateSupplier = new ComponentStateSupplier<IMovementStyle>(_owner.gameObject);
                _changeStyleDelayed = null;
                _changeStyleDelayedPrecedence = float.NegativeInfinity;
                _stackStyleDelayed.Clear();
                _styleStack = new Deque<IMovementStyle>();
            }

            #endregion

            #region Properties

            /// <summary>
            /// The current state ignoring any stacked states that may be on top of it. This is the state that would be returned to if you popped all the states.
            /// </summary>
            public IMovementStyle CurrentUnstacked
            {
                get
                {
                    if (_styleStack.Count > 0)
                        return _styleStack[0];
                    else
                        return _current;
                }
            }

            #endregion

            #region Methods

            internal void DoChange()
            {
                if (_changeStyleDelayed != null)
                {
                    _changeStyleDelayed();
                    _changeStyleDelayed = null;
                    _changeStyleDelayedPrecedence = float.NegativeInfinity;
                }

                if(_stackStyleDelayed.HasEntries)
                {
                    _stackStyleDelayed.Invoke();
                    _stackStyleDelayed.Clear();
                }
            }

            public IMovementStyle ReleaseCurrentState(ReleaseMode mode, float precedence = 0f)
            {
                switch(mode)
                {
                    case ReleaseMode.Pop:
                        if (_styleStack.Count > 0)
                            return this.PopState(precedence);
                        else
                            return _current;
                    case ReleaseMode.PopAll:
                        if (_styleStack.Count > 0)
                            return this.PopAllStates(precedence);
                        else
                            return _current;
                    case ReleaseMode.Default:
                        return this.ChangeState(_owner._defaultMovementStyle as IMovementStyle, precedence);
                    case ReleaseMode.Null:
                        this.ChangeStateToNull(precedence);
                        return null;
                    case ReleaseMode.PopOrDefault:
                        if (_styleStack.Count > 0)
                            return this.PopState(precedence);
                        else
                            return this.ChangeState(_owner._defaultMovementStyle as IMovementStyle, precedence);
                    case ReleaseMode.PopAllOrDefault:
                        if (_styleStack.Count > 0)
                            return this.PopAllStates(precedence);
                        else
                            return this.ChangeState(_owner._defaultMovementStyle as IMovementStyle, precedence);
                    case ReleaseMode.PopOrNull:
                        if (_styleStack.Count > 0)
                            return this.PopState(precedence);
                        else
                        {
                            this.ChangeStateToNull(precedence);
                            return null;
                        }
                    case ReleaseMode.PopAllOrNull:
                        if (_styleStack.Count > 0)
                            return this.PopAllStates(precedence);
                        else
                        {
                            this.ChangeStateToNull(precedence);
                            return null;
                        }

                }

                throw new System.ArgumentException("ReleaseMode was of an indeterminate configuration.");
            }

            public void ChangeStateToNull(float precedence = 0f)
            {
                this.ChangeState((IMovementStyle)null, precedence);
            }

            public void PurgeStackedState(IMovementStyle style)
            {
                if (object.ReferenceEquals(style, null)) throw new System.ArgumentNullException("style");
                if (_styleStack.Count == 0) return;

                //purge it
                //int depth = _styleStack.Depth(style);
                //if (depth > 0)
                //{
                //    var st = new Stack<IMovementStyle>(depth);
                //    while (st.Count < depth) st.Push(_styleStack.Pop());
                //    _styleStack.Pop();
                //    style.OnPurgedFromStack();
                //    while (st.Count > 0) _styleStack.Push(st.Pop());
                //}
                //else if (depth == 0)
                //{
                //    _styleStack.Pop();
                //    style.OnPurgedFromStack();
                //}

                int index = _styleStack.IndexOf(style);
                if(index > 0)
                {
                    _styleStack.RemoveAt(index);
                    style.OnPurgedFromStack();
                }
                //we don't purge if the stack doesn't contain the style, or if it's the bottom entry... the bottom entry can only be swapped out
            }

            public void ChangeCurrentUnstackedState(IMovementStyle style)
            {
                if(_styleStack.Count > 0)
                {
                    var oldStyle = _styleStack[0];
                    if (oldStyle == style) return;

                    _styleStack[0] = style;
                    if (oldStyle != null) oldStyle.OnPurgedFromStack();
                }
                else
                {
                    this.ChangeState(style);
                }
            }


            private void ChangeState_Imp(IMovementStyle style, float precedence, bool dumpStack)
            {
                if (_owner.InUpdateSequence)
                {
                    //test if we should replace the last ChangeStyle call... test if null so that one can ChangeStyle with a NegativeInfinity precendance
                    if (precedence >= _changeStyleDelayedPrecedence || _changeStyleDelayed == null)
                    {
                        _changeStyleDelayedPrecedence = precedence;
                        _changeStyleDelayed = delegate()
                        {
                            if(dumpStack)
                            {
                                var e = _styleStack.GetEnumerator();
                                while(e.MoveNext())
                                {
                                    if (e.Current != null & e.Current != style) e.Current.OnPurgedFromStack();
                                }
                                _styleStack.Clear();
                            }

                            this.SwapStateOut(style);
                        };
                    }
                }
                else
                {
                    if (dumpStack)
                    {
                        var e = _styleStack.GetEnumerator();
                        while(e.MoveNext())
                        {
                            if (e.Current != null && e.Current != style) e.Current.OnPurgedFromStack();
                        }
                        _styleStack.Clear();
                    }

                    this.SwapStateOut(style);
                }
            }

            private void SwapStateOut(IMovementStyle style)
            {
                if (object.Equals(style, _current)) return;

                var oldState = _current;
                _current = style;

                if (oldState != null) oldState.OnDeactivate(style, _stackingState);
                if (style != null) style.OnActivate(oldState, _stackingState);

                if (this.StateChanged != null) this.StateChanged(this, new StateChangedEventArgs<IMovementStyle>(oldState, style));
                if (_owner.StyleChanged != null) _owner.StyleChanged(_owner, new StyleChangedEventArgs(oldState, style, _stackingState));
            }

            #endregion

            #region IStateStack Interface

            public IEnumerable<IMovementStyle> CurrentStack { get { return _styleStack; } }

            IMovementStyle IStateStack<IMovementStyle>.StackState(IMovementStyle state)
            {
                return this.StackState(state, 0f);
            }

            public IMovementStyle StackState(IMovementStyle style, float precedence = 0)
            {
                if (!object.ReferenceEquals(style, null) && !_stateSupplier.Contains(style)) throw new System.ArgumentException("MovementStyle '" + style.GetType().Name + "' is not a member of the state machine.", "style");

                if (_owner.InUpdateSequence)
                {
                    //test if we should replace the last ChangeStyle call... test if null so that one can ChangeStyle with a NegativeInfinity precendance
                    if (precedence >= _changeStyleDelayedPrecedence || _changeStyleDelayed == null)
                    {
                        //_changeStyleDelayedPrecedence = precedence;
                        //_changeStyleDelayed = delegate()
                        //{
                        //    _styleStack.Push(_stateMachine.Current);
                        //    _stateMachine.ChangeState(style);
                        //};

                        _stackStyleDelayed.Add(delegate()
                        {
                            if (this.Current == style) return;
                            _styleStack.Push(this.Current);
                            _stackingState = true;
                            this.ChangeState_Imp(style, precedence, false);
                            _stackingState = false;
                        }, precedence);
                    }
                }
                else
                {
                    if (this.Current == style) return style;
                    _styleStack.Push(this.Current);
                    _stackingState = true;
                    this.ChangeState_Imp(style, precedence, false);
                    _stackingState = false;
                }

                return style;
            }

            TSub ITypedStateStack<IMovementStyle>.StackState<TSub>()
            {
                return this.StackState<TSub>();
            }

            public T StackState<T>(float precedence = 0) where T : class, IMovementStyle
            {
                var style = _stateSupplier.GetState<T>();
                if (style == null) throw new System.ArgumentException("MovementStyle '" + typeof(T).Name + "' is not a member of the state machine.", "style");
                return this.StackState(style, precedence) as T;
            }

            IMovementStyle ITypedStateStack<IMovementStyle>.StackState(System.Type tp)
            {
                return this.StackState(tp, 0f);
            }

            public IMovementStyle StackState(System.Type tp, float precedence = 0)
            {
                if (!TypeUtil.IsType(tp, typeof(IMovementStyle))) throw new TypeArgumentMismatchException(tp, typeof(IMovementStyle), "tp");

                var style = _stateSupplier.GetState(tp);
                if (style == null) throw new System.ArgumentException("MovementStyle '" + tp.Name + "' is not a member of the state machine.", "style");
                return this.StackState(style, precedence);
            }

            IMovementStyle IStateStack<IMovementStyle>.PopState()
            {
                return this.PopState(0f);
            }

            public IMovementStyle PopState(float precedence = 0f)
            {
                if (_styleStack.Count > 0)
                {
                    var style = _styleStack.Pop();
                    while (_styleStack.Count > 0 && !ReferenceEquals(style, null) && (style.IsDestroyed() || !_stateSupplier.Contains(style)))
                    {
                        style = _styleStack.Pop();
                    }
                    if (!ReferenceEquals(style, null) && (style.IsDestroyed() || !_stateSupplier.Contains(style)))
                    {
                        style = null;
                    }
                    this.ChangeState_Imp(style, precedence, false);
                    return style;
                }
                else
                {
                    return null;
                }
            }

            IMovementStyle IStateStack<IMovementStyle>.PopAllStates()
            {
                return this.PopAllStates(0f);
            }

            public IMovementStyle PopAllStates(float precedence = 0f)
            {
                if (_styleStack.Count > 0)
                {
                    var style = _styleStack.Unshift(); //get the 0 entry, removing it from the deque
                    if (!ReferenceEquals(style, null) && (style.IsDestroyed() || !_stateSupplier.Contains(style)))
                    {
                        style = null;
                    }
                    this.ChangeState_Imp(style, precedence, true);
                    return style;
                }
                else
                {
                    return null;
                }
            }

            #endregion

            #region IStateMachine Interface

            public event StateChangedEventHandler<IMovementStyle> StateChanged;

            public IMovementStyle Current
            {
                get { return _current; }
            }

            public bool Contains(IMovementStyle state)
            {
                return _stateSupplier.Contains(state);
            }

            IMovementStyle IStateMachine<IMovementStyle>.ChangeState(IMovementStyle state)
            {
                return this.ChangeState(state, 0f);
            }

            public IMovementStyle ChangeState(IMovementStyle style, float precedence = 0)
            {
                if (!object.ReferenceEquals(style, null) && !_stateSupplier.Contains(style)) throw new System.ArgumentException("MovementStyle '" + style.GetType().Name + "' is not a member of the state machine.", "style");

                this.ChangeState_Imp(style, precedence, true);
                return style;
            }

            #endregion

            #region ITypedStateMachine Interface

            public bool Contains<T>() where T : class, IMovementStyle
            {
                return _stateSupplier.Contains<T>();
            }

            public bool Contains(System.Type tp)
            {
                return _stateSupplier.Contains(tp);
            }

            public T GetState<T>() where T : class, IMovementStyle
            {
                return _stateSupplier.GetState<T>();
            }

            public IMovementStyle GetState(System.Type tp)
            {
                return _stateSupplier.GetState(tp);
            }

            TSub ITypedStateMachine<IMovementStyle>.ChangeState<TSub>()
            {
                return this.ChangeState<TSub>(0f);
            }

            public T ChangeState<T>(float precedence = 0) where T : class, IMovementStyle
            {
                var style = _stateSupplier.GetState<T>();
                if (style == null) throw new System.ArgumentException("MovementStyle '" + typeof(T).Name + "' is not a member of the state machine.", "style");
                this.ChangeState_Imp(style, precedence, true);
                return style;
            }

            IMovementStyle ITypedStateMachine<IMovementStyle>.ChangeState(System.Type tp)
            {
                return this.ChangeState(tp, 0f);
            }

            public IMovementStyle ChangeState(System.Type tp, float precedence = 0)
            {
                if (!TypeUtil.IsType(tp, typeof(IMovementStyle))) throw new TypeArgumentMismatchException(tp, typeof(IMovementStyle), "tp");

                var style = _stateSupplier.GetState(tp);
                if (style == null) throw new System.ArgumentException("MovementStyle '" + tp.Name + "' is not a member of the state machine.", "style");
                this.ChangeState_Imp(style, precedence, true);
                return style;
            }

            #endregion

            #region IEnumerable Interface

            public IEnumerator<IMovementStyle> GetEnumerator()
            {
                return _stateSupplier.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _stateSupplier.GetEnumerator();
            }

            #endregion

        }

        #endregion

    }

}
