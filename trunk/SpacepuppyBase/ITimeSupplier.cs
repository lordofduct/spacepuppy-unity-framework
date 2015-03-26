using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{
    public interface ITimeSupplier
    {

        float Total { get; }
        float Delta { get; }
        float Scale { get; set; }
        bool Paused { get; set; }

    }
}
