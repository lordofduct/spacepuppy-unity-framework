using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_Enable : TriggerableMechanism
    {

        public enum EnableMode
        {
            TriggerArg = -1,
            Enable = 0,
            Disable = 1,
            Toggle = 2
        }

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(GameObject))]
        [UnityEngine.Serialization.FormerlySerializedAs("TargetObject")]
        private TriggerableTargetObject _targetObject = new TriggerableTargetObject(TriggerableTargetObject.TargetSource.Configurable);

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
            var targ = _targetObject.GetTarget<GameObject>(arg);
            if (targ == null) return;

            var mode = _mode;
            if (mode == EnableMode.TriggerArg) mode = ConvertUtil.ToEnum<EnableMode>(arg, EnableMode.Toggle);

            switch (mode)
            {
                case EnableMode.Enable:
                    targ.SetActive(true);
                    break;
                case EnableMode.Disable:
                    targ.SetActive(false);
                    break;
                case EnableMode.Toggle:
                    targ.SetActive(!targ.activeSelf);
                    break;
            }
        }

        #endregion

        #region ITriggerableMechanism Interface

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