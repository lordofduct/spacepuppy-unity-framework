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
                _typeHash = _type.Assembly.GetName().Name + "|" + _type.FullName;
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
            if (_typeHash != null)
            {
                var arr = StringUtil.SplitFixedLength(_typeHash, "|", 2);
                _type = TypeUtil.ParseType(arr[0], arr[1]);
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

        #region Operators

        public static implicit operator System.Type(TypeReference a)
        {
            if (a != null) return a.Type;
            else return null;
        }

        #endregion


        #region Special Types

        [System.AttributeUsage(System.AttributeTargets.Field)]
        public class ConfigAttribute : System.Attribute
        {

            public System.Type InheritsFromType;
            public bool allowAbstractClasses = false;
            public bool allowInterfaces = false;
            public System.Type defaultType = null;
            public TypeDropDownListingStyle dropDownStyle = TypeDropDownListingStyle.Namespace;

            public ConfigAttribute(System.Type inheritsFromType)
            {
                this.InheritsFromType = inheritsFromType;
            }

        }

        #endregion

    }

}