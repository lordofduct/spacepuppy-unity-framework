#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.AI.Components
{
    public class a_ActionGroup : SPComponent, IAIAction, IAIActionGroup, IAIEditorSyncActionsCallbackReceiver
    {

        #region Fields
        
        [SerializeField()]
        [ConfigurableAIActionGroup.Config(true)]
        private GameObjectConfigurableAIActionGroup _loop;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.SyncActions();
        }

        #endregion

        #region Properties

        public ParallelPassOptions PassOptions
        {
            get { return _loop.PassOptions; }
            set
            {
                _loop.PassOptions = value;
            }
        }

        #endregion

        #region Methods

        public void SyncActions()
        {
            _loop.SyncActions();
        }

        #endregion

        #region IAIAction Interface

        public string DisplayName
        {
            get
            {
                return _loop.DisplayName;
            }
        }

        public int ActionCount
        {
            get
            {
                return _loop.ActionCount;
            }
        }

        bool IAIAction.Enabled { get { return this.isActiveAndEnabled; } }

        public RepeatMode Repeat
        {
            get { return _loop.Repeat; }
            set { _loop.Repeat = value; }
        }

        public bool AlwaysSucceed
        {
            get { return _loop.AlwaysSucceed; }
            set { _loop.AlwaysSucceed = value; }
        }

        public ActionResult ActionState
        {
            get { return _loop.ActionState; }
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
