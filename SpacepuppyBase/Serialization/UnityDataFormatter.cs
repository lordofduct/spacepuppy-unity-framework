using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Serialization
{

    public sealed class UnityDataFormatter : System.IDisposable
    {

        #region Fields

        private IFormatter _formatter;
        private UnitySerializationSurrogate _surrogate;
        private System.Type _pooledTypeKey;

        #endregion

        #region CONSTRUCTOR

        public UnityDataFormatter()
        {
            _formatter = new BinaryFormatter();
            _surrogate = new UnitySerializationSurrogate();

            _formatter.SurrogateSelector = _surrogate;
        }

        public UnityDataFormatter(IFormatter formatter)
        {
            if (formatter == null) throw new System.ArgumentNullException("formatter");

            _formatter = formatter;
            _surrogate = new UnitySerializationSurrogate();

            _formatter.SurrogateSelector = _surrogate;
        }

        #endregion

        #region Serialization Interface

        public void Serialize(IUnityData data, object obj)
        {
            if (data == null) throw new System.ArgumentNullException("data");
            if (obj == null) throw new System.ArgumentNullException("obj");
            if (obj is UnityEngine.Object) throw new System.ArgumentException("Can not serialize Unity Reference Objects directly.", "obj");
            var tp = obj.GetType();
            if (!IsUnitySerializable(tp)) throw new System.ArgumentException("Type of object is not serializable.");

            if (obj != null)
            {
                _surrogate.StartSerialization();
                using (var strm = new System.IO.MemoryStream())
                {
                    _formatter.Serialize(strm, obj);
                    strm.Position = 0;

                    byte[] arr = new byte[strm.Length];
                    strm.Read(arr, 0, arr.Length);

                    data.SetData(strm, _surrogate.StopSerialization());
                }
            }
        }

        public object Deserialize(IUnityData data)
        {
            if (data == null || data.Size == 0) return null;

            using (var strm = new System.IO.MemoryStream())
            {
                UnityEngine.Object[] refs;
                data.GetData(strm, out refs);

                strm.Position = 0;
                _surrogate.StartDeserialization(refs);
                var result = _formatter.Deserialize(strm);
                _surrogate.StopDeserialization();
                return result;
            }
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            ObjectCachePool<UnityDataFormatter> pool;
            if (_pooledTypeKey != null && _formatterPools.TryGetValue(_pooledTypeKey, out pool))
            {
                pool.Release(this);
            }
        }

        #endregion

        #region Static Utils

        public static bool IsUnitySerializable(System.Type tp, bool supportLists = true)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");

            if (tp.IsListType(true))
            {
                //if (supportLists)
                //{
                //    return IsUnitySerializable(tp.GetElementTypeOfListType());
                //}
                //else
                //{
                //    return false;
                //}
                return supportLists;
            }
            if (tp.IsEnum) return true;
            if (TypeUtil.IsType(tp, typeof(UnityEngine.Object))) return true;
            if (System.Attribute.IsDefined(tp, typeof(System.SerializableAttribute), false)) return true;
            if (tp.IsValueType && tp.Assembly.FullName.StartsWith("UnityEngine"))
            {
                return TypeUtil.IsType(tp, typeof(UnityEngine.Vector2),
                                          typeof(UnityEngine.Vector3),
                                          typeof(UnityEngine.Vector4),
                                          typeof(UnityEngine.Quaternion),
                                          typeof(UnityEngine.Matrix4x4),
                                          typeof(UnityEngine.Color),
                                          typeof(UnityEngine.LayerMask));
            }

            return false;
        }

        #endregion

        #region Static Factory

        private static Dictionary<System.Type, ObjectCachePool<UnityDataFormatter>> _formatterPools;

        public static UnityDataFormatter GetFormatter<T>() where T : IFormatter
        {
            if (_formatterPools == null) _formatterPools = new Dictionary<System.Type, ObjectCachePool<UnityDataFormatter>>();

            var tp = typeof(T);
            if(_formatterPools.ContainsKey(tp))
            {
                return _formatterPools[tp].GetInstance();
            }
            else
            {
                var pool = new ObjectCachePool<UnityDataFormatter>(5, () =>
                {
                    var f = new UnityDataFormatter(System.Activator.CreateInstance<T>());
                    f._pooledTypeKey = typeof(T);
                    return f;
                });
                _formatterPools.Add(tp, pool);
                return pool.GetInstance();
            }
        }

        #endregion

    }

}
