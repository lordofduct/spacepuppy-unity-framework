using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{
    public interface IForeachEnumerator<T>
    {

        void Foreach(System.Action<T> callback);

    }
}
