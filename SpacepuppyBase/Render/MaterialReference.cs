using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Render
{

    [System.Serializable()]
    public class MaterialReference
    {

        public enum MaterialType
        {
            Config = 0,
            Skybox = 1
        }

        #region Fields

        [SerializeField()]
        private Material _material;
        [SerializeField()]
        private MaterialType _type;

        #endregion

        #region Properties

        public Material Material
        {
            get
            {
                switch(_type)
                {
                    case MaterialType.Config:
                        return _material;
                    case MaterialType.Skybox:
                        return RenderSettings.skybox;
                    default:
                        return null;
                }
            }
        }

        #endregion

    }
}
