using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;

namespace com.spacepuppy.AI
{
    
    public class TriggerableMechanismAsAIActionWrapper : IAIAction
    {

        #region Fields

        private ITriggerableMechanism _mechanism;
        private ActionResult _state;
        private BlockingTriggerYieldInstruction _blocking;

        #endregion

        #region CONSTRUCTOR

        public TriggerableMechanismAsAIActionWrapper(ITriggerableMechanism mechanism)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");
            _mechanism = mechanism;
        }

        #endregion

        #region IAIAction Interface

        bool IAIAction.Enabled { get { return _mechanism.CanTrigger; } }

        public ActionResult ActionState
        {
            get
            {
                return _state;
            }
        }

        public bool AlwaysSucceed
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public string DisplayName
        {
            get
            {
                return (_mechanism != null && _mechanism.component != null) ? _mechanism.component.name + "(a_i_Triggerable_Wrapper)" : "null (a_i_Triggerable_Wrapper)";
            }
        }

        public RepeatMode Repeat
        {
            get
            {
                return RepeatMode.Never;
            }
            set
            {

            }
        }

        public void Reset()
        {
            _state = ActionResult.None;
        }

        public ActionResult Tick(IAIController ai)
        {
            if (_blocking != null)
            {
                if(_blocking.IsComplete)
                {
                    _blocking = null;
                    _state = ActionResult.Success;
                }
                else
                {
                    _state = ActionResult.Waiting;
                }
            }
            else if(_mechanism != null)
            {
                _state = ActionResult.Success;
                if (_mechanism.CanTrigger)
                {
                    if(_mechanism is IBlockingTriggerableMechanism)
                    {
                        var obj = BlockingTriggerYieldInstruction.Create();
                        (_mechanism as IBlockingTriggerableMechanism).Trigger(ai, obj);
                        if(obj.Count > 0)
                        {
                            _state = ActionResult.Waiting;
                            _blocking = obj;
                        }
                        else
                        {
                            (obj as System.IDisposable).Dispose();
                        }
                    }
                    else
                    {
                        _mechanism.Trigger(ai);
                    }
                }
            }
            else
            {
                _state = ActionResult.Failed;
            }

            return _state;
        }

        #endregion

    }
}
