using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.AI.Sensors
{
    public interface IAspect : IGameObjectSource
    {

        bool IsActive { get; }

        float Precedence { get; }

    }
}
