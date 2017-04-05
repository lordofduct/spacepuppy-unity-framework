using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.AI
{
    public interface IAINode
    {

        string DisplayName { get; }

        ActionResult Tick(IAIController ai);
        void Reset();

    }
}
