using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [System.Obsolete("Use i_SetValueOnTarget Instead.")]
    public class i_SetValueOnEntity : AutoTriggerableMechanism
    {
        
        #region Fields

        [SerializeField()]
        private TriggerableTargetObject _target = new TriggerableTargetObject(true);

        [SerializeField()]
        private bool _searchEntity = true;

        [SerializeField()]
        [TypeReference.Config(typeof(Component), dropDownStyle = TypeDropDownListingStyle.ComponentMenu)]
        private TypeReference _componentType;

        [SerializeField()]
        private string _memberName;
        [SerializeField()]
        private VariantReference _value;
        [SerializeField()]
        private i_SetValue.SetMode _mode;

        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var go = _target.GetTarget<GameObject>(arg);
            if (go == null) return false;

            var targ = (_searchEntity) ? go.FindComponent(_componentType.Type) : go.GetComponent(_componentType.Type);
            if (targ == null) return false;

            switch (_mode)
            {
                case i_SetValue.SetMode.Set:
                    return DynamicUtil.SetValue(targ, _memberName, _value.Value);
                case i_SetValue.SetMode.Increment:
                    {
                        var v = DynamicUtil.GetValue(targ, _memberName);
                        v = Evaluator.TrySum(v, _value.Value);
                        return DynamicUtil.SetValue(targ, _memberName, v);
                    }
                case i_SetValue.SetMode.Decrement:
                    {
                        var v = DynamicUtil.GetValue(targ, _memberName);
                        v = Evaluator.TryDifference(v, _value.Value);
                        return DynamicUtil.SetValue(targ, _memberName, v);
                    }
                case i_SetValue.SetMode.Toggle:
                    {
                        var v = DynamicUtil.GetValue(targ, _memberName);
                        v = Evaluator.TryToggle(v);
                        return DynamicUtil.SetValue(targ, _memberName, v);
                    }
            }

            return false;
        }

        #endregion

    }
}
