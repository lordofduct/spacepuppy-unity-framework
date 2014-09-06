using UnityEngine;

namespace com.spacepuppy.Scenario
{

    public interface ITriggerableMechanism : IComponent
    {
        bool CanTrigger { get; }
        object Trigger(object arg);
    }

}
