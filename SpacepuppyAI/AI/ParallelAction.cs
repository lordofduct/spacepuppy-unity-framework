using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI
{

    /// <summary>
    /// Operates each child action at the same time.
    /// </summary>
    public class ParallelAction : AIActionGroup
    {

        #region Fields

        private ParallelPassOptions _passOption;

        private IAIAction[] _actions;

        private bool _inTick;

        #endregion

        #region CONSTRUCTOR

        public ParallelAction()
        {

        }

        public ParallelAction(IEnumerable<IAIAction> actions)
        {
            this.SetActions(actions);
        }

        #endregion

        #region Properties

        public ParallelPassOptions PassOptions
        {
            get { return _passOption; }
            set { _passOption = value; }
        }

        #endregion

        #region Methods

        public override void SetActions(IEnumerable<IAIAction> actions)
        {
            if (actions == null)
            {
                _actions = null;
            }
            else
            {
                _actions = actions.ToArray();
            }
        }

        #endregion

        #region IAIAction Interface

        public override string DisplayName { get { return "[Parrallel]"; } }

        public override int ActionCount { get { return (_actions != null) ? _actions.Length : 0; } }

        protected override IEnumerable<IAIAction> GetActions()
        {
            return _actions;
        }

        protected override ActionResult OnTick(IAIController ai)
        {
            if (_actions == null || _actions.Length == 0) return ActionResult.Success;

            int cntFail = 0;
            int cntSucc = 0;
            int cntWait = 0;
            _inTick = true;
            for(int i = 0; i < _actions.Length; i++)
            {
                if (!_actions[i].Enabled) continue;

                switch(_actions[i].Tick(ai))
                {
                    case ActionResult.Success:
                        cntSucc++;
                        break;
                    case ActionResult.Failed:
                        cntFail++;
                        break;
                    case ActionResult.Waiting:
                        cntWait++;
                        break;
                }

                if(!_inTick)
                {
                    //an action cancelled us
                    return ActionResult.Failed;
                }
            }
            _inTick = false;

            const ParallelPassOptions mask = ParallelPassOptions.SucceedOnAny | ParallelPassOptions.FailOnAny;
            if ((_passOption & mask) > 0)
            {
                if (_passOption.HasFlag(ParallelPassOptions.SucceedOnTie))
                {
                    if (_passOption.HasFlag(ParallelPassOptions.FailOnAny) && cntFail > 0)
                        return ActionResult.Failed;
                    if (_passOption.HasFlag(ParallelPassOptions.SucceedOnAny) && cntSucc > 0)
                        return ActionResult.Success;
                }
                else
                {
                    if (_passOption.HasFlag(ParallelPassOptions.SucceedOnAny) && cntSucc > 0)
                        return ActionResult.Success;
                    if (_passOption.HasFlag(ParallelPassOptions.FailOnAny) && cntFail > 0)
                        return ActionResult.Failed;
                }
            }

            if (cntWait > 0)
                return ActionResult.Waiting;
            else if (cntFail == _actions.Length)
                return ActionResult.Failed;
            else
                return ActionResult.Success;
        }

        protected override void OnReset()
        {
            if (_actions != null)
            {
                foreach (var a in _actions)
                {
                    a.Reset();
                }
            }
            _inTick = false;
        }

        #endregion

    }
}
