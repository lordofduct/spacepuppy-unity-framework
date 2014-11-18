using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppyeditor
{
    public static class MaterialHelper
    {

        private static Material _defaultMaterial;
        private static Material _defaultLineMaterial;

        private static Texture2D _outlineTexture;

        public static Material DefaultMaterial
        {
            get
            {
                if (_defaultMaterial == null)
                {
                    _defaultMaterial = new Material("Shader \"SPEditor/DefaultShader\"{   SubShader   {      Pass      {         BindChannels { Bind \"Color\", color }         Blend SrcAlpha OneMinusSrcAlpha         ZWrite Off Cull Off Fog { Mode Off }         Color(1, 1, 1, 1)      }   }}");
                    _defaultMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _defaultMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                }
                return _defaultMaterial;
            }
        }

        public static Material DefaultLineMaterial
        {
            get
            {
                if (_defaultLineMaterial == null)
                {
                    _defaultLineMaterial = new Material("Shader \"SPEditor/LineShader\"{   SubShader   {      Pass      {         BindChannels { Bind \"Color\", color }         Blend SrcAlpha OneMinusSrcAlpha         ZWrite Off Cull Off Fog { Mode Off }      }   }}");
                    _defaultLineMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _defaultLineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                }
                return _defaultLineMaterial;
            }
        }

    }
}
