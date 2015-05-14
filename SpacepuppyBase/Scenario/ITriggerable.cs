using UnityEngine;

namespace com.spacepuppy.Scenario
{

    public interface ITriggerableMechanism : IComponent
    {
        int Order { get; }
        bool CanTrigger { get; }
        object Trigger(object arg);
    }

}
