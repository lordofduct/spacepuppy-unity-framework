using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Serialization
{
    public interface IUnitySerializable
    {

        void GetObjectData(UnitySerializationInfo info);
        void SetObjectData(UnitySerializationInfo info);

    }
}
