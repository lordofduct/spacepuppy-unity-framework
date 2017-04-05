using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

using com.spacepuppy.Collections;
using com.spacepuppy.Project;

namespace com.spacepuppy.Serialization
{

    public class SPSerializer : SurrogateSelector, IDisposable
    {

        #region Fields

        private ISurrogateSelector _formattersSurrogateSelector;
        private SPSerializationSurrogate _specialSurrogate = new SPSerializationSurrogate();
        
        #endregion

        #region CONSTRUCTOR

        public SPSerializer()
        {
        }

        #endregion

        #region Properties

        public IAssetBundle AssetBundle
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public void Serialize(IFormatter formatter, Stream serializationStream, object graph)
        {
            if (formatter == null) throw new System.ArgumentNullException("formatter");
            if (serializationStream == null) throw new System.ArgumentNullException("serializationStream");

            _formattersSurrogateSelector = formatter.SurrogateSelector;
            formatter.SurrogateSelector = this;
            formatter.Serialize(serializationStream, graph);
            formatter.SurrogateSelector = _formattersSurrogateSelector;
        }
        
        public object Deserialize(IFormatter formatter, Stream serializationStream)
        {
            if (formatter == null) throw new System.ArgumentNullException("formatter");
            if (serializationStream == null) throw new System.ArgumentNullException("serializationStream");

            _formattersSurrogateSelector = formatter.SurrogateSelector;
            formatter.SurrogateSelector = this;
            var result = formatter.Deserialize(serializationStream);
            formatter.SurrogateSelector = _formattersSurrogateSelector;

            return result;
        }
        
        #endregion

        #region ISurrogateSelector Interface

        public override ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            ISerializationSurrogate surrogate = null;
            selector = null;
            if (_formattersSurrogateSelector != null)
            {
                surrogate = _formattersSurrogateSelector.GetSurrogate(type, context, out selector);
            }
            if(surrogate == null)
            {
                surrogate = base.GetSurrogate(type, context, out selector);
            }
            if (surrogate == null && SPSerializer.IsSpeciallySerialized(type))
            {
                selector = this;
                surrogate = _specialSurrogate;
            }

            return surrogate;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            _formattersSurrogateSelector = null;
            this.AssetBundle = null;
            _pool.Release(this);
        }

        #endregion

        #region Static Is Serialized By This Test
        
        public static bool IsSpeciallySerialized(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");

            if (typeof(IPersistantUnityObject).IsAssignableFrom(tp)) return true;

            return SimpleUnityStructureSurrogate.IsSpeciallySerialized(tp);
        }

        #endregion


        #region Static Factory

        private static ObjectCachePool<SPSerializer> _pool = new ObjectCachePool<SPSerializer>(10, () => new SPSerializer());
        public static SPSerializer Create()
        {
            return _pool.GetInstance();
        }

        #endregion


    }

}
