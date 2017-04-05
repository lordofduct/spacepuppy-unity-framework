using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Serialization.Json
{
    public class JsonException : System.Exception
    {

        public JsonException(string msg) : base(msg)
        {

        }

        public JsonException(string msg, System.Exception innerException) : base(msg, innerException)
        {

        }

    }
}
