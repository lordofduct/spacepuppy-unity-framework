using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Sensors;
using com.spacepuppy.AI.Sensors.Visual;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.Sensors.Visual
{

    [InitializeOnLoad]
    [CustomEditor(typeof(VisualAspect), true)]
    public class VisualAspectInspector : SPEditor
    {
        
        static VisualAspectInspector()
        {
            SceneView.onSceneGUIDelegate -= OnGlobalSceneGUI;
            SceneView.onSceneGUIDelegate += OnGlobalSceneGUI;
        }
        private static void OnGlobalSceneGUI(SceneView view)
        {
            var go = Selection.activeGameObject;
            if (go == null) return;

            using (var lst = TempCollection.GetList<VisualAspect>())
            {
                go.FindComponents<VisualAspect>(lst);
                if(lst.Count > 0)
                {
                    var e = lst.GetEnumerator();
                    while(e.MoveNext())
                    {
                        DrawIcon(e.Current);
                    }
                }
            }
        }

        private static void DrawIcon(VisualAspect targ)
        {
            SensorRenderUtil.AspectMaterial.SetColor("_colorSolid", Color.black);
            SensorRenderUtil.AspectMaterial.SetFloat("_colorAlpha", 0.4f);

            Matrix4x4 mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.1f);

            for (int i = 0; i < SensorRenderUtil.AspectMaterial.passCount; ++i)
            {
                SensorRenderUtil.AspectMaterial.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }

            SensorRenderUtil.AspectMaterial.SetColor("_colorSolid", targ.AspectColor);
            SensorRenderUtil.AspectMaterial.SetFloat("_colorAlpha", 0.8f);

            mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.08f);

            for (int i = 0; i < SensorRenderUtil.AspectMaterial.passCount; ++i)
            {
                SensorRenderUtil.AspectMaterial.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy | GizmoType.Active)]
        private static void DrawSphereGizmo(VisualAspect aspect, GizmoType gizmoType)
        {
            if(aspect.Radius > 0f)
            {
                Gizmos.color = aspect.AspectColor;
                Gizmos.DrawWireSphere(aspect.transform.position, aspect.Radius);
            }
        }
        
    }

}
