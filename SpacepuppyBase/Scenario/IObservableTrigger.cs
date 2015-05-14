using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Scenario
{
    public interface IObservableTrigger : INotificationDispatcher, IComponent
    {

        bool IsComplex { get; }
        string[] GetComplexIds();

    }

}
