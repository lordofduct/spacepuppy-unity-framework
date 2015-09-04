using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Render
{

    [System.Serializable()]
    public class MaterialTransition
    {

        public enum MatValueType
        {
            Float = 0,
            Color = 1,
            Vector = 2,
        }

        #region Fields

        [SerializeField()]
        private Material _material;
        [SerializeField()]
        private MatValueType _valueType;
        [SerializeField()]
        private string _propertyName;
        [SerializeField()]
        private VariantReference _valueStart;
        [SerializeField()]
        private VariantReference _valueEnd;

        [SerializeField()]
        [Range(0f, 1f)]
        private float _position;

        #endregion

        #region Properties

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
            if(_material == null) return;
            switch(_valueType)
            {
                case MatValueType.Float:
                    _material.SetFloat(_propertyName, Mathf.Lerp(_valueStart.FloatValue, _valueEnd.FloatValue, _position));
                    break;
                case MatValueType.Color:
                    _material.SetColor(_propertyName, Color.Lerp(_valueStart.ColorValue, _valueEnd.ColorValue, _position));
                    break;
                case MatValueType.Vector:
                    _material.SetVector(_propertyName, Vector4.Lerp(_valueStart.Vector4Value, _valueEnd.Vector4Value, _position));
                    break;
            }
        }

        #endregion

    }

}
