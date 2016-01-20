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

        public static Material GetMaterialFromSource(object obj, bool useSharedMaterial)
        {
            if (obj is Material) return obj as Material;
            if (obj is Renderer)
            {
                if (useSharedMaterial)
                    return (obj as Renderer).sharedMaterial;
                else
                    return (obj as Renderer).material;
            }

            //var go = GameObjectUtil.GetGameObjectFromSource(obj);
            //if(go != null)
            //{
            //    var r = go.GetComponent<Renderer>();
            //    if (useSharedMaterial)
            //        return r.sharedMaterial;
            //    else
            //        return r.material;
            //}

            return null;
        }

        public static Material GetMaterialFromSource(object obj, int index, bool useSharedMaterial)
        {
            if (obj is Material) return obj as Material;
            if (obj is Renderer)
            {
                if(index < 0) return null;
                var arr = (useSharedMaterial) ? (obj as Renderer).sharedMaterials : (obj as Renderer).materials;
                if (index >= arr.Length) return null;
                return arr[index];
            }

            //var go = GameObjectUtil.GetGameObjectFromSource(obj);
            //if (go != null)
            //{
            //    var r = go.GetComponent<Renderer>();
            //    var arr = (useSharedMaterial) ? r.sharedMaterials : r.materials;
            //    if (index >= arr.Length) return null;
            //    return arr[index];
            //}

            return null;
        }

    }
}
