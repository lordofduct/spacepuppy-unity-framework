using UnityEngine;
using System.Collections;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    [System.Serializable()]
    public class TypeReference : ISerializationCallbackReceiver, System.Runtime.Serialization.ISerializable
    {

        #region Fields

        [SerializeField()]
        private string _typeHash;

        [System.NonSerialized()]
        private System.Type _type;

        #endregion

        #region CONSTRUCTOR

        public TypeReference()
        {
        }

        public TypeReference(System.Type tp)
        {
            this.Type = tp;
        }

        #endregion

        #region Properties

        public System.Type Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                this.HashType();
            }
        }


        private void HashType()
        {
            if (_type != null)
            {
                _typeHash = _type.Assembly.FullName + "|" + _type.FullName;
            }
            else
            {
                _typeHash = null;
            }
        }

        #endregion


        #region ISerializationCallbackReceiver Interface

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if(_typeHash != null)
            {
                var arr = StringUtil.SplitFixedLength(_typeHash, "|", 2);
                var assemb = (from a in System.AppDomain.CurrentDomain.GetAssemblies() 
                              where a.FullName == arr[0] 
                              select a).FirstOrDefault();
                if(assemb != null)
                {
                    _type = (from t in assemb.GetTypes() 
                             where t.FullName == arr[1] 
                             select t).FirstOrDefault();
                }
                else
                {
                    _type = null;
                }
            }
            else
            {
                _type = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.HashType();
        }

        #endregion

        #region ISerializable Interface

        protected TypeReference(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _typeHash = info.GetString("hash");
            (this as ISerializationCallbackReceiver).OnAfterDeserialize();
        }

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.HashType();
            info.AddValue("hash", _typeHash);
        }

        #endregion



        public static implicit operator System.Type(TypeReference a)
        {
            if (a != null) return a.Type;
            else return null;
        }

    }

}