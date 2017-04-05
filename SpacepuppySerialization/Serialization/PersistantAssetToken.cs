using System;
using System.Runtime.Serialization;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Serialization
{

    [System.Serializable()]
    public class PersistantAssetToken : IPersistantUnityObject
    {

        #region Fields

        [System.NonSerialized()]
        protected IPersistantAsset _asset;

        [System.NonSerialized()]
        private SerializationInfo _info;
        [System.NonSerialized()]
        private StreamingContext _context;
        [System.NonSerialized()]
        private IAssetBundle _bundle;

        #endregion

        #region CONSTRUCTOR

        public PersistantAssetToken(IPersistantAsset obj)
        {
            _asset = obj;
        }

        public PersistantAssetToken()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// The asset to serialize, this value will be null after deserialization. See 'Create' for after deserialization.
        /// </summary>
        public IPersistantAsset Asset
        {
            get { return _asset; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create just the root object from the asset, will still need to apply all serialized data to the object.
        /// </summary>
        /// <returns></returns>
        protected object CreateRoot()
        {
            if (_bundle == null) return null;

            var resourceId = _info.GetString("sp*id");
            var obj = _bundle.LoadAsset(resourceId);
            if (obj == null) return null;

            obj = UnityEngine.Object.Instantiate(obj);
            var pobj = ObjUtil.GetAsFromSource<IPersistantAsset>(obj);
            return (object)pobj ?? (object)obj;
        }

        protected void SetObjectData(object obj)
        {
            if (_bundle == null) return;

            var pobj = ObjUtil.GetAsFromSource<IPersistantAsset>(obj);
            if (pobj != null) pobj.OnDeserialize(_info, _context, _bundle);
        }

        /// <summary>
        /// After deserializing the token, call this to get a copy of the UnityObject.
        /// </summary>
        /// <returns></returns>
        public object Create()
        {
            var obj = this.CreateRoot();
            this.SetObjectData(obj);
            return obj;
        }

        #endregion

        #region IPersistantUnityObject Interface

        public void OnSerialize(SerializationInfo info, StreamingContext context)
        {
            if (_asset == null) return;
            
            info.AddValue("sp*id", _asset.AssetId);
            _asset.OnSerialize(info, context);
        }

        public void OnDeserialize(SerializationInfo info, StreamingContext context, IAssetBundle assetBundle)
        {
            _info = info;
            _context = context;
            _bundle = assetBundle;
        }
        
        #endregion

    }

}
