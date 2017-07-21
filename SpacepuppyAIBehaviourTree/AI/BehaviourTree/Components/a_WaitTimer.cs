using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree.Components
{

    /// <summary>
    /// TODO - remove obsolete deltaType
    /// </summary>
    public class a_WaitTimer : AIActionComponent
    {

        #region Fields
        
        [SerializeField()]
        private float _duration;

        [SerializeField()]
        private SPTime _timeSupplier;

        [System.NonSerialized()]
        private float _startTime;

        #endregion

        #region Properties

        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
        
        public ITimeSupplier TimeSupplier
        {
            get
            {
                return _timeSupplier.TimeSupplier;
            }
            set
            {
                _timeSupplier.TimeSupplier = value;
            }
        }

        #endregion

        #region IAIAction Interface

        protected override ActionResult OnStart(IAIController ai)
        {
            var ts = _timeSupplier.TimeSupplier;
            if (ts == null) return ActionResult.Failed;

            _startTime = ts.Total;
            return ActionResult.None;
        }

        protected override ActionResult OnTick(IAIController ai)
        {
            var ts = _timeSupplier.TimeSupplier;
            if (ts == null) return ActionResult.Failed;

            var t = ts.Total - _startTime;
            return (t >= _duration) ? ActionResult.Success : ActionResult.Waiting;
        }

        #endregion
        
    }
}
