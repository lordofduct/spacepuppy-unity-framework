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

            return false;
        }

        public static Material GetMaterialFromSource(object obj)
        {
            if (obj is Material) return obj as Material;
            if (obj is Renderer)
            {
                return (obj as Renderer).sharedMaterial;
            }

            //var go = GameObjectUtil.GetGameObjectFromSource(obj);
            //if (go != null)
            //{
            //    var r = go.GetComponent<Renderer>();
            //    if (r == null) return null;

            //    return r.sharedMaterial;
            //}

            return null;
        }

        public static Material GetMaterialFromSource(object obj, int index)
        {
            if (obj is Material) return obj as Material;
            if (obj is Renderer)
            {
                if(index < 0) return null;
                var arr = (obj as Renderer).sharedMaterials;
                if (index >= arr.Length) return null;
                return arr[index];
            }

            //var go = GameObjectUtil.GetGameObjectFromSource(obj);
            //if (go != null)
            //{
            //    var r = go.GetComponent<Renderer>();
            //    if (r == null) return null;

            //    var arr = r.sharedMaterials;
            //    if (index >= arr.Length) return null;
            //    return arr[index];
            //}

            return null;
        }

        public static Material GetUniqueMaterial(this Renderer renderer, bool forceUnique = false)
        {
            if (renderer == null) throw new System.ArgumentNullException("renderer");

            var source = MaterialSource.GetMaterialSource(renderer);
            if (!source.IsUnique || forceUnique)
                return source.GetUniqueMaterial();
            else
                return source.Material;
        }

    }
}
