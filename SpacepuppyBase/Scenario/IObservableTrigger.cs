namespace com.spacepuppy.Scenario
{

    /// <summary>
    /// A trigger that can be observed by other triggers for activation. 
    /// </summary>
    public interface IObservableTrigger
    {

        /// <summary>
        /// Returns an array of all events that may trigger.
        /// </summary>
        /// <returns></returns>
        Trigger[] GetTriggers();

    }

    /// <summary>
    /// A trigger that can be observed by other triggers. Of which 2 of the events that may trigger are an Enter and Exit state. 
    /// Think like a collider enter/exit, or a mouse enter/exit, or other event where there is a start and end.
    /// </summary>
    public interface IOccupiedTrigger : IObservableTrigger
    {

        Trigger EnterTrigger { get; }
        Trigger ExitTrigger { get; }
        bool IsOccupied { get; }

    }

}
