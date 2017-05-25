using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Render
{
    public static class MaterialUtil
    {

        public static bool IsMaterialSource(object obj)
        {
            if (obj is Material) return true;
            if (obj is Renderer) return true;
            if (obj is UnityEngine.UI.Graphic) return true;

            return false;
        }

        /// <summary>
        /// Reduces obj to a Material source type (Material, Renderer, UI.Graphics), and returns the material used by it. 
        /// Uses the sharedMaterial by default.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reduceFromGameObjectSource">If the object is a GameObjectSource, when true attempts to retrieve a Renderer or UI.Graphics from said source.</param>
        /// <returns></returns>
        public static Material GetMaterialFromSource(object obj, bool reduceFromGameObjectSource = false)
        {
            if (obj is Material)
                return obj as Material;
            else if (obj is Renderer)
                return (obj as Renderer).sharedMaterial;
            else if (obj is UnityEngine.UI.Graphic)
                return (obj as UnityEngine.UI.Graphic).material ?? (obj as UnityEngine.UI.Graphic).defaultMaterial;

            if(reduceFromGameObjectSource)
            {
                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go == null) return null;

                var rend = go.GetComponent<Renderer>();
                if (rend != null)
                    return rend.sharedMaterial;

                var graph = go.GetComponent<UnityEngine.UI.Graphic>();
                if (graph != null)
                    return graph.material ?? graph.defaultMaterial;
            }

            return null;
        }

        /// <summary>
        /// Reduces obj to a Material source type (Material, Renderer, UI.Graphics), and returns the material used by it. 
        /// Uses the sharedMaterial by default.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="index">Index of the material if multiple.</param>
        /// <param name="reduceFromGameObjectSource">If the object is a GameObjectSource, when true attempts to retrieve a Renderer or UI.Graphics from said source.</param>
        /// <returns></returns>
        public static Material GetMaterialFromSource(object obj, int index, bool reduceFromGameObjectSource = false)
        {
            if (obj is Material)
                return obj as Material;
            else if (obj is Renderer)
            {
                if(index < 0) return null;
                var arr = (obj as Renderer).sharedMaterials;
                if (index >= arr.Length) return null;
                return arr[index];
            }
            else if (obj is UnityEngine.UI.Graphic)
                return (obj as UnityEngine.UI.Graphic).material ?? (obj as UnityEngine.UI.Graphic).defaultMaterial;

            if (reduceFromGameObjectSource)
            {
                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go == null) return null;

                var rend = go.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (index < 0) return null;
                    var arr = rend.sharedMaterials;
                    if (index >= arr.Length) return null;
                    return arr[index];
                }

                var graph = go.GetComponent<UnityEngine.UI.Graphic>();
                if (graph != null)
                    return graph.material ?? graph.defaultMaterial;
            }
            return null;
        }

        /// <summary>
        /// Reduces obj to source type, and returns a copy of the material on it. 
        /// Works like Renderer.material, but also for UI.Graphics.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reduceFromGameObjectSource"></param>
        /// <returns></returns>
        public static Material CopyMaterialFromSource(object obj, bool reduceFromGameObjectSource = false)
        {
            if (obj is Renderer)
                return (obj as Renderer).material;
            else if (obj is UnityEngine.UI.Graphic)
            {
                var graph = obj as UnityEngine.UI.Graphic;
                var source = RendererMaterialSource.GetMaterialSource(graph);
                return source.GetUniqueMaterial();
            }

            if (reduceFromGameObjectSource)
            {
                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go == null) return null;

                var rend = go.GetComponent<Renderer>();
                if (rend != null)
                    return rend.material;

                var graph = go.GetComponent<UnityEngine.UI.Graphic>();
                if (graph != null)
                {
                    var source = RendererMaterialSource.GetMaterialSource(graph);
                    return source.GetUniqueMaterial();
                }
            }

            return null;
        }

        public static Material GetUniqueMaterial(this Renderer renderer, bool forceUnique = false)
        {
            if (renderer == null) throw new System.ArgumentNullException("renderer");

            var source = RendererMaterialSource.GetMaterialSource(renderer);
            if (!source.IsUnique || forceUnique)
                return source.GetUniqueMaterial();
            else
                return source.Material;
        }

        public static Material GetUniqueMaterial(this UnityEngine.UI.Graphic graphic, bool forceUnique = false)
        {
            if (graphic == null) throw new System.ArgumentNullException("graphic");

            var source = GraphicMaterialSource.GetMaterialSource(graphic);
            if (!source.IsUnique || forceUnique)
                return source.GetUniqueMaterial();
            else
                return source.Material;
        }

        public static Material GetUniqueMaterial(object src, bool forceUnique = false)
        {
            if (src == null) throw new System.ArgumentNullException("src");

            var source = MaterialSource.GetMaterialSource(src);
            if (source == null) return null;

            if (!source.IsUnique || forceUnique)
                return source.GetUniqueMaterial();
            else
                return source.Material;
        }

    }
}
