using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Serialization
{

    /// <summary>
    /// Allows serializing an object that doesn't conform to Unity's serialization requirements, but still has references to unity objects. 
    /// You'd serialize the object into this container, and deserialize it from this container during the ISerializationCallbackReceiver 
    /// callbacks.
    /// 
    /// Lets say for instance you have some component that has different operating modes. Lets say these modes are defined by some interface 
    /// or base abstract class. Lets call this 'ICustomMode'. You have a field in the script file where you store the chosen operating mode. 
    /// But, when unity serializes your component, this field does not get included, because interfaces and abstract classes aren't serialized. 
    /// And if you have an object that inherits from the field's type, it will be deserialized as the class that the field is for.
    /// 
    /// Instead, you have a field of type UnityData. This is a serializable class. Then you implement ISerializationCallbackReceiver and do the 
    /// following:
    /// 
    /// <code>
    /// [System.NonSerialized()]
    /// private ICustomMode _mode;
    /// [SerializeField()]
    /// private UnityData _customData;
    /// 
    /// void ISerializationCallbackReceiver.OnAfterDeserialize()
    /// {
    ///     _mode = SerializationHelper.BinaryDeserialize(_customData) as ICustomMode;
    ///     _customData.Clear(); //clear the serialized data from memory so we don't have bloat
    /// }
    /// 
    /// void ISerializationCallbackReceiver.OnBeforeSerialize()
    /// {
    ///     if (_customData == null) _customData = new UnityData();
    ///     SerializationHelper.BinarySerialize(_customData, _mode);
    /// }
    /// </code>
    /// 
    /// Now, after serialization, the object will retain its state. 
    /// Do note the serializable object MUST meet .Net serialization standards.
    /// </summary>
    [System.Serializable()]
    public class UnityData
    {

        #region Fields

        [SerializeField()]
        private byte[] _data;

        [SerializeField()]
        private UnityEngine.Object[] _unityObjectReferences;

        #endregion

        #region Properties

        public int Size
        {
            get
            {
                var dl = (_data != null) ? _data.Length : 0;
                var rl = (_unityObjectReferences != null) ? _unityObjectReferences.Length : 0;
                return dl + rl;
            }
        }

        #endregion

        #region Methods

        public void Clear()
        {
            _data = new byte[] { };
            _unityObjectReferences = new UnityEngine.Object[] { };
        }

        internal void SetData(System.IO.Stream data, Object[] refs)
        {
            _data = data.ToByteArray();
            _unityObjectReferences = refs ?? ArrayUtil.Empty<UnityEngine.Object>();
        }

        internal void GetData(System.IO.Stream data, out Object[] refs)
        {
            data.Write(_data, 0, _data.Length);
            refs = _unityObjectReferences;
        }



        public void Serialize(object graph)
        {
            if(graph != null)
            {
                using (var formatter = UnityDataFormatter.Create())
                {
                    formatter.Serialize(this, graph);
                }
            }
            else
            {
                _data = ArrayUtil.Empty<byte>();
                _unityObjectReferences = ArrayUtil.Empty<UnityEngine.Object>();
            }
        }

        public object Deserialize()
        {
            if(_data != null && _data.Length > 0)
            {
                using (var formatter = UnityDataFormatter.Create())
                {
                    return formatter.Deserialize(this);
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
