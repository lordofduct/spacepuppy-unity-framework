using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree
{

    /// <summary>
    /// Selects a random action and performs it, include a weight supplier to create odds.
    /// </summary>
    public class RandomAction : AIActionGroup
    {

        #region Fields

        private IAIAction[] _actions;
        private IAIAction _currentAction;

        private IAIActionWeightSupplier _weightSupplier;

        #endregion
        
        #region CONSTRUCTOR

        public RandomAction()
        {
        }

        public RandomAction(IEnumerable<IAIAction> actions)
        {
            this.SetActions(actions);
        }

        #endregion

        #region Properties

        public IAIActionWeightSupplier WeightSupplier
        {
            get { return _weightSupplier; }
            set { _weightSupplier = value; }
        }

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

        public override string DisplayName { get { return "[Random]"; } }

        public override int ActionCount { get { return (_actions != null) ? _actions.Length : 0; } }

        protected override IEnumerable<IAIAction> GetActions()
        {
            return _actions;
        }

        protected override ActionResult OnTick(IAIController ai)
        {
            if(_currentAction == null)
            {
                if (_actions == null) return ActionResult.Success;
                this.Reset();

                if (_weightSupplier != null)
                {
                    _currentAction = _actions.PickRandom((a) =>
                    {
                        return _weightSupplier.GetWeight(a);
                    });
                }
                else
                {
                    _currentAction = _actions.PickRandom();
                }
                if (_currentAction == null) return ActionResult.Success;
            }

            if(!_currentAction.Enabled)
            {
                _currentAction = null;
                return ActionResult.Failed;
            }

            var result = _currentAction.Tick(ai);
            if (result != ActionResult.Waiting)
            {
                if (_weightSupplier != null)
                {
                    switch(result)
                    {
                        case ActionResult.Failed:
                            _weightSupplier.OnActionFailure(_currentAction);
                            break;
                        case ActionResult.Success:
                            _weightSupplier.OnActionSuccess(_currentAction);
                            break;
                    }
                }
                _currentAction = null;
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
            _currentAction = null;
        }

        #endregion

    }
}
