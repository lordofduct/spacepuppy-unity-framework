using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_Destroy : TriggerableMechanism
    {

        [TriggerableTargetObject.Config(typeof(UnityEngine.GameObject))]
        public TriggerableTargetObject Target;

        public float Delay = 0f;

        #region ITriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this.Target.GetTarget<UnityEngine.Object>(arg, false);
            if (targ == null) return false;

            if (this.Delay > 0f)
            {
                this.Invoke(() =>
                {
                    if (targ is GameObject)
                    {
                        (targ as GameObject).Kill();
                    }
                    else
                    {
                        Object.Destroy(targ);
                    }
                }, this.Delay);
            }
            else
            {
                if (targ is GameObject)
                {
                    (targ as GameObject).Kill();
                }
                else
                {
                    Object.Destroy(targ);
                }
            }

            return true;
        }

        #endregion

    }

}