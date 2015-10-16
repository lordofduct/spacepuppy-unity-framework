using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.Serialization;

namespace com.spacepuppy.Serialization
{

    public class SimpleUnityStructureSurrogate : ISerializationSurrogate, ISurrogateSelector
    {

        #region Fields

        private ISurrogateSelector _nextSelector;

        #endregion

        #region ISurrogateSelector Interface

        public void ChainSelector(ISurrogateSelector selector)
        {
            _nextSelector = selector;
        }

        public ISurrogateSelector GetNextSelector()
        {
            return _nextSelector;
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if(SPSerializationSurrogate.IsSpeciallySerialized(type, false))
            {
                selector = this;
                return this;
            }
            else
            {
                selector = null;
                return null;
            }
        }

        #endregion

        #region ISerializationSurrogate Interface

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            SPSerializationSurrogate.AddValue(obj, info, context);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return SPSerializationSurrogate.GetValue(obj, info, context);
        }

        #endregion
        
    }

}
