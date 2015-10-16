using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;

namespace com.spacepuppy.Serialization
{
    public static class SerializationHelper
    {
        
        private static com.spacepuppy.Collections.ObjectCachePool<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter> _simpleFormatters = new com.spacepuppy.Collections.ObjectCachePool<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter>(10, () => {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Context = new StreamingContext(StreamingContextStates.All);
            formatter.SurrogateSelector = new SimpleUnityStructureSurrogate();
            return formatter;
        });


        private static com.spacepuppy.Collections.ObjectCachePool<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter> _fullSupportFormatters = new com.spacepuppy.Collections.ObjectCachePool<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter>(10, () => {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Context = new StreamingContext(StreamingContextStates.All);
            formatter.SurrogateSelector = new SPSerializationSurrogate();
            return formatter;
        });



        
        public static void Serialize(Stream strm, object obj)
        {
            if (strm == null) throw new System.ArgumentNullException("strm");

            if (obj != null)
            {
                var formatter = _simpleFormatters.GetInstance();
                try
                {
                    formatter.Serialize(strm, obj);
                }
                catch(System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    _simpleFormatters.Release(formatter);
                }
            }
        }

        public static object Deserialize(Stream strm)
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





        public static void Serialize(IUnityData data, object obj)
        {
            if (data == null) throw new System.ArgumentNullException("data");

            if (obj != null)
            {
                using (var strm = new MemoryStream())
                {
                    var formatter = _fullSupportFormatters.GetInstance();
                    try
                    {
                        (formatter.SurrogateSelector as SPSerializationSurrogate).StartSerialization();
                        formatter.Serialize(strm, obj);
                        strm.Position = 0;
                        (formatter.SurrogateSelector as SPSerializationSurrogate).EndSerialization(strm, data);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        _fullSupportFormatters.Release(formatter);
                    }
                }
            }
            else
            {
                data.Clear();
            }
        }
        
        public static object Deserialize(IUnityData data)
        {
            if (data == null || data.Size == 0) return null;

            using (var f = UnityDataFormatter.GetFormatter<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter>())
            {
                using (var strm = new MemoryStream())
                {
                    var formatter = _fullSupportFormatters.GetInstance();
                    try
                    {
                        UnityEngine.Object[] refs;
                        data.GetData(strm, out refs);
                        strm.Position = 0;
                        
                        (formatter.SurrogateSelector as SPSerializationSurrogate).StartDeserialization(refs);
                        var result = formatter.Deserialize(strm);
                        (formatter.SurrogateSelector as SPSerializationSurrogate).EndDeserialization();

                        return result;
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        _fullSupportFormatters.Release(formatter);
                    }
                }
            }
        }





        [System.Obsolete("Use Serialize")]
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

        [System.Obsolete("Use Deserialize")]
        public static object BinaryDeserialize(IUnityData data)
        {
            if (data == null || data.Size == 0) return null;

            using (var f = UnityDataFormatter.GetFormatter<System.Runtime.Serialization.Formatters.Binary.BinaryFormatter>())
            {
                return f.Deserialize(data);
            }
        }
        



    }
}
