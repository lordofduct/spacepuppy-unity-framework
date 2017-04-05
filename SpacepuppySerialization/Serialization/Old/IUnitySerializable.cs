namespace com.spacepuppy.Serialization.Old
{

    [System.Obsolete("No longer used.")]
    public interface IUnitySerializable
    {

        void GetObjectData(UnitySerializationInfo info);
        void SetObjectData(UnitySerializationInfo info);

    }
}
