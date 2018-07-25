using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_Destroy : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Target")]
        [TriggerableTargetObject.Config(typeof(UnityEngine.GameObject))]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Delay")]
        [TimeUnitsSelector()]
        private float _delay = 0f;

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public float Delay
        {
            get { return _delay; }
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this._target.GetTarget<UnityEngine.Object>(arg);
            if (targ == null) return false;

            if (_delay > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    ObjUtil.SmartDestroy(targ);
                }, _delay);
            }
            else
            {
                ObjUtil.SmartDestroy(targ);
            }

            return true;
        }

        #endregion

    }

}