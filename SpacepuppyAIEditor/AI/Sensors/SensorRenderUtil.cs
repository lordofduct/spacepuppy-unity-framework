using UnityEngine;
using UnityEditor;

namespace com.spacepuppyeditor.AI.Sensors
{

    internal static class SensorRenderUtil
    {

        #region Fields

        private static Material _arcMaterial;
        public static Material ArcMaterial
        {
            get
            {
                if (_arcMaterial == null)
                {
                    var shader = Shader.Find("SPEditor/VisualSensorArcShader");
                    if (shader == null)
                    {
                        shader = MaterialHelper.DefaultMaterial.shader;
                    }
                    _arcMaterial = new Material(shader);
                    _arcMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _arcMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                }
                return _arcMaterial;
            }
        }

        private static Material _lineMaterial;
        public static Material LineMaterial
        {
            get
            {
                if (_lineMaterial == null)
                {
                    var shader = Shader.Find("SPEditor/VisualSensorLineShader");
                    if (shader == null)
                    {
                        shader = MaterialHelper.DefaultLineMaterial.shader;
                    }
                    _lineMaterial = new Material(shader);
                    _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                }
                return _lineMaterial;
            }
        }

        private static Material _aspectMaterial;
        public static Material AspectMaterial
        {
            get
            {
                if (_aspectMaterial == null)
                {
                    var shader = Shader.Find("SPEditor/VisualAspectShader");
                    if (shader == null)
                    {
                        shader = MaterialHelper.DefaultMaterial.shader;
                    }
                    _aspectMaterial = new Material(shader);
                    _aspectMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _aspectMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                }
                return _aspectMaterial;
            }
        }

        #endregion

    }

}
