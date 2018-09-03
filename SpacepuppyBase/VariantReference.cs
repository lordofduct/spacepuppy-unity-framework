using UnityEngine;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Facilitates creating a serializable field that references any of multiple of common unity types. 
    /// This includes any of the prims easily serialized by unity (int, float, double, bool, etc), any 
    /// UnityEngine.Object reference, a property of said objects, or even evaluate a simple arithmetic operation.
    /// 
    /// In the case of arithmetic evaluations, you can evaluate them based on the properties of a referenced 
    /// object. See com.spacepuppy.Dynamic.Evaluator for more information on how to format eval statements.
    /// </summary>
    [System.Serializable()]
    public sealed class VariantReference : System.Runtime.Serialization.ISerializable, ISPDisposable
    {
        
        public enum RefMode : byte
        {
            Value = 0,
            Property = 1,
            Eval = 2
        }

        #region Fields

        //these used for serialization
        [SerializeField()]
        private RefMode _mode;
        [SerializeField()]
        private VariantType _type;

        [SerializeField()]
        private float _x;
        [SerializeField()]
        private float _y;
        [SerializeField()]
        private float _z;
        [SerializeField()]
        private double _w;
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("_valueReference")]
        private string _string;

        [SerializeField()]
        private UnityEngine.Object _unityObjectReference;

        [System.NonSerialized]
        private INumeric _numeric;

        #endregion

        #region CONSTRUCTOR

        public VariantReference()
        {
        }

        public VariantReference(object value)
        {
            this.Value = value;
        }

        #endregion

        #region Properties

        public object Value
        {
            get
            {
                if(_mode == RefMode.Eval)
                {
                    try
                    {
                        return Evaluator.EvalValue(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                    }
                    catch
                    {
                        Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                    }
                }
                else
                {
                    switch (_type)
                    {
                        case VariantType.Object:
                            return this.ObjectValue;
                        case VariantType.String:
                            return this.StringValue;
                        case VariantType.Boolean:
                            return this.BoolValue;
                        case VariantType.Integer:
                            return this.IntValue;
                        case VariantType.Float:
                            return this.FloatValue;
                        case VariantType.Double:
                            return this.DoubleValue;
                        case VariantType.Vector2:
                            return this.Vector2Value;
                        case VariantType.Vector3:
                            return this.Vector3Value;
                        case VariantType.Quaternion:
                            return this.QuaternionValue;
                        case VariantType.Color:
                            return this.ColorValue;
                        case VariantType.DateTime:
                            return this.DateValue;
                        case VariantType.GameObject:
                            return this.GameObjectValue;
                        case VariantType.Component:
                            return this.ComponentValue;
                        case VariantType.LayerMask:
                            return this.LayerMaskValue;
                        case VariantType.Rect:
                            return this.RectValue;
                        case VariantType.Numeric:
                            return this.NumericValue;
                    }
                }

                return null;
            }
            set
            {
                _mode = RefMode.Value;
                if (value == null)
                {
                    _x = 0f;
                    _y = 0f;
                    _z = 0f;
                    _w = 0d;
                    _string = null;
                    _unityObjectReference = null;
                    _type = VariantType.Null;
                }
                else
                {
                    _type = VariantReference.GetVariantType(value.GetType());
                    switch (_type)
                    {
                        case VariantType.Object:
                            this.ObjectValue = value as UnityEngine.Object;
                            break;
                        case VariantType.String:
                            this.StringValue = System.Convert.ToString(value);
                            break;
                        case VariantType.Boolean:
                            this.BoolValue = ConvertUtil.ToBool(value);
                            break;
                        case VariantType.Integer:
                            this.IntValue = ConvertUtil.ToInt(value);
                            break;
                        case VariantType.Float:
                            this.FloatValue = ConvertUtil.ToSingle(value);
                            break;
                        case VariantType.Double:
                            this.DoubleValue = ConvertUtil.ToDouble(value);
                            break;
                        case VariantType.Vector2:
                            this.Vector2Value = ConvertUtil.ToVector2(value);
                            break;
                        case VariantType.Vector3:
                            this.Vector3Value = ConvertUtil.ToVector3(value);
                            break;
                        case VariantType.Quaternion:
                            this.QuaternionValue = ConvertUtil.ToQuaternion(value);
                            break;
                        case VariantType.Color:
                            this.ColorValue = ConvertUtil.ToColor(value);
                            break;
                        case VariantType.DateTime:
                            this.DateValue = ConvertUtil.ToDate(value);
                            break;
                        case VariantType.GameObject:
                            this.GameObjectValue = value as GameObject;
                            break;
                        case VariantType.Component:
                            this.ComponentValue = value as Component;
                            break;
                        case VariantType.LayerMask:
                            this.LayerMaskValue = (LayerMask)value;
                            break;
                        case VariantType.Rect:
                            this.RectValue = (Rect)value;
                            break;
                        case VariantType.Numeric:
                            this.SetToNumeric(value as INumeric);
                            break;
                    }
                }
            }
        }

        public VariantType ValueType
        {
            get
            {
                return _type;
            }
        }
        
        public string StringValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return null;
                            case VariantType.String:
                                return _string;
                            case VariantType.Boolean:
                                return this.BoolValue.ToString();
                            case VariantType.Integer:
                                return this.IntValue.ToString();
                            case VariantType.Float:
                                return this.FloatValue.ToString();
                            case VariantType.Double:
                                return this.DoubleValue.ToString();
                            case VariantType.Vector2:
                                return this.Vector2Value.ToString();
                            case VariantType.Vector3:
                                return this.Vector3Value.ToString();
                            case VariantType.Vector4:
                                return this.Vector4Value.ToString();
                            case VariantType.Quaternion:
                                return this.QuaternionValue.ToString();
                            case VariantType.Color:
                                return this.ColorValue.ToString();
                            case VariantType.DateTime:
                                return this.DateValue.ToString();
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return (_unityObjectReference != null) ? _unityObjectReference.ToString() : null;
                            case VariantType.LayerMask:
                                return this.LayerMaskValue.ToString();
                            case VariantType.Rect:
                                return this.RectValue.ToString();
                            case VariantType.Numeric:
                                return ConvertUtil.ToString(this.NumericValue);
                        }
                        break;
                    case RefMode.Property:
                        return System.Convert.ToString(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return Evaluator.EvalString(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return null;
            }
            set
            {
                _x = 0f;
                _y = 0f;
                _z = 0f;
                _w = 0d;
                _string = value;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.String;
                _mode = RefMode.Value;
            }
        }
        public bool BoolValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch(_type)
                        {
                            case VariantType.Object:
                                return _unityObjectReference != null;
                            case VariantType.String:
                                return !string.IsNullOrEmpty(_string);
                            case VariantType.Boolean:
                                return _x != 0f;
                            case VariantType.Integer:
                            case VariantType.Float:
                            case VariantType.Double:
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                            case VariantType.Color:
                            case VariantType.DateTime:
                                return (_x + _y + _z + _w) != 0;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return _unityObjectReference != null;
                            case VariantType.LayerMask:
                                return (_x + _y + _z + _w) != 0;
                            case VariantType.Rect:
                                return (_x + _y + _z + _w) != 0;
                            case VariantType.Numeric:
                                return ConvertUtil.ToBool(this.NumericValue);
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToBool(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return Evaluator.EvalBool(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return false;
            }
            set
            {
                _x = (value) ? 1f : 0f;
                _y = 0f;
                _z = 0f;
                _w = 0d;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Boolean;
                _mode = RefMode.Value;
            }
        }
        public int IntValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return 0;
                            case VariantType.String:
                                return ConvertUtil.ToInt(_string);
                            case VariantType.Boolean:
                                return (_x != 0f) ? 1 : 0;
                            case VariantType.Integer:
                                return (int)_w;
                            case VariantType.Float:
                                return (int)_x;
                            case VariantType.Double:
                                return (int)_w;
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                                return (int)_x;
                            case VariantType.Color:
                                return ConvertUtil.ToInt(new Color(_x, _y, _z, (float)_w));
                            case VariantType.DateTime:
                                return 0;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return 0;
                            case VariantType.LayerMask:
                                return (int)_w;
                            case VariantType.Rect:
                                return (int)_x;
                            case VariantType.Numeric:
                                return ConvertUtil.ToInt(this.NumericValue);
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToInt(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return (int)Evaluator.EvalNumber(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return 0;
            }
            set
            {
                _x = 0f;
                _y = 0f;
                _z = 0f;
                _w = value;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Integer;
                _mode = RefMode.Value;
            }
        }
        public float FloatValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return 0;
                            case VariantType.String:
                                return ConvertUtil.ToSingle(_string);
                            case VariantType.Boolean:
                                return (_x != 0f) ? 1 : 0;
                            case VariantType.Integer:
                                return (float)_w;
                            case VariantType.Float:
                                return _x;
                            case VariantType.Double:
                                return (float)_w;
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                            case VariantType.Color:
                                return _x;
                            case VariantType.DateTime:
                                return 0f;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return 0f;
                            case VariantType.LayerMask:
                                return (int)_w;
                            case VariantType.Rect:
                                return _x;
                            case VariantType.Numeric:
                                return ConvertUtil.ToSingle(this.NumericValue);
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToSingle(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return Evaluator.EvalNumber(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return 0f;
            }
            set
            {
                _x = value;
                _y = 0f;
                _z = 0f;
                _w = 0d;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Float;
                _mode = RefMode.Value;
            }
        }
        public double DoubleValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return 0d;
                            case VariantType.String:
                                return ConvertUtil.ToDouble(_string);
                            case VariantType.Boolean:
                                return (_x != 0f) ? 1d : 0d;
                            case VariantType.Integer:
                                return _w;
                            case VariantType.Float:
                                return (double)_x;
                            case VariantType.Double:
                                return _w;
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                            case VariantType.Color:
                                return _x;
                            case VariantType.DateTime:
                                return 0d;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return 0d;
                            case VariantType.LayerMask:
                                return (int)_w;
                            case VariantType.Rect:
                                return _x;
                            case VariantType.Numeric:
                                return ConvertUtil.ToDouble(this.NumericValue);
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToDouble(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return (double)Evaluator.EvalNumber(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return 0d;
            }
            set
            {
                _x = 0f;
                _y = 0f;
                _z = 0f;
                _w = value;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Double;
                _mode = RefMode.Value;
            }
        }
        public Vector2 Vector2Value
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return Vector2.zero;
                            case VariantType.String:
                                return ConvertUtil.ToVector2(_string);
                            case VariantType.Boolean:
                                return (_x != 0f) ? Vector2.one : Vector2.zero;
                            case VariantType.Integer:
                                return new Vector2((float)_w, 0f);
                            case VariantType.Float:
                                return new Vector2(_x, 0f);
                            case VariantType.Double:
                                return new Vector2((float)_w, 0f);
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                            case VariantType.Color:
                                return new Vector2(_x, _y);
                            case VariantType.DateTime:
                                return Vector2.zero;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return Vector2.zero;
                            case VariantType.LayerMask:
                                return new Vector2((float)_w, 0f);
                            case VariantType.Rect:
                                return new Vector2(_x, _y);
                            case VariantType.Numeric:
                                return ConvertUtil.ToVector2(this.NumericValue);
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToVector2(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return Evaluator.EvalVector2(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return Vector2.zero;
            }
            set
            {
                _x = value.x;
                _y = value.y;
                _z = 0f;
                _w = 0d;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Vector2;
                _mode = RefMode.Value;
            }
        }
        public Vector3 Vector3Value
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return Vector3.zero;
                            case VariantType.String:
                                return ConvertUtil.ToVector3(_string);
                            case VariantType.Boolean:
                                return (_x != 0f) ? Vector3.one : Vector3.zero;
                            case VariantType.Integer:
                                return new Vector3((float)_w, 0f, 0f);
                            case VariantType.Float:
                                return new Vector3(_x, 0f, 0f);
                            case VariantType.Double:
                                return new Vector3((float)_w, 0f, 0f);
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                            case VariantType.Color:
                                return new Vector3(_x, _y, _z);
                            case VariantType.DateTime:
                                return Vector3.zero;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return Vector3.zero;
                            case VariantType.LayerMask:
                                return new Vector3((float)_w, 0f, 0f);
                            case VariantType.Rect:
                                return new Vector3(_x, _y, _z);
                            case VariantType.Numeric:
                                return ConvertUtil.ToVector3(this.NumericValue);
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToVector3(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return Evaluator.EvalVector3(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return Vector3.zero;
            }
            set
            {
                _x = value.x;
                _y = value.y;
                _z = value.z;
                _w = 0d;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Vector3;
                _mode = RefMode.Value;
            }
        }
        public Vector4 Vector4Value
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return Vector4.zero;
                            case VariantType.String:
                                return ConvertUtil.ToVector4(_string);
                            case VariantType.Boolean:
                                return (_x != 0f) ? Vector4.one : Vector4.zero;
                            case VariantType.Integer:
                                return new Vector4((float)_w, 0f, 0f, 0f);
                            case VariantType.Float:
                                return new Vector4(_x, 0f, 0f, 0f);
                            case VariantType.Double:
                                return new Vector4((float)_w, 0f, 0f, 0f);
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                            case VariantType.Color:
                                return new Vector4(_x, _y, _z, (float)_w);
                            case VariantType.DateTime:
                                return Vector4.zero;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return Vector4.zero;
                            case VariantType.LayerMask:
                                return new Vector4((float)_w, 0f, 0f, 0f);
                            case VariantType.Rect:
                                return new Vector4(_x, _y, _z, (float)_w);
                            case VariantType.Numeric:
                                return ConvertUtil.ToVector4(this.NumericValue);
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToVector4(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return Evaluator.EvalVector4(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return Vector4.zero;
            }
            set
            {
                _x = value.x;
                _y = value.y;
                _z = value.z;
                _w = value.w;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Vector4;
                _mode = RefMode.Value;
            }
        }
        public Quaternion QuaternionValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        if (_type == VariantType.Quaternion || _type == VariantType.Vector4)
                            return new Quaternion(_x, _y, _z, (float)_w);
                        else
                            return Quaternion.identity;
                    case RefMode.Property:
                        var obj = this.EvaluateProperty();
                        if (obj is Quaternion)
                            return (Quaternion)obj;
                        else if (obj is Vector3)
                            return Quaternion.Euler((Vector3)obj);
                        else
                            return Quaternion.identity;
                    case RefMode.Eval:
                        try
                        {
                            return Evaluator.EvalQuaternion(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return Quaternion.identity;
            }
            set
            {
                _x = value.x;
                _y = value.y;
                _z = value.z;
                _w = value.w;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Quaternion;
                _mode = RefMode.Value;
            }
        }
        public Color ColorValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return Color.black;
                            case VariantType.String:
                                return ConvertUtil.ToColor(_string);
                            case VariantType.Boolean:
                                return (_x != 0f) ? Color.white : Color.black;
                            case VariantType.Integer:
                                return ConvertUtil.ToColor((int)_w);
                            case VariantType.Float:
                                return ConvertUtil.ToColor((int)_x);
                            case VariantType.Double:
                                return ConvertUtil.ToColor((int)_w);
                            case VariantType.Vector2:
                                return new Color(_x, _y, 0f);
                            case VariantType.Vector3:
                                return new Color(_x, _y, _z);
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                                return new Color(_x, _y, _z, (float)_w);
                            case VariantType.Color:
                                return new Color(_x, _y, _z, (float)_w);
                            case VariantType.DateTime:
                                return Color.black;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return Color.black;
                            case VariantType.LayerMask:
                                return ConvertUtil.ToColor((int)_w);
                            case VariantType.Rect:
                                return new Color(_x, _y, _z, (float)_w);
                            case VariantType.Numeric:
                                return ConvertUtil.ToColor(this.NumericValue);
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToColor(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return Evaluator.EvalColor(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return Color.black;
            }
            set
            {
                _x = value.r;
                _y = value.g;
                _z = value.b;
                _w = value.a;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Color;
                _mode = RefMode.Value;
            }
        }
        public System.DateTime DateValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return new System.DateTime(0L);
                            case VariantType.String:
                                return ConvertUtil.ToDate(_string);
                            case VariantType.Boolean:
                            case VariantType.Integer:
                            case VariantType.Float:
                            case VariantType.Double:
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                            case VariantType.Color:
                                return new System.DateTime(0L);
                            case VariantType.DateTime:
                                return DecodeDateTime(_w, _x);
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return new System.DateTime(0L);
                            case VariantType.LayerMask:
                                return new System.DateTime(0L);
                            case VariantType.Rect:
                                return new System.DateTime(0L);
                            case VariantType.Numeric:
                                return new System.DateTime(0L);
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToDate(this.EvaluateProperty());
                }
                return new System.DateTime(0L);
            }
            set
            {
                EncodeDateTime(value, out _w, out _x);
                _y = 0f;
                _z = 0f;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.DateTime;
                _mode = RefMode.Value;
            }
        }
        public GameObject GameObjectValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        return _unityObjectReference as GameObject;
                    case RefMode.Property:
                        return GameObjectUtil.GetGameObjectFromSource(this.EvaluateProperty());
                }
                return null;
            }
            set
            {
                _x = 0f;
                _y = 0f;
                _z = 0f;
                _w = 0d;
                _string = null;
                _unityObjectReference = value;
                _numeric = null;
                _type = VariantType.GameObject;
                _mode = RefMode.Value;
            }
        }
        public Component ComponentValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        return _unityObjectReference as Component;
                    case RefMode.Property:
                        var obj = this.EvaluateProperty();
                        if (obj is Component)
                            return obj as Component;
                        else if (obj is IComponent)
                            return (obj as IComponent).component;
                        else
                            return null;
                }
                return null;
            }
            set
            {
                _x = 0f;
                _y = 0f;
                _z = 0f;
                _w = 0d;
                _string = null;
                _unityObjectReference = value;
                _numeric = null;
                _type = VariantType.Component;
                _mode = RefMode.Value;
            }
        }
        public UnityEngine.Object ObjectValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        return _unityObjectReference;
                    case RefMode.Property:
                        var obj = this.EvaluateProperty();
                        if (obj is UnityEngine.Object)
                            return obj as UnityEngine.Object;
                        else if (obj is IComponent)
                            return (obj as IComponent).component;
                        else if (obj is IGameObjectSource)
                            return (obj as IGameObjectSource).gameObject;
                        else
                            return null;
                }
                return null;
            }
            set
            {
                _mode = RefMode.Value;
                _x = 0f;
                _y = 0f;
                _z = 0f;
                _w = 0d;
                _string = null;
                _unityObjectReference = value;
                _numeric = null;
                if (_unityObjectReference is GameObject)
                    _type = VariantType.GameObject;
                else if (_unityObjectReference is Component)
                    _type = VariantType.Component;
                else
                    _type = VariantType.Object;
            }
        }
        public LayerMask LayerMaskValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return 0;
                            case VariantType.String:
                                return ConvertUtil.ToInt(_string);
                            case VariantType.Boolean:
                                return (_x != 0f) ? 1 : 0;
                            case VariantType.Integer:
                                return (int)_w;
                            case VariantType.Float:
                                return (int)_x;
                            case VariantType.Double:
                                return (int)_w;
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                                return (int)_x;
                            case VariantType.Color:
                                return ConvertUtil.ToInt(new Color(_x, _y, _z, (float)_w));
                            case VariantType.DateTime:
                                return 0;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return 0;
                            case VariantType.LayerMask:
                                return new LayerMask() { value = (int)_w };
                            case VariantType.Rect:
                                return new LayerMask();
                            case VariantType.Numeric:
                                {
                                    var n = this.NumericValue;
                                    return n != null ? n.ToInt32(null) : 0;
                                }
                        }
                        break;
                    case RefMode.Property:
                        return ConvertUtil.ToInt(this.EvaluateProperty());
                    case RefMode.Eval:
                        try
                        {
                            return (int)Evaluator.EvalNumber(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                            return 0;
                        }
                }
                return 0;
            }
            set
            {
                _x = 0f;
                _y = 0f;
                _z = 0f;
                _w = value;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.LayerMask;
                _mode = RefMode.Value;
            }
        }
        public Rect RectValue
        {
            get
            {
                switch (_mode)
                {
                    case RefMode.Value:
                        switch (_type)
                        {
                            case VariantType.Object:
                                return new Rect();
                            case VariantType.String:
                                {
                                    var v = ConvertUtil.ToVector4(_string);
                                    return new Rect(v.x, v.y, v.z, v.w);
                                }
                            case VariantType.Boolean:
                                return new Rect();
                            case VariantType.Integer:
                                return new Rect((float)_w, 0f, 0f, 0f);
                            case VariantType.Float:
                                return new Rect(_x, 0f, 0f, 0f);
                            case VariantType.Double:
                                return new Rect((float)_w, 0f, 0f, 0f);
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Vector4:
                            case VariantType.Quaternion:
                            case VariantType.Color:
                                return new Rect(_x, _y, _z, (float)_w);
                            case VariantType.DateTime:
                                return new Rect();
                            case VariantType.GameObject:
                            case VariantType.Component:
                                return new Rect();
                            case VariantType.LayerMask:
                                return new Rect();
                            case VariantType.Rect:
                                return new Rect(_x, _y, _z, (float)_w);
                            case VariantType.Numeric:
                                {
                                    var n = this.NumericValue;
                                    return n != null ? new Rect(n.ToSingle(null), 0f, 0f, 0f) : new Rect();
                                }
                        }
                        break;
                    case RefMode.Property:
                        {
                            var r = this.EvaluateProperty();
                            if (r is Rect)
                                return (Rect)r;
                            else
                                return new Rect();
                        }
                    case RefMode.Eval:
                        try
                        {
                            return Evaluator.EvalRect(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }
                return new Rect();
            }
            set
            {
                _x = value.xMin;
                _y = value.yMin;
                _z = value.width;
                _w = value.height;
                _string = null;
                _unityObjectReference = null;
                _numeric = null;
                _type = VariantType.Rect;
                _mode = RefMode.Value;
            }
        }

        public INumeric NumericValue
        {
            get
            {
                switch(_mode)
                {
                    case RefMode.Value:
                        switch(_type)
                        {
                            case VariantType.Object:
                            case VariantType.Component:
                                return _unityObjectReference as INumeric;
                            case VariantType.Numeric:
                                {
                                    if (_numeric == null)
                                    {
                                        _numeric = this.UnravelNumeric();
                                        if (_numeric == null) this.Value = null; //couldn't resolve numeric, set value to null
                                    }
                                    return _numeric;
                                }
                            default:
                                return null;
                        }
                    case RefMode.Property:
                        {
                            var result = this.EvaluateProperty();
                            if (result is INumeric)
                                return result as INumeric;
                            else
                                return null;
                        }
                    case RefMode.Eval:
                        try
                        {
                            var result = Evaluator.EvalValue(_string, ObjUtil.ReduceIfProxy(_unityObjectReference));
                            if (result is INumeric)
                                return result as INumeric;
                            else
                                return null;
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to evaluate statement defined in VariantRefernce");
                        }
                        break;
                }

                return null;
            }
            set
            {
                this.SetToNumeric(value);
            }
        }

        #endregion

        #region Methods

        public object GetValue(VariantType etp)
        {
            switch (etp)
            {
                case VariantType.Object:
                    return this.ObjectValue;
                case VariantType.Null:
                    return null;
                case VariantType.String:
                    return this.StringValue;
                case VariantType.Boolean:
                    return this.BoolValue;
                case VariantType.Integer:
                    return this.IntValue;
                case VariantType.Float:
                    return this.FloatValue;
                case VariantType.Double:
                    return this.DoubleValue;
                case VariantType.Vector2:
                    return this.Vector2Value;
                case VariantType.Vector3:
                    return this.Vector3Value;
                case VariantType.Vector4:
                    return this.Vector4Value;
                case VariantType.Quaternion:
                    return this.QuaternionValue;
                case VariantType.Color:
                    return this.ColorValue;
                case VariantType.DateTime:
                    return this.DateValue;
                case VariantType.GameObject:
                    return this.GameObjectValue;
                case VariantType.Component:
                    return this.ComponentValue;
                case VariantType.LayerMask:
                    return this.LayerMaskValue;
                case VariantType.Rect:
                    return this.RectValue;
                case VariantType.Numeric:
                    return this.NumericValue;
                default:
                    return null;
            }
        }

        /// <summary>
        /// If variant reference is in Property mode, this will SET the target property.
        /// </summary>
        /// <param name="value"></param>
        public void ModifyProperty(object value)
        {
            if (_mode != RefMode.Property) return;
            
            DynamicUtil.SetValue(ObjUtil.ReduceIfProxy(_unityObjectReference), _string, value);
        }

        public void SetToProperty(UnityEngine.Object obj, string property)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");
            var member = DynamicUtil.GetMember(obj, property, false);
            if(member == null)
            {
                _type = VariantType.Null;
            }
            else
            {
                var tp = DynamicUtil.GetReturnType(member);
                if (tp != null)
                    _type = VariantReference.GetVariantType(tp);
                else
                    _type = VariantType.Null;
            }

            _mode = RefMode.Property;
            _x = 0f;
            _y = 0f;
            _z = 0f;
            _w = 0d;

            _string = property;
            _unityObjectReference = obj;
        }

        public void SetToEvalStatement(string statement, UnityEngine.Object optionalParam = null)
        {
            _mode = RefMode.Eval;
            _x = 0f;
            _y = 0f;
            _z = 0f;
            _w = 0d;

            _string = statement;
            _unityObjectReference = optionalParam;
            _type = VariantType.Null;
        }

        private object EvaluateProperty()
        {
            if (_unityObjectReference == null) return null;
            return DynamicUtil.GetValue(ObjUtil.ReduceIfProxy(_unityObjectReference), _string);
        }



        public void SetToNumeric(INumeric value)
        {
            _x = 0f;
            _y = 0f;
            _z = 0f;
            _w = 0d;
            _string = null;
            _unityObjectReference = null;
            _numeric = value;
            _type = (_numeric != null) ? VariantType.Numeric : VariantType.Null;
            _mode = RefMode.Value;

            if (_numeric != null)
            {
                var tc = _numeric.GetUnderlyingTypeCode();
                switch (tc)
                {
                    case System.TypeCode.Boolean:
                    case System.TypeCode.Char:
                    case System.TypeCode.Byte:
                    case System.TypeCode.Int16:
                    case System.TypeCode.UInt16:
                    case System.TypeCode.Int32:
                    case System.TypeCode.UInt32:
                        {
                            _x = (int)tc;
                            _string = TypeReference.HashType(_numeric.GetType());
                            _w = _numeric.ToDouble(null);
                        }
                        break;
                    case System.TypeCode.Int64:
                    case System.TypeCode.UInt64:
                        {
                            _x = (int)tc;
                            _string = TypeReference.HashType(_numeric.GetType());
                            long v = _numeric.ToInt64(null);
                            _w = (double)(v >> 16);
                            _z = (float)(v & 0xFFFF);
                        }
                        break;
                    case System.TypeCode.Single:
                    case System.TypeCode.Double:
                        {
                            _x = (int)tc;
                            _string = TypeReference.HashType(_numeric.GetType());
                            _w = _numeric.ToDouble(null);
                        }
                        break;
                    default:
                        {
                            _x = 0;
                            _string = EncodeB64Numeric(_numeric);
                        }
                        break;
                }
            }
        }

        private INumeric UnravelNumeric()
        {
            var tc = (System.TypeCode)((int)_x);
            switch (tc)
            {
                case System.TypeCode.Boolean:
                case System.TypeCode.Char:
                case System.TypeCode.Byte:
                case System.TypeCode.Int16:
                case System.TypeCode.UInt16:
                case System.TypeCode.Int32:
                case System.TypeCode.UInt32:
                    {
                        var tp = TypeReference.UnHashType(_string);
                        if (tp == null || !typeof(INumeric).IsAssignableFrom(tp)) return null;

                        return Numerics.CreateNumeric(tp, _w);
                    }
                case System.TypeCode.Int64:
                case System.TypeCode.UInt64:
                    {
                        var tp = TypeReference.UnHashType(_string);
                        if (tp == null || !typeof(INumeric).IsAssignableFrom(tp)) return null;

                        long v = (long)_w;
                        v = v << 16;
                        v |= (long)((int)_z & 0xFFFF);
                        return Numerics.CreateNumeric(tp, v);
                    }
                case System.TypeCode.Single:
                case System.TypeCode.Double:
                    {
                        var tp = TypeReference.UnHashType(_string);
                        if (tp == null || !typeof(INumeric).IsAssignableFrom(tp)) return null;

                        return Numerics.CreateNumeric(tp, _w);
                    }
                default:
                    return DecodeB64Numeric(_string);
            }
        }

        #endregion
        
        #region ISerializable Interface

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.SetValue("mode", (byte)_mode);
            switch(_mode)
            {
                case RefMode.Value:
                    {
                        info.AddValue("type", (int)_type);
                        switch (_type)
                        {
                            case VariantType.Object:
                            case VariantType.Null:
                                //do nothing
                                break;
                            case VariantType.String:
                                info.SetValue("value", this.StringValue);
                                break;
                            case VariantType.Boolean:
                                info.SetValue("value", this.BoolValue);
                                break;
                            case VariantType.Integer:
                                info.SetValue("value", this.IntValue);
                                break;
                            case VariantType.Float:
                                info.SetValue("value", this.FloatValue);
                                break;
                            case VariantType.Double:
                                info.SetValue("value", this.DoubleValue);
                                break;
                            case VariantType.Vector2:
                            case VariantType.Vector3:
                            case VariantType.Quaternion:
                            case VariantType.Color:
                                info.SetValue("value", string.Format("{0},{1},{2},{3}", _x, _y, _z, _w));
                                break;
                            case VariantType.DateTime:
                                info.SetValue("value", this.DateValue);
                                break;
                            case VariantType.GameObject:
                            case VariantType.Component:
                                //do nothing
                                break;
                            case VariantType.Numeric:
                                {
                                    var n = this.NumericValue;
                                    if(n == null)
                                    {
                                        info.SetValue("value", null);
                                    }
                                    else if(n.GetType().IsSerializable)
                                    {
                                        info.SetValue("value", n);
                                    }
                                    else
                                    {
                                        info.SetValue("value", EncodeB64Numeric(n));
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case RefMode.Property:
                    //do nothing
                    break;
            }
        }

        private VariantReference(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            var mode = (RefMode)info.GetByte("mode");
            switch(mode)
            {
                case RefMode.Value:
                    _mode = RefMode.Value;
                    _type = (VariantType)info.GetInt32("type");
                    switch (_type)
                    {
                        case VariantType.Object:
                        case VariantType.Null:
                            this.Value = null;
                            break;
                        case VariantType.String:
                            this.StringValue = info.GetString("value");
                            break;
                        case VariantType.Boolean:
                            this.BoolValue = info.GetBoolean("value");
                            break;
                        case VariantType.Integer:
                            this.IntValue = info.GetInt32("value");
                            break;
                        case VariantType.Float:
                            this.FloatValue = info.GetSingle("value");
                            break;
                        case VariantType.Double:
                            this.DoubleValue = info.GetDouble("value");
                            break;
                        case VariantType.Vector2:
                        case VariantType.Vector3:
                        case VariantType.Quaternion:
                        case VariantType.Color:
                            var arr = StringUtil.SplitFixedLength(info.GetString("value"), ",", 4);
                            _x = ConvertUtil.ToSingle(arr[0]);
                            _y = ConvertUtil.ToSingle(arr[1]);
                            _z = ConvertUtil.ToSingle(arr[2]);
                            _w = ConvertUtil.ToDouble(arr[3]);
                            break;
                        case VariantType.DateTime:
                            this.DateValue = info.GetDateTime("value");
                            break;
                        case VariantType.GameObject:
                        case VariantType.Component:
                            this.Value = null;
                            break;
                        case VariantType.Numeric:
                            {
                                var data = info.GetValue("value", typeof(object));
                                if(data is INumeric)
                                {
                                    this.NumericValue = data as INumeric;
                                }
                                else if(data is string)
                                {
                                    this.NumericValue = DecodeB64Numeric(data as string);
                                }
                                else
                                {
                                    this.Value = null;
                                }
                            }
                            break;
                    }
                    break;
                case RefMode.Property:
                    this.Value = null; //just set to null value
                    break;
            }
        }

        #endregion

        #region IDisposable Interface

        bool ISPDisposable.IsDisposed
        {
            get
            {
                if(!object.ReferenceEquals(_unityObjectReference, null))
                {
                    return !ObjUtil.IsObjectAlive(_unityObjectReference);
                }
                else
                {
                    return _mode == RefMode.Value && _type == VariantType.Null;
                }
            }
        }

        void System.IDisposable.Dispose()
        {
            _mode = RefMode.Value;
            _type = VariantType.Null;
            _x = 0f;
            _y = 0f;
            _z = 0f;
            _w = 0.0;
            _string = null;
            _unityObjectReference = null;
            _numeric = null;
        }

        #endregion

        #region Static Utils

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
            else if (tp == typeof(Vector4)) return true;
            else if (tp == typeof(Quaternion)) return true;
            else if (tp == typeof(Color)) return true;
            else if (tp == typeof(LayerMask)) return true;
            else if (tp == typeof(Rect)) return true;
            else if (tp == typeof(GameObject)) return true;
            else if (typeof(Component).IsAssignableFrom(tp)) return true;
            else if (typeof(UnityEngine.Object).IsAssignableFrom(tp)) return true;
            else if (typeof(IComponent).IsAssignableFrom(tp)) return true;
            else if (typeof(INumeric).IsAssignableFrom(tp)) return true;
            else if (tp.IsInterface) return true;
            else if (tp == typeof(Variant)) return true;
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
            else if (tp == typeof(Vector4)) return VariantType.Vector4;
            else if (tp == typeof(Quaternion)) return VariantType.Quaternion;
            else if (tp == typeof(Color)) return VariantType.Color;
            else if (tp == typeof(LayerMask)) return VariantType.LayerMask;
            else if (tp == typeof(Rect)) return VariantType.Rect;
            else if (tp == typeof(GameObject)) return VariantType.GameObject;
            else if (typeof(Component).IsAssignableFrom(tp)) return VariantType.Component;
            else if (typeof(UnityEngine.Object).IsAssignableFrom(tp)) return VariantType.Object;
            else if (typeof(IComponent).IsAssignableFrom(tp)) return VariantType.Component;
            else if (typeof(INumeric).IsAssignableFrom(tp)) return VariantType.Numeric;
            else if (tp.IsInterface) return VariantType.Object;

            return VariantType.Null;
        }

        public static System.Type GetTypeFromVariantType(VariantType etp)
        {
            switch (etp)
            {
                case VariantType.Object:
                    return typeof(UnityEngine.Object);
                case VariantType.String:
                    return typeof(string);
                case VariantType.Boolean:
                    return typeof(bool);
                case VariantType.Integer:
                    return typeof(System.Int32);
                case VariantType.Float:
                    return typeof(float);
                case VariantType.Double:
                    return typeof(double);
                case VariantType.Vector2:
                    return typeof(Vector2);
                case VariantType.Vector3:
                    return typeof(Vector3);
                case VariantType.Quaternion:
                    return typeof(Quaternion);
                case VariantType.Color:
                    return typeof(UnityEngine.Color);
                case VariantType.DateTime:
                    return typeof(System.DateTime);
                case VariantType.GameObject:
                    return typeof(GameObject);
                case VariantType.Component:
                    return typeof(Component);
                case VariantType.LayerMask:
                    return typeof(LayerMask);
                case VariantType.Rect:
                    return typeof(Rect);
                case VariantType.Numeric:
                    return typeof(INumeric);
            }

            return typeof(object);
        }

        private static void EncodeDateTime(System.DateTime dt, out double low, out float high)
        {
            const long MASK_LOW = ((1L << 48) - 1L);
            const long MASK_HIGH = ((1L << 8) - 1L) << 48;

            low = (double)(dt.Ticks & MASK_LOW);
            high = (float)((dt.Ticks & MASK_HIGH) >> 48);
        }

        private static System.DateTime DecodeDateTime(double low, float high)
        {
            const long MASK_LOW = ((1L << 48) - 1L);
            const long MASK_HIGH = ((1L << 8) - 1L);

            long ticks = (long)low & MASK_LOW;
            ticks |= ((long)high & MASK_HIGH) << 48;
            return new System.DateTime(ticks);
        }

        private static string EncodeB64Numeric(INumeric n)
        {
            if (n == null) return null;
            return System.Convert.ToBase64String(n.ToByteArray()) + "|" + TypeReference.HashType(n.GetType());
        }

        private static INumeric DecodeB64Numeric(string sdata)
        {
            if (sdata == null) return null;
            try
            {
                int i = sdata.IndexOf('|', 0);
                if (i < 0) return null;
                var tp = TypeReference.UnHashType(sdata.Substring(i + 1));
                if (tp == null) return null;
                var data = System.Convert.FromBase64String(sdata.Substring(0, i));
                return Numerics.CreateNumeric(tp, data);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        #endregion

        #region Special Types

        public class EditorHelper
        {

            private VariantReference _variant;

            public EditorHelper()
            {
                _variant = new VariantReference();
            }

            public EditorHelper(VariantReference obj)
            {
                _variant = obj;
            }

            public VariantReference Target
            {
                get { return _variant; }
            }

            public RefMode _mode
            {
                get { return _variant._mode; }
                set { _variant._mode = value; }
            }

            public VariantType _type
            {
                get { return _variant._type; }
                set { _variant._type = value; }
            }

            public float _x
            {
                get { return _variant._x; }
                set { _variant._x = value; }
            }

            public float _y
            {
                get { return _variant._y; }
                set { _variant._y = value; }
            }

            public float _z
            {
                get { return _variant._z; }
                set { _variant._z = value; }
            }

            public double _w
            {
                get { return _variant._w; }
                set { _variant._w = value; }
            }

            public string _string
            {
                get { return _variant._string; }
                set { _variant._string = value; }
            }

            public UnityEngine.Object _unityObjectReference
            {
                get { return _variant._unityObjectReference; }
                set { _variant._unityObjectReference = value; }
            }


            public void PrepareForValueTypeChange(VariantType type)
            {
                _variant._type = type;
                _variant._mode = RefMode.Value;
                _variant._x = 0f;
                _variant._y = 0f;
                _variant._z = 0f;
                _variant._w = 0d;
                _variant._string = string.Empty;
                switch(type)
                {
                    case VariantType.Object:
                        break;
                    case VariantType.Null:
                    case VariantType.String:
                    case VariantType.Boolean:
                    case VariantType.Integer:
                    case VariantType.Float:
                    case VariantType.Double:
                    case VariantType.Vector2:
                    case VariantType.Vector3:
                    case VariantType.Quaternion:
                    case VariantType.Color:
                    case VariantType.DateTime:
                        _variant._unityObjectReference = null;
                        break;
                    case VariantType.GameObject:
                        _variant._unityObjectReference = GameObjectUtil.GetGameObjectFromSource(_variant._unityObjectReference);
                        break;
                    case VariantType.Component:
                        _variant._unityObjectReference = _variant._unityObjectReference as Component;
                        break;
                    case VariantType.Numeric:
                        _variant._unityObjectReference = null;
                        break;
                }
            }

            public void PrepareForRefModeChange(RefMode mode)
            {
                _variant._mode = mode;
                _variant._x = 0f;
                _variant._y = 0f;
                _variant._z = 0f;
                _variant._w = 0d;
                _variant._string = string.Empty;
                switch(mode)
                {
                    case RefMode.Value:
                        //_variant._type = ...;
                        _variant._unityObjectReference = null;
                        break;
                    case RefMode.Property:
                        _variant._type = VariantType.Null;
                        break;
                    case RefMode.Eval:
                        //_variant._type = VariantType.Double;
                        break;
                }
            }


        }

        #endregion

    }

}
