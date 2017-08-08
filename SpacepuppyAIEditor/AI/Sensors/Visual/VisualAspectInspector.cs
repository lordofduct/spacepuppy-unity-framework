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

        #region Fields

        private static Material _material;
        private static Material Material
        {
            get
            {
                if(_material == null)
                {
                    var shader = Shader.Find("SPEditor/VisualAspectShader");
                    if(shader == null)
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

        #endregion


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
            VisualAspectInspector.Material.SetColor("_colorSolid", Color.black);
            VisualAspectInspector.Material.SetFloat("_colorAlpha", 0.4f);

            Matrix4x4 mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.1f);

            for (int i = 0; i < VisualAspectInspector.Material.passCount; ++i)
            {
                VisualAspectInspector.Material.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }

            VisualAspectInspector.Material.SetColor("_colorSolid", targ.AspectColor);
            VisualAspectInspector.Material.SetFloat("_colorAlpha", 0.8f);

            mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.08f);

            for (int i = 0; i < VisualAspectInspector.Material.passCount; ++i)
            {
                VisualAspectInspector.Material.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }
        }


        /*
         * Obsolete method
         * 
        #region OnSceneGUI
        
        protected virtual void OnSceneGUI()
        {
            var targ = this.target as VisualAspect;
            if (targ == null) return;
            if (!targ.enabled) return;

            VisualAspectInspector.Material.SetColor("_colorSolid", Color.black);
            VisualAspectInspector.Material.SetFloat("_colorAlpha", 0.4f);

            Matrix4x4 mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.1f);

            for(int i = 0; i < VisualAspectInspector.Material.passCount; ++i)
            {
                VisualAspectInspector.Material.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }

            VisualAspectInspector.Material.SetColor("_colorSolid", targ.AspectColor);
            VisualAspectInspector.Material.SetFloat("_colorAlpha", 0.8f);

            mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.08f);

            for (int i = 0; i < VisualAspectInspector.Material.passCount; ++i)
            {
                VisualAspectInspector.Material.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }

        }

        #endregion
        */
    }

}
