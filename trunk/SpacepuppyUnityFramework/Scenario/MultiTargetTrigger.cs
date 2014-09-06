using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class MultiTargetTrigger : SPComponent, ITriggerableMechanism
    {

        #region Fields

        public TriggerTarget[] Targets;

        #endregion

        #region ITriggerableMechanism Interface

        public bool CanTrigger
        {
            get { return this.enabled && Targets != null; }
        }

        public object Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            foreach(var targ in Targets)
            {
                if (targ != null) targ.Trigger();
            }

            return true;
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        public class TriggerTarget
        {

            [ComponentTypeRestriction(typeof(ITriggerableMechanism), order = 1)]
            public Component Triggerable;
            public VariantReference TriggerableArg;
            public bool TriggerAllOnTarget = true;

            public void Trigger()
            {
                var arg = (TriggerableArg != null) ? TriggerableArg.Value : null;
                if (TriggerAllOnTarget)
                {
                    if (Triggerable != null)
                    {
                        foreach (var t in Triggerable.GetLikeComponents<ITriggerableMechanism>())
                        {
                            t.Trigger(arg);
                        }
                    }
                }
                else
                {
                    if (Triggerable != null && Triggerable is ITriggerableMechanism)
                    {
                        (Triggerable as ITriggerableMechanism).Trigger(arg);
                    }
                }
            }

        }

        #endregion

    }
}
