using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.AI
{

    public interface IAIAction : IAINode
    {

        bool Enabled { get; }

        RepeatMode Repeat { get; set; }
        bool AlwaysSucceed { get; set; }
        ActionResult ActionState { get; }

    }

    public abstract class AIAction : IAIAction
    {

        #region Fields

        private RepeatMode _repeat = RepeatMode.Never;
        private bool _alwaysSucceed;

        [System.NonSerialized()]
        private ActionResult _actionState = ActionResult.None;
        [System.NonSerialized()]
        private bool _resetOnRepeat;

        #endregion

        #region Methods

        protected abstract ActionResult OnTick(IAIController ai);

        protected virtual void OnReset()
        {
            
        }

        #endregion

        #region IAIAction Interface

        public virtual string DisplayName
        {
            get { return this.GetType().Name; }
        }

        bool IAIAction.Enabled { get { return true; } }

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
            set { _actionState = value; }
        }

        public ActionResult Tick(IAIController ai)
        {
            if (_resetOnRepeat)
            {
                this.Reset();
            }
            _actionState = this.OnTick(ai);
            if (_actionState > ActionResult.Waiting)
            {
                _resetOnRepeat = true;

                if (_repeat != RepeatMode.Never && (int)_repeat != (int)_actionState)
                {
                    _actionState = ActionResult.Waiting;
                }

                if(_alwaysSucceed && _actionState == ActionResult.Failed)
                {
                    _actionState = ActionResult.Success;
                }
            }
            return _actionState;
        }

        public void Reset()
        {
            _resetOnRepeat = false;
            _actionState = ActionResult.None;
            this.OnReset();
        }

        #endregion

    }

}
