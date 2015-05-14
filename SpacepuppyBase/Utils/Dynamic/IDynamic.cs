using System;
using System.Collections.Generic;

namespace com.spacepuppy.Utils.Dynamic
{
    public interface IDynamic
    {
        object this[string sMemberName] { get; set; }

        void SetValue(string sMemberName, object value);
        object GetValue(string sMemberName, params object[] args);
    }
}
