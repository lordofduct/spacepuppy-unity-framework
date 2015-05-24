using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{

    public interface IRandom
    {

        float Next();
        double NextDouble();
        int Next(int size);
        int Next(int low, int high);

    }

}
