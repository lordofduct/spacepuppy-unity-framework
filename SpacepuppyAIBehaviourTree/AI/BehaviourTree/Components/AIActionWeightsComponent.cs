#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils.Diminish;

namespace com.spacepuppy.AI.BehaviourTree.Components
{
    public class AIActionWeightsComponent : SPComponent, IAIActionWeightSupplier
    {

        #region Fields

        [SerializeField()]
        private float _defaultWeight = 1f;

        [SerializeField()]
        private Component[] _actions;
        [SerializeField()]
        private DiminishingWeightOverDuration[] _weights;

        [System.NonSerialized()]
        private bool _valid;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _valid = (_actions != null && _weights != null && _actions.Length > 0 && _actions.Length < _weights.Length);
        }

        #endregion

        #region IAIActionWeightSupplier Interface

        public float GetWeight(IAIAction action)
        {
            if(_valid)
            {
                int i = System.Array.IndexOf(_actions, action);
                if (i >= 0) return _weights[i].GetAdjustedWeight();
                else return _defaultWeight;
            }
            else
            {
                return _defaultWeight;
            }
        }

        public void OnActionSuccess(IAIAction action)
        {
            if(_valid)
            {
                int i = System.Array.IndexOf(_actions, action);
                if (i >= 0) _weights[i].Signal();
            }
        }

        public void OnActionFailure(IAIAction action)
        {
            if (_valid)
            {
                int i = System.Array.IndexOf(_actions, action);
                if (i >= 0) _weights[i].Signal();
            }
        }

        #endregion

    }
}
