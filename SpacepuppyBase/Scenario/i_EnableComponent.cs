using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_EnableComponent : TriggerableMechanism
    {

        public enum EnableMode
        {
            Enable = 0,
            Disable = 1,
            Toggle = 2
        }

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(Component))]
        private TriggerableTargetObject _target = new TriggerableTargetObject(TriggerableTargetObject.TargetSource.Configurable);

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Mode")]
        private EnableMode _mode;

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        private float _delay = 0f;

        #endregion

        #region Methods

        private void SetEnabledByMode(object arg)
        {
            var targ = _target.GetTarget<Component>(arg);

            switch (_mode)
            {
                case EnableMode.Enable:
                    targ.SetEnabled(true);
                    break;
                case EnableMode.Disable:
                    targ.SetEnabled(false);
                    break;
                case EnableMode.Toggle:
                    targ.SetEnabled(!targ.IsEnabled());
                    break;
            }
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            if (_delay > 0f)
            {
                this.Invoke(() =>
                {
                    this.SetEnabledByMode(arg);
                }, _delay);
            }
            else
            {
                this.SetEnabledByMode(arg);
            }

            return true;
        }

        #endregion

    }

}