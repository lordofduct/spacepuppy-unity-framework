using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree.Components
{

    /// <summary>
    /// A conditional branch in the behaviour tree. Works similar to a_ActionGroup, only that it resolves some condition before operating the branch.
    /// </summary>
    public abstract class AITrapActionComponent : SPComponent, IAIAction, IAIActionGroup, IAIEditorSyncActionsCallbackReceiver
    {

        private enum OperationalState
        {
            Inactive = 0,
            Active = 1,
            CompleteAndWaitingToReset = 2
        }

        #region Fields

        [SerializeField()]
        private GameObjectConfigurableAIActionGroup _onSucceed;

        [System.NonSerialized()]
        private ActionResult _trapState;
        
        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.SyncActions();
        }

        #endregion

        #region Properties

        public ActionGroupType Mode
        {
            get { return _onSucceed.Mode; }
            set { _onSucceed.Mode = value; }
        }

        public ParallelPassOptions PassOptions
        {
            get { return _onSucceed.PassOptions; }
            set
            {
                _onSucceed.PassOptions = value;
            }
        }

        public GameObject ActionSequenceContainer
        {
            get { return _onSucceed.ActionSequenceContainer; }
            set { _onSucceed.ActionSequenceContainer = value; }
        }

        #endregion

        #region Methods

        public void SyncActions()
        {
            _onSucceed.SyncActions();
        }

        /// <summary>
        /// Returns true if the trap should evaluate its group, false if should fail.
        /// </summary>
        /// <returns></returns>
        protected abstract ActionResult EvaluateTrap(IAIController ai);

        #endregion

        #region IAIAction Interface

        public virtual string DisplayName
        {
            get
            {
                //return _onSucceed.DisplayName;
                if (_onSucceed.ActionSequenceContainer != null)
                    return string.Format("[Trap {0}] : {1}", _onSucceed.Mode, _onSucceed.ActionSequenceContainer.name);
                else
                    return string.Format("[Trap {0}] : ...", _onSucceed.Mode);
            }
        }

        public int ActionCount { get { return _onSucceed.ActionCount; } }

        bool IAIAction.Enabled { get { return this.isActiveAndEnabled; } }

        public RepeatMode Repeat
        {
            get { return _onSucceed.Repeat; }
            set { _onSucceed.Repeat = value; }
        }

        public bool AlwaysSucceed
        {
            get { return _onSucceed.AlwaysSucceed; }
            set { _onSucceed.AlwaysSucceed = value; }
        }

        public ActionResult ActionState
        {
            get
            {
                if(_trapState == ActionResult.Success)
                {
                    return _onSucceed.ActionState;
                }
                else
                {
                    return _trapState;
                }
            }
        }

        ActionResult IAINode.Tick(IAIController ai)
        {
            switch(_trapState)
            {
                case ActionResult.None:
                case ActionResult.Waiting:
                    {
                        _trapState = this.EvaluateTrap(ai);
                        switch(_trapState)
                        {
                            case ActionResult.None:
                            case ActionResult.Waiting:
                                return _trapState;
                            case ActionResult.Success:
                                if (_onSucceed.ActionCount > 0)
                                {
                                    return _onSucceed.Tick(ai);
                                }
                                else
                                {
                                    return _trapState;
                                }
                            case ActionResult.Failed:
                            default:
                                return ActionResult.Failed;
                        }
                    }
                case ActionResult.Success:
                    return _onSucceed.Tick(ai);
                case ActionResult.Failed:
                default:
                    return ActionResult.Failed;
            }



        }

        void IAINode.Reset()
        {
            _trapState = ActionResult.None;
            _onSucceed.Reset();
        }



        
        public IEnumerator<IAIAction> GetEnumerator()
        {
            if (_onSucceed == null) return Enumerable.Empty<IAIAction>().GetEnumerator();
            return _onSucceed.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }
}
