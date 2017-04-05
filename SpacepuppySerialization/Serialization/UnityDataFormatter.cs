using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Serialization
{
    internal class UnityDataFormatter : ISurrogateSelector, ISerializationSurrogate, IDisposable
    {

        #region Fields

        private BinaryFormatter _formatter = new BinaryFormatter();
        private List<UnityEngine.Object> _unityObjects = new List<UnityEngine.Object>();

        #endregion

        #region Methods

        public void Serialize(UnityData data, object graph)
        {
            if (data == null) throw new System.ArgumentNullException("data");
            
            using (var strm = new MemoryStream())
            {
                this.StartSerialization();

                _formatter.SurrogateSelector = this;
                _formatter.Serialize(strm, graph);

                strm.Position = 0;
                this.EndSerialization(strm, data);
            }
        }

        public object Deserialize(UnityData data)
        {
            if (data == null) throw new System.ArgumentNullException("data");
            
            using (var strm = new MemoryStream())
            {
                UnityEngine.Object[] refs;
                data.GetData(strm, out refs);
                strm.Position = 0;

                this.StartDeserialization(refs);

                _formatter.SurrogateSelector = this;
                var result = _formatter.Deserialize(strm);

                this.EndDeserialization();

                return result;
            }
        }





        private void StartSerialization()
        {
            _unityObjects.Clear();
        }

        private void EndSerialization(System.IO.Stream strm, UnityData data)
        {
            if (data != null)
            {
                data.SetData(strm, _unityObjects.ToArray());
            }
            _unityObjects.Clear();
        }

        private void StartDeserialization(IEnumerable<UnityEngine.Object> refs)
        {
            _unityObjects.Clear();
            _unityObjects.AddRange(refs);
        }

        private void EndDeserialization()
        {
            _unityObjects.Clear();
        }

        #endregion

        #region ISurrogateSelector Interface

        void ISurrogateSelector.ChainSelector(ISurrogateSelector selector)
        {
            throw new System.NotSupportedException();
        }

        ISurrogateSelector ISurrogateSelector.GetNextSelector()
        {
            return null;
        }

        ISerializationSurrogate ISurrogateSelector.GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(type) || typeof(UnityObjectPointer).IsAssignableFrom(type))
            {
                selector = this;
                return this;
            }
            else
            {
                selector = null;
                return null;
            }
        }

        #endregion

        #region ISerializationSurrogate Interface

        void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is UnityEngine.Object)
            {
                info.SetType(typeof(UnityObjectPointer));
                info.AddValue("Index", _unityObjects.Count);
                _unityObjects.Add(obj as UnityEngine.Object);
            }
        }

        object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (obj is UnityObjectPointer)
            {
                int index = info.GetInt32("Index");
                if (index >= 0 && index < _unityObjects.Count) return _unityObjects[index];
            }

            return null;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            _unityObjects.Clear();
            _pool.Release(this);
        }

        #endregion

        #region Special Types

        /// <summary>
        /// Serializable token that represents a UnityObject in SP Serialized data.
        /// </summary>
        [System.Serializable()]
        private struct UnityObjectPointer
        {
        }

        #endregion

        #region Static Factory

        private static ObjectCachePool<UnityDataFormatter> _pool = new ObjectCachePool<UnityDataFormatter>(10, () => new UnityDataFormatter());
        public static UnityDataFormatter Create()
        {
            return _pool.GetInstance();
        }

        #endregion

    }
}
