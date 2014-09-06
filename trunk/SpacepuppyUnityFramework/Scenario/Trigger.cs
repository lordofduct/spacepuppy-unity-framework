using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public abstract class Trigger : SPComponent
    {

        #region Fields

        [ComponentTypeRestriction(typeof(ITriggerableMechanism), order = 1)]
        public Component Triggerable;
        public VariantReference TriggerableArg;
        public bool TriggerAllOnTarget = true;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if(!this.HasComponent<MultiTargetTrigger>() && Triggerable == null)
            {
                Triggerable = this.GetFirstLikeComponent<ITriggerableMechanism>() as Component;
            }
        }

        #endregion

        #region Methods

        public void ActivateTrigger()
        {
            if (this.HasComponent<MultiTargetTrigger>())
            {
                this.GetComponent<MultiTargetTrigger>().Trigger(null);
            }
            else
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
