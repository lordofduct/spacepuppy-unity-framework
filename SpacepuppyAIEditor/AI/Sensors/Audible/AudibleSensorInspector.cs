using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Sensors;
using com.spacepuppy.AI.Sensors.Audible;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.Sensors.Audible
{

    [InitializeOnLoad]
    [CustomEditor(typeof(AudibleSensor), true)]
    public class AudibleSensorInspector : SPEditor
    {

        /*

        #region OnSceneGUI

        void OnSceneGUI()
        {
            var targ = this.target as AudibleSensor;
            if (targ == null) return;
            if (!targ.enabled) return;
            
            Vector3 pos = targ.transform.position;
            Quaternion rot = targ.transform.rotation;
            
            var color = targ.SensorColor;
            color.a = 0.4f;

            //draw ring
            var mat = SensorRenderUtil.ArcMaterial;
            mat.SetColor("_Color", color);
            mat.SetFloat("_angle", 1f);
            for (int i = 0; i < mat.passCount; ++i)
            {
                mat.SetFloat("_tiltAngle", 0f);
                mat.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot, Vector3.one * targ.Range * 2f));
            }

            var rot2 = rot * Quaternion.Euler(0f, 0f, 90f);
            mat.SetFloat("_angle", 1f);
            mat.SetFloat("_tiltAngle", 0f);
            for (int i = 0; i < mat.passCount; ++i)
            {
                mat.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot2, Vector3.one * targ.Range * 2f));
                Graphics.DrawMeshNow(PrimitiveUtil.RingMesh, Matrix4x4.TRS(pos, rot2 * Quaternion.Euler(180f, 0f, 0f), Vector3.one * targ.Range * 2f));
            }
        }

        #endregion

        */


        [DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy | GizmoType.Active | GizmoType.Pickable)]
        static void DrawGizmoForMyScript(AudibleSensor targ, GizmoType gizmoType)
        {
            Vector3 pos = targ.transform.position;
            var color = targ.SensorColor;
            color.a = 0.4f;

            Gizmos.color = color;
            Gizmos.DrawSphere(pos, targ.Range);
        }

    }

}
