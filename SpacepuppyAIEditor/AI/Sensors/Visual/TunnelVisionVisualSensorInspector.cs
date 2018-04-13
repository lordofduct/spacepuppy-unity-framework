using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Sensors;
using com.spacepuppy.AI.Sensors.Visual;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.Sensors.Visual
{

    [CustomEditor(typeof(TunnelVisionVisualSensor))]
    public class TunnelVisionVisualSensorInspector : SPEditor
    {
        
        #region OnInspector

        #endregion

        #region OnSceneGUI

        void OnSceneGUI()
        {
            var targ = this.target as TunnelVisionVisualSensor;
            if (targ == null) return;
            if (!targ.enabled) return;

            var color = targ.SensorColor;
            color.a = 0.4f;

            var globalMat = Matrix4x4.TRS(targ.transform.position, targ.transform.rotation, Vector3.one);

            var localStart = new Vector3(0, 0f, 0f);
            var localEnd = new Vector3(0f, 0f, targ.Range);


            var lineMat = SensorRenderUtil.LineMaterial;
            lineMat.SetColor("_Color", color);
            for (int i = 0; i < lineMat.passCount; ++i)
            {
                lineMat.SetPass(i);

                Vector3 linePos;
                Quaternion lineRot;
                Vector3 scale = new Vector3(1f, 0.5f, targ.Range) * 2f;

                for (int j = 0; j < 8; j++)
                {
                    //get local
                    linePos = new Vector3(Mathf.Cos(j * MathUtil.PI_4), Mathf.Sin(j * MathUtil.PI_4), 0f);
                    linePos *= targ.Radius - 0.05f;
                    lineRot = Quaternion.Euler(0f, 0f, 45f * j - 90f);

                    var m = Matrix4x4.TRS(linePos, lineRot, scale);

                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                }
            }

            var mat = SensorRenderUtil.ArcMaterial;
            mat.SetColor("_Color", color);
            mat.SetFloat("_angle", 360f / 360f);
            for (int i = 0; i < mat.passCount; ++i)
            {
                mat.SetFloat("_tiltAngle", 0f);
                mat.SetPass(i);
                mat.SetPass(i);

                //get local
                var start = localStart;
                var end = localEnd;
                var arcRot = Quaternion.Euler(90f, 0f, 0f);
                var scale = Vector3.one * targ.Radius * 2f;

                var m1 = Matrix4x4.TRS(start, arcRot, scale);
                var m2 = Matrix4x4.TRS(end, arcRot, scale);

                Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, globalMat * m1);
                Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, globalMat * m2);
            }

        }

        #endregion

    }
}
