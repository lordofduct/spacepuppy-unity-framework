using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;


namespace com.spacepuppy.Render
{
    public class i_SetMaterialProperty : TriggerableMechanism
    {

        public enum SetMode
        {
            Set = 0,
            Increment = 1,
            Decrement = 2
        }

        #region Fields

        [SerializeField()]
        private MaterialPropertyReference _target;
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
                return base.CanTrigger && _target != null && _value != null;
            }
        }

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            switch (_mode)
            {
                case SetMode.Set:
                    _target.SetValue(_value.Value);
                    return true;
                case SetMode.Increment:
                    {
                        var v = _target.GetValue();
                        v = Evaluator.TrySum(v, _value.Value);
                        _target.SetValue(v);
                        return true;
                    }
                case SetMode.Decrement:
                    {
                        var v = _target.GetValue();
                        v = Evaluator.TryDifference(v, _value.Value);
                        _target.SetValue(v);
                        return true;
                    }
            }

            return false;
        }

        #endregion

    }
}
