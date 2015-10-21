namespace com.spacepuppy.Scenario
{

    public interface ITriggerableMechanism : IComponent
    {

        int Order { get; }
        bool CanTrigger { get; }
        bool Trigger(object arg);

    }

    public interface IBlockingTriggerableMechanism : ITriggerableMechanism
    {

        bool Trigger(object arg, BlockingTriggerYieldInstruction instruction);

    }
    
}
