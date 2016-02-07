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

        #endregion

        #region CONSTRUCTOR

        public MaterialPropertyReference()
        {

        }

        public MaterialPropertyReference(Material mat, string propName, MaterialPropertyValueType valueType)
        {
            _material = mat;
            _propertyName = propName;
            _valueType = valueType;
        }

        public MaterialPropertyReference(Renderer renderer, string propName, MaterialPropertyValueType valueType)
        {
            _material = renderer;
            _propertyName = propName;
            _valueType = valueType;
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
                    mat.SetColor(_propertyName, ConvertUtil.ToColor(value));
                    break;
                case MaterialPropertyValueType.Vector:
                    mat.SetVector(_propertyName, ConvertUtil.ToVector4(value));
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

            mat.SetFloat(_propertyName, value);
        }

        public void SetValue(Color value)
        {
            if (_valueType != MaterialPropertyValueType.Color) return;

            var mat = this.Material;

            if (mat == null) return;

            mat.SetColor(_propertyName, value);
        }

        public void SetValue(Vector4 value)
        {
            if (_valueType != MaterialPropertyValueType.Vector) return;

            var mat = this.Material;

            if (mat == null) return;

            mat.SetVector(_propertyName, value);
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
