using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    [System.Serializable()]
    public sealed class VariantReference : ISerializationCallbackReceiver, System.Runtime.Serialization.ISerializable
    {

        public enum VariantType
        {
            Object = -1,
            Null = 0,
            String = 1,
            Boolean = 2,
            Integer = 3,
            Float = 4,
            Double = 5,
            Vector2 = 6,
            Vector3 = 7,
            Quaternion = 8,
            Color = 9,
            DateTime = 10,
            GameObject = 11,
            Component = 12
        }

        #region Fields

        [System.NonSerialized()]
        private object _value;

        [SerializeField()]
        private VariantType _type;

        //these used for serialization
        [SerializeField()]
        private string _valueReference;
        [SerializeField()]
        private UnityEngine.Object _unityObjectReference;

        #endregion

        #region CONSTRUCTOR

        public VariantReference()
        {

        }

        #endregion

        #region Properties

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = Convert(_value, _type);
            }
        }

        public VariantType ValueType
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    _value = Convert(_value, _type);
                }
            }
        }

        public string StringValue
        {
            get { return _value as string; }
            set
            {
                _value = value;
                _type = VariantType.String;
            }
        }
        public bool BoolValue
        {
            get
            {
                if (_type == VariantType.Boolean) return (bool)_value;
                else return false;
            }
            set
            {
                _value = value;
                _type = VariantType.Boolean;
            }
        }
        public int IntValue
        {
            get
            {
                if (_type == VariantType.Integer) return (int)_value;
                else if (_type == VariantType.Double || _type == VariantType.Float) return ConvertUtil.ToInt(_value);
                else return 0;
            }
            set
            {
                _value = value;
                _type = VariantType.Integer;
            }
        }
        public float FloatValue
        {
            get
            {
                if (_type == VariantType.Float) return (float)_value;
                else if (_type == VariantType.Double || _type == VariantType.Integer) return ConvertUtil.ToSingle(_value);
                else return float.NaN;
            }
            set
            {
                _value = value;
                _type = VariantType.Float;
            }
        }
        public double DoubleValue
        {
            get
            {
                if (_type == VariantType.Double) return (double)_value;
                else if (_type == VariantType.Double || _type == VariantType.Float) return ConvertUtil.ToDouble(_value);
                else return double.NaN;
            }
            set
            {
                _value = value;
                _type = VariantType.Double;
            }
        }
        public Vector2 Vector2Value
        {
            get
            {
                if (_type == VariantType.Vector2) return (Vector2)_value;
                else return Vector2.zero;
            }
            set
            {
                _value = value;
                _type = VariantType.Vector2;
            }
        }
        public Vector3 Vector3Value
        {
            get
            {
                if (_type == VariantType.Vector3) return (Vector3)_value;
                else return Vector3.zero;
            }
            set
            {
                _value = value;
                _type = VariantType.Vector3;
            }
        }
        public Quaternion QuaternionValue
        {
            get
            {
                if (_type == VariantType.Quaternion) return (Quaternion)_value;
                else return Quaternion.identity;
            }
            set
            {
                _value = value;
                _type = VariantType.Quaternion;
            }
        }
        public Color ColorValue
        {
            get
            {
                if (_type == VariantType.Color) return (Color)_value;
                else return Color.black;
            }
            set
            {
                _value = value;
                _type = VariantType.Color;
            }
        }
        public System.DateTime DateValue
        {
            get
            {
                if (_type == VariantType.DateTime) return (System.DateTime)_value;
                else return System.DateTime.MinValue;
            }
            set
            {
                _value = value;
                _type = VariantType.DateTime;
            }
        }
        public GameObject GameObjectValue
        {
            get { return _value as GameObject; }
            set
            {
                _value = value;
                _type = VariantType.GameObject;
            }
        }
        public Component ComponentValue
        {
            get { return _value as Component; }
            set
            {
                _value = value;
                _type = VariantType.Component;
            }
        }
        public UnityEngine.Object ObjectValue
        {
            get { return _value as UnityEngine.Object; }
            set
            {
                _value = value;
                _type = (Object.ReferenceEquals(_value, null)) ? VariantType.Object : VariantReference.GetVariantType(_value.GetType());
            }
        }

        #endregion

        #region Methods

        public VariantReference Clone()
        {
            var c = new VariantReference();
            c._value = _value;
            c._type = _type;
            return c;
        }

        #endregion

        #region ISerializationCallbackReceiver Interface

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _valueReference = null;
            _unityObjectReference = null;
            switch (_type)
            {
                case VariantType.Object:
                    _unityObjectReference = _value as UnityEngine.Object;
                    break;
                case VariantType.GameObject:
                    _unityObjectReference = _value as GameObject;
                    break;
                case VariantType.Component:
                    _unityObjectReference = _value as Component;
                    break;
                case VariantType.Vector2:
                    _valueReference = VectorUtil.Stringify(this.Vector2Value);
                    break;
                case VariantType.Vector3:
                    _valueReference = VectorUtil.Stringify(this.Vector3Value);
                    break;
                case VariantType.Quaternion:
                    _valueReference = VectorUtil.Stringify(this.QuaternionValue);
                    break;
                case VariantType.Color:
                    _valueReference = ConvertUtil.ToInt(this.ColorValue).ToString();
                    break;
                default:
                    _valueReference = ConvertUtil.ToString(_value);
                    break;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            switch (_type)
            {
                case VariantType.Object:
                    _value = _unityObjectReference;
                    break;
                case VariantType.GameObject:
                    _value = _unityObjectReference as GameObject;
                    break;
                case VariantType.Component:
                    _value = _unityObjectReference as Component;
                    break;
                case VariantType.Vector2:
                    _value = VectorUtil.ToVector2(_valueReference);
                    break;
                case VariantType.Vector3:
                    _value = VectorUtil.ToVector3(_valueReference);
                    break;
                case VariantType.Quaternion:
                    _value = VectorUtil.ToQuaternion(_valueReference);
                    break;
                case VariantType.Color:
                    _value = ConvertUtil.ToColor(_valueReference);
                    break;
                default:
                    _value = Convert(_valueReference, _type);
                    break;
            }
            _valueReference = null;
            _unityObjectReference = null;
        }

        #endregion

        #region ISerializable Interface

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (_type == VariantType.GameObject || _type == VariantType.Component || _type == VariantType.Object)
            {
                info.AddValue("type", _type);
            }
            else
            {
                (this as ISerializationCallbackReceiver).OnBeforeSerialize();
                info.AddValue("type", _type);
                switch (_type)
                {
                    case VariantType.Object:
                    case VariantType.GameObject:
                    case VariantType.Component:
                        //do nothing
                        break;
                    case VariantType.Vector2:
                    case VariantType.Vector3:
                    case VariantType.Quaternion:
                    case VariantType.Color:
                        //info.SetValue("vector", _vectorStore.x.ToString() + "," + _vectorStore.y.ToString() + "," + _vectorStore.z.ToString() + "," + _vectorStore.w);
                        break;
                    default:
                        info.SetValue("value", _valueReference);
                        break;
                }
                _valueReference = null;
                _unityObjectReference = null;
            }

        }

        private VariantReference(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _valueReference = null;
            _unityObjectReference = null;

            _type = (VariantType)info.GetInt32("type");
            switch (_type)
            {
                case VariantType.Object:
                case VariantType.GameObject:
                case VariantType.Component:
                    //do nothing
                    break;
                case VariantType.Vector2:
                case VariantType.Vector3:
                case VariantType.Quaternion:
                case VariantType.Color:
                    var arr = StringUtil.SplitFixedLength(info.GetString("vector"), ",", 4);
                    //_vectorStore = new Vector4(ConvertUtil.ToSingle(arr[0]),
                    //                           ConvertUtil.ToSingle(arr[1]),
                    //                           ConvertUtil.ToSingle(arr[2]),
                    //                           ConvertUtil.ToSingle(arr[3]));
                    break;
                default:
                    _valueReference = info.GetString("value");
                    break;
            }
            (this as ISerializationCallbackReceiver).OnAfterDeserialize();
        }

        #endregion

        #region Static Utils

        public static object Convert(object obj, VariantType tp)
        {
            switch (tp)
            {
                case VariantType.Object:
                    return obj as UnityEngine.Object;
                case VariantType.Null:
                    return null;
                case VariantType.String:
                    return ConvertUtil.ToString(obj);
                case VariantType.Boolean:
                    return ConvertUtil.ToBool(obj);
                case VariantType.Integer:
                    return ConvertUtil.ToInt(obj);
                case VariantType.Float:
                    return ConvertUtil.ToSingle(obj);
                case VariantType.Double:
                    return ConvertUtil.ToDouble(obj);
                case VariantType.Vector2:
                    if (obj is Vector2) return (Vector2)obj;
                    return VectorUtil.ToVector2(ConvertUtil.ToString(obj));
                case VariantType.Vector3:
                    if (obj is Vector3) return (Vector3)obj;
                    return VectorUtil.ToVector3(ConvertUtil.ToString(obj));
                case VariantType.Quaternion:
                    if (obj is Quaternion) return (Quaternion)obj;
                    return VectorUtil.ToQuaternion(ConvertUtil.ToString(obj));
                case VariantType.Color:
                    return ConvertUtil.ToColor(obj);
                case VariantType.DateTime:
                    return ConvertUtil.ToDate(obj);
                case VariantType.GameObject:
                    return GameObjectUtil.GetGameObjectFromSource(obj);
                case VariantType.Component:
                    return obj as Component;
            }
            return null;
        }

        /// <summary>
        /// Returns true if the supplied type is an acceptable type for a value stored in a VariantReference
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static bool AcceptableType(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");

            switch (System.Type.GetTypeCode(tp))
            {
                case System.TypeCode.String:
                case System.TypeCode.Boolean:
                case System.TypeCode.Int32:
                case System.TypeCode.Single:
                case System.TypeCode.Double:
                case System.TypeCode.DateTime:
                    return true;
            }

            if (tp == typeof(Vector2)) return true;
            else if (tp == typeof(Vector3)) return true;
            else if (tp == typeof(Quaternion)) return true;
            else if (tp == typeof(Color)) return true;
            else if (tp == typeof(GameObject)) return true;
            else if (typeof(Component).IsAssignableFrom(tp)) return true;
            else if (typeof(UnityEngine.Object).IsAssignableFrom(tp)) return true;
            else return false;
        }

        public static VariantType GetVariantType(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");

            switch (System.Type.GetTypeCode(tp))
            {
                case System.TypeCode.String:
                    return VariantType.String;
                case System.TypeCode.Boolean:
                    return VariantType.Boolean;
                case System.TypeCode.Int32:
                    return VariantType.Integer;
                case System.TypeCode.Single:
                    return VariantType.Float;
                case System.TypeCode.Double:
                    return VariantType.Double;
                case System.TypeCode.DateTime:
                    return VariantType.DateTime;
            }

            if (tp == typeof(Vector2)) return VariantType.Vector2;
            else if (tp == typeof(Vector3)) return VariantType.Vector3;
            else if (tp == typeof(Quaternion)) return VariantType.Quaternion;
            else if (tp == typeof(Color)) return VariantType.Color;
            else if (tp == typeof(GameObject)) return VariantType.GameObject;
            else if (typeof(Component).IsAssignableFrom(tp)) return VariantType.Component;
            else if (typeof(UnityEngine.Object).IsAssignableFrom(tp)) return VariantType.Object;

            return VariantType.Null;
        }

        #endregion

    }
}
