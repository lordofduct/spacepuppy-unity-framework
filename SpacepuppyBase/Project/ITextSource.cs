using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Project
{
    public interface ITextSource : IEnumerable<string>
    {

        string text { get; }
        string this[int index] { get; }
        int Count { get; }

    }
}
