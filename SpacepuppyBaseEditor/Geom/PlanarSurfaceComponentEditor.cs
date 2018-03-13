using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Geom;

namespace com.spacepuppyeditor.Geom
{

    [CustomEditor(typeof(PlanarSurfaceComponent))]
    public class PlanarSurfaceComponentEditor : SPEditor
    {


        [DrawGizmo(GizmoType.Selected, drawnType = typeof(PlanarSurfaceComponent))]
        static void DrawGizmos(PlanarSurfaceComponent surface, GizmoType gizmoType)
        {
            var c = Color.green;
            c.a = 0.3f;
            Gizmos.color = c;
            Gizmos.matrix = Matrix4x4.TRS(surface.transform.position, surface.transform.rotation, Vector3.one);

            Gizmos.DrawCube(Vector3.zero, new Vector3(10f, 10f, 0.05f));

            Gizmos.matrix = Matrix4x4.identity;
        }


    }

}
