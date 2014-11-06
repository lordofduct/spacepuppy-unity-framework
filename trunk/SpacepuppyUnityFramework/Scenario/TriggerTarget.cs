using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenario
{
    [System.Serializable()]
    public class TriggerTarget
    {

        [ComponentTypeRestriction(typeof(ITriggerableMechanism), order = 1)]
        public Component Triggerable;
        public VariantReference[] TriggerableArgs;
        public TriggerActivationType ActivationType;
        public string MethodName;

    }
}
