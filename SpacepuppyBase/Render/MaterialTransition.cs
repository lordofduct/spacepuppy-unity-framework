using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        private UnityEngine.Object _material;
        [SerializeField()]
        private bool _useSharedMaterial = true;
        [SerializeField()]
        private MaterialPropertyValueType _valueType;
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
            get { return MaterialUtil.GetMaterialFromSource(_material, _useSharedMaterial); }
            set { _material = value; }
        }

        public bool UseSharedMaterial
        {
            get { return _useSharedMaterial; }
            set { _useSharedMaterial = value; }
        }

        public MaterialPropertyValueType PropertyValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
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
                    //_material.SetFloat(_propertyName, Mathf.Lerp(_startValue.FloatValue, _endValue.FloatValue, _position));
                    mat.SetFloat(_propertyName, Mathf.Lerp(_values[iLow].FloatValue, _values[iHigh].FloatValue, t));
                    break;
                case MaterialPropertyValueType.Color:
                    //_material.SetColor(_propertyName, Color.Lerp(_startValue.ColorValue, _endValue.ColorValue, _position));
                    mat.SetColor(_propertyName, Color.Lerp(_values[iLow].ColorValue, _values[iHigh].ColorValue, t));
                    break;
                case MaterialPropertyValueType.Vector:
                    //_material.SetVector(_propertyName, Vector4.Lerp(_startValue.Vector4Value, _endValue.Vector4Value, _position));
                    mat.SetVector(_propertyName, Vector4.Lerp(_values[iLow].Vector4Value, _values[iHigh].Vector4Value, t));
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
                        return mat.GetColor(_propertyName);
                    case MaterialPropertyValueType.Vector:
                        return mat.GetVector(_propertyName);
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
