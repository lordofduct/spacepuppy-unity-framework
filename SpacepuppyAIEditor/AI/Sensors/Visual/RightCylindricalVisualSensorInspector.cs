using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Sensors;
using com.spacepuppy.AI.Sensors.Visual;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.Sensors.Visual
{

    [CustomEditor(typeof(RightCylindricalVisualSensor))]
    public class RightCylindricalVisualSensorInspector : SPEditor
    {
        
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
            var targ = this.target as RightCylindricalVisualSensor;
            if (targ == null) return;
            if (!targ.enabled) return;

            var color = targ.SensorColor;
            color.a = 0.4f;

            var globalMat = Matrix4x4.TRS(targ.GetCenterInWorldSpace(), targ.transform.rotation, Vector3.one);
            //var globalMat = Matrix4x4.TRS(targ.Center, Quaternion.identity, Vector3.one);
            var halfHeight = targ.Height / 2.0f;

            var localStart = new Vector3(halfHeight, 0f, 0f);
            var localEnd = new Vector3(-halfHeight, 0f, 0f);

            if (targ.Angle < 360f)
            {
                //##################
                //PARTIAL CYLINDER

                var halfAngle = targ.Angle / 2.0f;
                int cnt = (int)Mathf.Floor(halfAngle / 45f);

                var lineMat = SensorRenderUtil.LineMaterial;
                lineMat.SetColor("_Color", color);
                for (int i = 0; i < lineMat.passCount; ++i)
                {
                    lineMat.SetPass(i);

                    Vector3 linePos;
                    Quaternion lineRot;
                    Vector3 scale = new Vector3(1f, 0.5f, targ.Height) * 2f;
                    Matrix4x4 m;

                    for (int j = -cnt; j <= cnt; j++)
                    {
                        linePos = new Vector3(0, Mathf.Sin(j * MathUtil.PI_4), Mathf.Cos(j * MathUtil.PI_4));
                        linePos *= targ.Radius - 0.05f;
                        linePos.x += halfHeight;
                        lineRot = Quaternion.Euler(0f, -90f, 45f * j - 90f);
                        m = Matrix4x4.TRS(linePos, lineRot, scale);
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                    }

                    //Draw Ends
                    var halfRadian = halfAngle * Mathf.Deg2Rad;

                    //CW END LINE
                    linePos = new Vector3(0, Mathf.Sin(halfRadian), Mathf.Cos(halfRadian));
                    linePos *= targ.Radius - 0.05f;
                    linePos.x += halfHeight;
                    lineRot = Quaternion.Euler(0f, -90f, halfAngle - 90f);
                    m = Matrix4x4.TRS(linePos, lineRot, scale);
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);

                    //CW END LINE
                    linePos = new Vector3(0, Mathf.Sin(-halfRadian), Mathf.Cos(-halfRadian));
                    linePos *= targ.Radius - 0.05f;
                    linePos.x += halfHeight;
                    lineRot = Quaternion.Euler(0f, -90f, -halfAngle - 90f);
                    m = Matrix4x4.TRS(linePos, lineRot, scale);
                    Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);

                    //DRAW EXTENTS
                    if(targ.InnerRadius > 0f)
                    {
                        var extRotUp = Quaternion.Euler(halfAngle - 2.5f, 0f, 0f);
                        var extRotDown = Quaternion.Euler(-halfAngle + 2.5f, 0f, 0f);
                        var r = (targ.Radius - targ.InnerRadius);
                        var vUp = extRotUp * Vector3.forward * targ.InnerRadius;
                        var vDown = extRotDown * Vector3.forward * targ.InnerRadius;
                        var extScale = new Vector3(1f, 0.5f, r - 0.1f) * 2f;
                        m = Matrix4x4.TRS(localStart + vUp, extRotUp, extScale);
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                        m = Matrix4x4.TRS(localEnd + vUp, extRotUp, extScale);
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                        m = Matrix4x4.TRS(localStart + vDown, extRotDown, extScale);
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                        m = Matrix4x4.TRS(localEnd + vDown, extRotDown, extScale);
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                    }
                    else
                    {
                        var extRotUp = Quaternion.Euler(halfAngle - 2.5f, 0f, 0f);
                        var extRotDown = Quaternion.Euler(-halfAngle + 2.5f, 0f, 0f);
                        var extScale = new Vector3(1f, 0.5f, targ.Radius - 0.1f) * 2f;
                        m = Matrix4x4.TRS(localStart, extRotUp, extScale);
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                        m = Matrix4x4.TRS(localEnd, extRotUp, extScale);
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                        m = Matrix4x4.TRS(localStart, extRotDown, extScale);
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                        m = Matrix4x4.TRS(localEnd, extRotDown, extScale);
                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                    }
                }

                //Ring Caps
                this.DrawCap(globalMat, color, localStart, targ.Radius, targ.Angle);
                this.DrawCap(globalMat, color, localEnd, targ.Radius, targ.Angle);

                //inner ring caps
                if (targ.InnerRadius > 0f)
                {
                    this.DrawCap(globalMat, color, localStart, targ.InnerRadius, targ.Angle);
                    this.DrawCap(globalMat, color, localEnd, targ.InnerRadius, targ.Angle);
                }
            }
            else
            {
                //##################
                //FULL CYLINDER

                var lineMat = SensorRenderUtil.LineMaterial;
                lineMat.SetColor("_Color", color);
                for (int i = 0; i < lineMat.passCount; ++i)
                {
                    lineMat.SetPass(i);

                    Vector3 linePos;
                    Quaternion lineRot;
                    Vector3 scale = new Vector3(1f, 0.5f, targ.Height) * 2f;
                    Matrix4x4 m;

                    for (int j = 0; j < 8; j++)
                    {
                        linePos = new Vector3(0, Mathf.Sin(j * MathUtil.PI_4), Mathf.Cos(j * MathUtil.PI_4));
                        linePos *= targ.Radius - 0.05f;
                        linePos.x += halfHeight;
                        lineRot = Quaternion.Euler(0f, -90f, 45f * j - 90f);

                        m = Matrix4x4.TRS(linePos, lineRot, scale);

                        Graphics.DrawMeshNow(PrimitiveUtil.LineMesh, globalMat * m);
                    }
                }

                //Ring Caps
                this.DrawCap(globalMat, color, localStart, targ.Radius, 360f);
                this.DrawCap(globalMat, color, localEnd, targ.Radius, 360f);

                //inner ring caps
                if(targ.InnerRadius > 0f)
                {
                    this.DrawCap(globalMat, color, localStart, targ.InnerRadius, 360f);
                    this.DrawCap(globalMat, color, localEnd, targ.InnerRadius, 360f);
                }
            }

        }

        private void DrawCap(Matrix4x4 globalMat, Color color, Vector3 center, float radius, float angle)
        {
            var mat = SensorRenderUtil.ArcMaterial;
            mat.SetColor("_Color", color);
            mat.SetFloat("_angle", angle / 360f);
            mat.SetFloat("_tiltAngle", 0f);
            for (int i = 0; i < mat.passCount; ++i)
            {
                mat.SetPass(i);

                //get local
                var arcRot = Quaternion.Euler(0f, 0f, 90f);
                var scale = Vector3.one * radius * 2f;

                var m = Matrix4x4.TRS(center, arcRot, scale);
                Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, globalMat * m);
            }
        }

        #endregion

    }

}
