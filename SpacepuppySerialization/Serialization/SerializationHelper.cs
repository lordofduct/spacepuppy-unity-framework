using System;
using System.IO;
using System.Runtime.Serialization;

using com.spacepuppy.Project;

namespace com.spacepuppy.Serialization
{

    [System.Obsolete("Use SPSerializer directly.")]
    public static class SerializationHelper
    {

        #region Formatter Factory

        private static com.spacepuppy.Collections.ObjectCachePool<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter> _simpleFormatters = new com.spacepuppy.Collections.ObjectCachePool<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter>(10, () => {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Context = new StreamingContext(StreamingContextStates.All);
            formatter.SurrogateSelector = new SimpleUnityStructureSurrogate();
            return formatter;
        });
        
        #endregion




        public static void SimpleSerialize(Stream strm, object obj)
        {
            if (strm == null) throw new System.ArgumentNullException("strm");

            if (obj != null)
            {
                var formatter = _simpleFormatters.GetInstance();
                try
                {
                    formatter.Serialize(strm, obj);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    _simpleFormatters.Release(formatter);
                }
            }
        }

        public static object SimpleDeserialize(Stream strm)
        {
            if (strm == null) throw new System.ArgumentNullException("strm");

            var formatter = _simpleFormatters.GetInstance();
            try
            {
                return formatter.Deserialize(strm);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                _simpleFormatters.Release(formatter);
            }
        }

        public static void Serialize(Stream strm, object obj)
        {
            if (strm == null) throw new System.ArgumentNullException("strm");

            if (obj != null)
            {
                using (var serializer = SPSerializer.Create())
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    serializer.Serialize(formatter, strm, obj);
                }
            }
        }

        public static object Deserialize(Stream strm)
        {
            if (strm == null) throw new System.ArgumentNullException("strm");
            
            using (var serializer = SPSerializer.Create())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return serializer.Deserialize(formatter, strm);
            }
        }

        public static void Serialize(Stream strm, object obj, IAssetBundle assetBundle)
        {
            if (strm == null) throw new System.ArgumentNullException("strm");

            if (obj != null)
            {
                using (var serializer = SPSerializer.Create())
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    serializer.AssetBundle = assetBundle;
                    serializer.Serialize(formatter, strm, obj);
                }
            }
        }

        public static object Deserialize(Stream strm, IAssetBundle assetBundle)
        {
            if (strm == null) throw new System.ArgumentNullException("strm");

            using (var serializer = SPSerializer.Create())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                serializer.AssetBundle = assetBundle;
                return serializer.Deserialize(formatter, strm);
            }
        }

        
    }
}
