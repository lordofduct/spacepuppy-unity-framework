using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_SetValue : TriggerableMechanism
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
        [SelectableComponent()]
        private Component _target;
        [SerializeField()]
        private string _memberName;
        [SerializeField()]
        private VariantReference _value;
        [SerializeField()]
        private SetMode _mode;

        #endregion


        #region TriggerableMechanism Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _target != null && _memberName != null && _value != null;
            }
        }

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            switch(_mode)
            {
                case SetMode.Set:
                    return DynamicUtil.SetValue(_target, _memberName, _value.Value);
                case SetMode.Increment:
                    {
                        var v = DynamicUtil.GetValue(_target, _memberName);
                        v = DynamicUtil.TrySum(v, _value.Value);
                        return DynamicUtil.SetValue(_target, _memberName, v);
                    }
                case SetMode.Decrement:
                    {
                        var v = DynamicUtil.GetValue(_target, _memberName);
                        v = DynamicUtil.TryDifference(v, _value.Value);
                        return DynamicUtil.SetValue(_target, _memberName, v);
                    }
                case SetMode.Toggle:
                    {
                        var v = DynamicUtil.GetValue(_target, _memberName);
                        v = DynamicUtil.TryToggle(v);
                        return DynamicUtil.SetValue(_target, _memberName, v);
                    }
            }

            return false;
        }

        #endregion

    }

}
