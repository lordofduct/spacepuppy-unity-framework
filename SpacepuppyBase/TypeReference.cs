using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Allows for a serializable reference to a type in the namespace.
    /// 
    /// If the serialized type deserializes improperly, the Type returned with be the 'void' type. 
    /// A property 'IsVoid' exists to easily test this. This state implies that the serialized data 
    /// was loaded with out the required namespace loaded.
    /// </summary>
    [System.Serializable()]
    public class TypeReference : System.Runtime.Serialization.ISerializable
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
                if (_type == null) _type = UnHashType(_typeHash);
                return _type;
            }
            set
            {
                _type = value;
                _typeHash = HashType(_type);
            }
        }

        public bool IsVoid
        {
            get
            {
                return this.Type == typeof(void);
            }
        }

        #endregion


        #region ISerializable Interface

        protected TypeReference(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _typeHash = info.GetString("hash");
        }

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _typeHash = HashType(_type);
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

            public System.Type inheritsFromType;
            public bool allowAbstractClasses = false;
            public bool allowInterfaces = false;
            public System.Type defaultType = null;
            public System.Type[] excludedTypes = null;
            public TypeDropDownListingStyle dropDownStyle = TypeDropDownListingStyle.Namespace;

            public ConfigAttribute(System.Type inheritsFromType)
            {
                this.inheritsFromType = inheritsFromType;
            }

            public ConfigAttribute(System.Type inheritsFromType, params System.Type[] excludedTypes)
            {
                this.inheritsFromType = inheritsFromType;
                this.excludedTypes = excludedTypes;
            }

        }
        
        #endregion

        #region Util Methods

        public static string HashType(System.Type tp)
        {
            if (tp != null)
            {
                return tp.Assembly.GetName().Name + "|" + tp.FullName;
            }
            else
            {
                return null;
            }
        }

        public static System.Type UnHashType(string hash)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                var arr = StringUtil.SplitFixedLength(hash, "|", 2);
                var tp = TypeUtil.ParseType(arr[0], arr[1]);

                //set type to void if the type is unfruitful, this way we're not constantly retesting this
                if (tp == null) tp = typeof(void);
                return tp;
            }
            else
            {
                return null;
            }
        }

        #endregion

    }

}