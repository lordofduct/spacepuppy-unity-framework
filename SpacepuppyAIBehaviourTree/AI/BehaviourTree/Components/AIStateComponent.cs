#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree.Components
{

    /// <summary>
    /// Used with AITreeComponent
    /// </summary>
    public class AIStateComponent : SPComponent, IAIState, IAIActionGroup, IAIEditorSyncActionsCallbackReceiver
    {

        #region Fields

        [SerializeField()]
        private ConfigurableAIActionGroup _loop;

        [SerializeField()]
        private Trigger _onEnterState;

        [System.NonSerialized()]
        private bool _isActive;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.SyncActions();
        }

        #endregion

        #region Properties

        public IAIActionGroup Loop { get { return _loop; } }

        #endregion

        #region Methods

        public void SyncActions()
        {
            _loop.SyncActions(this.gameObject, true);
        }

        protected virtual void OnStateEntered(IAIStateMachine machine, IAIState lastState)
        {

        }

        protected virtual void OnStateExited(IAIStateMachine machine, IAIState nextState)
        {

        }

        #endregion

        #region IAIState Interface

        string IAINode.DisplayName
        {
            get
            {
                if (Application.isPlaying)
                    return string.Format("[State {0} - {1}] ({2})", _loop.Mode, this.name, (this.IsActive) ? "active" : "inactive");
                else
                    return string.Format("[State {0} - {1}]", _loop.Mode, this.name);
            }
        }

        int IAIActionGroup.ActionCount { get { return _loop.ActionCount; } }

        public bool IsActive { get { return _isActive; } }

        public IAIStateMachine StateMachine
        {
            get;
            private set;
        }

        void IAIState.Init(IAIStateMachine machine)
        {
            this.StateMachine = machine;
        }

        void IAIState.OnStateEntered(IAIStateMachine machine, IAIState lastState)
        {
            _isActive = true;
            this.OnStateEntered(machine, lastState);
            _onEnterState.ActivateTrigger(this, null);
        }

        void IAIState.OnStateExited(IAIStateMachine machine, IAIState nextState)
        {
            _loop.Reset();
            this.OnStateExited(machine, nextState);
            _isActive = false;
        }




        ActionResult IAINode.Tick(IAIController ai)
        {
            return _loop.Tick(ai);
        }

        void IAINode.Reset()
        {
            _loop.Reset();
        }
        
        public IEnumerator<IAIAction> GetEnumerator()
        {
            if (_loop == null) return Enumerable.Empty<IAIAction>().GetEnumerator();
            return _loop.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }

}
