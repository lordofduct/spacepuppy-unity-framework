using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree
{

    [System.Serializable()]
    public class ConfigurableAIActionGroup : IAIAction, IAIActionGroup
    {

        #region Fields

        [SerializeField()]
        private RepeatMode _repeat;
        [SerializeField()]
        private bool _alwaysSucceed;

        [SerializeField()]
        [Tooltip("Sequential - performs each action in order until one fails\nSelector - performs each action in order until one succeeds\nParrallel - performs all action at the same time\nRandom - performs a random action, include an AIActionWeightSupplier to assign odds")]
        private ActionGroupType _loopMode;
        [SerializeField()]
        private ParallelPassOptions _passOptions = ParallelPassOptions.FailOnAny;

        [System.NonSerialized()]
        private AIActionGroup _loop = AIActionGroup.Null;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// protected constructor used when deserialized by unity. If inheriting from this class, 
        /// make sure this is being called also as a deserialized call, OR you must call SyncActions.
        /// </summary>
        protected ConfigurableAIActionGroup()
        {
            //protected constructor used when deserialized by unity
        }

        public ConfigurableAIActionGroup(ActionGroupType loopMode)
        {
            _loopMode = loopMode;
            this.SetEmpty();
        }

        #endregion

        #region Properties

        public ActionGroupType Mode
        {
            get { return _loopMode; }
            set
            {
                if (_loopMode == value) return;

                _loopMode = value;
                this.Resync();
            }
        }

        public ParallelPassOptions PassOptions
        {
            get { return _passOptions; }
            set
            {
                _passOptions = value;
                if (_loop is ParallelAction) (_loop as ParallelAction).PassOptions = value;
            }
        }

        #endregion

        #region Methods

        public void SetEmpty()
        {
            _loop = AIActionGroup.SyncActions(_loopMode, _loop, Enumerable.Empty<IAIAction>(), _passOptions);
        }

        public void SyncActions(IEnumerable<IAIAction> actions)
        {
            _loop = AIActionGroup.SyncActions(_loopMode, _loop, actions, _passOptions);
        }

        public void SyncActions(GameObject source, bool findWeightSupplier)
        {
            _loop = AIActionGroup.SyncActions(_loopMode, _loop, source, findWeightSupplier, _passOptions);
        }

        private void Resync()
        {
            var actions = (_loop != null) ? _loop : Enumerable.Empty<IAIAction>();
            _loop = AIActionGroup.SyncActions(_loopMode, _loop, actions, _passOptions);
        }

        #endregion

        #region IAIActionGroup Interface

        public virtual string DisplayName
        {
            get { return string.Format("[{0}]", _loopMode); }
        }

        public int ActionCount { get { return _loop.ActionCount; } }

        bool IAIAction.Enabled { get { return true; } }

        public RepeatMode Repeat
        {
            get { return _repeat; }
            set
            {
                _repeat = value;
                _loop.Repeat = value;
            }
        }

        public bool AlwaysSucceed
        {
            get { return _alwaysSucceed; }
            set
            {
                _alwaysSucceed = value;
                _loop.AlwaysSucceed = value;
            }
        }

        public ActionResult ActionState
        {
            get { return _loop.ActionState; }
        }

        public ActionResult Tick(IAIController ai)
        {
            return _loop.Tick(ai);
        }

        public void Reset()
        {
            _loop.Reset();
        }
        
        public IEnumerator<IAIAction> GetEnumerator()
        {
            return _loop.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion




        #region Special Types

        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
        public class ConfigAttribute : System.Attribute
        {

            public bool DrawFlat;

            public ConfigAttribute(bool drawFlat)
            {
                this.DrawFlat = drawFlat;
            }
        }

        #endregion

    }
}
