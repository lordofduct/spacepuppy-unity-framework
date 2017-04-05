using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Sensors;
using com.spacepuppy.AI.Sensors.Visual;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.Sensors.Visual
{

    [CustomEditor(typeof(SphericalVisualSensor))]
    public class SphericalVisualSensorInspector : VisualSensorInspector
    {

        #region Fields

        private static Material _material;
        private static Material _lineMaterial;
        private static Material Material
        {
            get
            {
                if (_material == null)
                {
                    var shader = Shader.Find("SPEditor/VisualSensorArcShader");
                    if (shader == null)
                    {
                        shader = MaterialHelper.DefaultMaterial.shader;
                    }
                    _material = new Material(shader);
                    _material.hideFlags = HideFlags.HideAndDontSave;
                    _material.shader.hideFlags = HideFlags.HideAndDontSave;
                }
                return _material;
            }
        }
        private static Material LineMaterial
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

        #endregion

        #region OnInspector

        protected override void OnSPInspectorGUI()
        {
            base.OnSPInspectorGUI();

            var radiusProp = this.serializedObject.FindProperty("_radius");
            var innerRadProp = this.serializedObject.FindProperty("_innerRadius");
            if (innerRadProp.floatValue < 0f) innerRadProp.floatValue = 0f;
            else if (innerRadProp.floatValue > radiusProp.floatValue) innerRadProp.floatValue = radiusProp.floatValue;

            this.serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region OnSceneGUI

        void OnSceneGUI()
        {
            var targ = this.target as SphericalVisualSensor;
            if (targ == null) return;
            if (!targ.enabled) return;

            Vector3 pos = targ.transform.position;
            Quaternion rot = targ.transform.rotation;
            float horAngle = Mathf.Clamp(targ.HorizontalAngle, 0f, 360f);
            float verAngle = Mathf.Clamp(targ.VerticalAngle, 0f, 360f);

            if(horAngle <= 0f || verAngle <= 0f)
            {
                return;
            }

            var color = targ.SensorColor;
            color.a = 0.4f;

            //draw lines
            var lineMat = SphericalVisualSensorInspector.LineMaterial;
            lineMat.SetColor("_Color", color);
            for (int i = 0; i < lineMat.passCount; ++i)
            {
                lineMat.SetPass(i);
                if(horAngle < 360f)
                {
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(0, horAngle * 0.5f, 0f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(0, horAngle * -0.5f, 0f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                    if(verAngle >= 180f)
                    {
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(90f, 0f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(-90f, 0f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                    }
                }
                if(verAngle < 180f)
                {
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(verAngle * 0.5f, 0f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(verAngle * -0.5f, 0f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                    if(horAngle >= 360f)
                    {
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(verAngle * 0.5f, 180f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(verAngle * -0.5f, 180f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                    }
                }
                if(horAngle < 360f)
                {
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(verAngle * 0.5f, horAngle * 0.5f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(verAngle * 0.5f, horAngle * -0.5f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(verAngle * -0.5f, horAngle * 0.5f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(verAngle * -0.5f, horAngle * -0.5f, 90f), new Vector3(1f, 1f, targ.Radius * 1.8f)));
                }
            }

            //draw ring
            var mat = SphericalVisualSensorInspector.Material;
            mat.SetColor("_Color", color);
            mat.SetFloat("_angle", horAngle / 360f);
            for (int i = 0; i < mat.passCount; ++i)
            {
                mat.SetFloat("_tiltAngle", 0f);
                mat.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot, Vector3.one * targ.Radius * 2f));
                if(verAngle < 180.0f)
                {
                    mat.SetFloat("_tiltAngle", verAngle * 0.5f * Mathf.Deg2Rad);
                    mat.SetPass(i);
                    Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot, Vector3.one * targ.Radius * 2f));
                    Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot, new Vector3(1f, -1f, 1f) * targ.Radius * 2f));
                }
            }

            var rot2 = rot * Quaternion.Euler(0f, 0f, 90f);
            mat.SetFloat("_angle", verAngle / 360f);
            mat.SetFloat("_tiltAngle", 0f);
            for(int i = 0; i < mat.passCount; ++i)
            {
                mat.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot2, Vector3.one * targ.Radius * 2f));
                if(horAngle >= 360f)
                {
                    Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot2 * Quaternion.Euler(180f, 0f, 0f), Vector3.one * targ.Radius * 2f));
                }
                else
                {
                    Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot2 * Quaternion.Euler(horAngle * 0.5f, 0f, 0f), Vector3.one * targ.Radius * 2f));
                    Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot2 * Quaternion.Euler(horAngle * -0.5f, 0f, 0f), Vector3.one * targ.Radius * 2f));
                }
            }
        }

        #endregion

    }
}
