using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace com.spacepuppy.Serialization.Json
{
    public class JsonFormatter : IFormatter
    {

        #region Fields

        private StringBuilder _builder = new StringBuilder();

        #endregion

        #region IFormatter Interface

        public SerializationBinder Binder
        {
            get;
            set;
        }

        public StreamingContext Context
        {
            get;
            set;
        }

        public ISurrogateSelector SurrogateSelector
        {
            get;
            set;
        }

        public void Serialize(Stream serializationStream, object graph)
        {

        }

        public object Deserialize(Stream serializationStream)
        {
            return null;
        }

        #endregion

    }
}
