using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_SetValueOnEntity : AutoTriggerableMechanism
    {

        public enum SetMode
        {
            Set = 0,
            Increment = 1,
            Decrement = 2,
            Toggle = 3
        }

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
        private SetMode _mode;

        #endregion

        #region Methods

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var go = _target.GetTarget<GameObject>(arg);
            if (go == null) return false;

            var targ = (_searchEntity) ? go.FindComponent(_componentType.Type) : go.GetComponent(_componentType.Type);
            if (targ == null) return false;

            switch (_mode)
            {
                case SetMode.Set:
                    return DynamicUtil.SetValue(targ, _memberName, _value.Value);
                case SetMode.Increment:
                    {
                        var v = DynamicUtil.GetValue(targ, _memberName);
                        v = Evaluator.TrySum(v, _value.Value);
                        return DynamicUtil.SetValue(targ, _memberName, v);
                    }
                case SetMode.Decrement:
                    {
                        var v = DynamicUtil.GetValue(targ, _memberName);
                        v = Evaluator.TryDifference(v, _value.Value);
                        return DynamicUtil.SetValue(targ, _memberName, v);
                    }
                case SetMode.Toggle:
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
