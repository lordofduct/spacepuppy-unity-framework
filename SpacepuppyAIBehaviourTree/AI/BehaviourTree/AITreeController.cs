#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using System;
using System.Reflection;

namespace com.spacepuppy.AI.BehaviourTree
{

    [System.Serializable()]
    public class AITreeController : SPComponent, IAIController, IAIActionGroup, IAIEditorSyncActionsCallbackReceiver
    {

        public enum Mode
        {
            ChildSourced = 0,
            SelfSourced = 1
        }

        #region Fields

        [SerializeField()]
        private ConfigurableAIActionGroup _loop;

        [SerializeField()]
        [MinRange(0.02f)]
        private float _interval = 0.1f;

        [SerializeField()]
        private AIVariableCollection _variables;

        [System.NonSerialized()]
        private float _t;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.SyncActions();
        }

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            _t = Time.time;
        }

        #endregion

        #region Properties

        public IAIActionGroup ActionLoop { get { return _loop; } }

        public ActionGroupType ActionLoopMode
        {
            get { return _loop.Mode; }
            set { _loop.Mode = value; }
        }

        public AIVariableCollection Variables { get { return _variables; } }

        public float Interval
        {
            get { return _interval; }
            set
            {
                _interval = Mathf.Max(0.02f, value);
            }
        }

        #endregion

        #region Methods

        public void SyncActions()
        {
            if (_loop == null)
                _loop = new ConfigurableAIActionGroup(ActionGroupType.Sequential);
            _loop.SyncActions(this.gameObject, true);
        }

        private void Update()
        {
            if (_loop == null) return;

            if (Time.time - _t > _interval)
            {
                _t = Time.time;
                if (_loop.Tick(this) > ActionResult.Waiting)
                {
                    _loop.Reset();
                }
            }
        }

        #endregion

        #region IAIActionGroup Interface

        string IAINode.DisplayName { get { return string.Format("ROOT ({0})", this.name); } }

        int IAIActionGroup.ActionCount { get { return (_loop != null) ? _loop.ActionCount : 0; } }

        ActionResult IAINode.Tick(IAIController ai)
        {
            return ActionResult.Failed;
        }

        void IAINode.Reset()
        {
            //do nothing
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
