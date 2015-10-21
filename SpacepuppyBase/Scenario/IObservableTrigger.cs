namespace com.spacepuppy.Scenario
{
    public interface IObservableTrigger : INotificationDispatcher, IComponent
    {

        bool IsComplex { get; }
        string[] GetComplexIds();

    }

}
