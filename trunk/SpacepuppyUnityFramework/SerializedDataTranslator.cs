using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.spacepuppy
{

    [System.Serializable()]
    public class SerializedDataTranslator : ISerializable, ISerializationCallbackReceiver
    {

        #region Fields

        [System.NonSerialized()]
        private Dictionary<string, object> _dict = new Dictionary<string, object>();

        [SerializeField()]
        private byte[] _data;

        #endregion

        #region CONSTRUCTOR

        public SerializedDataTranslator()
        {

        }

        protected SerializedDataTranslator(SerializationInfo info, StreamingContext context)
        {
            _dict = info.GetValue("dict", typeof(Dictionary<string, object>)) as Dictionary<string, object>;
        }

        #endregion

        #region Properties

        public object this[string name]
        {
            get
            {
                if (_dict.ContainsKey(name))
                    return _dict[name];
                else
                    return null;
            }
            set
            {
                if (value != null && !value.GetType().IsSerializable) throw new SerializationException("Type " + value.GetType().Name + " is not marked as Serializable.");
                _dict[name] = value;
            }
        }

        #endregion

        #region Methods

        public void AddValue(string name, object value)
        {
            if (value != null && !value.GetType().IsSerializable) throw new SerializationException("Type " + value.GetType().Name + " is not marked as Serializable.");
            _dict[name] = value;
        }

        public object GetValue(string name)
        {
            if (_dict.ContainsKey(name))
                return _dict[name];
            else
                return null;
        }

        #endregion

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _data = SerializedDataTranslator.Serialize(_dict);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _dict = SerializedDataTranslator.Deserialize(_data) as Dictionary<string, object>;
            _data = null;
        }

        #endregion

        #region ISerializable Interface

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("dict", _dict);
        }

        #endregion

        #region Static Methods

        public static byte[] Serialize(object value)
        {
            if (value == null) return null;
            if (!value.GetType().IsSerializable) throw new SerializationException();

            var formatter = new BinaryFormatter();
            using (var strm = new MemoryStream())
            {
                formatter.Serialize(strm, value);
                return strm.ToArray();
            }
        }

        public static object Deserialize(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                var formatter = new BinaryFormatter();
                using (var strm = new MemoryStream(data))
                {
                    return formatter.Deserialize(strm);
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

    }

}
