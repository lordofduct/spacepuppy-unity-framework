using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Render
{

    [System.Serializable()]
    public class MaterialPropertyReference
    {
        
        #region Fields

        [SerializeField()]
        [DisableOnPlay()]
        private UnityEngine.Object _material;
        [SerializeField()]
        private MaterialPropertyValueType _valueType;
        [SerializeField()]
        private string _propertyName;
        [SerializeField]
        private MaterialPropertyValueTypeMember _member;

        #endregion

        #region CONSTRUCTOR

        public MaterialPropertyReference()
        {

        }

        public MaterialPropertyReference(Material mat, string propName, MaterialPropertyValueType valueType, MaterialPropertyValueTypeMember member = MaterialPropertyValueTypeMember.None)
        {
            _material = mat;
            _propertyName = propName;
            _valueType = valueType;
            _member = member;
        }

        public MaterialPropertyReference(Renderer renderer, string propName, MaterialPropertyValueType valueType, MaterialPropertyValueTypeMember member = MaterialPropertyValueTypeMember.None)
        {
            _material = renderer;
            _propertyName = propName;
            _valueType = valueType;
            _member = member;
        }

        #endregion

        #region Properties

        public Material Material
        {
            get { return MaterialUtil.GetMaterialFromSource(_material); }
            set { _material = value; }
        }
        
        public MaterialPropertyValueType PropertyValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
        }

        /// <summary>
        /// If the PropertyValueType is Color/Vector, this allows referencing one of the specific members of the Color/Vector.
        /// </summary>
        public MaterialPropertyValueTypeMember PropertyValueTypeMember
        {
            get { return _member; }
            set { _member = value; }
        }

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        public object Value
        {
            get
            {
                return this.GetValue();
            }
            set
            {
                this.SetValue(value);
            }
        }

        #endregion

        #region Methods

        public void SetValue(object value)
        {
            var mat = this.Material;

            if (mat == null) return;

            switch (_valueType)
            {
                case MaterialPropertyValueType.Float:
                    mat.SetFloat(_propertyName, ConvertUtil.ToSingle(value));
                    break;
                case MaterialPropertyValueType.Color:
                    {
                        switch(_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                mat.SetColor(_propertyName, ConvertUtil.ToColor(value));
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.r = ConvertUtil.ToSingle(value);
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.g = ConvertUtil.ToSingle(value);
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.b = ConvertUtil.ToSingle(value);
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.a = ConvertUtil.ToSingle(value);
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                        }
                    }
                    break;
                case MaterialPropertyValueType.Vector:
                    {
                        switch(_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                mat.SetVector(_propertyName, ConvertUtil.ToVector4(value));
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.x = ConvertUtil.ToSingle(value);
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.y = ConvertUtil.ToSingle(value);
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.z = ConvertUtil.ToSingle(value);
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.w = ConvertUtil.ToSingle(value);
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                        }
                    }
                    break;
                case MaterialPropertyValueType.Texture:
                    mat.SetTexture(_propertyName, value as Texture);
                    break;
            }
        }

        public void SetValue(float value)
        {
            if (_valueType != MaterialPropertyValueType.Float) return;

            var mat = this.Material;
            if (mat == null) return;

            switch (_valueType)
            {
                case MaterialPropertyValueType.Float:
                    mat.SetFloat(_propertyName, value);
                    break;
                case MaterialPropertyValueType.Color:
                    {
                        switch (_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                //do nothing
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.r = value;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.g = value;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.b = value;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.a = value;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                        }
                    }
                    break;
                case MaterialPropertyValueType.Vector:
                    {
                        switch (_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                mat.SetVector(_propertyName, ConvertUtil.ToVector4(value));
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.x = value;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.y = value;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.z = value;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.w = value;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                        }
                    }
                    break;
                case MaterialPropertyValueType.Texture:
                    //do nothing
                    break;
            }
        }

        public void SetValue(Color value)
        {
            if (_valueType != MaterialPropertyValueType.Color) return;

            var mat = this.Material;

            if (mat == null) return;

            switch (_valueType)
            {
                case MaterialPropertyValueType.Float:
                    //do nothing
                    break;
                case MaterialPropertyValueType.Color:
                    {
                        switch (_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                mat.SetColor(_propertyName, value);
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.r = value.r;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.g = value.g;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.b = value.b;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.a = value.a;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                        }
                    }
                    break;
                case MaterialPropertyValueType.Vector:
                    {
                        switch (_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                mat.SetVector(_propertyName, ConvertUtil.ToVector4(value));
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.x = value.r;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.y = value.g;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.z = value.b;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.w = value.a;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                        }
                    }
                    break;
                case MaterialPropertyValueType.Texture:
                    //do nothing
                    break;
            }
        }

        public void SetValue(Vector4 value)
        {
            if (_valueType != MaterialPropertyValueType.Vector) return;

            var mat = this.Material;

            if (mat == null) return;

            switch (_valueType)
            {
                case MaterialPropertyValueType.Float:
                    //do nothing
                    break;
                case MaterialPropertyValueType.Color:
                    {
                        switch (_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                mat.SetColor(_propertyName, ConvertUtil.ToColor(value));
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.r = value.x;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.g = value.y;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.b = value.z;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.a = value.w;
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                        }
                    }
                    break;
                case MaterialPropertyValueType.Vector:
                    {
                        switch (_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                mat.SetVector(_propertyName, value);
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.x = value.x;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.y = value.y;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.z = value.z;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.w = value.w;
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                        }
                    }
                    break;
                case MaterialPropertyValueType.Texture:
                    //do nothing
                    break;
            }
        }

        public object GetValue()
        {
            var mat = this.Material;

            if (mat == null) return null;

            try
            {
                switch (_valueType)
                {
                    case MaterialPropertyValueType.Float:
                        return mat.GetFloat(_propertyName);
                    case MaterialPropertyValueType.Color:
                        switch(_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                return mat.GetColor(_propertyName);
                            case MaterialPropertyValueTypeMember.X:
                                return mat.GetColor(_propertyName).r;
                            case MaterialPropertyValueTypeMember.Y:
                                return mat.GetColor(_propertyName).g;
                            case MaterialPropertyValueTypeMember.Z:
                                return mat.GetColor(_propertyName).b;
                            case MaterialPropertyValueTypeMember.W:
                                return mat.GetColor(_propertyName).a;
                            default:
                                return 0f;
                        }
                    case MaterialPropertyValueType.Vector:
                        switch (_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                return mat.GetVector(_propertyName);
                            case MaterialPropertyValueTypeMember.X:
                                return mat.GetVector(_propertyName).x;
                            case MaterialPropertyValueTypeMember.Y:
                                return mat.GetVector(_propertyName).y;
                            case MaterialPropertyValueTypeMember.Z:
                                return mat.GetVector(_propertyName).z;
                            case MaterialPropertyValueTypeMember.W:
                                return mat.GetVector(_propertyName).w;
                            default:
                                return 0f;
                        }
                    case MaterialPropertyValueType.Texture:
                        return mat.GetTexture(_propertyName);
                }
            }
            catch
            {

            }

            return null;
        }

        #endregion

    }

}
