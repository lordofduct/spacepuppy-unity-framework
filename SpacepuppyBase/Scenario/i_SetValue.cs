#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Dynamic;

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
        [SelectableObject()]
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
                        v = Evaluator.TrySum(v, _value.Value);
                        return DynamicUtil.SetValue(_target, _memberName, v);
                    }
                case SetMode.Decrement:
                    {
                        var v = DynamicUtil.GetValue(_target, _memberName);
                        v = Evaluator.TryDifference(v, _value.Value);
                        return DynamicUtil.SetValue(_target, _memberName, v);
                    }
                case SetMode.Toggle:
                    {
                        var v = DynamicUtil.GetValue(_target, _memberName);
                        v = Evaluator.TryToggle(v);
                        return DynamicUtil.SetValue(_target, _memberName, v);
                    }
            }

            return false;
        }

        #endregion

    }

}
