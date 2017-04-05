using System;
using System.Collections.Generic;

using System.Runtime.Serialization;

using com.spacepuppy.Project;

namespace com.spacepuppy.Serialization
{
    internal class SPSerializationSurrogate : ISerializationSurrogate, ISurrogateSelector, IDisposable
    {

        #region Fields

        private ISurrogateSelector _nextSelector;
        
        private IAssetBundle _assets;

        private AutoPersistantAssetToken _proxy;

        #endregion
        
        #region Properties

        public IAssetBundle AssetBundle
        {
            get { return _assets; }
            set { _assets = value; }
        }

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
            if (SPSerializer.IsSpeciallySerialized(type))
            {
                selector = this;
                return this;
            }
            else if(_nextSelector != null)
            {
                return _nextSelector.GetSurrogate(type, context, out selector);
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
            if(obj is IPersistantUnityObject)
            {
                if (obj is PersistantAssetToken)
                {
                    (obj as PersistantAssetToken).OnSerialize(info, context);
                }
                else if (obj is IPersistantAsset)
                {
                    if (_proxy == null)
                        _proxy = new AutoPersistantAssetToken();

                    info.SetType(typeof(AutoPersistantAssetToken));
                    _proxy.SetObject(obj as IPersistantAsset);
                    _proxy.OnSerialize(info, context);
                    _proxy.SetObject(null);
                }
                else
                {
                    throw new SerializationException("IPersistantUnityObjects should be handled by an IPersistantAsset, not directly by the serializtion engine.");
                }
            }
            else
            {
                SimpleUnityStructureSurrogate.AddValue(obj, info, context);
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if(obj is IPersistantUnityObject)
            {
                (obj as IPersistantUnityObject).OnDeserialize(info, context, _assets);
                if (obj is AutoPersistantAssetToken)
                    return (obj as AutoPersistantAssetToken).PreemptiveCreate();
                return obj;
            }
            else
            {
                return SimpleUnityStructureSurrogate.GetValue(obj, info, context);
            }
        }

        #endregion

        #region IDisposable Interface

        void IDisposable.Dispose()
        {
            if (_proxy != null) _proxy.SetObject(null);
        }

        #endregion



        #region Special Types
        
        /// <summary>
        /// This is used internally to handle automatically creating an asset on deserialize, rather than requiring the user to call Create on the PersistantAssetToken. 
        /// This is used when someone has serialized a IPersistantAsset directly, rather than tokenizing it.
        /// </summary>
        [System.Serializable()]
        private class AutoPersistantAssetToken : PersistantAssetToken, IDeserializationCallback
        {


            //serialize helper
            public void SetObject(IPersistantAsset obj)
            {
                this._asset = obj;
            }

            //deserialize helper
            private object _obj;
            public object PreemptiveCreate()
            {
                _obj = this.CreateRoot();
                return _obj;
            }

            #region IDeserializationCallback Interface

            public void OnDeserialization(object sender)
            {
                this.SetObjectData(_obj);
            }

            #endregion
            
        }

        /// <summary>
        /// Serializable token that represents a UnityObject in SP Serialized data.
        /// </summary>
        [System.Serializable()]
        private struct UnityObjectPointer
        {
        }

        #endregion

    }
}
