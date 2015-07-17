using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Serialization
{
    public static class SerializationHelper
    {

        public static void BinarySerialize(IUnityData data, object obj)
        {
            if (data == null) throw new System.ArgumentNullException("data");

            if (obj != null)
            {
                using (var f = UnityDataFormatter.GetFormatter<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter>())
                {
                    f.Serialize(data, obj);
                }
            }
            else
            {
                data.Clear();
            }
        }

        public static object BinaryDeserialize(IUnityData data)
        {
            if (data == null || data.Size == 0) return null;

            using (var f = UnityDataFormatter.GetFormatter<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter>())
            {
                return f.Deserialize(data);
            }
        }

        public static void SoapSerialize(IUnityData data, object obj)
        {
            if (data == null) throw new System.ArgumentNullException("data");

            if (obj != null)
            {
                using (var f = UnityDataFormatter.GetFormatter<System.Runtime.Serialization.Formatters.Soap.SoapFormatter>())
                {
                    f.Serialize(data, obj);
                }
            }
            else
            {
                data.Clear();
            }
        }

        public static object SoapDeserialize(IUnityData data)
        {
            if (data == null || data.Size == 0) return null;

            using (var f = UnityDataFormatter.GetFormatter<System.Runtime.Serialization.Formatters.Soap.SoapFormatter>())
            {
                return f.Deserialize(data);
            }
        }

    }
}
