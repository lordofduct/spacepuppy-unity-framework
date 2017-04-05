using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI
{

    /// <summary>
    /// Iterates over children until one fails.
    /// </summary>
    public class SequenceAction : AIActionGroup
    {

        #region Fields

        private IAIAction[] _actions;
        private int _currentActionIndex;

        #endregion
        
        #region CONSTRUCTOR

        public SequenceAction()
        {
        }

        public SequenceAction(IEnumerable<IAIAction> actions)
        {
            this.SetActions(actions);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override void SetActions(IEnumerable<IAIAction> actions)
        {
            if (actions == null)
                _actions = null;
            else
                _actions = actions.ToArray();
        }

        #endregion

        #region IAIAction Interface

        public override string DisplayName { get { return "[Sequence]"; } }

        public override int ActionCount { get { return (_actions != null) ? _actions.Length : 0; } }

        protected override IEnumerable<IAIAction> GetActions()
        {
            return _actions;
        }

        protected override ActionResult OnTick(IAIController ai)
        {
            if (_actions == null || _actions.Length == 0) return ActionResult.Success;

            if (_currentActionIndex < 0)
            {
                _currentActionIndex = 0;
            }

            var result = ActionResult.Success;
            var a = _actions[_currentActionIndex];
            while(a != null)
            {
                if(!a.Enabled)
                {
                    _currentActionIndex++;
                    if (_currentActionIndex < _actions.Length)
                    {
                        a = _actions[_currentActionIndex];
                    }
                    else
                    {
                        a = null;
                        _currentActionIndex = -1;
                    }
                    continue;
                }

                result = a.Tick(ai);
                if(result == ActionResult.Success)
                {
                    _currentActionIndex++;
                    if(_currentActionIndex < _actions.Length)
                    {
                        a = _actions[_currentActionIndex];
                    }
                    else
                    {
                        a = null;
                        _currentActionIndex = -1;
                    }
                }
                else if(result == ActionResult.Failed)
                {
                    _currentActionIndex = -1;
                    break;
                }
                else
                {
                    break;
                }
            }
            return result;
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
            _currentActionIndex = -1;
        }

        #endregion

    }
}
