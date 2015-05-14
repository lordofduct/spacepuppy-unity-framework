using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenario
{

    public enum TriggerActivationType
    {
        TriggerAllOnTarget = 0,
        TriggerSelectedTarget = 1,
        SendMessage = 2,
        CallMethodOnSelectedTarget = 3
    }

}
