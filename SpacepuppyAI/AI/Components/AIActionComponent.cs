using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.AI.Components
{
    public abstract class AIActionComponent : SPComponent, IAIAction
    {

        private enum OperationalState
        {
            Inactive = 0,
            Active = 1,
            CompleteAndWaitingToReset = 2
        }

        #region Fields

        [SerializeField()]
        private RepeatMode _repeat = RepeatMode.Never;
        [SerializeField()]
        private bool _alwaysSucceed;

        [System.NonSerialized()]
        private ActionResult _actionState = ActionResult.None;
        [System.NonSerialized()]
        private OperationalState _internalState;

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// Return false if should cancel immediately
        /// </summary>
        /// <param name="ai"></param>
        /// <returns></returns>
        protected virtual ActionResult OnStart(IAIController ai)
        {
            return ActionResult.None;
        }

        protected virtual ActionResult OnTick(IAIController ai)
        {
            return ActionResult.Success;
        }

        protected virtual void OnComplete(IAIController ai)
        {

        }

        protected virtual void OnReset()
        {

        }

        #endregion

        #region IAIAction

        public virtual string DisplayName { get { return this.GetType().Name; } }

        bool IAIAction.Enabled { get { return this.isActiveAndEnabled; } }

        public RepeatMode Repeat
        {
            get { return _repeat; }
            set { _repeat = value; }
        }

        public bool AlwaysSucceed
        {
            get { return _alwaysSucceed; }
            set { _alwaysSucceed = value; }
        }

        public ActionResult ActionState
        {
            get { return _actionState; }
            //protected set { _actionState = value; }
        }

        public ActionResult Tick(IAIController ai)
        {
            if (_internalState == OperationalState.CompleteAndWaitingToReset)
            {
                this.Reset();
                _actionState = this.OnStart(ai);
                if (_actionState > ActionResult.Waiting) return _actionState;
            }
            else if(_actionState == ActionResult.None)
            {
                _actionState = this.OnStart(ai);
                if (_actionState > ActionResult.Waiting) return _actionState;
            }

            _internalState = OperationalState.Active;
            _actionState = this.OnTick(ai);
            if (_internalState == OperationalState.Inactive)
            {
                //Reset was called during tick, probably by a 'ChangeState' call on a parent state machine.
                //exit cleanly
                _actionState = ActionResult.None;
                return _actionState;
            }

            if(_actionState > ActionResult.Waiting)
            {
                _internalState = OperationalState.CompleteAndWaitingToReset;

                this.OnComplete(ai);
                if(_repeat != RepeatMode.Never && (int)_repeat != (int)_actionState)
                {
                    _actionState = ActionResult.Waiting;
                }

                if (_alwaysSucceed && _actionState == ActionResult.Failed)
                {
                    _actionState = ActionResult.Success;
                }
            }

            return _actionState;
        }

        public void Reset()
        {
            _actionState = ActionResult.None;
            _internalState = OperationalState.Inactive;
            this.OnReset();
        }

        #endregion

    }
}
