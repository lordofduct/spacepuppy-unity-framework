using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class i_SetValueOnTarget : AutoTriggerableMechanism
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
        [TypeReference.Config(typeof(UnityEngine.Object), dropDownStyle = TypeDropDownListingStyle.ComponentMenu)]
        private TypeReference _restrictedType;

        [SerializeField()]
        private string _memberName;
        [SerializeField()]
        private VariantReference[] _values;
        [SerializeField()]
        private SetMode _mode;

        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            //var targ = _target.GetTarget<object>(arg);
            //if (targ == null) return false;

            //if (_restrictedType != null && _restrictedType.Type != null) targ = ObjUtil.GetAsFromSource(_restrictedType.Type, targ);
            //if (targ == null) return false;

            object targ;
            if(_restrictedType != null && _restrictedType.Type != null)
                targ = _target.GetTarget(_restrictedType.Type, arg);
            else
                targ = _target.GetTarget<object>(arg);
            if (targ == null) return false;

            switch (_mode)
            {
                case SetMode.Set:
                    {
                        if (_values == null || _values.Length == 0) return false;
                        if (_values.Length == 1)
                        {
                            return DynamicUtil.SetValue(targ, _memberName, _values[0].Value);
                        }
                        else
                        {
                            var args = (from v in _values select v.Value).ToArray();
                            DynamicUtil.InvokeMethod(targ, _memberName, args);
                            return true;
                        }
                    }
                case SetMode.Increment:
                    {
                        if (_values == null || _values.Length == 0) return false;
                        var v = DynamicUtil.GetValue(targ, _memberName);
                        v = Evaluator.TrySum(v, _values[0].Value);
                        return DynamicUtil.SetValue(targ, _memberName, v);
                    }
                case SetMode.Decrement:
                    {
                        if (_values == null || _values.Length == 0) return false;
                        var v = DynamicUtil.GetValue(targ, _memberName);
                        v = Evaluator.TryDifference(v, _values[0].Value);
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
