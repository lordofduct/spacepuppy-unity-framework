using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils.SpecializedComponents;

namespace com.spacepuppy.Utils
{
    public static class PrefabUtil
    {


        public static GameObject Create(GameObject prefab)
        {
            return UnityEngine.Object.Instantiate(prefab) as GameObject;
        }

        public static GameObject Create(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            return UnityEngine.Object.Instantiate(prefab, pos, rot) as GameObject;
        }

        public static GameObject Create(GameObject prefab, Transform parent)
        {
            if (parent == null) return Create(prefab);

            prefab.AddOrGetComponent<EarlyParentSetter>().Init(parent);
            return UnityEngine.Object.Instantiate(prefab, parent.position, parent.rotation) as GameObject;
        }

        public static GameObject Create(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
        {
            if (parent == null) return Create(prefab, pos, rot);

            prefab.AddOrGetComponent<EarlyParentSetter>().Init(parent);
            return UnityEngine.Object.Instantiate(prefab, pos, rot) as GameObject;
        }
        
    }
}
