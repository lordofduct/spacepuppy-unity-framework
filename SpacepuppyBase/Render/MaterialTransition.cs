using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Render
{

    /// <summary>
    /// TODO - add an index for multiple-material renderers.
    /// </summary>
    [System.Serializable()]
    public class MaterialTransition
    {

        #region Fields

        [SerializeField()]
        [DisableOnPlay()]
        private UnityEngine.Object _material;
        [SerializeField()]
        private MaterialPropertyValueType _valueType;
        [SerializeField]
        private MaterialPropertyValueTypeMember _member;
        [SerializeField()]
        private string _propertyName;
        [SerializeField()]
        private List<VariantReference> _values = new List<VariantReference>();

        [SerializeField()]
        [Range(0f, 1f)]
        private float _position;

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

        public List<VariantReference> Values
        {
            get { return _values; }
        }

        public float Position
        {
            get { return _position; }
            set
            {
                _position = Mathf.Clamp01(value);
                this.Sync();
            }
        }

        #endregion

        #region Methods

        public void Sync()
        {
            var mat = this.Material;

            if (mat == null) return;
            if (_values.Count == 0) return;

            int iLow = Mathf.Clamp(Mathf.FloorToInt(_position * (_values.Count - 1)), 0, _values.Count - 1);
            int iHigh = iLow + 1;
            if (iHigh >= _values.Count) iHigh = iLow;

            var dt = (_values.Count > 1) ? 1f / (float)(_values.Count - 1) : 1f;
            var t = _position % dt;

            switch(_valueType)
            {
                case MaterialPropertyValueType.Float:
                    mat.SetFloat(_propertyName, Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t));
                    break;
                case MaterialPropertyValueType.Color:
                    {
                        switch(_member)
                        {
                            case MaterialPropertyValueTypeMember.None:
                                mat.SetColor(_propertyName, Color.Lerp(_values[iLow].ColorValue, _values[iHigh].ColorValue, t));
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.r = Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t);
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.g = Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t);
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.b = Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t);
                                    mat.SetColor(_propertyName, c);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var c = mat.GetColor(_propertyName);
                                    c.a = Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t);
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
                                mat.SetVector(_propertyName, Vector4.Lerp(_values[iLow].Vector4Value, _values[iHigh].Vector4Value, t));
                                break;
                            case MaterialPropertyValueTypeMember.X:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.x = Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t);
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Y:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.y = Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t);
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.Z:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.z = Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t);
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                            case MaterialPropertyValueTypeMember.W:
                                {
                                    var v = mat.GetVector(_propertyName);
                                    v.w = Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t);
                                    mat.SetVector(_propertyName, v);
                                }
                                break;
                        }
                    }
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
                        switch (_member)
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
