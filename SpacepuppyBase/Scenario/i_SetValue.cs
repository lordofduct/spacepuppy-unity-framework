﻿#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_SetValue : AutoTriggerableMechanism
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
        [SelectableObject]
        [DefaultFromSelf(HandleOnce =true)]
        private UnityEngine.Object _target;
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
                return base.CanTrigger && _target != null && !string.IsNullOrEmpty(_memberName);
            }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = ObjUtil.ReduceIfProxy(_target);
            switch(_mode)
            {
                case SetMode.Set:
                    return DynamicUtil.SetValueRecursively(targ, _memberName, _value.Value);
                case SetMode.Increment:
                    {
                        var v = DynamicUtil.GetValueRecursively(targ, _memberName);
                        v = Evaluator.TrySum(v, _value.Value);
                        return DynamicUtil.SetValueRecursively(targ, _memberName, v);
                    }
                case SetMode.Decrement:
                    {
                        var v = DynamicUtil.GetValueRecursively(targ, _memberName);
                        v = Evaluator.TryDifference(v, _value.Value);
                        return DynamicUtil.SetValueRecursively(targ, _memberName, v);
                    }
                case SetMode.Toggle:
                    {
                        var v = DynamicUtil.GetValueRecursively(targ, _memberName);
                        v = Evaluator.TryToggle(v);
                        return DynamicUtil.SetValueRecursively(targ, _memberName, v);
                    }
            }

            return false;
        }

        #endregion

    }

}
