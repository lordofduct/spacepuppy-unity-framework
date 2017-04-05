
using System.Runtime.Serialization;

using com.spacepuppy.Project;

namespace com.spacepuppy.Serialization
{

    /// <summary>
    /// Allows serializing UnityObjects in a persistant manner at runtime. 
    /// This can be used in game save files to serialize an entity that is based 
    /// on some prefab. 
    /// 
    /// Any script that needs to save persistant data will have the messages called 
    /// on it with a SerializationInfo to either store its state in, or to retrieve 
    /// its state from. 
    /// </summary>
    public interface IPersistantUnityObject
    {

        void OnSerialize(SerializationInfo info, StreamingContext context);

        void OnDeserialize(SerializationInfo info, StreamingContext context, IAssetBundle assetBundle);

    }

    public interface IPersistantAsset : IPersistantUnityObject
    {

        string AssetId { get; }

    }

}
