#pragma warning disable 0618 // ignore obsolete material
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{
    public static class GizmosHelper
    {

        #region Internal

        private static Material _gizmoWireMaterial;
        internal static Material gizmoWireMaterial
        {
            get
            {
                if (_gizmoWireMaterial == null)
                {
                    _gizmoWireMaterial = new Material("Shader \"SPEditor/DefaultShader\"{   SubShader   {      Pass      {         BindChannels { Bind \"Color\", color }         Blend SrcAlpha OneMinusSrcAlpha         ZWrite Off Cull Off Fog { Mode Off }         Color(1, 1, 1, 1)      }   }}");
                    _gizmoWireMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _gizmoWireMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                }
                return _gizmoWireMaterial;
            }
        }

        internal static void SetDiscSectionPoints(Vector3[] dest, int count, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            from.Normalize();
            Quaternion quaternion = Quaternion.AngleAxis(angle / (float)(count - 1), normal);
            Vector3 vector3 = from * radius;
            for (int index = 0; index < count; ++index)
            {
                dest[index] = center + vector3;
                vector3 = quaternion * vector3;
            }
        }

        #endregion




        public static void DrawArrow(Vector3 start, Vector3 end, float headSize, int detail = 4)
        {
            Gizmos.DrawLine(start, end);

            var n = Vector3.Normalize(start - end) * headSize;
            var da = 360f / (float)detail;
            for (int i = 0; i < detail; i++)
            {
                var v = (VectorUtil.NearSameAxis(n, Vector3.up)) ? Vector3.Cross(n, Vector3.up) : Vector3.Cross(n, Vector3.right);

                var q = Quaternion.AngleAxis(da * i, n) * Quaternion.FromToRotation(n, n + v * 0.25f);
                Gizmos.DrawLine(end, end + q * n);
            }
        }

        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            if (Event.current.type != EventType.Repaint) return;

            Vector3[] dest = new Vector3[60];
            GizmosHelper.SetDiscSectionPoints(dest, 60, center, normal, from, angle, radius);
            //Shader.SetGlobalColor("_HandleColor", Gizmos.color * new Color(1f, 1f, 1f, 0.5f));
            //Shader.SetGlobalFloat("_HandleSize", 1f);
            GizmosHelper.gizmoWireMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(Gizmos.matrix);
            GL.Begin(4);
            for (int index = 1; index < dest.Length; ++index)
            {
                GL.Color(Gizmos.color);
                GL.Vertex(center);
                GL.Vertex(dest[index - 1]);
                GL.Vertex(dest[index]);
                GL.Vertex(center);
                GL.Vertex(dest[index]);
                GL.Vertex(dest[index - 1]);
            }
            GL.End();
            GL.PopMatrix();
        }

    }
}
