using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Serialization
{
    
    [System.Obsolete("No longer used.")]
    public interface IUnitySerializable
    {

        void GetObjectData(UnitySerializationInfo info);
        void SetObjectData(UnitySerializationInfo info);

    }
}
