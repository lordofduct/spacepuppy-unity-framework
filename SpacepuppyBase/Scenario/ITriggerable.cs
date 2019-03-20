namespace com.spacepuppy.Scenario
{

    public interface ITriggerableMechanism
    {

        int Order { get; }
        /// <summary>
        /// For consistent behaviour this should return false if 'isActiveAndEnabled' is false.
        /// </summary>
        bool CanTrigger { get; }
        bool Trigger(object sender, object arg);

    }

    public interface IBlockingTriggerableMechanism : ITriggerableMechanism
    {
        
        bool Trigger(object sender, object arg, BlockingTriggerYieldInstruction instruction);

    }
    
}
